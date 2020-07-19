using NtFreX.Audio.AdapterInfrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace NtFreX.Audio.Devices
{
    internal static class AsssemblyAudioDeviceLoader
    {
        [return: NotNull]
        public static IAudioDevice Initialize(string assemblyName, string typeName)
        {
#if DEBUG && NETCOREAPP3_1
            string path = Path.Combine(Directory.GetCurrentDirectory(), $@"..\..\..\..\..\src\{assemblyName}\bin\debug\netcoreapp3.1\{assemblyName}.dll");
#else
            string path = Path.Combine(Directory.GetCurrentDirectory(), $@"{assemblyName}.dll");
#endif

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

            return audioDevice;
        }
    }
}
