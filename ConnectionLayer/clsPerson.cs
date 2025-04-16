using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.ComponentModel.DataAnnotations;




public class DTOPerson
{


     public int PersonID { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string Phone { get; set; }
    public string Country { get; set; }
    public string PostCode { get; set; }
    public string  City { get; set; }


    public DTOPerson(int personID, string firstName, string lastName, string email, string phone, string Country,string City,string PostCode)
    {
        PersonID = personID;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        this.Country = Country;
        this.City = City;
        this.PostCode = PostCode;
    }
}
 

namespace ConnectionLayer
{
  static  class  clsConnectionGenral
    {

         public static string ConnectionString  = "";
        private static IConfiguration _configuration;
        static clsConnectionGenral()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();

            _configuration = builder.Build();

            var st = _configuration["ConnectionSetting:Defualt"];
           if (!string.IsNullOrEmpty(st)) ConnectionString =st ;
        }
    }
    public static class clsPerson
    {

        public static async Task<DTOPerson?> Find(int ID)
        {

           
            string qery = @"select * From ""People"" where ""PersonID""=@PersonID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOPerson Person = new DTOPerson(-1, "", "", "", "", "", "", "");

                               
                                if((int.TryParse(Reader["PersonID"].ToString(),out int PersonID) && Reader["FirstName"] != null && Reader["LastName"] != null || Reader["Email"] != null || Reader["Phone"] != null || Reader["Country"] != null
                                    && Reader["City"] != null && Reader["PostCode"] != null))
                                {

                                    Person.PersonID = PersonID;
                                    Person.FirstName = Reader["FirstName"].ToString();
                                    Person.LastName = Reader["LastName"].ToString();
                                    Person.Email = Reader["Email"].ToString();
                                    Person.Phone = Reader["Phone"].ToString();
                                    Person.Country = Reader["Country"].ToString();
                                    Person.City = Reader["City"].ToString();
                                    Person.PostCode = Reader["PostCode"].ToString();

                                    return Person;
                                }


 



                            }

                        }


                    }

                }
            }


            catch 
            {

                return null;
            }


            return null;



        }



        public static async Task<List<DTOPerson>?> GetPeoPle()
        {
            string qery = @"select*From ""People""";

            List<DTOPerson> people = new List<DTOPerson>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        using (NpgsqlDataReader Reader =await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {

                                if ((int.TryParse(Reader["PersonID"].ToString(), out int PersonID) || Reader["FirstName"] == null || Reader["LastName"] == null || Reader["Email"] == null || Reader["Phone"] == null || Reader["Country"] == null
                                                               || Reader["City"] == null || Reader["PostCode"] == null))
                                {
                                    continue;
                                }


                           else    people.Add(new DTOPerson(PersonID, Reader["FirstName"].ToString(), Reader["LastName"].ToString(), Reader["Email"].ToString(), Reader["Phone"].ToString(), Reader["Country"].ToString(), Reader["City"].ToString(), Reader["PostCode"].ToString()));





                            }

                        }


                    }

                }
            }


            catch 
            {

                return null;
            }




            return people;
        }


        public static async Task<int> AddPerson(DTOPerson person)
        {

            string qery = @"insert into ""People""(""FirstName"",""LastName"" ,""Email"",""Phone"",""Country"",""City"",""PostCode"")
                values(@FirstName,@LastName,@Email,@Phone,@Country,@City,@PostCode)
                    RETURNING ""PersonID"";";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@FirstName", person.FirstName);
                        command.Parameters.AddWithValue("@LastName", person.LastName);
                        command.Parameters.AddWithValue("@Email", person.Email);
                        command.Parameters.AddWithValue("@Phone", person.Phone);
                        command.Parameters.AddWithValue("@Country", person.Country); 
                        command.Parameters.AddWithValue("@City", person.City);
                        command.Parameters.AddWithValue("@PostCode", person.PostCode);



                        object? objPersonID =await command.ExecuteScalarAsync();

                        if (objPersonID != null)
                        {

                            if (int.TryParse(objPersonID.ToString(), out int ID))
                            {
                                return ID;
                            }
                            else
                            {
                                return -1;
                            }
                        }


                    }

                }
            }


            catch 
            {

                return -1;
            }




            return -1;

        }



        public static async Task<bool> UpdatePerson(DTOPerson Person)
        {

            string qery = @"Update ""People"" set 
                       
               ""FirstName""=  @FirstName,
              ""LastName""=  @LastName,
              ""Email""=  @Email,
              ""Phone""=  @Phone,
              ""Country""=  @Country,
              ""City""=  @City,
              ""PostCode""=  @PostCode

where ""PersonID""=@PersonID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@FirstName", Person.FirstName);
                        command.Parameters.AddWithValue("@LastName", Person.LastName);
                        command.Parameters.AddWithValue("@Email", Person.Email);
                        command.Parameters.AddWithValue("@Phone", Person.Phone);
                        command.Parameters.AddWithValue("@Country", Person.Country);
                        command.Parameters.AddWithValue("@City", Person.City);
                        command.Parameters.AddWithValue("@PostCode", Person.PostCode);
                        command.Parameters.AddWithValue("@PersonID", Person.PersonID);


                        int NumberRowAffected =await command.ExecuteNonQueryAsync();

                        if (NumberRowAffected == 0)
                        {

                            return false;

                        }


                    }

                }
            }


            catch 
            {

                return false;
            }




            return true;

        }

        public static async Task<bool> Delete(int ID)
        {

            string qery = @"Delete from ""People""  where ""PersonID""=@PersonID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", ID);


                        int NumberRowAffected = await command.ExecuteNonQueryAsync();

                        if (NumberRowAffected == 0)
                        {

                            return false;

                        }


                    }

                }
            }


            catch 
            {

                return false;
            }




            return true;

        }


        public static async Task<string?> GetCountryCode(string countryName)
        {
            string query =@"select    ""CountryCode"" from ""Countries"" WHERE ""CountryName"" = @CountryName";
            string ?countryCode = "";

            using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", countryName);

                try
                {
                    connection.Open();
                    object? result =await command.ExecuteScalarAsync();

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


    }
}