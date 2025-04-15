using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsValidatingEmail
    {
        public int PersonID { get; set; }
        public Guid GUID_ID { get; set; }

        public DTOValidatingEmail DTO { get { return new DTOValidatingEmail(this.PersonID, this.GUID_ID.ToString()); } }
      
    public    clsValidatingEmail(int PersonID, Guid GUID_ID)
        {

            this.PersonID = PersonID;
             this .GUID_ID = GUID_ID;
           
        }


        public async Task<bool> Add()
        {
            return await ConnectionLayer.clsValidatingEmail.Add(this.DTO);
        }

   
        public static async Task<bool> Delete(int PersonID)
        {
            return await ConnectionLayer.clsValidatingEmail.Delete(PersonID);

        }

        public static async Task<bool> Delete(Guid GUID_ID)
        {
            return await ConnectionLayer.clsValidatingEmail.Delete(GUID_ID.ToString());

        }

        public static async Task<clsValidatingEmail?>Find(int PersonID)
        {
            DTOValidatingEmail? validatingEmail = await ConnectionLayer.clsValidatingEmail.Find(PersonID);

            return validatingEmail == null ? null : new clsValidatingEmail(validatingEmail.PersonID, Guid.Parse(validatingEmail.GUID_ID));
        }


        public static async Task<clsValidatingEmail?> Find(string GUID_ID)
        {
            DTOValidatingEmail? validatingEmail = await ConnectionLayer.clsValidatingEmail.Find(GUID_ID);

            if (validatingEmail == null) return null;
            return  new clsValidatingEmail(validatingEmail.PersonID, Guid.Parse(validatingEmail.GUID_ID));
        }



    }


}

