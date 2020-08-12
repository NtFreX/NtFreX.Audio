namespace NtFreX.Audio.Encoders
{
    internal enum FlacDecoderState
    {
        /// <summary>
        /// The decoder is ready to search for metadata.
        /// </summary>
        FLAC__STREAM_DECODER_SEARCH_FOR_METADATA = 0,

        /// <summary>
        /// The decoder is ready to or is in the process of reading metadata.
        /// </summary>
        FLAC__STREAM_DECODER_READ_METADATA,

        /// <summary>
        /// The decoder is ready to or is in the process of searching for the frame sync code.
        /// </summary>
        FLAC__STREAM_DECODER_SEARCH_FOR_FRAME_SYNC,

        /// <summary>
        /// The decoder is ready to or is in the process of reading a frame.
        /// </summary>
        FLAC__STREAM_DECODER_READ_FRAME,

        /// <summary>
        /// The decoder has reached the end of the stream.
        /// </summary>
        FLAC__STREAM_DECODER_END_OF_STREAM,

        /// <summary>
        /// An error occurred in the underlying Ogg layer.
        /// </summary>
        FLAC__STREAM_DECODER_OGG_ERROR,

        /// <summary>
        /// An error occurred while seeking. 
        /// The decoder must be flushed with FLAC__stream_decoder_flush() 
        /// or reset with FLAC__stream_decoder_reset() before decoding can continue.
        /// </summary>
        FLAC__STREAM_DECODER_SEEK_ERROR,

        /// <summary>
        /// The decoder was aborted by the read or write callback.
        /// </summary>
        FLAC__STREAM_DECODER_ABORTED,

        /// <summary>
        /// An error occurred allocating memory.  
        /// The decoder is in an invalid state and can no longer be used.
        /// </summary>
        FLAC__STREAM_DECODER_MEMORY_ALLOCATION_ERROR,

        /// <summary>
        /// The decoder is in the uninitialized state; one of the FLAC__stream_decoder_init_*() 
        /// functions must be called before samples can be processed.
        /// </summary>
        FLAC__STREAM_DECODER_UNINITIALIZED
    }

}
