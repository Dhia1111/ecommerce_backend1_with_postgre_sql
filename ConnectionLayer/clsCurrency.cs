using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCurrency {

    public int ID { get; set; }

    public int CountryID { get; set; }

    public int CurrencyID { get; set; }

    public bool IsSported { get; set; }
    public string ? CurrecyCode {  get; set; }

    public DTOCurrency() {
        ID = -1     ;
        CountryID = -1
      ; CurrencyID = -1;
        IsSported = false;
        CurrecyCode = "";
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



    }
}
