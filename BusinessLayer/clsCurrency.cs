using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace BusinessLayer
{
    public class clsCurrency
    {
        public class clsCurrencyData
        {
            public double Amount { get; set; }
            public string Base { get; set; }
            public DateTime Date { get; set; }
            public Dictionary<string, double> Rates { get; set; }
        }


        public static float GetCurrencyExchange(string CurrencyCode, List<KeyValuePair<string,float>> ListOFCurrencies)
        {

            foreach (var e in ListOFCurrencies)
            {

                if (e.Key.ToLower() == CurrencyCode.ToLower()) { 
                
                return e.Value;
                }

                

            }
            return 0;

        }


        public static List<KeyValuePair<string, float>> GetCurrencyRates(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // Ignore case differences
                                                     // Add converters if needed (e.g., for DateOnly)
            };
            var data = JsonSerializer.Deserialize<clsCurrencyData>(json,options)
            ;
            var result = new List<KeyValuePair<string, float>>();

            foreach (var rate in data.Rates)
            {
                result.Add(new KeyValuePair<string, float>(rate.Key, (float)rate.Value));
            }

            return result;
        }


        public static clsCurrencyData GetCurrencyInfo(string json)
        {
            var data = JsonSerializer.Deserialize<clsCurrencyData>(json);
            return new clsCurrencyData
            {
                Base = data.Base,
                Date = data.Date,
                Amount = (float)data.Amount
            };
        }
    
    
    }
}

