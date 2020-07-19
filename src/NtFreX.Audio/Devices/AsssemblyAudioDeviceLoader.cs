using NtFreX.Audio.AdapterInfrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace NtFreX.Audio.Devices
{
    internal static class AsssemblyAudioDeviceLoader
    {
        private static readonly Dictionary<string, IAudioDevice> initializedAdapters = new Dictionary<string, IAudioDevice>();

        [return: NotNull]
        public static IAudioDevice Initialize(string assemblyName, string typeName)
        {
            var key = $"{assemblyName}:{typeName}";
            if (initializedAdapters.ContainsKey(key))
            {
                return initializedAdapters[key];
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), $@"{assemblyName}.dll");
            Type type = null;
            if (!TryLoadTypeFrom(File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : Assembly.GetExecutingAssembly(), typeName, out type))
            {
                if (!TryLoadTypeFrom(Assembly.GetEntryAssembly(), typeName, out type))
                {
                    TryLoadTypeFrom(Assembly.Load(assemblyName), typeName, out type);
                }
            }
            
            if (type == null)
            {
                throw new Exception("The audio device adapter could not be loaded");
            }

            var audioDevice = Activator.CreateInstance(type) as IAudioDevice;
            if (audioDevice == null)
            {
                throw new Exception("The audio device could not be instantiated");
            }

            initializedAdapters.Add(key, audioDevice);
            return audioDevice;
        }

        private static bool TryLoadTypeFrom(Assembly? assembly, string name, out Type? type)
        {
            type = assembly?.GetExportedTypes().FirstOrDefault(x => x.Name == name);
            return type != null;
        }
    }
}
