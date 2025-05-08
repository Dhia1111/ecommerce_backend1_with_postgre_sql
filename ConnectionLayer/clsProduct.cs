using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOProduct { 

    public int ID { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }


    public string? ImageName { get; set; }
    public string? ImageUrl { get; set; }

    public List<DTOCatygory.enCatigories> Catigories { get; set; }
    public DTOProduct(int ID, string Name, decimal Price, string Imagepath, string ImageUrl = "", List<DTOCatygory.enCatigories> catigories=null )
    {
        this.ID = ID;
        this.Name = Name;
        this.Price = Price;
        this.ImageName = Imagepath;
        this.ImageUrl =ImageUrl;
        {
            this.Catigories = catigories;
        }
    }

    public DTOProduct()
    {

        this.ID = -1;
        this.Name = "";
        this.Price = 0;
        this.ImageName = "";
        this.ImageUrl = "";
        this.Catigories =new List<DTOCatygory.enCatigories>(10);


    }
}

namespace ConnectionLayer
{
    public static class clsProduct

    {

        public static async Task<DTOProduct?> Find(int ID)
        {


            string qery = @"select*  From ""Products"" where ""ProductID""=@ProductID";

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

                            if (Reader.Read())
                            {


                                DTOProduct Product = new DTOProduct(-1,  "", 0, "");


                                if (
                                    int.TryParse(Reader["ProductID"].ToString(), out int ProductID) &&
                                    Reader["ProductName"] != null &&
                                   decimal.TryParse(Reader["ProductPrice"].ToString(),out decimal Price) &&
                                    Reader["ProductImagePath"] != null 

                                    
                                    )
                                {

                                    Product.ID = ProductID;
                                    Product.Price = Price;
                                    Product.Name = Reader["ProductName"].ToString();
                                    Product.ImageName = Reader["ProductImagePath"].ToString();
                                 
                                    return Product;
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



        public static async Task<List<DTOProduct>?> GetAll()
        {
            string qery = @"select*from ""Products""";

            List<DTOProduct> Products = new List<DTOProduct>();

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

                                if ((
                             int.TryParse(Reader["ProductID"].ToString(), out int ProductID) &&
                             Reader["ProductName"] != null &&
                            decimal.TryParse(Reader["ProductPrice"].ToString(), out decimal Price) &&
                             Reader["ProductImagePath"] != null
 
                             )
                             )
                                {
                                    DTOProduct Product = new DTOProduct(-1, "", 0, "");
                                    Product.ID = ProductID;
                                    Product.Price = Price;
                                    Product.Name = Reader["ProductName"].ToString();
                                    Product.ImageName = Reader["ProductImagePath"].ToString();

                                    Products.Add(Product);
                                 }

                                else continue;




                            }

                        }


                    }

                }
            }


            catch
            {

                return null;
            }




            return Products;
        }

        public static async Task<List<DTOProduct>?> GetAllForCatigory(DTOCatygory.enCatigories Catigory)
        {
            string qery = @"select * from ""Products"" join ""CatigoriesManager"" on ""CatigoriesManager"".""ProductID""=""Products"".""ProductID"" where ""CatigoriesManager"".""CatigoryID""=@CatigoryID";

 
            List<DTOProduct> Products = new List<DTOProduct>();

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {
                        command.Parameters.AddWithValue("@CatigoryID", (int)Catigory);
                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {

                                if ((
                             int.TryParse(Reader["ProductID"].ToString(), out int ProductID) &&
                             Reader["ProductName"] != null &&
                            decimal.TryParse(Reader["ProductPrice"].ToString(), out decimal Price) &&
                             Reader["ProductImagePath"] != null

                             )
                             )
                                {
                                    DTOProduct Product = new DTOProduct(-1, "", 0, "");
                                    Product.ID = ProductID;
                                    Product.Price = Price;
                                    Product.Name = Reader["ProductName"].ToString();
                                    Product.ImageName = Reader["ProductImagePath"].ToString();

                                    Products.Add(Product);
                                }

                                else continue;




                            }

                        }


                    }

                }
            }


            catch
            {

                return null;
            }




            return Products;

        }

        public static async Task<int> Add(DTOProduct Product)
        {

            string qery = @"insert into ""Products""(""ProductPrice"",""ProductImagePath"" ,""ProductName"")
                values(@ProductPrice,@ProductImagePath,@ProductName)
                    RETURNING ""ProductID"";";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductPrice", Product.Price);
                        command.Parameters.AddWithValue("@ProductImagePath", Product.ImageName);
                        command.Parameters.AddWithValue("@ProductName", Product.Name);
                      

                        object? objPersonID = await command.ExecuteScalarAsync();

                        if (objPersonID != null)
                        {

                            if (int.TryParse(objPersonID.ToString(), out int ID))
                            {
                                Product.ID = ID;
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



        public static async Task<bool> Update(DTOProduct Product)
        {

            string qery = @"Update ""Products"" set 
                       
              ""ProductPrice""=     @ProductPrice,
              ""ProductImagePath""= @ProductImagePath,
              ""ProductName""      =      @ProductName
  
         where ""ProductID""=@ProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductPrice", Product.Price);
                        command.Parameters.AddWithValue("@ProductImagePath", Product.ImageName);
                        command.Parameters.AddWithValue("@ProductName", Product.Name);
                        command.Parameters.AddWithValue("@ProductID", Product.ID);
                   

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
             

            string qery = @"Delete from ""Products""  where ""ProductID""=@ProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@ProductID", ID);


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
