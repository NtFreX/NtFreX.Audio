using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Devices.Adapters;

namespace NtFreX.Audio
{
    public sealed class AudioEnvironment
    {
        public static AudioContainerSerializerFactory Serializer => AudioContainerSerializerFactory.Instance;
        public static AudioDeviceAdapterFactory Device => AudioDeviceAdapterFactory.Instance;
    }
}
