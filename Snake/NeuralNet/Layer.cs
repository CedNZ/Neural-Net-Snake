using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake.NeuralNet
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        public int NeuronCount => Neurons.Count();

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }
    }
}
