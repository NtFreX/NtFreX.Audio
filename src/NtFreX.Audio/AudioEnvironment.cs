using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Converters;
using NtFreX.Audio.Devices.Adapters;
using NtFreX.Audio.Samplers;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio
{
    public sealed class AudioEnvironment
    {
        public static AudioSamplerFactory Sampler { [return:NotNull] get; } = AudioSamplerFactory.Instance;
        public static AudioContainerSerializerFactory Serializer { [return:NotNull] get; } = AudioContainerSerializerFactory.Instance;
        public static AudioConverterFactory Converter { [return: NotNull] get; } = AudioConverterFactory.Instance;
        public static AudioPlatformAdapterFactory Platform { [return:NotNull] get; } = AudioPlatformAdapterFactory.Instance;
    }
}
