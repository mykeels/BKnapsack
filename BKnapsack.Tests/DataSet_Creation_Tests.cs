using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BKnapsack;
using BKnapsack.Tests.Helpers;
using Newtonsoft.Json;
using BKnapsack.Tests.Models;

namespace BKnapsack.Tests
{
    [TestClass]
    public class DataSet_Creation_Tests
    {
        private const string DIR_PATH = "data";

        private List<BestResultModel> bestResults;

        private enum Stage
        {
            NoOfTests,
            Info,
            Values,
            Weights,
            TotalWeights
        }

        //[TestMethod]
        public void Test_Create_Dataset_Files()
        {
            this.getBestResults();

            int fileIndex = 0;
            int datasetIndex = 0;

            var fileDatasets = getFiles(DIR_PATH).Where(file => file.Name.EndsWith("txt")).Select(file =>
            {
                string contents = File.ReadAllText(file.FullName);
                return getDatasets(contents);
            }).ToList();

            fileDatasets.ForEach(datasets =>
            {
                fileIndex++;
                datasets.Select((dataset, index) =>
                {
                    return new
                    {
                        items = dataset.items,
                        weights = dataset.weights,
                        noOfKnapsacks = dataset.noOfKnapsacks,
                        noOfItems = dataset.noOfItems,
                        goal = bestResults.Find(result => (result.filename == "mknapcb" + fileIndex) && (result.index == (index + 1))).best
                    };
                }).ToList().ForEach(dataset =>
                {
                    datasetIndex++;
                    var fileContent = JsonConvert.SerializeObject(dataset);
                    string fileStartName = "mknapcb" + fileIndex;
                    File.WriteAllText("data/" + fileStartName + "/" + fileStartName + "-" + datasetIndex + ".json", fileContent);
                });
                datasetIndex = 0;
                Console.WriteLine($"There are {datasets.Count} datasets");
                Console.WriteLine(JsonConvert.SerializeObject(datasets.FirstOrDefault().items.Select(item => item.weights.Count)));
                Console.WriteLine(JsonConvert.SerializeObject(datasets, Formatting.Indented));
            });
        }

        private List<BestResultModel> getBestResults()
        {
            var fileContents = File.ReadAllText("data/best-results.json");
            bestResults = JsonConvert.DeserializeObject<List<BestResultModel>>(fileContents);
            return bestResults;
        }

        private Stage incrementStage(Stage stage)
        {
            if (stage == Stage.TotalWeights) return Stage.Info;
            else return (Stage)(Convert.ToInt32(stage) + 1);
        }

        private List<BinaryKnapsack> getDatasets(string contents)
        {
            var knapsacks = new List<BinaryKnapsack>();
            string[] lines = contents.Split('\n').Select(line => line.Trim()).ToArray();
            int noOfTests = 0;
            int weightsCounter = 1;
            Stage stage = Stage.NoOfTests;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (stage == Stage.NoOfTests)
                {
                    knapsacks.Add(new BinaryKnapsack());
                    noOfTests = Convert.ToInt32(line);
                    stage = incrementStage(stage);
                }
                else if (stage == Stage.Info)
                {
                    knapsacks.Last().noOfItems = Convert.ToInt32(line.Split(' ').First());
                    knapsacks.Last().noOfKnapsacks = Convert.ToInt32(line.Split(' ')[1]);
                    knapsacks.Last().goal = Convert.ToInt32(line.Split(' ').Last());
                    stage = incrementStage(stage);
                }
                else if (stage == Stage.Values)
                {
                    knapsacks.Last().items.AddRange(line.Split(' ').Select(item => new Knapsack.Item() { value = Convert.ToInt32(item), weights = new List<double>() }));
                    if (knapsacks.Last().items.Count == knapsacks.Last().noOfItems)
                    {
                        stage = incrementStage(stage);
                    }
                }
                else if (stage == Stage.Weights)
                {
                    int itemCounter = knapsacks.Last().items.FindIndex(item => item.weights.Count < weightsCounter);
                    if (itemCounter < 0)
                    {
                        itemCounter = 0;
                        weightsCounter++;
                    }
                    line.Split(' ').Select(item => Convert.ToInt32(item)).ToList().ForEach((weight) =>
                    {
                        knapsacks.Last().items[itemCounter].weights.Add(weight);
                        itemCounter++;
                    });
                    if (knapsacks.Last().items.All(item => item.weights.Count == knapsacks.Last().noOfKnapsacks))
                    {
                        stage = incrementStage(stage);
                    }
                }
                else if (stage == Stage.TotalWeights)
                {
                    knapsacks.Last().weights.AddRange(line.Split(' ').Select(item => Convert.ToDouble(item)));
                    if (knapsacks.Last().weights.Count == knapsacks.Last().noOfKnapsacks)
                    {
                        stage = incrementStage(stage);
                        weightsCounter = 0;
                        if (knapsacks.Count < noOfTests) knapsacks.Add(new BinaryKnapsack());
                    }
                }
            }
            return knapsacks;
        }

        private FileInfo[] getFiles(string dirName)
        {
            var dir = new DirectoryInfo(dirName);
            return dir.GetFiles();
        }
    }
}
