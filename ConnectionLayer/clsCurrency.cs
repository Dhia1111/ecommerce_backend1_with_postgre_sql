using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCurrency {


    public int CountryID { get; set; }

    public int CurrencyID { get; set; }

    public bool IsSported { get; set; }
    public string? CurrecyCode { get; set; }
    public string? CurrecyName { get; set; }

    public DTOCurrency() {
        CountryID = -1
      ; CurrencyID = -1;
        IsSported = false;
        CurrecyCode = "";
        CurrecyName = "";
    }

    public DTOCurrency(int countryID, int currencyID, bool isSported, string? currecyCode, string? currecyName)
    {
        CountryID = countryID;
        CurrencyID = currencyID;
        IsSported = isSported;
        CurrecyCode = currecyCode;
        CurrecyName = currecyName;
    }

}

namespace ConnectionLayer
{
    public interface ICurrencyRepo {

        Task<List<DTOCurrency>?> GetCountryCurrencies(int countryId,bool Sported);
        Task<List<DTOCurrency>?> GetCountryCurrencies(string CountryName, bool Sported);
        Task<DTOCurrency?> GetCurrencyInfo(string currencyCode);

    }
    public   class clsCurrency: ICurrencyRepo
    {


        public   async Task<List<DTOCurrency>?> GetCountryCurrencies(int CountryID,bool SprtedCurrencies=true)
        {
            List<DTOCurrency> CurreciesForCountry = new List<DTOCurrency>();

            string query = $@"
                select * from country_currencies left join currencies 
                on country_currencies.currency_id= currencies.id
                where country_currencies.country_id=@ID and is_sported=@is_sported";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ID", CountryID);
                        command.Parameters.AddWithValue("@is_sported", SprtedCurrencies);

                        connection.Open();

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {


                                int? ID = int.Parse(reader["id"].ToString());

                                string? CurrecyName = reader["currency_name"]?.ToString();

                                string? CurrecyCode = reader["currency_code"]?.ToString();

                                CurreciesForCountry.Add(new DTOCurrency(CountryID,ID.Value,SprtedCurrencies,CurrecyCode,CurrecyName));
                                    
                                




                            }

                        }



                        return CurreciesForCountry;

                    }



                }
            }
            catch (Exception ex) {


                return null
                        ;
            }
        }

        public async Task<List<DTOCurrency>?> GetCountryCurrencies(string CountryName, bool SprtedCurrencies = true)
        {
            List<DTOCurrency> CurreciesForCountry = new List<DTOCurrency>();
            string query = $@"
                select * from country_currencies  left join ""Countries"" on 
                country_currencies.country_id=""Countries"".""CountryID"" left join currencies 
                on country_currencies.currency_id= currencies.id
                where ""Countries"".""CountryName""=@CountryName and is_sported=@is_sported ";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@CountryName", CountryName);
                        command.Parameters.AddWithValue("@is_sported", SprtedCurrencies);

                        connection.Open();

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {


                                int? ID = int.Parse(reader["id"].ToString());

                                string? CurrecyName = reader["currency_name"]?.ToString();

                                string? CurrecyCode = reader["currency_code"]?.ToString();
                                int ? CountryID = int.Parse(reader["country_id"]?.ToString());

                                CurreciesForCountry.Add(new DTOCurrency(CountryID.Value, ID.Value, SprtedCurrencies, CurrecyCode, CurrecyName));






                            }

                        }



                        return CurreciesForCountry;

                    }



                }
            }
            catch (Exception ex)
            {


                return null
                        ;
            }
        }



        public   async Task<DTOCurrency?> GetCurrencyInfo(string CurrencyCode)
        {

            DTOCurrency Currency = new DTOCurrency();
            string query = $@"
                select * from currencies  where currencies.currency_code=@CurrencyCode ";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);

                        connection.Open();

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            if (reader.Read())
                            {


                                if (int.TryParse(reader["id"].ToString(), out int id) &&
                                    reader["currency_name"].ToString()!=null &&
                                    bool.TryParse(reader["stripe_supported"].ToString(), out bool IsSported)&&
                                     reader["currency_code"].ToString()!=null
                                    )

                                {


                                    Currency.CurrencyID = id;
                                    Currency.CurrecyCode= CurrencyCode;
                                    Currency.IsSported = IsSported;
                                    Currency.CurrecyName = reader["currency_name"].ToString();




                                }
                                else
                                {
                                    return null;
                                }




                            }
                            else
                            {
                                return null;
                            }
                        }



                        return Currency;

                    }



                }
            }
            catch (Exception ex)
            {


                return null
                        ;
            }
        }

    }
}
