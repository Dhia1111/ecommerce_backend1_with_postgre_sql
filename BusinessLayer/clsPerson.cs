using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsPerson
    {
        enum enMode { Add, Update }

        enMode _Mode;
        public DTOPerson DTOperson { get { return new(this.PersonID, this.FirstName, this.LastName, this.Email, this.Phone,this.Country,this.City,this.PostCode); } }

        int _PersonID;

        public int PersonID { get { return _PersonID; } }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Email { get; set; }


        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }


        public clsPerson(string firstName, string lastName, string email, string phone,string Country,string City,string PostCode)
        {
            _PersonID = -1;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            this.City = City;
            this.Country= Country;
            this .PostCode = PostCode;
            _Mode = enMode.Add
;
        }
        clsPerson(int PersonID, string firstName, string lastName, string email, string phone,string Country, string City, string PostCode)
        {
            _PersonID = PersonID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            this.City = City;
            this.Country = Country;
            this.PostCode = PostCode;
            _Mode = enMode.Update;

        }



        public static async Task<clsPerson?> Find(int ID)
        {
            DTOPerson? Person=await ConnectionLayer.clsPerson.Find(ID);
            if (Person != null) return new clsPerson( Person.PersonID,Person.FirstName, Person.LastName, Person.Email, Person.Phone,Person.Country, Person.City, Person.PostCode);
            return null;
        }


        public static async Task<List<DTOPerson>?> People()
        {
            return await ConnectionLayer.clsPerson.GetPeoPle();
        }


        async Task<bool> _Add()
        {
            this._PersonID = await ConnectionLayer.clsPerson.AddPerson(this.DTOperson);
            return _PersonID != -1;
        }

        async Task<bool> _Update()
        {
           return  await ConnectionLayer.clsPerson.UpdatePerson(this.DTOperson);
          
        }




        public async Task<bool> Save()
        {
            bool result = false;

            if (this._Mode == enMode.Add)
            {
                result = await _Add();
              if(result)  _Mode= enMode.Update;
            }
            else { 
            result= await _Update();
            
            }

            return result;
        }


        public static async Task<bool> Delete(int ID)
        {
            return await ConnectionLayer.clsPerson.Delete(ID);
        }


        public static async Task<string?>GetCountryCode(string CountryName)
        {
            return await ConnectionLayer.clsPerson.GetCountryCode(CountryName);
        }
    }
}
