using System.Text.Json;

namespace Ecommerce1
{
    public static class clsValidation
    {
        private static readonly HttpClient client = new HttpClient();

        private static  IHttpClientFactory _httpClientFactory ;
            public static void Insilaze(IHttpClientFactory If)
        {
            _httpClientFactory = If;
        }
        static string _GetTextUntilTheFirstSeprater(string Text)
        {
            int Index = Text.IndexOf("//");



            if (Index > 0) return Text.Substring(0, Index);
            else
            {
                return Text;
            }
        }

        static string _GetTextAftertheSeprater(string Text)
        {

            int Index = Text.IndexOf("//");



            if (Index > 0) return Text.Substring(Index + 2);
            else return Text;
        }

        public static async Task<bool> ValidateLocationAsync(string postalCode, string city, string countryCode)
        {
            var Myclient = _httpClientFactory.CreateClient("GeoClient");
            try
            {
                string username = clsGlobale.GetgeonamesUserName();
                string PlaceFromThePostCode = _GetTextAftertheSeprater(postalCode);
                postalCode = _GetTextUntilTheFirstSeprater(postalCode);
                var response = await Myclient.GetAsync($"findNearbyPostalCodesJSON?postalcode={postalCode}&placename={PlaceFromThePostCode}&country={countryCode}&username={username}");

                if (!response.IsSuccessStatusCode)
                    return false;

                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("postalCodes", out JsonElement postalCodes) && postalCodes.GetArrayLength() > 0)
                {
                    foreach (var place in postalCodes.EnumerateArray())
                    {
                        if (place.TryGetProperty("placeName", out JsonElement placeName) &&
                        (placeName.GetString())?.Equals(PlaceFromThePostCode, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            return true; // Match found
                        }
                    }
                }

                return false; // No match
            }

            catch (Exception ex)
            {
                return false;
            }

        }

       public static bool IsValidDecimal(decimal Number)
        {

            string input = Number.ToString();
            
            if (decimal.TryParse(input, out var number))
            {
                var parts = input.Split('.');
                if (parts.Length == 2)
                {
                    return parts[1].Length <= 2;
                }
                return true; // No decimal part
            }
            return false; // Not a valid decimal
        }
    }
}
