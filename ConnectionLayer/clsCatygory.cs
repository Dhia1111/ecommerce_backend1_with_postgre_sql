

using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCatygory {

   public enum enCatigories {BestSaling=1, populer,New ,PhoneCases,Chargers,Cables,ScreenProtectors,Luxury_Leather,Tech_Accessories,EveryDay_Essentials} 
    public int ID {  get; set; }
    public string Name { get; set; }
    public DTOCatygory(int ID, string Name)
    {

        this.ID = ID;
        this.Name = Name;
    }
}

namespace ConnectionLayer
{
    
    public static class clsCatygory
    
    {

        public static async Task<DTOCatygory?> Find(int ID)
        {


            string qery = @"select*  From ""Catigories"" where ""CatygoryID""=@CatygoryID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOCatygory CatyGory = new DTOCatygory(-1,"");


                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatygoryID) ||
                                   Reader["CatygoryName"]==null))

                                {

                                    CatyGory.ID = CatygoryID;
                                    CatyGory.Name = Reader["CatygoryName"].ToString();
 


                                    return CatyGory;
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
        public static async Task<DTOCatygory?> Find(string Name)
        {

            string qery = @"select *From ""Catigories"" where ""CatygoryName""=@CatygoryName";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", Name);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOCatygory CatyGory = new DTOCatygory(-1, "");


                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatygoryID) ||
                                   Reader["CatygoryName"] == null))

                                {

                                    CatyGory.ID = CatygoryID;
                                    CatyGory.Name = Reader["CatygoryName"].ToString();



                                    return CatyGory;
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

        public static async Task<List<DTOCatygory>?> GetAll()
        {
            string qery = @"Select*From ""Catigories"" ";

            List<DTOCatygory> Catygories = new List<DTOCatygory>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {
                                if (!(int.TryParse(Reader["CatygoryID"].ToString(), out int CatyGoryID) ||
                                 Reader["CatygoryName"]!=null))
                                {
                                    Catygories.Add(new DTOCatygory(CatyGoryID, Reader["CatygoryName"].ToString()));
                                }
                                else
                                {
                                    continue;
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




            return Catygories;
        }

        public static async Task<List<DTOCatygory.enCatigories>?> GetAll(int ID)
        {
            string qery = @"Select*From ""CatigoriesManager"" where ""ProductID""=@ProductID";


            List<DTOCatygory.enCatigories> Catygories = new List<DTOCatygory.enCatigories>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductID", ID);
                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {
                                if (int.TryParse(Reader["CatigoryID"].ToString(), out int CatyGoryID))
                                {
                                    Catygories.Add((DTOCatygory.enCatigories)CatyGoryID);
                                }
                                else
                                {
                                    continue;
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




            return Catygories;
        }


        public static async Task<int> Add(DTOCatygory dTOCatygory)
        {

            string qery = @"insert into ""Catigories""(""CatygoryName"")
                values(@CatygoryName)
                    RETURNING ""CatigoryID""; ";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", dTOCatygory.Name);



                        object? objPersonID = await command.ExecuteScalarAsync();

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


        public static async Task<bool> Update(DTOCatygory Catigory)
        {

            string qery = @"Update ""Catigories"" set 
                       
               ""CatygoryName""=  @CatygoryName,
              
        where         ""CatygoryID""=@CatygoryID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatygoryName", Catigory.ID);
                        command.Parameters.AddWithValue("@CatygoryID", Catigory.Name);
                      


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

        public static async Task<bool> Delete(int ID)
        {

            string qery = @"Delete from  ""Catigories""  where ""CatigoryID""=@CatigoryID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatigoryID", ID);


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

        public static async Task<bool> DeleteAllCatigories(int ProductID)
        {

            string qery = @"Delete from  ""CatigoriesManager""  where ""ProductID""=@ProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductID", ProductID);


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


        public static async Task<int> AddCtaigoryToProduct(int ProductID,int CatigoryID)
        {

            string qery = @"insert into ""CatigoriesManager""(""ProductID"",""CatigoryID"")
                values(@ProductID,@CatigoryID)RETURNING ""CatigoriesProductManagerID"";;";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@CatigoryID", CatigoryID);
                        command.Parameters.AddWithValue("@ProductID", ProductID);



                        object? objPersonID = await command.ExecuteScalarAsync();

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



    }

}
