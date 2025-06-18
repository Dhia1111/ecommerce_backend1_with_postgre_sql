using ConnectionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BusinessLayer
{
    public  class clsTransaction
    {
        enum enMode{Add,Update}

        enMode _Mode;

        int _ID;
        public int ID { get { return _ID; } }

        List<DTOCartItem>? _ProductIdsList;
        DateTime _TransactionDate;
        public string PaymentMethodID { get; set; }

        public DTOTransaction.enState State { get; set; }

        public DateTime TrasactionDate { get {return _TransactionDate ; } }

        public decimal TotolePrice { get; set; }

        public int CustomerID { get; set; }

        public Guid TransactionGUID { get; set; }

        public DTOTransaction dtoTransaction { get { return new DTOTransaction(this._ID, this.PaymentMethodID, this.State, this.TotolePrice, this.CustomerID, this.TransactionGUID.ToString(),_TransactionDate); } }

         clsTransaction(DTOTransaction dto)  {
            if (dto == null) return;
            this._ID = dto.ID.Value;
            this.PaymentMethodID = dto.PaymentMethodID;
            this.State = dto.State.Value;
            this.TotolePrice = dto.TotolePrice.Value;
            this.CustomerID = dto.CustomerID.Value;
            this.TransactionGUID =Guid.TryParse(dto.TransactionGUID,out Guid GUID)?GUID:Guid.Empty ;
            this._Mode = enMode.Update;
            this._TransactionDate = DateTime.Parse(dto.TransactionDate);
        
        }


        public clsTransaction( string PaymentMethodID, DTOTransaction.enState State, decimal TotolePrice, int CustomerID, string TransactionGUID,List<DTOCartItem>?ProductIncludedIDsList)
        {
            this._ID = -1;
            this.PaymentMethodID = PaymentMethodID;
            this.State = State;
            this.TotolePrice = TotolePrice;
            this.CustomerID = CustomerID;
            this.TransactionGUID = Guid.TryParse(TransactionGUID, out Guid GUID) ? GUID : Guid.Empty;
            this._Mode = enMode.Add;
            this._ProductIdsList = ProductIncludedIDsList;
            this._TransactionDate = DateTime.Now;


        }

        async Task<bool> _Add()
        {
            bool SavingProductResult = false;

           
             this._ID= await ConnectionLayer.clsTransaction.Add(this.dtoTransaction);
            if (_ID == -1) return false;

            if (this._ProductIdsList == null || this._ProductIdsList.Count == 0) return false;

            foreach (DTOCartItem item in this._ProductIdsList)
            {

                clsIncludedProduct IncludedProduct = new clsIncludedProduct(this._ID,item.ProductID,item.NumberOfItems);

                SavingProductResult = await IncludedProduct.Save();

                if (!SavingProductResult) return false;
            }
            return true;

        }
        async Task<bool> _Update()
        {

          return await ConnectionLayer.clsTransaction.Update(this.dtoTransaction);
        }

        public static async Task<List<DTOTransaction>?> GetAll()
        {
            return await ConnectionLayer.clsTransaction.GetAll();
        }
        public static async Task<bool> Delete(int ID)
        {
            return await ConnectionLayer.clsTransaction.Delete(ID);
        }

        public static async Task<bool> DeleteAll(int CustomerID)
        {
            return await ConnectionLayer.clsTransaction.DeleteAll(CustomerID);
        }

        public async Task<bool> Save()
        {
            bool result = false;
            if (_Mode == enMode.Add)
            {
                result =await _Add();
                if(result)
                {
                    _Mode = enMode.Update;

                }
            }
            else
            {
               result= await _Update();
            }

            return result;
        }
   
        public static async Task <clsTransaction?>Find(Guid Id)
        {
          DTOTransaction ?DTO= await  ConnectionLayer.clsTransaction.Find(Id);
            return DTO != null ? new clsTransaction(DTO) : null;
        }

        public static async Task<clsTransaction?> FindFirst(int UserId)
        {
            DTOTransaction? DTO = await ConnectionLayer.clsTransaction.FindFirst(UserId);
            return DTO != null ? new clsTransaction(DTO) : null;
        }

        public static async Task<clsTransaction?> Find(int Id)
        {
            DTOTransaction? DTO = await ConnectionLayer.clsTransaction.Find(Id);
            return DTO != null ? new clsTransaction(DTO) : null;
        }




        public static async Task<Guid?> GetUnfinshedPayment(int UserId)
        {
            return await ConnectionLayer.clsTransaction.GetTransactionGuidIdFroUnfinshedPayment(UserId);
        }


        public static (string? TransactionGuid, string? PaymentStatus) ParseStripePaymentEvent(string json)
        {
            try
            {
                // Check for empty/null JSON
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new ArgumentException("JSON input cannot be null or empty");
                }

                // Parse the JSON

                var jsonObject = JObject.Parse(json);
                var DataObject = jsonObject["data"];
                var paymentObject = DataObject["object"];

                // Check if the object exists
                if (paymentObject == null)
                {
                    throw new KeyNotFoundException("'object' property not found in JSON");
                }

                // Extract TransactionGUID from metadata
                string? transactionGuid = null;
                var metadata = paymentObject["metadata"];
                if (metadata != null)
                {
                    transactionGuid = metadata["TransactionGUID"]?.ToString();

                    // Alternative case-insensitive check
                    if (transactionGuid == null)
                    {
                        transactionGuid = metadata.Children<JProperty>()
                            .FirstOrDefault(x => x.Name.Equals("TransactionGUID", StringComparison.OrdinalIgnoreCase))?
                            .Value.ToString();
                    }
                }

                // Extract payment status
                var paymentStatus = paymentObject["status"]?.ToString();

                // Validate required fields
                if (paymentStatus == null)
                {
                    throw new KeyNotFoundException("Payment status not found in JSON");
                }

                return (transactionGuid, paymentStatus);
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Invalid JSON format: {ex.Message}");
                return (null, null);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is KeyNotFoundException)
            {
                Console.WriteLine($"Data parsing error: {ex.Message}");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return (null, null);
            }
        }




        public static (string? Code, string? Message, string? PaymentMethodId) GetLastPaymentError(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return (null, null, null);
                var objJson = JObject.Parse(json);
                var Data = objJson["data"];
                var ObjectsHolder = Data["object"];
                var paymentError = ObjectsHolder["last_payment_error"];

                if (paymentError == null || paymentError.Type == JTokenType.Null)
                    return (null, null, null);

                return (
                    Code: paymentError["code"]?.ToString(),
                    Message: paymentError["message"]?.ToString(),
                    PaymentMethodId: paymentError["payment_method"]?["id"]?.ToString()
                );
            }
            catch
            {
                // Handle parsing errors silently
                return (null, null, null);
            }
        }



        public static async Task<List<string>?> getCurrecies(int CountryID)
        {
            return await ConnectionLayer.clsCurrency.GetCountryCurrecies(CountryID);
        }

        public static async Task<List<string>?> getCurrecies(string CountryName)
        {
            return await ConnectionLayer.clsCurrency.GetCountryCurrecies(CountryName);
        }


    }
}
