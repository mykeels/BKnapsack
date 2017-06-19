using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKnapsack.Console
{
    /// <summary>
    /// Handles the command line arguments
    /// </summary>
    public class Setup
    {
        public int runs { get; set; }
        public Algorithms algorithm { get; set; }
        public string setupFile { get; set; }
        public string dataFile { get; set; }
        public string configFile { get; set; }
        public string problemName { get; set; }
        public double alpha { get; set; }
        public double beta { get; set; }
        public int populationSize { get; set; }
        /// <summary>
        /// aka pValue
        /// </summary>
        public double switchProbability { get; set; }
        public enum Algorithms
        {
            ABC,
            Flowers,
            GA,
            HillClimb,
            SA
        }

        public enum Flags
        {
            none,
            setupFile,
            dataFile,
            configFile,
            algorithm,
            runs,
            alpha,
            beta,
            pvalue,
            popSize
        }

        private Flags flag;

        public Setup()
        {
            this.runs = 1;
            this.algorithm = Algorithms.Flowers;
        }

        public Setup(string[] args)
        {
            this.runs = 1;
            this.flag = Flags.none;
            this.algorithm = Algorithms.Flowers;
            var snippets = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (this.flag == Flags.none)
                { 
                    if (arg == "-s" || arg == "-setup") this.flag = Flags.setupFile;
                    else if (arg == "-c" || arg == "-cfg" || arg == "-config") this.flag = Flags.configFile;
                    else if (arg == "-d" || arg == "-data") this.flag = Flags.dataFile;
                    else if (arg == "-algo" || arg == "-algorithm") this.flag = Flags.algorithm;
                    else if (arg == "-r" || arg == "-runs") this.flag = Flags.runs;
                    else if (arg == "-a" || arg == "-alpha") this.flag = Flags.alpha;
                    else if (arg == "-b" || arg == "-beta") this.flag = Flags.beta;
                    else if (arg == "-p" || arg == "-pvalue") this.flag = Flags.pvalue;
                    else if (arg == "-size" || arg == "-popSize") this.flag = Flags.popSize;
                    else throw new Exception(Program.Console.WriteLine($"Unknown Flag [{arg}] found", ConsoleColor.Yellow));
                }
                else if (this.flag == Flags.setupFile)
                {
                    //expects just one snippet/item
                    this.setupFile = arg;
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.configFile)
                {
                    //expects just one snippet/item
                    this.configFile = arg;
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.alpha)
                {
                    //expects just one snippet/item
                    this.alpha = Convert.ToDouble(arg);
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.beta)
                {
                    //expects just one snippet/item
                    this.beta = Convert.ToDouble(arg);
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.pvalue)
                {
                    //expects just one snippet/item
                    this.switchProbability = Convert.ToDouble(arg);
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.popSize)
                {
                    //expects just one snippet/item
                    this.populationSize = Convert.ToInt32(arg);
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.runs)
                {
                    //expects just one snippet/item
                    int snippet = 0;
                    if (Int32.TryParse(arg, out snippet)) this.runs = snippet;
                    else throw new Exception(Program.Console.WriteLine($"Invalid Runs Flag [{arg}] ... Please enter a valid integer value", ConsoleColor.Yellow));
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.algorithm)
                {
                    //expects just one snippet/item
                    int snippet = 0;
                    if (Int32.TryParse(arg, out snippet)) this.algorithm = (Algorithms)snippet;
                    else
                    {
                        throw new Exception(Program.Console.WriteLine($"Invalid Algorithm Flag [{arg}] ... Please enter a valid integer value", ConsoleColor.Yellow));
                    }
                    this.flag = Flags.none;
                }
                else if (this.flag == Flags.dataFile)
                {
                    //expects just one/two snippets/items
                    int snippet = 0;
                    if (Int32.TryParse(arg, out snippet))
                    {
                        snippets.Add(arg);
                        if (snippets.Count == 2)
                        {
                            this.dataFile = $"data/mknapcb{snippets[0]}/mknapcb{snippets[0]}-{snippets[1]}.json";
                            this.problemName = $"mknapcb{snippets[0]}-{snippets[1]}";
                            this.flag = Flags.none;
                        }
                    }
                    else
                    {
                        this.dataFile = arg;
                        this.problemName = arg?.Split('/').LastOrDefault().Split('.').FirstOrDefault();
                        this.flag = Flags.none;
                    }
                }
            }
        }

        public bool hasSetupFile() => !String.IsNullOrEmpty(this.setupFile);

        public bool hasDataFile() => !String.IsNullOrEmpty(this.dataFile);

        public bool hasConfigFile() => !String.IsNullOrEmpty(this.configFile);

        public Setup ensureFilesExist()
        {
            if (this.hasSetupFile())
            {
                if (!System.IO.File.Exists(this.setupFile))
                {
                    throw new Exception(Program.Console.WriteLine($"Setup File [{setupFile}] does not exist in [{Environment.CurrentDirectory}]", ConsoleColor.Yellow));
                }
            }
            if (this.hasDataFile())
            {
                if (!System.IO.File.Exists(this.dataFile))
                {
                    throw new Exception(Program.Console.WriteLine($"Data File [{dataFile}] does not exist in [{Environment.CurrentDirectory}]", ConsoleColor.Yellow));
                }
            }
            if (this.hasConfigFile())
            {
                if (!System.IO.File.Exists(this.configFile))
                {
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    throw new Exception(Program.Console.WriteLine($"Config File [{configFile}] does not exist in [{Environment.CurrentDirectory}]", ConsoleColor.Yellow));
                }
            }
            return this;
        }

        public static Setup Create(string[] args)
        {
            return new Setup(args);
        }
    }
}