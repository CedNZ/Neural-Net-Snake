using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake.NeuralNet
{
    public class Neuron
    {
        public List<Dendrite> Dendrites { get; set; }
        public double Bias { get; set; }
        public double Delta { get; set; } //used for training
        public double Value { get; set; }

        public int DendriteCount => Dendrites.Count();

        public Neuron()
        {
            CryptoRandom cryptoRandom = new CryptoRandom();
            Bias = cryptoRandom.RandomBetween(-1.0f, 1.0f);

            Dendrites = new List<Dendrite>();
        }
    }
}
