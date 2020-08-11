namespace NtFreX.Audio.Encoders
{
    // TODO: encoder/decoder, converter, container, sink

    internal interface IFlacPredictor
    {
        void ReadSubFrame();
    }

    internal class FlacFrameHeader
    {
        public int Channels { get; }
    }

    /// <summary>
    /// https://xiph.org/flac/format.html
    /// </summary>
    internal class FlacDecoder
    {
        private bool DecodeFrame()
        {
            // https://github.com/xiph/flac/blob/master/src/libFLAC/stream_decoder.c#L2016

            if(!ReadFrameHeader(out var frameHeader))
            {
                return false;
            }

            // TODO: /* means we didn't sync on a valid header */

            for (var channel = 0; channel < frameHeader.Channels; channel++)
            {
                // TODO: first figure the correct bits-per-sample of the subframe

                if(!DecodeSubFrame())
                {
                    return false;
                }

                // TODO: /* means bad sync or got corruption */
            }

        }
        private bool ReadFrameHeader(out FlacFrameHeader header);
        private bool DecodeSubFrame();
    }

    /// <summary>
    /// This is essentially a zero-order predictor of the signal. 
    /// The predicted signal is zero, meaning the residual is the signal itself, and the compression is zero. 
    /// This is the baseline against which the other predictors are measured. 
    /// If you feed random data to the encoder, the verbatim predictor will probably be used for every subblock. 
    /// Since the raw signal is not actually passed through the residual coding stage (it is added to the stream 'verbatim'), 
    /// the encoding results will not be the same as a zero-order linear predictor.
    /// </summary>
    internal class FlacVerbatimPredictor : IFlacPredictor
    {
        
    }

    /// <summary>
    /// This predictor is used whenever the subblock is pure DC ("digital silence"), i.e. a constant value throughout. 
    /// The signal is run-length encoded and added to the stream.
    /// </summary>
    internal class FlacConstantPredictor : IFlacPredictor
    {

    }

    internal class FlacFixedLinearPredictor : IFlacPredictor
    {

    }

    internal class FlacFIRLinearPredictor : IFlacPredictor
    {

    }

    internal class RiceCoding
    {

    }

}
