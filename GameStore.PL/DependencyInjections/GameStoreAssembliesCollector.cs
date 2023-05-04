using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GameStore.PL.DependencyInjections
{
    public static class GameStoreAssembliesCollector
    {
        public static Assembly[] CollectAssemblies()
        {
            var saved = new List<AssemblyName>();
            var gameStoreAssemblies = new List<AssemblyName>();

            var rootAssembly = Assembly.GetExecutingAssembly();
            gameStoreAssemblies.Add(rootAssembly.GetName());

            SaveReferencedAssemblyNames(rootAssembly, saved, gameStoreAssemblies);

            var loadedAssemblies = LoadAssemblies(gameStoreAssemblies);

            return loadedAssemblies.ToArray();
        }

        private static void SaveReferencedAssemblyNames(Assembly rootAssembly, List<AssemblyName> assemblyNames, List<AssemblyName> gameStoreAssemblies)
        {
            var references = rootAssembly.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                if (!assemblyNames.Any(names => names.FullName == reference.FullName))
                {
                    assemblyNames.Add(reference);

                    if (reference.FullName.Contains("GameStore"))
                    {
                        gameStoreAssemblies.Add(reference);
                    }

                    try
                    {
                        SaveReferencedAssemblyNames(Assembly.Load(reference), assemblyNames, gameStoreAssemblies);
                    }
                    catch (FileNotFoundException)
                    {
                        continue;
                    }
                }
            }
        }

        private static Assembly[] LoadAssemblies(List<AssemblyName> assemblyNames)
        {
            var loadedAssemblies = new List<Assembly>();

            foreach (var assemblyName in assemblyNames)
            {
                loadedAssemblies.Add(Assembly.Load(assemblyName));
            }

            return loadedAssemblies.ToArray();
        }
    }
}
