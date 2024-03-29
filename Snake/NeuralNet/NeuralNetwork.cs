﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private Func<double, double> _activationFunc;

        public NeuralNetwork(double learningRate, int[] layers, Func<double, double> activationFunction = null)
        {
            if (layers.Length < 2)
            {
                return;
            }

            _layers = layers;
            LearningRate = learningRate;
            Layers = new List<Layer>();

            _activationFunc = activationFunction ?? ActivationFunctions.TanH;

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

        public double ActivationFunction(double x)
        {
            return _activationFunc(x);
            //return Math.Tanh(x);
            //return x / (1 + Math.Exp(-x));
        }

        public void Mutate(float chance, float mutationStrength)
        {
            int strongMutationFactor = 10;
            foreach(var layer in Layers)
            {
                foreach(var neuron in layer.Neurons)
                {
                    double randomMutationChance = new CryptoRandom().RandomValue;
                    if(randomMutationChance < chance)
                    {
                        neuron.Bias += new CryptoRandom().RandomBetween(-mutationStrength, mutationStrength);
                    }
                    if(randomMutationChance < chance / strongMutationFactor)
                    {
                        neuron.Bias += new CryptoRandom().RandomBetween(-(mutationStrength * strongMutationFactor), (mutationStrength * strongMutationFactor));
                    }

                    foreach(var dendrite in neuron.Dendrites)
                    {
                        double randomMutationChanceDendrite = new CryptoRandom().RandomValue;
                        if(randomMutationChanceDendrite < chance)
                        {
                            dendrite.Weight += new CryptoRandom().RandomBetween(-mutationStrength, mutationStrength);
                        }
                        if(randomMutationChanceDendrite < chance / strongMutationFactor)
                        {
                            dendrite.Weight += new CryptoRandom().RandomBetween(-(mutationStrength * strongMutationFactor), (mutationStrength * strongMutationFactor));
                        }
                    }
                }
            }
        }

        public void Breed(NeuralNetwork firstParent, NeuralNetwork secondParent)
        {
            for(int l = 0; l < LayerCount; l++)
            {
                var layer = Layers[l];

                for(int n = 0; n < layer.NeuronCount; n++)
                {
                    var neuron = layer.Neurons[n];

                    for (int d = 0; d < neuron.DendriteCount; d++)
                    {
                        var dendrite = neuron.Dendrites[d];

                        dendrite.Weight = RandomBetween((float)firstParent.Layers[l].Neurons[n].Dendrites[d].Weight, (float)secondParent.Layers[l].Neurons[n].Dendrites[d].Weight);
                    }

                    neuron.Bias = RandomBetween((float)firstParent.Layers[l].Neurons[n].Bias, (float)secondParent.Layers[l].Neurons[n].Bias);
                }
            }
        }

        public float RandomBetween(float first, float second)
        {
            var min = Math.Min(first, second);
            var max = Math.Max(first, second);
            var result = _random.Next(0, 3) switch
            {
                0 => min,
                1 => max,
                _ => (float)new CryptoRandom().RandomBetween(min, max)
            };
            return result;
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
                            neuron.Value += Layers[l - 1].Neurons[np].Value * neuron.Dendrites[np].Weight;
                        }
                        neuron.Value = ActivationFunction(neuron.Value + neuron.Bias);
                    }
                }
            }

            return Layers.Last().Neurons.Select(n => n.Value).ToArray();
        }

        public void Save(string file, int generation)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{generation},{Fitness},");
            foreach(var layer in Layers)
            {
                foreach(var neuron in layer.Neurons)
                {
                    sb.Append(neuron.Bias + ",");

                    foreach(var dendrite in neuron.Dendrites)
                    {
                        sb.Append(dendrite.Weight + ",");
                    }
                }
            }
            if(!File.Exists($"{file}.csv"))
            {
                using(var stream = File.CreateText($"{file}.csv"))
                {
                    stream.Write(sb.AppendLine().ToString().TrimEnd(','));
                }                    
            }
            else
            {
                File.AppendAllText($"{file}.csv", sb.AppendLine().ToString().TrimEnd(','));
            }
        }

        public void Load(string folder, int citizen, string oldFile = "")
        {
            try
            {
                string fileName = "";
                if(!string.IsNullOrEmpty(oldFile))
                {
                    fileName = oldFile;
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                    fileName = directoryInfo.GetFiles("*.csv").OrderByDescending(x => x.LastWriteTime).First().FullName;
                }

                var lines = File.ReadAllLines(fileName).ToList();

                if (citizen >= lines.Count())
                {
                    return;
                }

                lines = lines.OrderByDescending(l => double.Parse(l.Split(',')[1])).ToList();

                int i = 0;
                foreach(var layer in Layers)
                {
                    var line = lines.Skip(citizen).First();
                    foreach(var neuron in layer.Neurons)
                    {
                        neuron.Bias = double.Parse(line.Split(',').Skip(2 + i).First());
                        i++;
                        foreach(var dendrite in neuron.Dendrites)
                        {
                            dendrite.Weight = double.Parse(line.Split(',').Skip(2 + i).First());
                            i++;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
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

        public override string ToString()
        {
            return $"Fitness: {Fitness}";
        }
    }
}
