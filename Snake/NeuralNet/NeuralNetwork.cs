using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake.NeuralNet
{
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        private Random _random;
        public List<Layer> Layers { get; set; }
        public double LearningRate { get; set; }
        public double Fitness { get; set; }

        public int LayerCount => Layers.Count();

        private int[] _layers;

        public NeuralNetwork(double learningRate, int[] layers)
        {
            if (layers.Length < 2)
            {
                return;
            }

            _layers = layers;
            LearningRate = learningRate;
            Layers = new List<Layer>();

            for (int l = 0; l < layers.Length; l++)
            {
                Layer layer = new Layer(layers[l]);
                Layers.Add(layer);

                for(int n = 0; n < layers[l]; n++)
                {
                    layer.Neurons.Add(new Neuron());
                }

                layer.Neurons.ForEach(nn =>
                {
                    if (l == 0)
                    {
                        nn.Bias = 0;
                    }
                    else
                    {
                        for (int d = 0; d < layers[l - 1]; d++)
                        {
                            nn.Dendrites.Add(new Dendrite());
                        }
                    }
                });
            }

            _random = new Random();
        }

        public NeuralNetwork Clone()
        {
            NeuralNetwork newNet = new NeuralNetwork(LearningRate, _layers);

            for(int l = 0; l < _layers.Length; l++)
            {
                for(int n = 0; n < _layers[l]; n++)
                {
                    newNet.Layers[l].Neurons[n].Bias = this.Layers[l].Neurons[n].Bias;

                    for (int d = 0; d < newNet.Layers[l].Neurons[n].DendriteCount; d++)
                    {
                        newNet.Layers[l].Neurons[n].Dendrites[d].Weight = this.Layers[l].Neurons[n].Dendrites[d].Weight;
                    }
                }
            }

            return newNet;
        }

        public double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public void Mutate(int chance, float val) //simple mutatution algorithm, taken from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/NeuralNetwork.cs
        {
            foreach(var layer in Layers)
            {
                foreach(var neuron in layer.Neurons)
                {
                    neuron.Bias = _random.Next(chance) > 5 ? neuron.Bias += (_random.NextDouble() - val) : neuron.Bias;

                    foreach(var dendrite in neuron.Dendrites)
                    {
                        dendrite.Weight = _random.Next(chance) > 5 ? dendrite.Weight += (_random.NextDouble() - val) : dendrite.Weight;
                    }
                }
            }
        }

        public double[] Run(List<double> input)
        {
            if(input.Count != Layers.First().NeuronCount)
            {
                return null;
            }
            
            for (int l = 0; l < LayerCount; l++)
            {
                var layer = Layers[l];

                for (int n = 0; n < layer.NeuronCount; n++)
                {
                    var neuron = layer.Neurons[n];

                    if(l == 0)
                    {
                        neuron.Value = input[n];
                    }
                    else
                    {
                        neuron.Value = 0;
                        for(int np = 0; np < Layers[l - 1].NeuronCount; np++)
                        {
                            neuron.Value = neuron.Value + Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
                        }
                        neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                    }
                }
            }

            return Layers.Last().Neurons.Select(n => n.Value).ToArray();
        }

        public bool Train(List<double> input, List<double> output)
        {
            if (input.Count != Layers.First().NeuronCount || output.Count != Layers.Last().NeuronCount)
            {
                return false;
            }

            var results = Run(input);

            foreach (var neuron in Layers.Last().Neurons)
            {
                var neuronIndex = Layers.Last().Neurons.IndexOf(neuron);
                neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[neuronIndex] - neuron.Value); //???this is wrong: should be  neuron.Delta = neuron.Value * (1 - neuron.Value) *(neuron.Value-target value) apparently???

                for (int j = LayerCount - 2; j > 2; j--)
                {
                    for (int k = 0; k < Layers[j].NeuronCount; k++)
                    {
                        var n = Layers[j].Neurons[k];

                        n.Delta += n.Value * (1 - n.Value) * Layers[j + 1].Neurons[neuronIndex].Dendrites[k].Weight * Layers[j + 1].Neurons[neuronIndex].Delta;
                    }
                }
            }

            for (int i = LayerCount - 1; i > 1; i--)
            {
                for (int j = 0; j < Layers[i].NeuronCount; j++)
                {
                    var neuron = Layers[i].Neurons[j];
                    neuron.Bias = neuron.Bias + (LearningRate * neuron.Delta);

                    for (int k = 0; k < neuron.DendriteCount; k++)
                    {
                        neuron.Dendrites[k].Weight = neuron.Dendrites[k].Weight + (LearningRate * Layers[i - 1].Neurons[k].Value * neuron.Delta);
                    }
                }
            }

            return true;
        }

        public int CompareTo(NeuralNetwork obj)
        {
            return Fitness.CompareTo(obj.Fitness);
        }
    }
}
