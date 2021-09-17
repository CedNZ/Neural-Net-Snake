using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.NeuralNet
{
    public class Dendrite
    {
        public double Weight { get; set; }

        public Dendrite()
        {
            CryptoRandom cryptoRandom = new CryptoRandom();
            Weight = cryptoRandom.RandomBetween(-1.0f, 1.0f);
        }
    }
}
