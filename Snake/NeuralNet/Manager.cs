using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.NeuralNet
{
    public class Manager
    {
        private NeuralNetwork _current;
        private List<NeuralNetwork> _neuralNetworks;
        private int _populationSize;
        private int _citizen;
        private int _generation;
        private Guid _runId;
        private string _saveFile;
        private double _learningRate;
        private int[] _layers;
        private double _bestFitness;

        public Manager(int[] layers, int populationSize = 500, double learningRate = 0.5, bool loadPrevious = true, string loadFrom = @"C:\Temp\SnakeAI")
        {
            _populationSize = populationSize;
            _neuralNetworks = new List<NeuralNetwork>(populationSize);
            _learningRate = learningRate;
            _layers = layers;

            _runId = Guid.NewGuid();

            _saveFile = $@"{loadFrom}\{_runId}";

            _citizen = 0;
            _generation = 0;
            _bestFitness = 0;

            for (int i = 0; i < populationSize; i++)
            {
                _neuralNetworks.Add(new NeuralNetwork(learningRate, layers));
                if(loadPrevious)
                {
                    _neuralNetworks[i].Load(loadFrom, i);
                }
            }
            _current = _neuralNetworks.First();
        }

        public List<double> RunCurrent(List<double> nerualInputs)
        {
            return _current.Run(nerualInputs).ToList();
        }

        public void UpdateFitness(double fitness)
        {
            _current.Fitness = fitness;
            if (fitness > _bestFitness)
                _bestFitness = fitness;
        }

        public void Next()
        {
            Console.WriteLine($"Snake G:{Generation} C:{_citizen} F:{_current.Fitness}\t\tLF:{BestLastFitness}, BF:{_bestFitness}");
            _citizen++;
            if (_citizen == _populationSize)
            {
                _citizen = 0;

                _neuralNetworks = _neuralNetworks.OrderBy(nn => nn.Fitness).ToList();

                _neuralNetworks.Last().Save(_saveFile, _generation);

                var lastFitness = _neuralNetworks.Last().Fitness;

                Console.WriteLine($"Last Fitness: {lastFitness}");

                _generation++;

                var ratioImproved = _neuralNetworks[^2].Fitness / _neuralNetworks.First().Fitness;

                MutantCount = 0;
                FreshCount = 0;
                ChildCount = 0;
                CloneCount = 0;

                var freshRatio = 0.1;
                var mutantRatio = 0.3;

                mutantRatio += freshRatio;

                for (int i = 0; i < _populationSize; i++)
                {
                    if (i <= _populationSize * freshRatio)
                    {
                        _neuralNetworks[i] = new NeuralNetwork(_learningRate, _layers);
                        FreshCount++;
                    }
                    else if (i <= _populationSize * mutantRatio)
                    {
                        _neuralNetworks[i] = _neuralNetworks.Last().Clone();
                        _neuralNetworks[i].Mutate(0.1f, 0.25f);
                        MutantCount++;
                    }
                    else if (i >= _populationSize - 3)
                    {
                        _neuralNetworks[i] = _neuralNetworks[i].Clone();
                        CloneCount++;
                    }
                    else
                    {
                        _neuralNetworks[i] = new NeuralNetwork(_learningRate, _layers);
                        _neuralNetworks[i].Breed(_neuralNetworks[^1], _neuralNetworks[^2]);
                        ChildCount++;
                    }
                }
                _neuralNetworks.Last().Fitness = lastFitness;
            }
            _current = _neuralNetworks[_citizen];
        }

        public List<NeuralNetwork> NeuralNetworks => _neuralNetworks;
        public int Generation => _generation;
        public int Current => _citizen;
        public double Fitness => _current.Fitness;
        public double BestLastFitness => _neuralNetworks.Last().Fitness;

        public int MutantCount;
        public int FreshCount;
        public int ChildCount;
        public int CloneCount;
    }
}
