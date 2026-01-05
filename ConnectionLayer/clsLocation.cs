using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DTOLocation {

    public string CountryCode {  get; set; }
    public string CountryName { get; set; }
  
   public DTOLocation(string CountryCode,string CountryName)
    {
        this.CountryCode = CountryCode;
        this.CountryName = CountryName;
    }


}




namespace ConnectionLayer
{
    public interface ILocationRepo
    {

        public Task<string?> GetCountryCode(string CountryName);
        public Task<List<DTOLocation>?> GetAllCountries();


    }

    public class clsLocation :ILocationRepo
    {


        public  async Task<string?> GetCountryCode(string countryName)
        {
            string query =@$"SELECT   ""CountryCode"" FROM ""Countries"" WHERE ""CountryName"" = @CountryName LIMIT @LIMIT";
            string? countryCode = "";

            using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", countryName);
                command.Parameters.AddWithValue("@LIMIT", 1);

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


        public  async Task<List<DTOLocation>?> GetAllCountries()
        {
            string query = @$"SELECT * FROM ""Countries""";

            List<DTOLocation> Countries = new List<DTOLocation>();

            using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {

                try
                {
                    connection.Open();
                    using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                    {

                        while (Reader.Read())
                        {
                            if (Reader["CountryName"] != null&& Reader["CountryCode"]!=null)
                            {
                                Countries.Add(new DTOLocation(Reader["CountryCode"].ToString(),Reader["CountryName"].ToString()));

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
