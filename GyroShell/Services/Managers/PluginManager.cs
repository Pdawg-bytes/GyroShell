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
using GyroShell.Library.Services.Managers;

namespace GyroShell.Services.Managers
{
    public class PluginManager : IPluginManager
    {
        private AssemblyLoadContext pluginLoadContext;
        private string pluginDirectory;
        private readonly Dictionary<string, Assembly> loadedPlugins = new Dictionary<string, Assembly>();

        public void InitializePluginList(string directory)
        {
            pluginDirectory = directory;
            pluginLoadContext = new AssemblyLoadContext("PluginLoadContext", isCollectible: true);
        }

        public void LoadAndRunPlugins()
        {
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
                            plugin.Initialize();
                            loadedPlugins[plugin.PluginInformation.Name] = assembly;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[-] PluginManager: Error loading and running plugin from {dllFile}: {ex.Message}");
                }
            }
        }

        public List<PluginUIModel> GetPlugins()
        {
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
                                    Description = plugin.PluginInformation.Description,
                                    PublisherName = plugin.PluginInformation.Publisher,
                                    PluginVersion = "Version " + plugin.PluginInformation.Version, 
                                    PluginId = plugin.PluginInformation.PluginId, 
                                    IsLoaded = false 
                                });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[-] PluginManager: Error getting plugin from {dllFile}: {ex.Message}");
                    return null;
                }
            }
            pluginLoadContext.Unload();
            return returnList;
        }

        public void UnloadPlugins()
        {
            foreach (KeyValuePair<string, Assembly> plugin in loadedPlugins)
            {
                string pluginName = plugin.Key;
                if (loadedPlugins.TryGetValue(pluginName, out var pluginAssembly))
                {
                    foreach (Type type in pluginAssembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin pluginObj = Activator.CreateInstance(type) as IPlugin;
                            pluginObj.Shutdown();
                            loadedPlugins.Remove(pluginName);
                            Debug.WriteLine($"[+] ModuleManager: Module '{pluginName}' unloaded.");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[-] PluginManager: plugin '{pluginName}' not found or already unloaded.");
                }
                pluginLoadContext.Unload();
            }
        }
    }
}
