using NtFreX.Audio.Wasapi.Interop;
using System;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal static class ComObject
    {
        public static T Initialize<T>(string id) 
            where T : class
            => Initialize<T>(new Guid(id));

        public static T Initialize<T>(Guid id) 
            where T : class
        {
            Type? type = Type.GetTypeFromCLSID(id, true);
            if (type == null)
            {
                throw new Exception($"A type with the id '{id}' is not registered.");
            }

            var obj = Activator.CreateInstance(type) as T;
            if(obj == null)
            {
                throw new Exception($"The type with the id '{id}' could not be activated.");
            }

            return obj;
        }
    }
}
