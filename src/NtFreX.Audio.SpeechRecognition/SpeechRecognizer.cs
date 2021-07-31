using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NtFreX.Audio.SpeechRecognition
{
    // one to build words and one to build sentences
    public class StatisticalModel
    {
        public static State EmptyRootState { get; } = new State()
        {
            Value = string.Empty,
            NextStates = Array.Empty<State>()
        };

        private readonly State rootState;

        private State[] currentStates;
        private float[] currentProbabilities;

        private StatisticalModel(State rootState)
        {
            this.rootState = rootState;
        }

        public static StatisticalModel Initialize()
            => new StatisticalModel(EmptyRootState);

        public static async Task<StatisticalModel> InitializeFromFileAsync(string path)
        {
            using var stream = File.OpenRead(path);
            var rootState = await JsonSerializer.DeserializeAsync<State>(stream).ConfigureAwait(false);
            return new StatisticalModel(rootState ?? throw new Exception($"Loading the root state from a '{path}' failed"));
        }

    }

    public class State
    {
        // TODO: replace string with int which represents a string in the background to make it more generic
        public string Value { get; set; }
        public float[] NextStateProbabilities { get; set; }
        public State[] NextStates { get; set; }
    }

    public class SpeechRecognizer
    {
        public IAudioFormat Format { get; } = new AudioFormat(WellKnownSampleRate.Hz48000, 32, 1, AudioFormatType.IeeFloat);
        public NeuralNetwork NeuralNetwork { get; } = new NeuralNetwork();

        public const int MilisecondsPerAnalysis = 20;

        /*  sounds???
         *  Last layer letter???
         *  words???
         */
        public int[] Layers => new[] {
            /* values in fft */ SamplesPerAnalysis,
            SamplesPerAnalysis / 10,
            SamplesPerAnalysis / 100,
            /* letters in alphabet + 1 (for slience) */ 27
        };

        public int SamplesPerAnalysis => (int)(Format.SampleRate / 1000 * MilisecondsPerAnalysis * Format.Channels);

        private SpeechRecognizer() { }

        public static SpeechRecognizer Initialize()
        {
            var speechRecognizer = new SpeechRecognizer();
            speechRecognizer.NeuralNetwork.Initialize(speechRecognizer.Layers);
            return speechRecognizer;
        }

        public static async Task<SpeechRecognizer> InitializeFromModelAsync(string modelPath)
        {
            var speechRecognizer = new SpeechRecognizer();
            await speechRecognizer.NeuralNetwork.LoadFromFileAsync(modelPath).ConfigureAwait(false);
            return speechRecognizer;
        }

        public async Task TrainAsync(Sample[] inputs, char expected)
        {
            // TODO gradiant descent, backpropagation
            _ = inputs ?? throw new ArgumentNullException(nameof(inputs));

            /*var networks = new List<(NeuralNetwork Network, float Cost)>();
            for (var i = 0; i < inputs.Length; i++)
            {
                var networkToTrain = new NeuralNetwork();
                networkToTrain.Initialize(Layers);

                var preparedData = PrepareData(inputs[i]).ToArray();
                var cost = 0f;
                foreach (var data in preparedData)
                {
                    cost += networkToTrain.Cost();
                }
                cost = cost / preparedData.Length;
                networks.Add((networkToTrain, cost));
            }

            var bestNetwork = networks.OrderByDescending(pair => pair.Cost).First();
            await bestNetwork.Network.SaveToFileAsync(path).ConfigureAwait(false);*/
        }

        private double[] FromLetter(char? letter)
        {
            var data = new double[27];
            data[letter == null ? 0 : letter.Value - 64] = 1.0d;
            return data;
        }

        private char? ToLetter(double[] output)
        {
            var max = output.Select((value, index) => (Value: value, Index: index)).OrderBy(pair => pair.Value).First();
            if (max.Index > 0)
            {
                return (char)(max.Index + 64);
            }
            return null;
        }

        private double[] PrepareData(Sample[] samples)
        {
            var complex = samples.Select(sample => new Complex(sample.Value, 0)).ToArray();
            return FourierTransform.Fast(complex).Select(x => x.Magnitude).Select(value => value).ToArray();
        }

        private readonly List<Sample> cache = new List<Sample>();

        public Task<string> ContinueRecognizeAsync(Sample[] samples)
        {
            _ = samples ?? throw new ArgumentNullException(nameof(samples));

            cache.AddRange(samples);

            var textBuilder = new StringBuilder();
            while (cache.Count >= SamplesPerAnalysis)
            {
                var nextBatch = cache.Take(SamplesPerAnalysis).ToArray();
                cache.RemoveRange(0, SamplesPerAnalysis);

                var result = NeuralNetwork.Think(PrepareData(nextBatch.ToArray()));
                var letter = ToLetter(result);
                if (letter != null)
                {
                    textBuilder.Append(letter.Value);
                }

                // 0) fourier transform as basis
                // 1) splitt by silence/breaks (or just by a fixed size?)
                // 2) classify letters/phonetics (simple image classifier?)
                // 3) probabilistic memory of sequence of occurrence for words
                // 4) probabilistic memory of sequence of occurrence for sentences
            }
            return Task.FromResult(textBuilder.ToString());
        }
    }
}
