using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class DTOValidatingEmail
{ 
    public string GUID_ID{ get; set; }
    public int  PersonID { get; set; }
    public DTOValidatingEmail(int PersonID, string GUID_ID)
    {
        this.PersonID = PersonID;
        this.GUID_ID = GUID_ID;
    }
}

namespace ConnectionLayer
{
    public static class clsValidatingEmail
    {
        public static async Task<DTOValidatingEmail?> Find(string GUID_ID)
        {


            string qery = @"select *  From ""ValidatingEmail"" where ""ValidatingEmailID""=@ValidatingEmailID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ValidatingEmailID", Guid.Parse(GUID_ID));


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOValidatingEmail ValidatingEmail = new DTOValidatingEmail(-1, "");


                                if ((int.TryParse(Reader["PersonID"].ToString(), out int PersonID) &&
                                   Reader["ValidatingEmailID"] != null))

                                {

                                    ValidatingEmail.PersonID = PersonID;
                                    ValidatingEmail.GUID_ID = Reader["ValidatingEmailID"].ToString();



                                    return ValidatingEmail;
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
        public static async Task<DTOValidatingEmail?> Find(int  PersonID)
        {


            string qery = @"select  * From ""ValidatingEmail"" where ""PersonID""=@PersonID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", PersonID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOValidatingEmail ValidatingEmail = new DTOValidatingEmail(-1, "");


                                if ((int.TryParse(Reader["PersonID"].ToString(), out int p) &&
                                   Reader["ValidatingEmailID"] != null))

                                {

                                    ValidatingEmail.PersonID = p;
                                    ValidatingEmail.GUID_ID = Reader["ValidatingEmailID"].ToString();



                                    return ValidatingEmail;
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
        public static async Task<bool> Add(DTOValidatingEmail ValidatingEmail)
        {

            string qery = @"insert into ""ValidatingEmail""(""ValidatingEmailID"",""PersonID"")
                values(@ValidatingEmailID,@PersonID)";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ValidatingEmailID", Guid.Parse(ValidatingEmail.GUID_ID));
                        command.Parameters.AddWithValue("@PersonID", ValidatingEmail.PersonID);



                        int NumberOfRowEffected = await command.ExecuteNonQueryAsync();

                        return NumberOfRowEffected > 0;

                    }

                }
            }


            catch
            {

                return false;
            }




 
 
        }
        public static async Task<bool> Delete(int PersonID)
        {

            string qery = @"Delete from  ""ValidatingEmail""  where ""PersonID""=@PersonID";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", PersonID);


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
        public static async Task<bool> Delete(string GUID_ID)
        {

            string qery = @"Delete from  ""ValidatingEmail""  where ""ValidatingEmailID""=@ValidatingEmailID";
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ValidatingEmailID", GUID_ID);


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
    }
}
