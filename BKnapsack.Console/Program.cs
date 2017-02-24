using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MSearch;
using MSearch.Flowers;
using MSearch.ABC;
using MSearch.HillClimb;
using MSearch.GA;

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
                knapsack.levyJumpIndex = Math.Max(setup.levyJumpIndex, knapsack.levyJumpIndex);
                if (setup.hasConfigFile()) Program.Console.WriteLine("Reading from Configuration File", ConsoleColor.Blue);
                else Program.Console.WriteLine("Configuration File Not Specified ... Using Default Configuration Instead", ConsoleColor.Blue);
                for (int i = 1; i <= setup.runs; i++)
                {
                    var config = new Configuration<double[]>();
                    if (setup.algorithm == Setup.Algorithms.Flowers)
                    {
                        Pollination<double[]> garden = new Pollination<double[]>();
                        config = setup.hasConfigFile() ? knapsack.getConfiguration(setup.configFile) : knapsack.getConfiguration();
                        garden.create(config);
                        var bestSol = garden.fullIteration();
                        saveExperiment(config, garden.getIterationSequence(), bestSol, garden.getBestFitness(), setup.problemName, setup.dataFile, knapsack.goal, i);
                        Program.Console.WriteLine($"= = = = = = = =\tCompleted Experiment Run #{i}\t= = = = = = = =", ConsoleColor.Cyan);
                    }
                }
            }
            else Program.Console.WriteLine("No Data File Specified ... Program will now exit", ConsoleColor.Yellow);
        }

        private static void saveExperiment(Configuration<double[]> config, List<double> iterations, double[] bestSol, double bestFitness, string problemName, string filePath, int goal, int run)
        {
            Program.Console.WriteLine($"End of Experiment on [{filePath}] ... Now Compiling Results", ConsoleColor.Yellow);
            var result = new
            {
                config = new {
                    enforceHardObjective = config.enforceHardObjective,
                    noOfIterations = config.noOfIterations,
                    populationSize = config.populationSize
                },
                iterations = "[" + iterations.Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b) + "]",
                bestSol = "[" + bestSol.Select(i => Convert.ToInt32(i)).Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b) + "]",
                bestFitness = bestFitness,
                problemName = problemName,
                filePath = filePath,
                goal = goal,
                run = run
            };
            System.IO.File.WriteAllText($"results/{problemName}_r{run}-file{getFileCountInFolder("results") + 1}.json", JsonConvert.SerializeObject(result, Formatting.Indented));
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
