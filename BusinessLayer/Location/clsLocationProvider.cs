using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



  namespace BusinessLayer

{

    public interface ILocationProvider
    {

        public Task<List<string>?> PostCodes(string CountryCode, string CityName);

        public Task<List<string>?> AllCities(string CountryName);
    }




    public class clsLocationProvider : ILocationProvider
    {
        private HttpClient _httpClient;



        public clsLocationProvider(IHttpClientFactory clientFactory) {

            _httpClient = clientFactory.CreateClient("GeoClient");



        }



        public async Task<List<string>?> PostCodes(string CountryCode, string CityName)
        {

            CityName = CityName.Replace(" City", "").Trim();

            if (string.IsNullOrEmpty(CountryCode))
            {
                return null;
            }

            try
            {

                using var response = await _httpClient.GetAsync($"postalCodeSearchJSON?placename={CityName}&country={CountryCode}&maxRows=500&orderby=population&username={BussnissclsGlobale.GetgeonamesUserName()}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (!root.TryGetProperty("postalCodes", out var postalCodesProp) || postalCodesProp.ValueKind != JsonValueKind.Array)
                {
                    return null;
                }

                List<string> postCodes = new List<string>();

                foreach (var element in postalCodesProp.EnumerateArray())
                {
                    if (element.TryGetProperty("postalCode", out var postalCodeElement) && element.TryGetProperty("placeName", out var CityElement))
                    {
                        var code = postalCodeElement.GetString();
                        if (!string.IsNullOrEmpty(code)) postCodes.Add(code + "//" + CityElement);
                    }
                }

                return (postCodes);
            }
            catch (HttpRequestException httpEx)
            {
                return null;

            }
            catch (JsonException jsonEx)
            {
                return null;
            }
            catch (Exception ex)
            {

                return null;


            }

        }
    

    public async Task<List<string>?> AllCities(string CountryCode)
        {

            var client = this._httpClient;
           
            if (CountryCode == null)
            {

                return null;
            
            }
        




            try
            {


                using var response = await client.GetAsync($"searchJSON?&country={(CountryCode)}&orderby=population&featureClass=P&maxRows=900&username={BussnissclsGlobale.GetgeonamesUserName()}");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (!root.TryGetProperty("geonames", out var geonamesProp) || geonamesProp.ValueKind != JsonValueKind.Array)
                {
                    return null;

                }

                List<string>? names = new List<string>();
                foreach (var element in geonamesProp.EnumerateArray())
                {
                    if (element.TryGetProperty("toponymName", out var toponymElement))
                    {
                        var name = toponymElement.GetString();
                        if (!string.IsNullOrEmpty(name)) { names.Add(name); }
                    }
                }

                return (names);
            }
            catch (HttpRequestException httpEx)
            {
                return null;
            }
            catch (JsonException jsonEx)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;

           

            }
        } 
    } 
}
         

