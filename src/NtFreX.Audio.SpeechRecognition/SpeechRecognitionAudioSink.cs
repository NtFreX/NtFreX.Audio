using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System.Threading.Tasks;

namespace NtFreX.Audio.SpeechRecognition
{
    public class SpeechRecognitionAudioSink : IAudioSink
    {
        private readonly IAudioFormat format;
        private readonly IAudioSink sink;
        private readonly object objectToLock = new object();
        private readonly SpeechRecognizer speechRecognizer;

        private Task? previousRecognitionTask;

        public Observable<EventArgs<string>> OnSpeechRecognized { get; } = new Observable<EventArgs<string>>();

        private SpeechRecognitionAudioSink(IAudioFormat format, SpeechRecognizer speechRecognizer, IAudioSink sink)
        {
            this.format = format;
            this.sink = sink;
            this.speechRecognizer = speechRecognizer;
        }

        public static async Task<SpeechRecognitionAudioSink> InitializeAsync(IAudioFormat format, string modelPath, IAudioSink sink = null)
        {
            var speechRecognizer = await SpeechRecognizer.InitializeFromModelAsync(modelPath).ConfigureAwait(false);
            return new SpeechRecognitionAudioSink(format, speechRecognizer, sink ?? new VoidAudioSink());
        }

        public static SpeechRecognitionAudioSink Initialize(IAudioFormat format, IAudioSink sink = null)
        {
            var speechRecognizer = SpeechRecognizer.Initialize();
            return new SpeechRecognitionAudioSink(format, speechRecognizer, sink ?? new VoidAudioSink());
        }

        public void DataReceived(byte[] data)
        {
            sink.DataReceived(data);

            lock (objectToLock)
            {
                if (previousRecognitionTask == null)
                {
                    previousRecognitionTask = RecognizeSpeechAsync(data);
                }
                else
                {
                    previousRecognitionTask = previousRecognitionTask.ContinueWith(
                        async previous =>
                        {
                            await previous.ConfigureAwait(false);
                            await RecognizeSpeechAsync(data).ConfigureAwait(false);
                        }, TaskScheduler.Default).Unwrap();
                }
            }
        }

        private async Task RecognizeSpeechAsync(byte[] data)
        {
            var container = await IntermediateAudioContainerBuilder
                .Build(format, data)
                .ToFormatAsync(speechRecognizer.Format)
                .ConfigureAwait(false);

            var samples = await container
                .GetAsyncEnumerator()
                .ToArrayAsync()
                .ConfigureAwait(false);

            var speech = await speechRecognizer
                .ContinueRecognizeAsync(samples)
                .ConfigureAwait(false);

            await OnSpeechRecognized
                .InvokeAsync(this, new EventArgs<string>(speech))
                .ConfigureAwait(false);
        }
    }
}
