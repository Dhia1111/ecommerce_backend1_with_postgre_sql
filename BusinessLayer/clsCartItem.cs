using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsCartItem
    {
        enum enMode { Add, Update }
        int _CartItemID;
        public int CartItemID { get { return _CartItemID; } }

        public int ProductID { get; set;}

        public uint NumberOfItems { get; set;}

        public int UserID { get; set;}

        enMode _Mode;

        public DTOCartItem CartItemDTO { get { return new DTOCartItem(this.CartItemID,this.UserID,this.ProductID,this.NumberOfItems); } }

        public clsCartItem() { 
        _CartItemID = -1;
            _Mode = enMode.Add;
        }
      
        clsCartItem(DTOCartItem CartItem)
        {
            this._CartItemID =CartItem.CartID==null?-1:CartItem.CartID.Value;
            this._Mode=enMode.Update;
            this.UserID = CartItem.UserID==null?-1:CartItem.UserID.Value;
            this.ProductID = CartItem.ProductID;
            this.NumberOfItems = CartItem.NumberOfItems;
        }
        async Task<bool> _Add()
        {
            _CartItemID = await ConnectionLayer.clsCartItem.Add(this.CartItemDTO);

            return _CartItemID!=-1;
        }
        async Task<bool> _Update()
        {
            return await ConnectionLayer.clsCartItem.Update(this.CartItemDTO);
        }
        public async Task<bool> Save()
        {
            bool result = false;
            if (_Mode == enMode.Add)
            {

                result=await _Add();
                if (result)
                {
                    _Mode = enMode.Update;

                }

            }
            else
            {
                result=await _Update();

            }

            return result;
        }
        public static async Task<bool> Delete(int UserID,int ProductID)
        {

            return  await ConnectionLayer.clsCartItem.Delete(UserID, ProductID);
        }
        public static async Task<List<DTOCartItem>?> GetCart(int UserID)
        {

            List<DTOCartItem>? Cart = await ConnectionLayer.clsCartItem.GetCart(UserID);

            return Cart;

        }
    
        public static async Task<clsCartItem?>Find(int UserID, int ProductID)
        {
            DTOCartItem? dto_cart= await ConnectionLayer.clsCartItem.Find(UserID, ProductID);
            if (dto_cart != null)
            {
                return new clsCartItem(dto_cart);
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> ClearCart(int UserID)
        {
            return await ConnectionLayer.clsCartItem.ClearCart(UserID);
        }
    }
}
