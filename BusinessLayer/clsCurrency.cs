using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using  BusinessLayer;






namespace BusinessLayer
{
    public interface ICurrencyService
    {
        float GetCurrencyExchange(string currencyCode, List<KeyValuePair<string, float>> listOfCurrencies);

        List<KeyValuePair<string, float>> GetCurrencyRates(string json);

        DTOCurrencyData GetCurrencyInfo(string json);

        Task<DTOCurrency?> Find(string currencyCode);

        Task<List<string>?> GetCurrencies(int countryId, bool Sported = true);

        Task<List<string>?> GetCurrencies(string countryName, bool Sported = true);

    }

    public class DTOCurrencyData
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
    public class clsCurrency:ICurrencyService
    {

        private readonly ConnectionLayer.ICurrencyRepo _currencyRepo;
        public clsCurrency(ConnectionLayer.ICurrencyRepo currencyRepo)
        {
            _currencyRepo = currencyRepo;
        }

        public  float GetCurrencyExchange(string CurrencyCode, List<KeyValuePair<string, float>> ListOFCurrencies)
        {

            foreach (var e in ListOFCurrencies)
            {

                if (e.Key.ToLower() == CurrencyCode.ToLower())
                {

                    return e.Value;
                }



            }
            return 0;

        }


        public  List<KeyValuePair<string, float>> GetCurrencyRates(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // Ignore case differences
                                                     // Add converters if needed (e.g., for DateOnly)
            };
            var data = JsonSerializer.Deserialize<DTOCurrencyData>(json, options)
            ;
            var result = new List<KeyValuePair<string, float>>();

            foreach (var rate in data.Rates)
            {
                result.Add(new KeyValuePair<string, float>(rate.Key, (float)rate.Value));
            }

            return result;
        }


        public  DTOCurrencyData GetCurrencyInfo(string json)
        {
            var data = JsonSerializer.Deserialize<DTOCurrencyData>(json);
            return new DTOCurrencyData
            {
                Base = data.Base,
                Date = data.Date,
                Amount = (float)data.Amount
            };
        }


        public  async Task<DTOCurrency?> Find(string CurrencyCode)
        {

            DTOCurrency? DTOC = await _currencyRepo.GetCurrencyInfo(CurrencyCode);

            return DTOC;
        }



        public  async Task<List<string>?> GetCurrencies(int CountryID,bool Sported=true)
        {
        
            List<DTOCurrency>?currencies = await _currencyRepo.GetCountryCurrencies(CountryID,Sported);

            if (currencies == null || currencies.Count == 0) return null;

            else return currencies.Select(c => c.CurrecyCode).ToList();
        }

        public  async Task<List<string>?> GetCurrencies(string CountryName,bool Sported=true)
        {
            List<DTOCurrency>? currencies= await _currencyRepo.GetCountryCurrencies(CountryName,Sported);
            if(currencies==null||currencies.Count==0) return null;   

          else  return currencies.Select(c => c.CurrecyCode).ToList();
        }


    }
}

