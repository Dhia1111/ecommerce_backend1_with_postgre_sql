using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public  class DTOIncludedProducts{

    public int ID {  get; set; }
    public int TransactionID {  get; set; }
    public int ProductID { get; set; }
    public uint NumberOfProduct { get; set; }

    public DTOIncludedProducts(int ID ,int TransactionID,int ProductID,uint NumberOfProduct)
    {
        this.ID = ID;
        this.TransactionID = TransactionID;
        this.ProductID = ProductID;
        this.NumberOfProduct = NumberOfProduct;
    }
      
    }

namespace ConnectionLayer
{
    public static class clsIncludedProducts
    {
        public static async Task<DTOIncludedProducts?> Find(int ID)
        {


            string qery =@"select * From ""IncludedProducts"" where ""IncludedProductID""=@IncludedProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOIncludedProducts IncludedProduct = new DTOIncludedProducts(-1, -1, -1,0);


                                if (int.TryParse(Reader["IncludedProductID"].ToString(), out int IncludedProductID) &&
                                    int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) &&
                                    int.TryParse(Reader["ProductID"].ToString(), out int ProductID)&&
                                     int.TryParse(Reader["NumberOfProducts"].ToString(), out int NumberOfProducts))

                                {

                                    IncludedProduct.TransactionID = TransactionID;
                                    IncludedProduct.ProductID = ProductID;
                                    IncludedProduct.ID = IncludedProductID;
                                    IncludedProduct.NumberOfProduct = (uint)NumberOfProducts;



                                    return IncludedProduct;
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

        public static async Task<List<DTOIncludedProducts>?> GetAll()
        {
            string qery = @"Select*From ""IncludedProducts"" ";

            List<DTOIncludedProducts> IncludedProudcts = new List<DTOIncludedProducts>();

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
                                if (int.TryParse(Reader["IncludedProducts"].ToString(), out int IncludedProducts) &&
                                 int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) &&
                                 int.TryParse(Reader["ProductID"].ToString(), out int ProductID) && 
                                 int.TryParse(Reader["NumberOfProducts"].ToString(), out int NumberOfProduct))
                                {

                                    IncludedProudcts.Add(new DTOIncludedProducts(IncludedProducts, TransactionID, ProductID,(uint)NumberOfProduct));
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




            return IncludedProudcts;
        }


        public static async Task<int> Add(DTOIncludedProducts IncludedProducts)
        {

            string qery = @"insert into ""IncludedProducts""(""TransactionID"",""ProductID"",""NumberOfProducts"")
                values(@TransactionID,@ProductID,@NumberOfProducts)
                    RETURNING ""IncludedProdectID"";";
            

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", IncludedProducts.TransactionID);
                        command.Parameters.AddWithValue("@ProductID", IncludedProducts.ProductID);
                        command.Parameters.AddWithValue("@NumberOfProducts", (int)IncludedProducts.NumberOfProduct);



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


        public static async Task<bool> Update(DTOIncludedProducts IncludedProduct)
        {

            string qery = @"Update ""IncludedProducts"" set 
                       
                            ""TransactionID""=  @TransactionID,
                            
                          ""ProductID""=  @ProductID,

                          ""NumberOfProducts""=  @NumberOfProducts
              
        where        
""IncludedProductID""=@IncludedProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", IncludedProduct.ID);
                        command.Parameters.AddWithValue("@TransactionID", IncludedProduct.TransactionID);
                        command.Parameters.AddWithValue("@ProductID", IncludedProduct.ProductID);
                        command.Parameters.AddWithValue("@NumberOfProducts", IncludedProduct.NumberOfProduct);


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

            string qery = @"Delete from  ""IncludedProducts""  where ""IncludedProductID""=@IncludedProductID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@IncludedProductID", ID);


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


