using CurrencyAPI.Services;
using System;

namespace CurrencyAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            string userInput = "";
            //SaveCurrencyResults();
            //DisplayLatestResults();
            while (userInput != "0")
            {
                Console.WriteLine("Enter what operation to run (type 0 to exit):");
                Console.WriteLine("1. Get and save currencies to file");
                Console.WriteLine("2. Print latest saved currencies");
                userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "0":
                        Console.WriteLine("\n\nGoodbye");
                        break;
                    case "1":
                        SaveCurrencyResults();
                        break;
                    case "2":
                        DisplayLatestResults();
                        break;
                    default:
                        Console.WriteLine("Invalid input, try again\n\n");
                        break;

                }
            }

        }

        private static void DisplayLatestResults()
        {
            ReadAndDisplayLatestCurrenciesResultService displayResults = new ReadAndDisplayLatestCurrenciesResultService();
            displayResults.StartService();
        }

        private static void SaveCurrencyResults()
        {
            GetAndSaveCurrenciesResultsService getResultsService = new GetAndSaveCurrenciesResultsService();
            getResultsService.StartService();
        }
    }
}
