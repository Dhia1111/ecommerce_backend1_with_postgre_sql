using ConnectionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        public  async Task<bool> Save()
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

    
    
    }
}
