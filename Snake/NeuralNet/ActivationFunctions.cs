using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.NeuralNet
{
    public static class ActivationFunctions
    {
        public static Func<double, double> TanH = (x) => Math.Tanh(x);

        public static Func<double, double> ReLU = (x) => x / (1 + Math.Exp(-x));

        public static Func<double, double> Softplus = (x) => Math.Log(1 + Math.Exp(x));
    }
}
