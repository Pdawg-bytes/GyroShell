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
        private readonly Dictionary<AssemblyLoadContext, Assembly> loadedPlugins = new Dictionary<AssemblyLoadContext, Assembly>();

        private string pluginDirectory;
        FileSystemWatcher pluginFolderWatcher;

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

            if(pluginDirectory != null || pluginDirectory != String.Empty) 
            {
                pluginFolderWatcher = new FileSystemWatcher(pluginDirectory);
                pluginFolderWatcher.NotifyFilter = NotifyFilters.LastWrite;
                pluginFolderWatcher.Filter = "*.dll";
                pluginFolderWatcher.Changed += OnPluginCreated;
                pluginFolderWatcher.EnableRaisingEvents = true;
            }
        }

        private void OnPluginCreated(object sender, FileSystemEventArgs e)
        {
            GetPlugins();
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
                            plugin.Initialize(m_pluginServiceBridge.GetPluginServiceProvider(plugin.PluginInformation.RequiredServices));
                            loadedPlugins[localPluginLoadContext] = assembly;
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
                            returnList.Add(
                                new PluginUIModel
                                {
                                    PluginName = plugin.PluginInformation.Name,
                                    FullName = Path.GetFileName(dllFile),
                                    Description = plugin.PluginInformation.Description,
                                    PublisherName = plugin.PluginInformation.Publisher,
                                    PluginVersion = "Version " + plugin.PluginInformation.Version,
                                    PluginId = plugin.PluginInformation.PluginId,
                                    IsLoaded = loadedPlugins.Any(kv => kv.Value.FullName == assembly.FullName)
                                });
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
            foreach (KeyValuePair<AssemblyLoadContext, Assembly> plugin in loadedPlugins.Where(asm => asm.Key.Name.Contains(pluginName)))
            {
                AssemblyLoadContext pluginContext = plugin.Key;
                if (loadedPlugins.TryGetValue(pluginContext, out var pluginAssembly))
                {
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin pluginObj = Activator.CreateInstance(type) as IPlugin;
                            pluginObj.Shutdown();
                            pluginContext.Unload();
                            loadedPlugins.Remove(pluginContext);
                            if (m_settingsService.SettingExists($"LoadPlugin_{pluginName}"))
                            {
                                m_settingsService.SetSetting($"LoadPlugin_{pluginName}", false);
                            }
                            else
                            {
                                m_settingsService.AddSetting($"LoadPlugin_{pluginName}", false);
                            }
                        }
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