using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Converters;
using NtFreX.Audio.Devices.Adapters;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Samplers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio
{
    public static class AudioEnvironment
    {
        public static AudioSamplerFactory Sampler { [return:NotNull] get; } = AudioSamplerFactory.Instance;
        public static AudioContainerSerializerFactory Serializer { [return:NotNull] get; } = AudioContainerSerializerFactory.Instance;
        public static AudioConverterFactory Converter { [return: NotNull] get; } = AudioConverterFactory.Instance;
        public static IAudioPlatform Platform { [return:NotNull] get; } = AudioPlatformAdapterFactory.Instance.Get();
        public static IAudioDeviceFactory DeviceFactory { [return: NotNull] get; } = Platform.AudioDeviceFactory;
    }

    public static class AudioContainer
    {
        public static Task<IAudioContainer> FromFileAsync(string path, CancellationToken cancellationToken = default) => AudioEnvironment.Serializer.FromFileAsync(path, cancellationToken);
        public static Task<IAudioContainer> FromFileAsync(string path, string extension, CancellationToken cancellationToken = default) => AudioEnvironment.Serializer.FromFileAsync(path, extension, cancellationToken);
    }

    public static class AudioDevice
    {
        public static IAudioDevice GetDefaultCaptureDevice() => AudioEnvironment.DeviceFactory.GetDefaultCaptureDevice();
        public static IAudioDevice GetDefaultRenderDevice() => AudioEnvironment.DeviceFactory.GetDefaultRenderDevice();
    }
}
