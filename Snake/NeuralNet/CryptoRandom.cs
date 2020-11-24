using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Snake.NeuralNet
{
    public class CryptoRandom
    {
        public double RandomValue { get; set; }

        public Func<float, float, double> RandomBetween = (minimum, maximum) =>
        {
            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                var seed = rng.GetHashCode();
                Random r = new Random(seed);
                return r.NextDouble() * (maximum - minimum) + minimum;
            }
        };

        public CryptoRandom()
        {
            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                var seed = rng.GetHashCode();
                Random r = new Random(seed);
                RandomValue = r.NextDouble();
            }
        }
    }
}
