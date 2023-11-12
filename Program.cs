using System;
using Demographic.FileOperations;

namespace Demographic.Exec
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // if (args.Length >= 2)
            // {
            //     // Получение данных
            //     string InitialAgeFileName = args[0];
            //     string DeathRulesFileName = args[1];
            //
            //     int startYear = 1970;
            //     int.TryParse(args[2], out startYear);
            //
            //     int endYear = 2021;
            //     int.TryParse(args[3], out endYear);
            //
            //     int populationBegin = 130000000;
            //     int.TryParse(args[4], out populationBegin);
            string InitialAgeFileName = "/Users/arinafedotova/Downloads/InitialAge.csv";
            string DeathRulesFileName = "/Users/arinafedotova/Downloads/DeathRules.csv";
            
            int startYear = 1970;
            
            int endYear = 2000;
            
            int populationBegin = 130000000;

                // Моделирование с заданными параметрами
                var engine = new Engine(InitialAgeFileName, DeathRulesFileName, startYear, endYear, populationBegin);
                Console.WriteLine("Init End");
                engine.Model();
                Console.WriteLine("Model End");
                // Сохранение результатов в файлы
                IWriteFile writing = new WriteFile();      
                writing.WriteData("YearPopulationFile.csv", "Year,Population,Men,Women", engine.YearPopulation);
                writing.WriteData("AgePopulation.csv", "Age,Population,Men,Women", engine.GroupedByAgePeople);
            
                Console.WriteLine("Моделирование завершено.");
            // }
            // else
            // {
            //     Console.WriteLine("Недостаточно аргументов командной строки.");
            // }
        }
    }
}