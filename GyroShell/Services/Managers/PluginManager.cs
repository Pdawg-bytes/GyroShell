#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using GyroShell.Library.Interfaces;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.Services.Bridges;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.Services.Managers;
using GyroShell.Services.Environment;

namespace GyroShell.Services.Managers
{
    public class PluginManager : IPluginManager
    {
        private readonly Dictionary<AssemblyLoadContext, IPlugin> loadedPlugins = new Dictionary<AssemblyLoadContext, IPlugin>();

        private string pluginDirectory;

        private readonly ISettingsService m_settingsService;
        private readonly IPluginServiceBridge m_pluginServiceBridge;


        public PluginManager(ISettingsService settingsService, IPluginServiceBridge pluginServiceBridge)
        {
            m_settingsService = settingsService;
            pluginDirectory = m_settingsService.ModulesFolderPath;

            m_pluginServiceBridge = pluginServiceBridge;

            foreach (string pluginName in m_settingsService.PluginsToLoad)
            {
                LoadAndRunPlugin(pluginName);
            }

            IsUnloadRestartPending = false;
        }

        private void OnPluginCreated(object sender, FileSystemEventArgs e)
        {
            GetPlugins();
        }

        private bool _isUnloadRestartPending;
        public bool IsUnloadRestartPending
        {
            get => _isUnloadRestartPending;
            set => _isUnloadRestartPending = value;
        }

        public void LoadAndRunPlugin(string pluginName)
        {
            foreach (string dllFile in Directory.GetFiles(pluginDirectory, "*.dll").Where(file => Path.GetFileName(file) == pluginName))
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
                            plugin.Initialize(m_pluginServiceBridge.CreatePluginServiceProvider(plugin.PluginInformation.RequiredServices));
                            loadedPlugins[localPluginLoadContext] = plugin;
                            if (m_settingsService.SettingExists($"LoadPlugin_{pluginName}"))
                            {
                                m_settingsService.SetSetting($"LoadPlugin_{pluginName}", true);
                            }
                            else
                            {
                                m_settingsService.AddSetting($"LoadPlugin_{pluginName}", true);
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
            foreach (string dllFile in Directory.GetFiles(pluginDirectory, "*.dll").Where(file => !file.Contains("GyroShell.Library")))
            {
                try
                {
                    Assembly assembly = pluginLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin plugin = Activator.CreateInstance(type) as IPlugin;
                            string name = plugin.PluginInformation.Name, fullName = Path.GetFileName(dllFile),
                                description = plugin.PluginInformation.Description, pubName = plugin.PluginInformation.Publisher, 
                                version = plugin.PluginInformation.Version;

                            Guid id = plugin.PluginInformation.PluginId;
                            bool isLoaded = loadedPlugins.Any(kv => kv.Value.PluginInformation.Name == name);

                            returnList.Add(
                                new PluginUIModel
                                {
                                    PluginName = name,
                                    FullName = fullName,
                                    Description = description,
                                    PublisherName = pubName,
                                    PluginVersion = "Version " + version,
                                    PluginId = id,
                                    IsLoaded = isLoaded
                                });

                            name = null; fullName = null; description = null; pubName = null; version = null;
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
            List<string> pluginsToLoad = m_settingsService.PluginsToLoad;
            foreach (string pluginName in pluginsToLoad)
            {
                if (!returnList.Any(p => p.FullName == pluginName))
                {
                    m_settingsService.RemoveSetting("LoadPlugin_" + pluginName);
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
                if (loadedPlugins.TryGetValue(pluginContext, out var pluginObj))
                {
                    pluginObj.Shutdown();
                    pluginContext.Unload();
                    IsUnloadRestartPending = true;
                    if (m_settingsService.SettingExists($"LoadPlugin_{pluginName}"))
                    {
                        m_settingsService.SetSetting($"LoadPlugin_{pluginName}", false);
                    }
                    else
                    {
                        m_settingsService.AddSetting($"LoadPlugin_{pluginName}", false);
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