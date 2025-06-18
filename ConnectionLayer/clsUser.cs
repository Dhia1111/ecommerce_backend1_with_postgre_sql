using ConnectionLayer;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOUser
{
    public enum enAtherizations { 
        
        ShowUsersList= 2048, UpdateUser = 1, DeleteUsers = 2,SowUserDetails=1024,

        ShowTransactionList = 4, DeleteTransaction = 8,ShowTransactionDetails=16,

        AddProduct=32,UpdateProduct=64,DeleteProduct=128,ShowProductList=256, ClearCatigoriesForAProduct = 512,

        GetProductDetails=4096
    }
   
    public enum enRole { Admine = 1, Customer = 2, AthorizedUser = 3 }

    public int UserID { get; set; }

    public int PersonID { get; set; }

    public enRole UserRole { get; set; }

    public int UserAtherization { get; set; }

    public string UserName {get;set;}

    public string UserPassword { get; set; }

    public string CreatedAt {  get; set; }

    public DTOPerson Person { get; set; }

    public DTOUser(int UserID,int PersonID ,enRole UserRole, int UserAtherization, string UserName,  string CreatedAt,string UserPassword= "",DTOPerson PersonInf=null) {
        this.UserID = UserID;
        this.PersonID = PersonID;
       
        this.UserRole = UserRole;
        this.UserName = UserName;
        this.UserPassword = UserPassword;
        this.UserAtherization = UserAtherization;
        this.CreatedAt = CreatedAt;
        if (PersonInf == null) this.Person = new DTOPerson(-1, "", "", "", "", "", "", "");
        else Person = PersonInf;
    }


    public DTOUser()
    {
        this.UserID = -1;
        this.PersonID = -1;

        this.UserRole = DTOUser.enRole.Customer;
        this.UserName = "";
        this.UserPassword = "";
        this.UserAtherization = 0;
        this.CreatedAt = DateTime.Now.ToString();
        this.Person = new DTOPerson(-1, "", "", "", "", "", "", "");
    }



}

namespace ConnectionLayer
{
    
    public static class clsUser
    {
        public static async Task<DTOUser?> Find(int ID)
        {


            string qery = @"select * From ""Users"" where ""UserID""=@UserID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOUser User = new DTOUser(-1,-1, 0, 0, "", "", "");


                                if ( int.TryParse(Reader["UserID"].ToString(), out int UserID) &&
                                     int.TryParse(Reader["PersonID"].ToString(), out int PersonID) &&
                                     byte.TryParse(Reader["UserRole"].ToString(), out byte UserRole) &&
                                     int.TryParse(Reader["UserAtherization"].ToString(), out int UserAtherization) &&
                                     (Reader["UserName"] != null) &&
                                     (Reader["UserPassWord"] != null) &&
                                     (Reader["CreateAT"] != null )){

                                    User.UserID = UserID;
                                    User.PersonID = PersonID;
                                    User.UserRole =(DTOUser.enRole) UserRole;
                                    User.UserAtherization = UserAtherization;
                                    User.UserName = Reader["UserName"].ToString();
                                    User.UserPassword = Reader["UserPassWord"].ToString();
                                    User.CreatedAt = Reader["CreateAt"].ToString();


                                    return User;
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

        public static async Task<DTOUser?> FindbyPersonID(int ID)
        {


            string qery = @"select * From ""Users"" where ""PersonID""=@PersonID";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", ID);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOUser User = new DTOUser(-1, -1, 0, 0, "", "", "");


                                if ((int.TryParse(Reader["UserID"].ToString(), out int UserID) &&
                                    int.TryParse(Reader["PersonID"].ToString(), out int PersonID) &&
                                    byte.TryParse(Reader["UserRole"].ToString(), out byte UserRole) &&
                                     int.TryParse(Reader["UserAtherization"].ToString(), out int UserAtherization) &&
                                    Reader["UserName"] != null &&
                                    Reader["UserPassWord"] != null &&
                                    Reader["CreateAT"] != null))
                                {

                                    User.UserID = UserID;
                                    User.PersonID = PersonID;
                                    User.UserRole = (DTOUser.enRole)UserRole;
                                    User.UserAtherization = UserAtherization;
                                    User.UserName = Reader["UserName"].ToString();
                                    User.UserPassword = Reader["UserPassWord"].ToString();
                                    User.CreatedAt = Reader["CreateAt"].ToString();


                                    return User;
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

        public static async Task<DTOUser?> Find(string UserName)
        {


            string qery = @"select * From ""Users"" where ""UserName""=@UserName ";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserName", UserName);


                        using (NpgsqlDataReader Reader = await command.ExecuteReaderAsync())
                        {

                            if (Reader.Read())
                            {


                                DTOUser User = new DTOUser(-1, -1, 0, 0, "", "", "");


                                if ((int.TryParse(Reader["UserID"].ToString(), out int UserID) &&
                                    int.TryParse(Reader["PersonID"].ToString(), out int PersonID) &&
                                    byte.TryParse(Reader["UserRole"].ToString(), out byte UserRole) &&
                                     int.TryParse(Reader["UserAtherization"].ToString(), out int UserAtherization) &&
                                    Reader["UserName"] != null &&
                                    Reader["UserPassWord"] != null &&
                                    Reader["CreateAT"] != null))
                                {

                                    User.UserID = UserID;
                                    User.PersonID = PersonID;
                                    User.UserRole = (DTOUser.enRole)UserRole;
                                    User.UserAtherization = UserAtherization;
                                    User.UserName = Reader["UserName"].ToString();
                                    User.UserPassword = Reader["UserPassWord"].ToString();
                                    User.CreatedAt = Reader["CreateAt"].ToString();


                                    return User;
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

        public static async Task<List<DTOUser>?> GetUsers()
        {
            string qery = @"select*From ""Users""";

            List<DTOUser> Users = new List<DTOUser>();

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
                                if (int.TryParse(Reader["UserID"].ToString(), out int UserID) &&
                                 int.TryParse(Reader["PersonID"].ToString(), out int PersonID) &&
                                 int.TryParse(Reader["UserRole"].ToString(), out int UserRole) &&
                                 int.TryParse(Reader["UserAtherization"].ToString(), out int UserAtherization) &&
                                 Reader["UserName"] != null &&
                                  Reader["CreateAt"] != null)
                                
                                  {
                                     
                                   Users.Add(new DTOUser(UserID, PersonID, (DTOUser.enRole)UserRole, UserAtherization, Reader["UserName"].ToString(), Reader["CreateAt"].ToString(),"",await clsPerson.Find(PersonID)));
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




            return Users;
        }


        public static async Task<int> AddUser(DTOUser User)
        {

            string qery = @"insert into ""Users""(""PersonID"",""UserRole"" ,""UserAtherization"",""UserName"",""UserPassWord"",""CreateAt"")
                values(@PersonID,@UserRole,@UserAtherization,@UserName,@UserPassWord,@CreateAt)

                    RETURNING ""UserID"";";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@PersonID", User.PersonID);
                        command.Parameters.AddWithValue("@UserRole",(int) User.UserRole);
                        command.Parameters.AddWithValue("@UserAtherization", User.UserAtherization);
                        command.Parameters.AddWithValue("@UserName", User.UserName);
                        command.Parameters.AddWithValue("@UserPassWord", User.UserPassword);
                        command.Parameters.AddWithValue("@CreateAt", DateTime.Parse(User.CreatedAt));



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


        public static async Task<bool> UpdateUser(DTOUser User)
        {

            string qery = @"Update ""Users"" set 
                       
               ""UserRole""=  @UserRole,
              ""UserAtherization""=  @UserAtherization,
              ""UserName""=  @UserName,
              ""UserPassWord""=  @UserPassWord,
              ""CreateAt""=  @CreateAt
    where         ""UserID""=@UserID";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserID", User.UserID);
                        command.Parameters.AddWithValue("@PersonID", User.PersonID);
                        command.Parameters.AddWithValue("@UserRole",(int) User.UserRole);
                        command.Parameters.AddWithValue("@UserAtherization", (int)User.UserAtherization);
                        command.Parameters.AddWithValue("@UserName", User.UserName);
                        command.Parameters.AddWithValue("@UserPassWord", User.UserPassword);
                        command.Parameters.AddWithValue("@CreateAt",DateTime.Parse(User.CreatedAt));


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

            string qery = @"Delete from  ""Users""  where ""UserID""=@UserID";


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


        public static async Task<int> GetNumberOfAdmines()
        {
            string qery = @"select Count(""UserID"") From ""Users"" where ""UserRole"" like @UserRole";


            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(clsConnectionGenral.ConnectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand command = new NpgsqlCommand(qery, connection))
                    {

                        command.Parameters.AddWithValue("@UserRole",((int)DTOUser.enRole.Admine).ToString());

                        object? objNumberOfUsers = await command.ExecuteScalarAsync();
                        {

                            if (objNumberOfUsers!=null)
                            {

                                if(int.TryParse(objNumberOfUsers.ToString(),out int NumberOfUsers))
                                {
                                    return NumberOfUsers;
                                }
                                else
                                {
                                    return -1;
                                }

                               



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




        }


    }
}
