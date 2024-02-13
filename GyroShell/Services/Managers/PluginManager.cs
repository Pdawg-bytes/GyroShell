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
            foreach (string dllFile in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = pluginLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes().Where(asm => !asm.FullName.Contains("GyroShell.Library")))
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type) && type.Name == "PluginRoot")
                        {
                            IPlugin plugin = Activator.CreateInstance(type) as IPlugin;
                            plugin.Initialize();
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
            foreach (string dllFile in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = pluginLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsClass)
                        {
                            MethodInfo initializeMethod = type.GetMethod("Initialize");
                            MethodInfo runMethod = type.GetMethod("Run");

                            if (initializeMethod != null && runMethod != null)
                            {
                                object pluginInstance = Activator.CreateInstance(type);
                                string pluginName = (string)type.GetProperty("PluginName")?.GetValue(pluginInstance);
                                string pluginVersion = (string)type.GetProperty("PluginVersion")?.GetValue(pluginInstance);
                                Guid pluginGuid = (Guid)type.GetProperty("PluginId")?.GetValue(pluginInstance);
                                returnList.Add(new PluginUIModel { PluginName = pluginName, PluginVersion = pluginVersion, PluginId = pluginGuid, IsLoaded = false });
                            }
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
                    MethodInfo stopMethod = pluginAssembly.GetTypes().SelectMany(t => t.GetMethods()).FirstOrDefault(m => m.Name == "Stop" && m.GetParameters().Length == 0);

                    if (stopMethod != null)
                    {
                        object pluginInstance = Activator.CreateInstance(stopMethod.DeclaringType);
                        if ((bool)stopMethod.Invoke(pluginInstance, null))
                        {
                            pluginLoadContext.Unload();
                            loadedPlugins.Remove(pluginName);
                            Debug.WriteLine($"[+] PluginManager: plugin '{pluginName}' unloaded.");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[-] PluginManager: plugin '{pluginName}' not found or already unloaded.");
                }
            }
        }
    }
}
