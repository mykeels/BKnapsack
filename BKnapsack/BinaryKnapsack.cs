using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using MSearch.Common;
using MSearch.Extensions;
using MSearch;

namespace BKnapsack
{
    public class BinaryKnapsack : Knapsack
    {
        public double beta = 1.5;
        public double alpha = 0.075;
        public int populationSize = 5;
        public double[] clone(double[] sol)
        {
            double[] newSol = new double[sol.Length];
            for (int i = 0; i < sol.Length; i++)
            {
                newSol[i] = sol[i];
            }
            return newSol;
        }

        public new double[] getInitialSolution()
        {
            double[] sol = new double[noOfItems];
            while (true)
            {
                int rIndex = Convert.ToInt32(Math.Floor(Number.Rnd(sol.Length)));
                sol[rIndex] = 1;
                if (getFitness(sol) == Double.MaxValue)
                {
                    sol[rIndex] = 0;
                    return sol;
                }
            }
        }

        private new double getFitness(IEnumerable<int> solution) { return base.getFitness(solution); }

        public IEnumerable<int> toKnapsackList(double[] sol)
        {
            var ret = new List<int>();
            int i = 0;
            foreach (double item in sol)
            {
                if (Convert.ToInt32(item) == 1) yield return i;
                i++;
            }
        }

        public double getFitness(double[] sol)
        {
            return this.getFitness(toKnapsackList(sol));
        }

        public double[] mutate(double[] sol)
        {
            double[] newSol = new double[sol.Length];
            for (int i = 0; i < sol.Length; i++)
            {
                newSol[i] = Distribution.generateLevy(alpha, beta) + sol[i];
            }
            return newSol;
        }

        public new Configuration<double[]> getConfiguration()
        {
            Configuration<double[]> config = new Configuration<double[]>();
            config.cloneFunction = this.clone;
            config.initializeSolutionFunction = this.getInitialSolution;
            config.movement = Search.Direction.Divergence;
            config.mutationFunction = this.mutate;
            config.noOfIterations = 50000;
            config.objectiveFunction = this.getFitness;
            config.populationSize = populationSize;
            config.selectionFunction = Selection.RoulleteWheel;
            config.writeToConsole = true;
            config.consoleWriteInterval = 100;
            config.consoleWriteFunction = (sol, fit, noOfIterations) =>
            {
                Console.WriteLine($"{noOfIterations}\tFitness\t{fit}");
            };
            config.hardObjectiveFunction = (double[] sol) =>
            {
                if (sol.Any(i => i < 0 || i > 1)) return false;
                double fitness = this.getFitness(sol);
                return fitness < Double.MaxValue && fitness >= 0;
            };
            config.enforceHardObjective = true;
            return config;
        }

        public Configuration<double[]> getConfiguration(string filename)
        {
            if (String.IsNullOrEmpty(filename)) throw new Exception("file name not specified");
            if (!filename.ToLower().EndsWith(".json")) throw new Exception($"File: [{filename}] must match pattern [*.json]");
            string contents = System.IO.File.ReadAllText(filename);
            var config = JsonConvert.DeserializeObject<Configuration<double[]>>(contents);
            config.cloneFunction = this.clone;
            config.initializeSolutionFunction = this.getInitialSolution;
            config.movement = Search.Direction.Divergence;
            config.mutationFunction = this.mutate;
            config.objectiveFunction = this.getFitness;
            config.selectionFunction = Selection.RoulleteWheel;
            config.consoleWriteFunction = (sol, fit, noOfIterations) =>
            {
                Console.WriteLine($"{noOfIterations}\tFitness\t{fit}");
            };
            config.hardObjectiveFunction = (double[] sol) =>
            {
                if (sol.Any(i => i < 0 || i > 1)) return false;
                double fitness = this.getFitness(sol);
                return fitness < Double.MaxValue && fitness >= 0;
            };
            return config;
        }
    }
}
