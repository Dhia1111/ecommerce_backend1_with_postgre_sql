using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConnectionLayer
{
    public static class clsLocation
    {


        public static async Task<string?> GetCountryCode(string countryName)
        {
            string query =@$"SELECT   ""CountryCode"" FROM ""Countries"" WHERE ""CountryName"" = @CountryName LIMIT 1";
            string? countryCode = "";

            using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", countryName);

                try
                {
                    connection.Open();
                    object? result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        countryCode = result.ToString();

                        return countryCode;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception or rethrow
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return null;
        }


        public static async Task<List<string>?> GetAllCountries()
        {
            string query = @$"SELECT ""CountryName"" FROM ""Countries""";

            List<string> Countries = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {

                try
                {
                    connection.Open();
                    using (NpgsqlDataReader Reader =await command.ExecuteReaderAsync()) {

                        while (Reader.Read()) {
                            if (Reader["CountryName"] != null) {
                                Countries.Add((Reader["CountryName"].ToString()));
                            
                            }
                        }
                    
                    }
            
                }
                catch (Exception ex)
                {
                    // Handle exception or rethrow
                    Console.WriteLine($"Error: {ex.Message}");
                    return null;
                }
            }

            return Countries;
        }


    }
}
