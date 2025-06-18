using ConnectionLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsUser
    {
        enum enMode { Add, Update }

        enMode _Mode;
        clsPerson _Person;

        int _UserID;

        public int UserID { get { return _UserID; } }


        public int PersonID { get; set; }

        public clsPerson Person{get{ return _Person ; } }
        public string UserName { get; set; }

        public string PassWord {  get; set; }

        public DTOUser.enRole Role { get; set; }
        
        public int Atherization { get; set; }
 
        public DTOUser DTOUser { get { return new DTOUser(this._UserID, this.PersonID, this.Role, this.Atherization, this.UserName, this._CreateAt.ToString(), this.PassWord, this.Person.DTOperson); } }

        DateTime _CreateAt { get; set; }

         public clsUser(DTOUser User)
        {
            _UserID = -1;
             this.UserName = User.UserName;
            this.PassWord = User.UserPassword;
            this.Role = User.UserRole;
            this.Atherization = User.UserAtherization;
            _Mode = enMode.Add;
            _CreateAt = DateTime.Now;
            _Person = new clsPerson(User.Person.FirstName, User.Person.LastName, User.Person.Email, User.Person.Phone, User.Person.Country, User.Person.City, User.Person.PostCodeAndLocation);
        }

         clsUser(int ID, int PersonID, string UserName, string PassWord, int Athorization , DTOUser.enRole Role,clsPerson Person,DateTime CreatedAt)
        {
            _UserID = ID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.PassWord = PassWord;
            this.Role = Role;
            this.Atherization = Athorization;
            _Mode = enMode.Update;
            this._Person = Person;
            _CreateAt = CreatedAt;
             
         }

        public async Task<bool> HasUnfinshedPayment()
        {
            bool result = ((await ConnectionLayer.clsTransaction.GetTransactionGuidIdFroUnfinshedPayment(this.UserID)) != Guid.Empty);

            return result;
        }
        public  async Task<List<DTOCartItem>?> Cart()
        {
            return await ConnectionLayer.clsCartItem.GetCart(this.UserID);  
        }
        public static async Task<List<DTOUser>?> GetAll()
        {
            return await ConnectionLayer.clsUser.GetUsers();
            
            
        }
        public static async Task<clsUser?>Find(int UserID)
        {
            DTOUser? user= await ConnectionLayer.clsUser.Find(UserID);

            if (user == null) {return null;}
            clsPerson? person = await clsPerson.Find(user.PersonID);
            if (person == null) return null;
            return new clsUser(user.UserID, user.PersonID, user.UserName, user.UserPassword, user.UserAtherization, user.UserRole, person,DateTime.Parse(user.CreatedAt));
        }
        public static async Task<clsUser?> FindByPersonID(int PersonID)
        {
            DTOUser? user = await ConnectionLayer.clsUser.FindbyPersonID(PersonID);

            if (user == null) { return null; }
            clsPerson? person = await clsPerson.Find(user.PersonID);
            if (person == null) return null;
            return new clsUser(user.UserID, user.PersonID, user.UserName, user.UserPassword, user.UserAtherization, user.UserRole, person, DateTime.Parse(user.CreatedAt));
        }
        public static async Task<clsUser?> Find(string UserName )
        {
            DTOUser? user = await ConnectionLayer.clsUser.Find(UserName);

            if (user == null) { return null; }
            clsPerson? person = await clsPerson.Find(user.PersonID);
            if (person == null) return null;
            return new clsUser(user.UserID, user.PersonID, user.UserName, user.UserPassword, user.UserAtherization, user.UserRole, person, DateTime.Parse(user.CreatedAt));
        }

        public static async Task<int> GetNumberOfAdmines()
        {
            return await ConnectionLayer.clsUser.GetNumberOfAdmines();
        }
        bool CleanCustomerAthorization()
        {

            this.Atherization = 0;
            return this.Atherization == 0;
        }
        bool CleanCustomerRole()
        {

            this.Role = DTOUser.enRole.Customer;
            return this.Role== DTOUser.enRole.Customer;
        }

        async Task<bool> _Add()
        {
            //AddPerson first
            await _Person.Save();
            if (_Person.PersonID == -1) return false;
            this.PersonID = _Person.PersonID;
            this._UserID =await  ConnectionLayer.clsUser.AddUser(this.DTOUser);
            return _UserID != -1;
        }

        async Task<bool> _Update()
        {

           return await ConnectionLayer.clsUser.UpdateUser(this.DTOUser);
          
        }



        public async Task<bool> SaveCustomer()
        {
            bool result = false;

             this.CleanCustomerAthorization();
            this.CleanCustomerRole();

            if (this._Mode == enMode.Add) { 
            

                result=await _Add();
                if(result)this._Mode = enMode.Update;
            
            }

            else
            {
                result = await _Update();

            }
            return result;
        }

        public async Task<bool> SaveAthorizedUser()
        {
            //Valaidate Admine 
             

            bool result = false;

        

            if (this._Mode == enMode.Add)
            {


                result = await _Add();
                if (result) this._Mode = enMode.Update;

            }

            else
            {
                result = await _Update();

            }
            return result;
        }

        public async Task<bool>AddToCart(DTOCartItem cartitem)
        {
            clsCartItem CartItem= new clsCartItem();

            CartItem.ProductID = cartitem.ProductID;
            CartItem.UserID= cartitem.UserID==null?-1:cartitem.UserID.Value;
            CartItem.NumberOfItems= cartitem.NumberOfItems;
           
            return await CartItem.Save();


            
        }
        public async Task<bool> UpdateToCart(DTOCartItem cartitem)
        {
            clsCartItem? FindCart =await clsCartItem.Find(this.UserID, cartitem.ProductID);
            if (FindCart != null)
            {
               
                FindCart.NumberOfItems=cartitem.NumberOfItems;
                return await FindCart.Save();  

            }
            else
            {

                clsCartItem CartItem = new clsCartItem();

                CartItem.ProductID = cartitem.ProductID;
                CartItem.UserID = this.UserID;
                CartItem.NumberOfItems = cartitem.NumberOfItems;
                return await CartItem.Save();

            }




        }
        public async Task<bool> ClearCart()
        {

         return await   clsCartItem.ClearCart(this.UserID);



        }
        public async Task<bool>DeleteProductFromUserCart(int ProductID)
        {
            return await clsCartItem.Delete(this.UserID, ProductID);
        }
        public static async Task<bool>Delete(int ID)
        {
            return await ConnectionLayer.clsUser.Delete(ID);  
        }
  

    public  bool IsAthorizedUser(DTOUser.enAtherizations atherizationsType)
        {
            int AtherizationsType=(int)atherizationsType;
         return (this.Role == DTOUser.enRole.AthorizedUser) && ((this.Atherization & AtherizationsType) == AtherizationsType);

        }



    }
} 
