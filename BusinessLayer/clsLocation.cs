using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsLocation
    {

        public static async Task<string?> GetCountryCode(string CountryName) {

            return await ConnectionLayer.clsLocation.GetCountryCode(CountryName);
        }

        public async static Task<List<string>?> GetAllCountries()
        {
            return await ConnectionLayer.clsLocation.GetAllCountries();
        }

        public static string ExtractPostCodeFromPostCodeAndLocation(string PostCodeAndLocation)
        {
            int Index = PostCodeAndLocation.IndexOf("//");



            if (Index > 0) return PostCodeAndLocation.Substring(0, Index);
            else
            {
                return PostCodeAndLocation;
            }
        }

    }
}
