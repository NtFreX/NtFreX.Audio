﻿using NtFreX.Audio.AdapterInfrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            var type = assembly.GetExportedTypes().First(x => x.Name == typeName);
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
    }
}
