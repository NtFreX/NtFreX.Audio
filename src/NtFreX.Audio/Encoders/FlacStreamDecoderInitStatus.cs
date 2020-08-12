namespace NtFreX.Audio.Encoders
{
    internal enum FlacStreamDecoderInitStatus
    {
        /// <summary>
        /// Initialization was successful.
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_OK = 0,
        
        /// <summary>
        /// The library was not compiled with support for the given container format.
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_UNSUPPORTED_CONTAINER,

        /// <summary>
        /// A required callback was not supplied.
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_INVALID_CALLBACKS,

        /// <summary>
        /// An error occurred allocating memory.
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_MEMORY_ALLOCATION_ERROR,
        
        /// <summary>
        /// fopen() failed in FLAC__stream_decoder_init_file() or FLAC__stream_decoder_init_ogg_file().
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_ERROR_OPENING_FILE,
        
        /// <summary>
        /// FLAC__stream_decoder_init_*() was called when the decoder was already initialized, 
        /// usually because FLAC__stream_decoder_finish() was not called.
        /// </summary>
        FLAC__STREAM_DECODER_INIT_STATUS_ALREADY_INITIALIZED
    }

}
