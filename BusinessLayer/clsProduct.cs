
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsProduct
    {
        enum enMode { Add,Update}

        int _ID;

        private List<DTOCatygory.enCatigories> _Catigories = new List<DTOCatygory.enCatigories>();
        public int ID { get { return _ID; }  }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageName { get; set; }

        public string ImageUrl {  get; set; }





        enMode _Mode;

        public DTOProduct DTOProduct { get { return new DTOProduct(this.ID, this.Name, this.Price, this.ImageName.ToString(),this.ImageUrl,this._Catigories); } }

        public clsProduct(DTOProduct p)
        {
            
                this._ID = -1;
                this.Name = p.Name;
                this.Price = p.Price;
           if(p.ImageName!=null) this.ImageName = p.ImageName;
                _Mode = enMode.Add;
            if(p.ImageUrl!=null)    this.ImageUrl = p.ImageUrl;
            


        }

         clsProduct(int ID, string Name, decimal Price, string ImageName)
        {
            this._ID= ID;
            this.Name = Name;
            this.Price = Price;
            this.ImageName = ImageName;

            _Mode = enMode.Update;

            ImageUrl = "";


        }

        public async Task<bool>AddNewCatigory(DTOCatygory.enCatigories C)
        {
            int result = -1;
             result =await ConnectionLayer.clsCatygory.AddCtaigoryToProduct(this._ID,(int)C);
            return result != -1;
           
        }
         public static async Task<clsProduct?>Find(int ID)
        {
            DTOProduct? p= await ConnectionLayer.clsProduct.Find(ID);
            if(p==null)return null;
            return new clsProduct(p.ID,p.Name,p.Price,p.ImageName);
        }

        async Task<bool> _Add()
        {

            this._ID = await ConnectionLayer.clsProduct.Add(this.DTOProduct);

            return _ID != -1;
        }

        async Task<bool> _Update()
        {


            return await ConnectionLayer.clsProduct.Update(this.DTOProduct);

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


        public static async Task<bool>Delete(int ID)
        {

            return await ConnectionLayer.clsProduct.Delete(ID);

        }

        public static async Task<List<DTOProduct>?>GetAll()
        {

            return await ConnectionLayer.clsProduct.GetAll();

        }


        public static async Task<List<DTOProduct>?> GetAllProductForCatigory(DTOCatygory.enCatigories c)
        {

            return await ConnectionLayer.clsProduct.GetAllForCatigory(c);

        }





        public async Task LoadProductCatigories()
        {
            _Catigories=await ConnectionLayer.clsCatygory.GetAll(this.ID);

     

        }


        public async Task<bool> ClearCatigories()
        {
           return await ConnectionLayer.clsCatygory.DeleteAllCatigories(this.ID);    
        }
    }
}
