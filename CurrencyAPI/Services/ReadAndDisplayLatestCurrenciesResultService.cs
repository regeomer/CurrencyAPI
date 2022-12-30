using CurrencyAPI.Infrastracture;
using System;
using System.Collections.Generic;
using System.IO;

namespace CurrencyAPI.Services
{
    public class ReadAndDisplayLatestCurrenciesResultService : SingletonBase<ReadAndDisplayLatestCurrenciesResultService>, IServiceRunner
    {
        private const string FileName = "CurrencyInfo.txt";
        private const string DirectoryPath = @".\Currency\";
        private List<string> _results;

        public ReadAndDisplayLatestCurrenciesResultService()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            _results = new List<string>();
        }

        public int StartService()
        {
            var result = GetLatestResults();
            if (result.Equals(StatusCodes.GeneralError))
            {
                return (int)StatusCodes.GeneralError;
            }
            PrintResults();
            return (int)StatusCodes.Success;
        }

        private void PrintResults()
        {
            Console.WriteLine("\n");
            foreach (var result in _results)
            {
                Console.WriteLine(result);
            }
            Console.WriteLine("\n\n");
        }

        private StatusCodes GetLatestResults()
        {
            try
            {
                var lines = File.ReadAllLines(DirectoryPath + FileName);
                for (var i = lines.Length - 4; i < lines.Length; i++)
                {
                    _results.Add(lines[i]);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("File does not exists");
                return StatusCodes.GeneralError;
            }
            return StatusCodes.Success;
        }
    }
}
