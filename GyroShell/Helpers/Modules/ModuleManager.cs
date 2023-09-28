using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Helpers.Modules
{
    internal class ModuleManager
    {
        private static AssemblyLoadContext moduleLoadContext;
        private static string moduleDirectory;
        private static readonly Dictionary<string, Assembly> loadedModules = new Dictionary<string, Assembly>();

        public static void InitializeModuleList(string directory)
        {
            moduleDirectory = directory;
            moduleLoadContext = new AssemblyLoadContext("ModuleLoadContext", isCollectible: true);
        }

        public static void LoadAndRunModules()
        {
            foreach (string dllFile in Directory.GetFiles(moduleDirectory, "*.dll"))
            {
                try
                {
                    Assembly assembly = moduleLoadContext.LoadFromAssemblyPath(Path.GetFullPath(dllFile));

                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsClass)
                        {
                            MethodInfo initializeMethod = type.GetMethod("Initialize");
                            MethodInfo runMethod = type.GetMethod("Run");

                            if (initializeMethod != null && runMethod != null)
                            {
                                Debug.WriteLine($"[+] ModuleManager: Module '{type.FullName}' loaded.");
                                object moduleInstance = Activator.CreateInstance(type);
                                if ((bool)initializeMethod.Invoke(moduleInstance, null))
                                {
                                    runMethod.Invoke(moduleInstance, null);
                                    string moduleName = type.FullName;
                                    loadedModules[moduleName] = assembly;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[-] ModuleManager: Error loading and running module from {dllFile}: {ex.Message}");
                }
            }
        }

        public static void UnloadModules()
        {
            foreach(KeyValuePair<string, Assembly> module in loadedModules)
            {
                string moduleName = module.Key;
                if (loadedModules.TryGetValue(moduleName, out var moduleAssembly))
                {
                    MethodInfo stopMethod = moduleAssembly.GetTypes().SelectMany(t => t.GetMethods()).FirstOrDefault(m => m.Name == "Stop" && m.GetParameters().Length == 0);

                    if (stopMethod != null)
                    {
                        object moduleInstance = Activator.CreateInstance(stopMethod.DeclaringType);
                        if ((bool)stopMethod.Invoke(moduleInstance, null))
                        {
                            moduleLoadContext.Unload();
                            loadedModules.Remove(moduleName);
                            Debug.WriteLine($"[+] ModuleManager: Module '{moduleName}' unloaded.");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[-] ModuleManager: Module '{moduleName}' not found or already unloaded.");
                }
            }
        }
    }
}
