using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
public class DTOTransaction
{
   public enum enState { Pending,Failed,Succeeded}

    public int ?ID { get; set; }

    public string PaymentMethodID { get; set; }


    public enState? State { get; set; }

    public decimal? TotolePrice { get; set; }

    public int? CustomerID { get; set; }

    public string? TransactionGUID { get; set; }

    public string TransactionDate { get; set; }

    public DTOTransaction(int ID, string PaymentMethodID, enState State, decimal TotolePrice, int CustomerID, string TransactionGUID, DateTime? Date = null)
    {
        this.ID = ID;
        this.PaymentMethodID = PaymentMethodID;
        this.State = State;
        this.TotolePrice = TotolePrice;
        this.CustomerID = CustomerID;
        this.TransactionGUID = TransactionGUID;
        this.TransactionDate = Date ==null?DateTime.Now.ToLongDateString():Date.Value.ToLongDateString();
    }

}

namespace ConnectionLayer
{
    
    public static class clsTransaction

    {

        public static async Task<DTOTransaction?> Find(int ID)
        {


            string qery = @"select * From ""Transactions"" where ""TransactionID""=@TransactionID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {
                                 DTOTransaction Transaction = new DTOTransaction(-1, "", 0, 0, -1, "");

                                if (
                            int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) &&
                            byte.TryParse(Reader["TransactionState"].ToString(), out byte State) &&
                            byte.TryParse(Reader["TransactionAtherization"].ToString(), out byte Atherization) &&
                            int.TryParse(Reader["TransactionTotolePrice"].ToString(), out int TotolePrice) &&
                            int.TryParse(Reader["TransactionUserID"].ToString(), out int CustomerID) &&
                            Reader["TransactionPaymentMethodID"] != null && Reader["TransactionGUID"] != null &&
                            Reader["Date"] != null)

                                {

                                    Transaction.ID = TransactionID;
                                    Transaction.PaymentMethodID = Reader["TransactionPaymentMethodID"].ToString();
                                    Transaction.State = (DTOTransaction.enState)State;
                                    Transaction.TotolePrice = TotolePrice;
                                    Transaction.CustomerID = CustomerID;
                                    Transaction.TransactionGUID = Reader["TransactionGUID"].ToString();
                                    Transaction.TransactionDate = Reader["Date"].ToString();
                                    return Transaction;
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
        public static async Task<DTOTransaction?> Find(Guid TransactionGuidID)
        {


            string qery =@"select * From ""Transactions"" where ""TransactionGUID""=@TransactionGUID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionGUID", TransactionGuidID.ToString());


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {
                                DTOTransaction Transaction = new DTOTransaction(-1, "", 0, 0, -1, "");

                                if (
                                        int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) &&
                                        byte.TryParse(Reader["TransactionState"].ToString(), out byte State) &&
                                        int.TryParse(Reader["TransactionTotlePrice"].ToString(), out int TotolePrice) &&
                                        int.TryParse(Reader["TransactionUserID"].ToString(), out int CustomerID) &&
                                        Reader["TransactionPaymentMethodID"] != null &&
                                        Reader["TransactionGUID"] != null&& 
                                        Reader["Date"] != null)
                                        
                                {

                                    Transaction.ID = TransactionID;
                                    Transaction.PaymentMethodID = Reader["TransactionPaymentMethodID"].ToString();
                                    Transaction.State = (DTOTransaction.enState)State;
                                    Transaction.TotolePrice = TotolePrice;
                                    Transaction.CustomerID = CustomerID;
                                    Transaction.TransactionGUID = Reader["TransactionGUID"].ToString();
                                    Transaction.TransactionDate = Reader["Date"].ToString();

                                    return Transaction;
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
        public static async Task<List<DTOTransaction>?> GetAll()
        {
            string qery = @"select*From ""Transactions""";

            List<DTOTransaction> Transaction = new List<DTOTransaction>();

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
                                if (
                                      int.TryParse(Reader["TransactionID"].ToString(), out int TransactionID) &&
                                      byte.TryParse(Reader["TransactionState"].ToString(), out byte State) &&
                                      int.TryParse(Reader["TransactionTotolePrice"].ToString(), out int TotolePrice) &&
                                      int.TryParse(Reader["TransactionUserID"].ToString(), out int CustomerID) &&
                                      Reader["TransactionPaymentMethodID"] != null &&
                                      Reader["TransactionGUID"] != null && Reader["Date"]!=null)

                                {
                                    Transaction.Add(new DTOTransaction(TransactionID, Reader["TransactionPaymentMethodID"].ToString(), (DTOTransaction.enState)State,TotolePrice ,CustomerID, Reader["TransaactionGUID"].ToString(), DateTime.Parse(Reader["Date"].ToString())));

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




            return Transaction;
        }

        public static async Task<int> Add(DTOTransaction Transaction)
        {

            string qery = @"insert into ""Transactions""(""TransactionPaymentMethodID"",""TransactionState"" ,""TransactionTotlePrice"",""TransactionUserID"",""TransactionGUID"",""Date"")

          values(@TransactionPaymentMethodID,@TransactionState,@TransactionTotlePrice,@TransactionUserID,@TransactionGUID,@Date)

                    RETURNING ""TransactionID"";";
            

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionPaymentMethodID", Transaction.PaymentMethodID);
                        command.Parameters.AddWithValue("@TransactionState", (int)Transaction.State);
                        command.Parameters.AddWithValue("@TransactionTotlePrice", Transaction.TotolePrice);
                        command.Parameters.AddWithValue("@TransactionUserID", Transaction.CustomerID);
                        command.Parameters.AddWithValue("@TransactionGUID", Guid.Parse(Transaction.TransactionGUID));
                        command.Parameters.AddWithValue("@Date", DateTime.Now);




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

        public static async Task<bool> Update(DTOTransaction Transaction)
        {

            string qery = @"Update ""Transactions"" set 
                       
              ""TransactionPaymentMethodID""=  @TransactionPaymentMethodID,
              ""TransactionState""=   @TransactionState,
              ""TransactionTotolePrice""=      @TransactionTotolePrice,
              ""TransactionUserID""=      @TransactionUserID,
              ""TransactionGUID""=    @TransactionGUID,
              

where ""TransactionD""=@TransactionID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", Transaction.ID);
                        command.Parameters.AddWithValue("@TransactionUserID", Transaction.CustomerID);
                        command.Parameters.AddWithValue("@TransactionState",(int) Transaction.State);
                        command.Parameters.AddWithValue("@TransactionTotolePrice", Transaction.TotolePrice);
                        command.Parameters.AddWithValue("@TransactionGUID", Guid.Parse(Transaction.TransactionGUID));
                        command.Parameters.AddWithValue("@TransactionPaymentMehodID", Transaction.PaymentMethodID);
                       

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

            string qery = @"Delete from ""Transactions""  where ""TransactionID""=@TransactionID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@TransactionID", ID);


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
