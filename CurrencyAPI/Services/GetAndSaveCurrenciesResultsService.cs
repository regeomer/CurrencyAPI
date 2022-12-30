using CurrencyAPI.Infrastracture;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CurrencyAPI.Services
{
    public class GetAndSaveCurrenciesResultsService : SingletonBase<GetAndSaveCurrenciesResultsService>, IServiceRunner
    {
        private Dictionary<string, string> _results;
        private List<RequestedCurrencies> _list;
        private const string endpointUri = "https://api.getgeoapi.com/v2/currency/convert?api_key=";
        private const string alternativeEndpointUri = "https://api.freecurrencyapi.com/v1/latest?apikey=";
        private const string APIKey = "5ce51dcc1d25c5d1256b0411a0a07718be71e05b";
        private const string aleternativeAPIKey = "n8l5i5nVHeVSCGuN3STVtoHDXRVLz4G2ImQjPLMt";
        private const string FileName = "CurrencyInfo.txt";
        public GetAndSaveCurrenciesResultsService()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            _results = new Dictionary<string, string>();
            _list = new List<RequestedCurrencies>();
            FillList();
        }

        private void FillList()
        {
            _list.Add(new RequestedCurrencies() { FromCurrency = "USD", ToCurrency = "ILS" });
            _list.Add(new RequestedCurrencies() { FromCurrency = "GBP", ToCurrency = "EUR" });
            _list.Add(new RequestedCurrencies() { FromCurrency = "EUR", ToCurrency = "JPY" });
            _list.Add(new RequestedCurrencies() { FromCurrency = "EUR", ToCurrency = "USD" });
        }

        private HttpResponseMessage GetCurrentCurrenciesValues(string uri)
        {
            using (var client = new HttpClient())
            {
                var endpoint = new Uri(uri);
                return client.GetAsync(endpoint).Result;
            }
        }

        private string GetResponse(string fromCurrency, string toCurrency)
        {
            var uri = endpointUri + APIKey + "&from=" + fromCurrency + "&to=" + toCurrency;
            var response = GetCurrentCurrenciesValues(uri);
            bool isConnectionOk = response == null ? false : response.IsSuccessStatusCode;
            if (!isConnectionOk) //switching source
            {
                uri = alternativeEndpointUri + aleternativeAPIKey + "&currencies=" + toCurrency + "&base_currency=" + fromCurrency;
                response = GetCurrentCurrenciesValues(uri);
                isConnectionOk = response.IsSuccessStatusCode;
            }
            if (!isConnectionOk)
            {
                Console.WriteLine("No connection available");
                return string.Empty;
            }
            return response.Content.ReadAsStringAsync().Result;
        }

        public int StartService()
        {
           foreach(var item in _list)
            {
                var response = GetResponse(item.FromCurrency, item.ToCurrency);
                if (response.Equals(string.Empty))
                    return (int)StatusCodes.NotFound;

                ExtractFields(response, item.FromCurrency, item.ToCurrency);
            }

            SaveToFile();
            Console.WriteLine("Saved successfully!");
            return (int)StatusCodes.Success;
        }

        private void SaveToFile()
        {
            string filePath = @".\Currency\" + FileName;
            string directoryPath = @".\Currency";
            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            if (!fileInfo.Exists)
            {
                fileInfo.Create();
            }
            using (StreamWriter writer = fileInfo.AppendText())
            {
                writer.WriteLine("----------------------------------------------------");
                var time = DateTime.Now.ToString();
                foreach(var kvp in _results)
                {
                    string currentLine = kvp.Key + "\t" + kvp.Value + "\t" + time;
                    writer.WriteLine(currentLine);
                }
            }
        }

        private void ExtractFields(string response, string fromCurrency, string toCurrency)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<Data>(response);
                var rate = result.Rates[toCurrency].Rate;
                _results.Add(fromCurrency + "/" + toCurrency, rate);
            }
            catch (Exception)
            {
                var result = JsonConvert.DeserializeObject<AlternativeData>(response);
                _results.Add(fromCurrency + "/" + toCurrency, result.Rates[toCurrency].ToString());

            }

        }
    }

    public partial class RequestedCurrencies
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
    }
    public partial class Rates
    {
        [JsonProperty("currency_name")]
        public string CurrencyName { get; set; }
        [JsonProperty("rate")]
        public string Rate { get; set; }
        [JsonProperty("rate_for_amount")]
        public string RateForAmount { get; set; }

    }
    public partial class Data
    {
        [JsonProperty("base_currency_code")]
        public string BaseCurrencyCode { get; set; }
        [JsonProperty("rates")]
        public Dictionary<string,Rates> Rates { get; set; }
    }
    public partial class AlternativeData
    {
        [JsonProperty("data")]
        public Dictionary<string, double> Rates { get; set; }
    }
}
