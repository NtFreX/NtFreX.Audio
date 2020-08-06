﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NtFreX.Audio.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NtFreX.Audio.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading audio samples failed.
        /// </summary>
        internal static string AudioSampleLoadingFailed {
            get {
                return ResourceManager.GetString("AudioSampleLoadingFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A cancelation has been requested.
        /// </summary>
        internal static string CancelationRequested {
            get {
                return ResourceManager.GetString("CancelationRequested", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given data stream is null.
        /// </summary>
        internal static string DataStreamNull {
            get {
                return ResourceManager.GetString("DataStreamNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value has to contain the letters &apos;data&apos; (0x64617461 big-endian form).
        /// </summary>
        internal static string DataSubChunkIdMissmatch {
            get {
                return ResourceManager.GetString("DataSubChunkIdMissmatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given audio format is not supported.
        /// </summary>
        internal static string FmtSubChunckFormatNotSupported {
            get {
                return ResourceManager.GetString("FmtSubChunckFormatNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given audio has a faulty fmt chunk size. Only size of 16 are supported.
        /// </summary>
        internal static string FmtSubChunckSizeMissmatch {
            get {
                return ResourceManager.GetString("FmtSubChunckSizeMissmatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value has to contain the letters &apos;fmt&apos; (0x666d7420 big-endian form).
        /// </summary>
        internal static string FmtSubChunkIdMissmatch {
            get {
                return ResourceManager.GetString("FmtSubChunkIdMissmatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value has to contain the letters &apos;WAVE&apos; (0x57415645 big-endian form).
        /// </summary>
        internal static string InvalidRiffChunkFormat {
            get {
                return ResourceManager.GetString("InvalidRiffChunkFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value has to contain the letters &apos;RIFF&apos; or &apos;RIFX&apos;.
        /// </summary>
        internal static string InvalidRiffChunkId {
            get {
                return ResourceManager.GetString("InvalidRiffChunkId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No audio device handler for the current platform has been found..
        /// </summary>
        internal static string NoAudioDeviceForPlatform {
            get {
                return ResourceManager.GetString("NoAudioDeviceForPlatform", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given stream is to short.
        /// </summary>
        internal static string StreamToShort {
            get {
                return ResourceManager.GetString("StreamToShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No data sub chunk has been found.
        /// </summary>
        internal static string WaveAudioContainerNoDataSubChunk {
            get {
                return ResourceManager.GetString("WaveAudioContainerNoDataSubChunk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No fmt sub chunk has been found.
        /// </summary>
        internal static string WaveAudioContainerNoFmtSubChunk {
            get {
                return ResourceManager.GetString("WaveAudioContainerNoFmtSubChunk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No riff sub chunk has been found.
        /// </summary>
        internal static string WaveAudioContainerNoRiffSubChunk {
            get {
                return ResourceManager.GetString("WaveAudioContainerNoRiffSubChunk", resourceCulture);
            }
        }
    }
}
