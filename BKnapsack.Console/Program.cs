using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using MSearch;
using MSearch.Flowers;
using MSearch.Pigeons;
using MSearch.Extensions;

namespace BKnapsack.Console
{
    public partial class Program
    {
        private static BinaryKnapsack knapsack;
        private static List<Setup> setups;
        public static void Main(string[] args)
        {
            createNecessaryFolders();
            knapsack = new BinaryKnapsack();
            setups = new List<Setup>();
            if (args.Any())
            {
                var setup = Setup.Create(args).ensureFilesExist();
                if (setup.hasSetupFile())
                {
                    foreach (string line in System.IO.File.ReadLines(setup.setupFile)) executeExperiment(Setup.Create(line.Split(' ')).ensureFilesExist());
                }
                else executeExperiment(setup);
            }
            else Program.Console.WriteLine("You have not specifed any FLAGS ... Press ENTER to exit!", ConsoleColor.Yellow);
            Console.Read();
        }

        private static void executeExperiment(Setup setup)
        {
            if (setup.hasDataFile())
            {
                knapsack.Load(setup.dataFile);
                if (setup.alpha > 0) knapsack.alpha = setup.alpha;
                if (setup.beta > 0) knapsack.beta = setup.beta;
                if (setup.populationSize > 0) knapsack.populationSize = setup.populationSize;
                if (setup.hasConfigFile()) Program.Console.WriteLine("Reading from Configuration File", ConsoleColor.Blue);
                else Program.Console.WriteLine("Configuration File Not Specified ... Using Default Configuration Instead", ConsoleColor.Blue);
                for (int i = 1; i <= setup.runs; i++)
                {
                    var config = new Configuration<double[]>();
                    if (setup.algorithm == Setup.Algorithms.Flowers)
                    {
                        Console.WriteLine($"Algorithm: Flower Pollination Algorithm");
                        Console.WriteLine($"\nAlpha:\t{knapsack.alpha}", ConsoleColor.DarkYellow);
                        Console.WriteLine($"Beta:\t{knapsack.beta}", ConsoleColor.DarkYellow);
                        Console.WriteLine($"Size:\t{knapsack.populationSize}\n", ConsoleColor.DarkYellow);
                        Pollination<double[]> garden = new Pollination<double[]>();
                        config = setup.hasConfigFile() ? knapsack.getConfiguration(setup.configFile) : knapsack.getConfiguration();
                        if (setup.noOfIterations > 0) config.noOfIterations = setup.noOfIterations;
                        garden.create(config, setup.switchProbability);
                        var bestSol = garden.fullIteration();
                        saveExperiment(config, garden.getIterationSequence(), bestSol, garden.getBestFitness(), setup.problemName, setup.dataFile, setup.algorithm.ToString(), knapsack.goal, i);
                        Program.Console.WriteLine($"= = = = = = = =\tCompleted Experiment Run #{i}\t= = = = = = = =", ConsoleColor.Cyan);
                    }
                    else if (setup.algorithm == Setup.Algorithms.BinaryPigeon)
                    {
                        Console.WriteLine($"Algorithm: Pigeon Inspired Algorithm");
                        Console.WriteLine($"Size:\t{knapsack.populationSize}\n", ConsoleColor.DarkYellow);
                        if (setup.tableSize == 0)
                        {
                            Console.WriteLine($"Warning: LAHC Table Size Not Set ... Defaulting to 1");
                            setup.tableSize = 1;
                        }
                        BinaryPIO<double> pio = new BinaryPIO<double>();
                        pio._switchProbability = setup.switchProbability;
                        config = setup.hasConfigFile() ? knapsack.getConfiguration(setup.configFile) : knapsack.getConfiguration();
                        config.populationSize = setup.populationSize; //200
                        config.tableSize = setup.tableSize; //150
                        if (setup.noOfIterations > 0) config.noOfIterations = setup.noOfIterations;
                        config.mutationFunction = (double[] sol) =>
                        {
                            double[] velocity = BinaryPigeon<double>.updateVelocity(Number.Rnd() <= setup.switchProbability ? pio.getLocalBestSolution() : pio.getGlobalBestSolution(), sol, pio.current.velocity, 
                                setup.mapFactor, config.noOfIterations);
                            return BinaryPigeon<double>.updateLocation(sol, velocity);
                        };
                        pio.create(config);
                        var bestSol = pio.fullIteration();
                        saveExperiment(config, pio.getIterationSequence(), bestSol, pio.getBestFitness(), setup.problemName, setup.dataFile, setup.algorithm.ToString() + "_m" + setup.mapFactor, knapsack.goal, i);
                        Program.Console.WriteLine($"= = = = = = = =\tCompleted Experiment Run #{i}\t= = = = = = = =", ConsoleColor.Cyan);
                    }
                }
            }
            else Program.Console.WriteLine("No Data File Specified ... Program will now exit", ConsoleColor.Yellow);
        }

        private static void saveExperiment(Configuration<double[]> config, List<double> iterations, double[] bestSol, double bestFitness, string problemName, string filePath, string algorithm, int goal, int run)
        {
            Program.Console.WriteLine($"End of Experiment on [{filePath}] ... Now Compiling Results", ConsoleColor.Yellow);
            var result = new
            {
                config = new {
                    enforceHardObjective = config.enforceHardObjective,
                    noOfIterations = config.noOfIterations,
                    populationSize = config.populationSize,
                    tableSize = config.tableSize,
                    movement = config.movement.ToString()
                },
                iterations = "[" + iterations.Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b) + "]",
                bestSol = "[" + bestSol.Select(i => Convert.ToInt32(i)).Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b) + "]",
                bestFitness = bestFitness,
                problemName = problemName,
                filePath = filePath,
                goal = goal,
                run = run
            };
            System.IO.File.WriteAllText($"results/{problemName}_{algorithm}_r{run}-file{getFileCountInFolder("results") + 1}.json", JsonConvert.SerializeObject(result, Formatting.Indented));
        }
        
        private static int getFileCountInFolder(string folderPath)
        {
            DirectoryInfo dr = new DirectoryInfo(folderPath);
            return dr.GetFiles().Count();
        }

        private static void createNecessaryFolders()
        {
            createFolder("data");
            createFolder("config");
            createFolder("setup");
            createFolder("results");
        }

        private static void createFolder(string folderName)
        {
            if (!System.IO.Directory.Exists(folderName))
            {
                System.IO.Directory.CreateDirectory(folderName);
            }
        }
    }
}
