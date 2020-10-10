using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace NtFreX.Audio.SpeechRecognition
{
    public class NeuralNetwork
    {
        private Neuron[][]? neurons;

        public void Initialize(int[] layers)
        {
            _ = layers ?? throw new ArgumentNullException(nameof(layers));

            // TODO good random
            var random = new Random();
            neurons = new Neuron[layers.Length][];
            for (var layerIndex = 0; layerIndex < layers.Length; layerIndex++)
            {
                neurons[layerIndex] = new Neuron[layers[layerIndex]];
                for (var neuronIndex = 0; neuronIndex < layers[layerIndex]; neuronIndex++)
                {
                    var weightCount = layerIndex + 1 == layers.Length ? 0 : layers[layerIndex + 1];

                    neurons[layerIndex][neuronIndex] = new Neuron()
                    {
                        Weights = new double[weightCount],
                        Value = random.NextDouble(),
                        Bias = random.NextDouble(),
                    };

                    for (var weightIndex = 0; weightIndex < weightCount; weightIndex++)
                    {
                        neurons[layerIndex][neuronIndex].Weights[weightIndex] = random.NextDouble();
                    }
                }
            }
        }

        public async Task LoadFromFileAsync(string file)
        {
            using var stream = File.OpenRead(file);
            neurons = await JsonSerializer.DeserializeAsync<Neuron[][]>(stream).ConfigureAwait(false) ?? throw new Exception();
        }

        public async Task SaveToFileAsync(string file)
        {
            using var stream = File.OpenWrite(file);
            await JsonSerializer.SerializeAsync(stream, neurons).ConfigureAwait(false);
        }

        public double Cost(double[] inputs, double[] expected)
        {
            var result = Think(inputs);
            return result.Select((value, index) => System.Math.Pow(value - expected[index], 2)).Sum();
        }

        public double[] Think(double[] inputs)
        {
            _ = inputs ?? throw new ArgumentNullException(nameof(inputs));
            _ = neurons ?? throw new Exception("The network needs to be initialized first");

            var currentLayer = 0;
            for (var i = 0; i < neurons[currentLayer].Length; i++)
            {
                neurons[currentLayer][i].Value = inputs[i];
            }

            for (; currentLayer < neurons.Length - 1; currentLayer++)
            {
                // TODO: parallelize
                for (var i = 0; i < neurons[currentLayer + 1].Length; i++)
                {
                    neurons[currentLayer + 1][i].Value = Sigmoid(neurons[currentLayer].Sum(neuron => neuron.Value + neuron.Weights[i]) + neurons[currentLayer + 1][i].Bias);
                }
            }

            return neurons[neurons.Length - 1].Select(neuron => neuron.Value).ToArray();
        }

        private static double Sigmoid(double value)
        {
            return 1.0d / (1.0d + System.Math.Exp(-value));
        }
    }
}
