using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOCartItem 
{
    
    public int? CartID {  get; set; }

    public  uint NumberOfItems {  get; set; }

    public int? UserID { get; set; }

    public int ProductID {  get; set; }
    public string? ImageURL { get; set; }

     public DTOCartItem() { 
             CartID = -1;
        this.UserID = -1;
        this.ProductID = -1;
        this.NumberOfItems = 1;
        this.ImageURL = "";
     }
    public DTOCartItem(int CartID,int UserID,int ProductID,uint NumberOfProducts)
    {
        this.CartID =CartID;
        this.UserID = UserID;
        this.ProductID = ProductID;
        this.NumberOfItems =NumberOfProducts;
        
        this.ImageURL = "";

    }
}

namespace ConnectionLayer
{

   
    public static class clsCartItem
    {
        public static async Task<DTOCartItem?> Find(int UserID, int ProductID)
        {


            string qery = @"select From ""CartItems"" where ""UserID""=@UserID and ""ProductID""=@ProductID ";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@ProductID", ProductID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOCartItem cartItem = new DTOCartItem();


                                if (int.TryParse(Reader["CartItemID"].ToString(), out int CartItemID) && int.TryParse(Reader["UserID"].ToString(), out int MyUserID) && int.TryParse(Reader["ProductID"].ToString(), out int MyProductID) && int.TryParse(Reader["Number"].ToString(), out int NumberOfItems))



                                {
                                    cartItem.CartID = CartItemID;
                                    cartItem.UserID = MyUserID;
                                    cartItem.ProductID = MyProductID;
                                    cartItem.NumberOfItems = uint.Parse(NumberOfItems.ToString());

                                    return cartItem;
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


        public static async Task<List<DTOCartItem>?> GetCart(int UserID)
        {
            string qery = @"Select*From ""CartItems"" where ""UserID""=@UserID ";

              List<DTOCartItem> Cart = new List<DTOCartItem>();
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", UserID);

                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {
                            while (Reader.Read())
                            {
                                if (int.TryParse(Reader["CartItemID"].ToString(),out int CartItemID )&& int.TryParse(Reader["ProductID"].ToString(), out int ProductID) && uint.TryParse(Reader["Number"].ToString(), out uint NumberOfProducts)) 
                                {
                                    DTOProduct? p =await ConnectionLayer.clsProduct.Find(ProductID);
                                    DTOCartItem cartItem = new DTOCartItem(CartItemID, UserID, ProductID, NumberOfProducts);
                                    Cart.Add(cartItem);
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




            return Cart;
        }
        public static async Task<int> Add(DTOCartItem CartItem)
        {
             string qery = @"insert into ""CartItems""(""UserID"",""ProductID"",""Number"",""IncludedProductDate"")
       values(@UserID,@ProductID,@Number,@IncludedProductDate)
                    RETURNING ""CartItemID"";";
             
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", CartItem.UserID);
                        command.Parameters.AddWithValue("@ProductID", CartItem.ProductID);
                        command.Parameters.AddWithValue("@Number", int.Parse(CartItem.NumberOfItems.ToString()));
                        command.Parameters.AddWithValue("@IncludedProductDate", DateTime.Now);


                        object? objCartItemID = await command.ExecuteScalarAsync();

                        if (objCartItemID != null)
                        {

                            if (int.TryParse(objCartItemID.ToString(), out int ID))
                            {
                                CartItem.CartID = ID;

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
        public static async Task<bool> Update(DTOCartItem CartItem)
        {

            string qery = @"Update ""CartItems"" set 
                       
               ""Number""=  @Number
              
        where         
 
               ""UserID""=@UserID and
               ""ProductID""=@ProductID       
                      
";



            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@Number",int.Parse(CartItem.NumberOfItems.ToString()));
                        command.Parameters.AddWithValue("@UserID", CartItem.UserID);
                        command.Parameters.AddWithValue("@ProductID", CartItem.ProductID);



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
        public static async Task<bool> Delete(int UserID,int ProductID)
        {

            string qery = @"Delete from  ""CartItem""  where ""UserID""=@UserID and ""ProductID""=@ProductID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", UserID);
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
        public static async Task<bool> DeleteAll()
        {

            string qery = @"Delete from  ""CartItem"" ";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {



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

        public static async Task<bool> ClearCart(int ID)
        {

            string qery = @"Delete from  ""CartItems""  where ""UserID""=@UserID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", ID);


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
