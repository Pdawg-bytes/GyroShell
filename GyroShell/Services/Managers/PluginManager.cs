#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;
using GyroShell.Library.Interfaces;
using GyroShell.Library.Models.Plugins;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Managers;
using Microsoft.Extensions.DependencyInjection;
using Windows.Storage;

namespace GyroShell.Services.Managers
{
    public class PluginManager : IPluginManager
    {
        private readonly Dictionary<AssemblyLoadContext, IPlugin> loadedPlugins = new Dictionary<AssemblyLoadContext, IPlugin>();

        private readonly ISettingsService _settingsService;
        private readonly PluginServiceContext _sharedPluginContext;


        public PluginManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            _sharedPluginContext = new PluginServiceContext
            {
                Settings = App.ServiceProvider.GetRequiredService<ISettingsService>(),
                Launcher = App.ServiceProvider.GetRequiredService<IInternalLauncher>(),
                Dispatcher = App.ServiceProvider.GetRequiredService<IDispatcherService>(),
                EnvironmentInfo = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>(),
                ShellHook = App.ServiceProvider.GetRequiredService<IShellHookService>(),

                Clock = App.ServiceProvider.GetRequiredService<ITimeService>(),
                Network = App.ServiceProvider.GetRequiredService<INetworkService>(),
                Battery = App.ServiceProvider.GetRequiredService<IBatteryService>(),
                Sound = App.ServiceProvider.GetRequiredService<ISoundService>(),

                Notifications = App.ServiceProvider.GetRequiredService<INotificationManager>()
            };

            foreach (string pluginName in _settingsService.PluginsToLoad)
                LoadAndRunPlugin(pluginName);

            IsUnloadRestartPending = false;
            Debug.WriteLine(ApplicationData.Current.TemporaryFolder.Path);
        }

        private void OnPluginCreated(object sender, FileSystemEventArgs e)
        {
            GetPlugins();
        }

        private void ExtractPluginResources(string pluginName)
        {
            // TODO: Implement extracting ZIP from Assembly resources.

            //string pluginResourceCollection = Directory.GetFiles(_settingsService.ModulesFolderPath, "*.zip").Where(file => Path.GetFileName(file) == pluginName + ".zip").First();
            //string tempFolder = ApplicationData.Current.TemporaryFolder.Path;

            //ZipFile.ExtractToDirectory(pluginResourceCollection, tempFolder);
            //Debug.WriteLine($"{pluginName}'s resources extracted to {tempFolder}");
        }

        private bool _isUnloadRestartPending;
        public bool IsUnloadRestartPending
        {
            get => _isUnloadRestartPending;
            set => _isUnloadRestartPending = value;
        }

        public void LoadAndRunPlugin(string pluginName)
        {
            foreach (string dllFile in Directory.GetFiles(_settingsService.ModulesFolderPath, "*.dll").Where(file => Path.GetFileName(file) == pluginName))
            {
                try
                {
                    AssemblyLoadContext localPluginLoadContext = new AssemblyLoadContext($"PluginLoadContext_{pluginName}", true);
                    Assembly assembly = localPluginLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin plugin = Activator.CreateInstance(type) as IPlugin;

                            switch (plugin.PluginInformation.PluginType)
                            {
                                case Library.Enums.PluginType.Backend:
                                    break;
                                case Library.Enums.PluginType.InternalUI: 
                                    break;
                                case Library.Enums.PluginType.ExternalUI:
                                    ExtractPluginResources(pluginName.Substring(0, pluginName.Length - 4));
                                    break;
                            }

                            plugin.Initialize(_sharedPluginContext);
                            loadedPlugins[localPluginLoadContext] = plugin;
                            if (_settingsService.SettingExists($"LoadPlugin_{pluginName}"))
                            {
                                _settingsService.SetSetting($"LoadPlugin_{pluginName}", true);
                            }
                            else
                            {
                                _settingsService.AddSetting($"LoadPlugin_{pluginName}", true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[-] PluginManager: Error loading and running plugin from {Path.GetFileName(dllFile)}: {ex.Message}");
                }
            }
        }

        public List<PluginUIModel> GetPlugins()
        {
            AssemblyLoadContext pluginLoadContext = new AssemblyLoadContext("PluginLoadContext", isCollectible: true);
            List<PluginUIModel> returnList = new List<PluginUIModel>();
            foreach (string dllFile in Directory.GetFiles(_settingsService.ModulesFolderPath, "*.dll").Where(file => !file.Contains("GyroShell.Library")))
            {
                try
                {
                    Assembly assembly = pluginLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin plugin = Activator.CreateInstance(type) as IPlugin; 

                            returnList.Add(
                                new PluginUIModel
                                {
                                    PluginName = plugin.PluginInformation.Name,
                                    FullName = Path.GetFileName(dllFile),
                                    Description = plugin.PluginInformation.Description,
                                    PublisherName = plugin.PluginInformation.Publisher,
                                    PluginVersion = "Version " + plugin.PluginInformation.Version,
                                    PluginId = plugin.PluginInformation.PluginId,
                                    IsLoaded = loadedPlugins.Any(kv => kv.Value.PluginInformation.Name == plugin.PluginInformation.Name)
                                });

                            GC.Collect();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[-] PluginManager: Error getting plugin from {dllFile}: {ex.Message}");
                    return new List<PluginUIModel>();
                }
            }
            List<string> pluginsToLoad = _settingsService.PluginsToLoad;
            foreach (string pluginName in pluginsToLoad)
            {
                if (!returnList.Any(p => p.FullName == pluginName))
                {
                    _settingsService.RemoveSetting("LoadPlugin_" + pluginName);
                }
            }
            pluginLoadContext.Unload();
            return returnList;
        }

        public void UnloadPlugin(string pluginName)
        {
            foreach (KeyValuePair<AssemblyLoadContext, IPlugin> plugin in loadedPlugins.Where(asm => asm.Key.Name.Contains(pluginName)))
            {
                AssemblyLoadContext pluginContext = plugin.Key;
                if (loadedPlugins.TryGetValue(pluginContext, out IPlugin pluginObj))
                {
                    pluginObj.Shutdown();
                    pluginContext.Unload();
                    IsUnloadRestartPending = true;
                    if (_settingsService.SettingExists($"LoadPlugin_{pluginName}"))
                    {
                        _settingsService.SetSetting($"LoadPlugin_{pluginName}", false);
                    }
                    else
                    {
                        _settingsService.AddSetting($"LoadPlugin_{pluginName}", false);
                    }
                }
                else
                {
                    Debug.WriteLine($"[-] PluginManager: plugin '{plugin.Key.Name}' not found or already unloaded.");
                }
            }
        }
    }
}