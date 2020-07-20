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
        private static readonly Dictionary<string, IAudioPlatform> InitializedAdapters = new Dictionary<string, IAudioPlatform>();

        [return: NotNull]
        public static IAudioPlatform Initialize(string assemblyName, string typeName)
        {
            var key = $"{assemblyName}:{typeName}";
            if (InitializedAdapters.ContainsKey(key))
            {
                return InitializedAdapters[key];
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), $@"{assemblyName}.dll");
            Type? type = null;
            if (!TryLoadTypeFrom(File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : Assembly.GetExecutingAssembly(), typeName, out type))
            {
                if (!TryLoadTypeFrom(Assembly.GetEntryAssembly(), typeName, out type))
                {
                    TryLoadTypeFrom(Assembly.Load(assemblyName), typeName, out type);
                }
            }
            
            if (type == null)
            {
                throw new Exception("The audio platform could not be loaded");
            }

            var audioDevice = Activator.CreateInstance(type) as IAudioPlatform;
            if (audioDevice == null)
            {
                throw new Exception("The audio platform could not be instantiated");
            }

            InitializedAdapters.Add(key, audioDevice);
            return audioDevice;
        }

        private static bool TryLoadTypeFrom(Assembly? assembly, string name, out Type? type)
        {
            type = assembly?.GetExportedTypes().FirstOrDefault(x => x.Name == name);
            return type != null;
        }
    }
}
