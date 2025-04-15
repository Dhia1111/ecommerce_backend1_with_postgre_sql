using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DTOUser
{
    public enum enAtherizations { MakeTransaction = 1, AddUser = 2, UpdateUser = 4,  DeleteUsers = 8, DeleteHisAccount = 16,AddProduct=32,UpdateProduct=64,DeleteProduct=128,ShowProductList=254 }
    public enum enRole { Admine = 1, Customer = 2, User = 3 }


    public int UserID { get; set; }

    public int PersonID { get; set; }

    public enRole UserRole { get; set; }

    public byte UserAtherization { get; set; }

    public string UserName {get;set;}

    public string UserPassword { get; set; }

    public string CreatedAt {  get; set; }

    public DTOPerson Person { get; set; }

    public DTOUser(int UserID,int PersonID ,enRole UserRole, byte UserAtherization, string UserName, string UserPassword, string CreatedAt) {
        this.UserID = UserID;
        this.PersonID = PersonID;
       
        this.UserRole = UserRole;
        this.UserName = UserName;
        this.UserPassword = UserPassword;
        this.UserAtherization = UserAtherization;
        this.CreatedAt = CreatedAt;
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
                                     byte.TryParse(Reader["UserAtherization"].ToString(), out byte UserAtherization) &&
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
                                     byte.TryParse(Reader["UserAtherization"].ToString(), out byte UserAtherization) &&
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
                                     byte.TryParse(Reader["UserAtherization"].ToString(), out byte UserAtherization) &&
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
                                if (!(int.TryParse(Reader["UserID"].ToString(), out int UserID) ||
                                 int.TryParse(Reader["PersonID"].ToString(), out int PersonID) ||
                                 byte.TryParse(Reader["UserRole"].ToString(), out byte UserRole) ||
                                  byte.TryParse(Reader["PersonID"].ToString(), out byte UserAtherization) ||
                                 Reader["UserName"] == null ||
                                 Reader["UserPassWord"] == null ||
                                 Reader["CreateAT"] == null))
                                {
                                 Users.Add(new DTOUser(UserID, PersonID, (DTOUser.enRole)UserRole, UserAtherization, Reader["UserName"].ToString(), Reader["UserPassWord"].ToString(), Reader["CreatedAt"].ToString()));
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
              ""CreateAt""=  @CreateAt,
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
                        command.Parameters.AddWithValue("@UserAtherization", User.UserAtherization);
                        command.Parameters.AddWithValue("@UserName", User.UserName);
                        command.Parameters.AddWithValue("@UserPassWOrd", User.UserPassword);
                        command.Parameters.AddWithValue("@CreateAt", User.CreatedAt);


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




    }
}
