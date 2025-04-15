using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    internal class clsIncludedProduct
    {
        enum enMode { Add, Update }
        enMode _Mode;
        int _ID;
        public int ID { get; set; }
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public uint NumberOfProduct { get; set; }
        public DTOIncludedProducts DTOIncludedProduct { get { return new DTOIncludedProducts(this.ID, this.TransactionID, this.ProductID,this.NumberOfProduct); } }

        public clsIncludedProduct(int TransactionID, int ProductID,uint NumberOfProducts)
        {
            this.TransactionID=TransactionID;
            this.NumberOfProduct=NumberOfProducts;
            this.ProductID=ProductID;
        }

         clsIncludedProduct(int ID, int TransactionID, int ProductID,uint NumberOfProducts)
        {
            this.ID = ID;
            this.TransactionID = TransactionID;
            this.ProductID = ProductID;
            this.NumberOfProduct = NumberOfProducts;
        }


        async Task<bool> _Add()
        {

            this._ID = await ConnectionLayer.clsIncludedProducts.Add(this.DTOIncludedProduct);

            return _ID != -1;
        }


        async Task<bool> _Update()
        {


            return await ConnectionLayer.clsIncludedProducts.Update(this.DTOIncludedProduct);

        }


        public async Task<bool> Save()
        {
            bool result = false;

            if (_Mode == enMode.Add)
            {

                result = await _Add();
                if (result) { _Mode = enMode.Update; }



            }
            else
            {
                result = await _Update();
            }

            return result;
        }


        public static async Task<bool> Delete(int ID)
        {

            return await ConnectionLayer.clsIncludedProducts.Delete(ID);

        }

        public static async Task<List<DTOIncludedProducts>?> GetAll()
        {

            return await ConnectionLayer.clsIncludedProducts.GetAll();

        }


     

    }
}
