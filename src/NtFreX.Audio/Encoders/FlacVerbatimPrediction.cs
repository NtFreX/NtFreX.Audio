using NtFreX.Audio.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

// FLAC__bitreader_read_raw_uint32 returns boolean stream.ReadUInt32Async does not! unhandled cases
namespace NtFreX.Audio.Encoders
{
    internal interface IFlacPredictor
    {
        void ReadSubFrame();
    }

    internal class FlacFrameHeader
    {
        public uint Channels { get; }
        public uint BitsPerSample { get; }
        public FlacChannelAssigment ChannelAssignment { get; }
        public uint Blocksize { get; }
        public FlacFrameNumberType NumberType { get; }
    }

    internal class FlacFrame
    {
        public FlacFrameHeader Header { get; }
    }

    internal static class Crc16
    {
        /// <summary>
        /// 16 bit CRC generator, MSB shifted first
        /// polynomial = x^16 + x^15 + x^2 + x^0
        /// init = 0
        /// </summary>
        private const ushort[][] Table = new ushort[8][256];

        public static uint Update(byte data, uint crc) => ((crc << 8) & 0xffff) ^ Table[0][(crc >> 8) ^ data];
    }

    /// <summary>
    /// bitreader
    /// </summary>
    internal class FlacStream
    {
        private uint readCrc16;
        private uint crc16Offset;
        private uint crc16Align;
        private uint consumedBits;
        private uint consumedWords;
        private uint[] buffer;
        private uint bufferIndex;

        public void ResetReadCrc16(ushort seed)
        {
            Debug.Assert(null != buffer);
            Debug.Assert((consumedBits & 7) == 0);

            readCrc16 = (uint) seed;
            crc16Offset = consumedWords;
            crc16Align = consumedBits;
        }

        public ushort GetReadCrc16()
        {
            Debug.Assert(null != buffer);

            // CRC consumed words up to here
            Crc16UpdateBlock();

            Debug.Assert((consumedBits & 7) == 0);
            Debug.Assert(crc16Align <= consumedBits);

            // CRC any tail bytes in a partially-consumed word
            if (consumedBits > 0)
            {
                uint tail = buffer[bufferIndex + consumedWords];
                for (; crc16Align < consumedBits; crc16Align += 8)
                    readCrc16 = Crc16.Update((uint)((tail >> (32 - 8 - crc16Align)) & 0xff), readCrc16);
            }
            return (ushort) readCrc16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Crc16UpdateBlock()
        {
            if (consumedWords > crc16Offset && crc16Align > 0)
                Crc16UpdateWord(buffer[bufferIndex + crc16Offset++]);

            /* Prevent OOB read due to wrap-around. */
            if (consumedWords > crc16Offset)
            {
                readCrc16 = Crc16UpdateWords32(bufferIndex + crc16Offset, consumedWords - crc16Offset, readCrc16);
            }

            crc16Offset = 0;
        }
    }

    internal class Constants
    {
        public const uint FLAC__STREAM_METADATA_APPLICATION_ID_LEN = 32; /* bits */
        public const uint FLAC__MAX_CHANNELS = 8 /* The maximum number of channels permitted by the format. */;

    }

    /// <summary>
    /// https://xiph.org/flac/format.html
    /// </summary>
    internal class FlacDecoder
    {
        private byte[] headerWarmup = new byte[2];
        private FlacDecoderState state;
        private FlacFrame frame;
        
        private FlacStream input;
        private uint metadataFilterIdsCapacity;
        private byte[] metadataFilterIds;
        private List<uint>[] output = new List<uint>[Constants.FLAC__MAX_CHANNELS];

        /// <summary>
        //// WATCHOUT: these are the aligned pointers; the real pointers that should be free()'d are residual_unaligned[] below
        /// </summary>
        private List<uint>[] residual = new List<uint>[Constants.FLAC__MAX_CHANNELS];

        /// <summary>
        /// unaligned (original) pointers to allocated data
        /// </summary>
        private List<uint>[] residualUnaligned = new List<uint>[Constants.FLAC__MAX_CHANNELS];

        public FlacDecoder()
        {
            input = new FlacStream();
            metadataFilterIdsCapacity = 16;
            metadataFilterIds = new byte[Constants.FLAC__STREAM_METADATA_APPLICATION_ID_LEN / 8 * metadataFilterIdsCapacity];

            // TODO continue below here filestream constructor is not done yet
            //for(var i = 0; i < Constants.FLAC__MAX_CHANNELS; i++)
            //{
            //    output[i] = new List<uint>();
            //    residualUnaligned[i] = residual[i] = new List<uint>();
            //}

            //decoder->private_->output_capacity = 0;
            //decoder->private_->output_channels = 0;
            //decoder->private_->has_seek_table = false;

            //for (i = 0; i < FLAC__MAX_CHANNELS; i++)
            //    FLAC__format_entropy_coding_method_partitioned_rice_contents_init(&decoder->private_->partitioned_rice_contents[i]);

            //decoder->private_->file = 0;

            //set_defaults_(decoder);

            //decoder->protected_->state = FLAC__STREAM_DECODER_UNINITIALIZED;
        }

        public void SetMd5Checking(bool value);
        public FlacStreamDecoderInitStatus InitializeFile(string file, Action writeCallback, Action metadataCallback, Action errorCallback);
        public bool ProcessUntilEndOfStream();

        private bool ReadFrame(bool doFullDecode, out bool gotAFrame)
        {
            // https://github.com/xiph/flac/blob/master/src/libFLAC/stream_decoder.c#L2016

            gotAFrame = false;

            // init the CRC
            uint frameCrc = 0;
            frameCrc = Crc16.Update(headerWarmup[0], frameCrc);
            frameCrc = Crc16.Update(headerWarmup[0], frameCrc);
            input.ResetReadCrc16((ushort) frameCrc);

            //if (!read_frame_header_(decoder))
            //    return false;

            if (state == FlacDecoderState.FLAC__STREAM_DECODER_SEARCH_FOR_FRAME_SYNC)
                return true;

            //if (!allocate_output_(decoder, decoder->private_->frame.header.blocksize, decoder->private_->frame.header.channels))
            //    return false;

            for (var channel = 0; channel < frame.Header.Channels; channel++)
            {
                // first figure the correct bits-per-sample of the subframe
                uint bps = frame.Header.BitsPerSample;
                switch (frame.Header.ChannelAssignment)
                {
                    case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_INDEPENDENT:
                        // no adjustment needed
                        break;
                    case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_LEFT_SIDE:
                        Debug.Assert(frame.Header.Channels == 2);
                        if (channel == 1)
                            bps++;
                        break;
                    case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_RIGHT_SIDE:
                        Debug.Assert(frame.Header.Channels == 2);
                        if (channel == 0)
                            bps++;
                        break;
                    case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_MID_SIDE:
                        Debug.Assert(frame.Header.Channels == 2);
                        if (channel == 1)
                            bps++;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                // now read it
                //if (!ReadSubFrame(channel, bps, doFullDecode))
                //    return false;

                if (state == FlacDecoderState.FLAC__STREAM_DECODER_SEARCH_FOR_FRAME_SYNC)
                {
                    // means bad sync or got corruption
                    return true;
                }
            }

            //if (!read_zero_padding_(decoder))
            //    return false;
            if (state == FlacDecoderState.FLAC__STREAM_DECODER_SEARCH_FOR_FRAME_SYNC)
            {
                // means bad sync or got corruption (i.e. "zero bits" were not all zeroes)
                return true;
            }

            frameCrc = input.GetReadCrc16();
            if (!input.ReadRawUInt32(out var x, FLAC__FRAME_FOOTER_CRC_LEN))
            {
                // read_callback_ sets the state for us
                return false;
            }

            if (frameCrc == x)
            {
                if (doFullDecode)
                {
                    // Undo any special channel coding
                    switch (frame.Header.ChannelAssignment)
                    {
                        case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_INDEPENDENT:
                            // do nothing
                            break;
                        case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_LEFT_SIDE:
                            Debug.Assert(frame.Header.Channels == 2);
                            for (var i = 0; i < frame.Header.Blocksize; i++)
                                output[1][i] = output[0][i] - output[1][i];
                            break;
                        case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_RIGHT_SIDE:
                            Debug.Assert(frame.Header.Channels == 2);
                            for (var i = 0; i < frame.Header.Blocksize; i++)
                                output[0][i] += output[1][i];
                            break;
                        case FlacChannelAssigment.FLAC__CHANNEL_ASSIGNMENT_MID_SIDE:
                            Debug.Assert(frame.Header.Channels == 2);
                            for (var i = 0; i < frame.Header.Blocksize; i++)
                            {
                                int mid = output[0][i];
                                int side = output[1][i];
                                mid = ((uint) mid) << 1;
                                mid |= (side & 1); /* i.e. if 'side' is odd... */
                                output[0][i] = (mid + side) >> 1;
                                output[1][i] = (mid - side) >> 1;
                            }
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }
                }
            }
            else
            {
                // Bad frame, emit error and zero the output signal
                throw new Exception("FLAC__STREAM_DECODER_ERROR_STATUS_FRAME_CRC_MISMATCH");
            }

            gotAFrame = true;

            /* we wait to update fixed_block_size until here, when we're sure we've got a proper frame and hence a correct blocksize */
            if (this.nextFixedBlockSize > 0)
                this.fixedBlockSize = this.nextFixedBlockSize;

            Debug.Assert(frame.Header.NumberType == FlacFrameNumberType.FLAC__FRAME_NUMBER_TYPE_SAMPLE_NUMBER);
            this.samplesDecoded = frame.Header.Number.SampleNumber + frame.Header.Blocksize;

            /* write it */
            if (doFullDecode)
            {
                if (write_audio_frame_to_client_(decoder, &decoder->private_->frame, (const FLAC__int32* const *)decoder->private_->output) != FLAC__STREAM_DECODER_WRITE_STATUS_CONTINUE) {
                    decoder->protected_->state = FLAC__STREAM_DECODER_ABORTED;
                    return false;
                }
            }

            state = FlacDecoderState.FLAC__STREAM_DECODER_SEARCH_FOR_FRAME_SYNC;
            return true;
        }
        private bool ReadFrameHeader(out FlacFrameHeader header)
        {
            // FLAC__ASSERT(FLAC__bitreader_is_consumed_byte_aligned(decoder->private_->input));

        }
        private async Task<bool> DecodeSubFrameAsync(Stream stream)
        {
            var magicNumber = await stream.ReadUInt32Async().ConfigureAwait(false);
            // TODO: return false when ReadUInt32 returns false

            var wastedBits = magicNumber & 1;

        }
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
