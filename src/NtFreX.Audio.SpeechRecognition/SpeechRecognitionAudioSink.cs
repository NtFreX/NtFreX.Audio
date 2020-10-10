using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
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
                Func<Task> taskStarter = () => IntermediateAudioContainerBuilder
                    .Build(format, data)
                    .ToFormatAsync(speechRecognizer.Format)
                    .ContinueWith(
                        async previous =>
                        {
                            var formated = await previous.ConfigureAwait(false);
                            await RecognizeSpeechAsync(formated).ConfigureAwait(false);
                        }, TaskScheduler.Default)
                    .Unwrap();

                if (previousRecognitionTask != null)
                {
                    previousRecognitionTask = previousRecognitionTask.ContinueWith(
                        async previous =>
                        {
                            await previous.ConfigureAwait(false);
                            await taskStarter.Invoke().ConfigureAwait(false);
                        }, TaskScheduler.Default).Unwrap();
                }
                else
                {
                    previousRecognitionTask = taskStarter.Invoke();
                }
            }
        }

        private async Task RecognizeSpeechAsync(IntermediateAudioContainer container)
        {
            var data = await container
                .GetAsyncEnumerator()
                .ToArrayAsync()
                .ConfigureAwait(false);

            var speech = await speechRecognizer.ContinueRecognizeAsync(data).ConfigureAwait(false);

            await OnSpeechRecognized.InvokeAsync(this, new EventArgs<string>(speech)).ConfigureAwait(false);
        }
    }
}
