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

}

namespace ConnectionLayer
{
    public static class clsCurrency
    {


        public static async Task<List<string>?> GetCountryCurrecies(int CountryID)
        {
            List<string> CurreciesForCountry = new List<string>();

            string query = $@"
                select * from country_currencies left join currencies 
                on country_currencies.currency_id= currencies.id
                where country_currencies.country_id=@ID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ID", CountryID);

                        connection.Open();

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {


                                if (
                                    bool.TryParse(reader["is_sported"].ToString(), out bool IsSported))
                                        
                                {

                                  
                                string? CurrecyCode = reader["currency_code"]?.ToString();


                                 if(IsSported&&CurrecyCode!=null)   CurreciesForCountry.Add(CurrecyCode);
                                    else
                                    {
                                        CurreciesForCountry.Add("not sported ");
                                    }
                                }




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


        public static async Task<List<string>?> GetCountryCurrecies(string CountryName)
        {
            List<string> CurreciesForCountry = new List<string>();

            string query = $@"
                select * from country_currencies  left join ""Countries"" on 
                country_currencies.country_id=""Countries"".""CountryID"" left join currencies 
                on country_currencies.currency_id= currencies.id
                where ""Countries"".""CountryName""=@CountryName ";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@CountryName", CountryName);

                        connection.Open();

                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {


                                if (int.TryParse(reader["id"].ToString(), out int id) &&
                                    int.TryParse(reader["country_id"].ToString(), out int CountryId) &&
                                    int.TryParse(reader["currency_id"].ToString(), out int CurrencyID) &&
                                    bool.TryParse(reader["is_sported"].ToString(), out bool IsSported))

                                {

                                   
                                 string? CurrecyName = reader["currency_code"]?.ToString();


                                  if(IsSported&&CurrecyName!=null)  CurreciesForCountry.Add(CurrecyName);
                                    else
                                    {
                                        CurreciesForCountry.Add("not sported");
                                    }
                                }




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


        public static async Task<DTOCurrency?>GetCurrncyInf(string CurrencyCode)
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
