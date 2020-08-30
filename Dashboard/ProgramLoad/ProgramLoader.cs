using Dashboard.Core;
using Shared.ExtensionMethods;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dashboard.ProgramLoad
{
    public static class ApplicationLoader
    {
        private static string ApplicationFolder = "Applications";

        public static List<Type> GetApplicationsInstances()
        {
            var directory = GetRelativeApplicationDirectory();
            var applicationDllPaths = Directory.GetFiles(directory, "*.dll");

            var instances = new List<Type>();

            // Loop over every dll in the folder, grab the types from the assembly, and check to see if any
            // of them implement our ApplicationInstance interface, if they do, grab a copy of the type.
            foreach (var dllPath in applicationDllPaths)
            {
                var assembly = Assembly.LoadFrom(dllPath);

                var types = assembly.GetTypes();

                foreach (var type in types.Where(t => t != typeof(HomeApplicationInstance)))
                {
                    if (type.Implements(typeof(IApplicationInstance)))
                    {
                        instances.Add(type);
                    }
                }
            }

            return instances;
        }

        private static string GetRelativeApplicationDirectory()
        {
            var directory = $"{Directory.GetCurrentDirectory()}\\{ApplicationFolder}";

            if (Directory.Exists(directory))
            {
                return directory;
            }

            throw new Exception("Application directory cannot be found.");
        }
    }
}
