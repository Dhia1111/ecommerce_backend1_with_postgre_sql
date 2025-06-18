


using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Ecommerce1.Controllers;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Linq;
using static DTOUser;

[Route("api/Ecommerce/clsUsersAPIs")]
[ApiController]
public class clsUsersAPIs : ControllerBase
{

    public clsUsersAPIs()
    {
       

    }

    //Add an Othorized User return a message add user as  a client then update it  after verfing the email 

    

    //Update User Change Role and Athorizations 

    [HttpPost("UpdateUserAthorizationAndRole", Name = "UpdateUserAthorizationAndRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOGeneralResponse?>> UpdateUser([FromBody] DTOUser SendedUser )
    {
        //CheckAthorization

        clsUser? User;

        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {


            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }

            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }



        if ((User.Role != DTOUser.enRole.Admine && !(User.IsAthorizedUser(DTOUser.enAtherizations.UpdateUser))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }


        clsUser? UpdatingUser = await clsUser.Find(SendedUser.UserID);

        if (UpdatingUser == null) {

            return BadRequest(new DTOGeneralResponse("In valaid User ID  Did not found user, try to refrech your Data",400,"Serch Error"));
        }

        if (UpdatingUser.UserName== "A1111")
        {
            return BadRequest(new DTOGeneralResponse("You can not Update  the Default Admmine  ", 400, "System breaking"));
        }

        if (UpdatingUser.Role == DTOUser.enRole.Admine && (await clsUser.GetNumberOfAdmines()) == 1)
        {
            return BadRequest(new DTOGeneralResponse("You can not Update  the last Admmine  ", 400, "System breaking"));
        }

        UpdatingUser.Atherization= SendedUser.UserAtherization;
        UpdatingUser.Role= SendedUser.UserRole;

        if(!(await UpdatingUser.SaveAthorizedUser())) return StatusCode(500,(new DTOGeneralResponse("User Updating  Failed", 500, "Saving failuer")));



        return Ok(new DTOGeneralResponse("User Updated Secsessfuly", 200, "None"));

    }

    //Delete User 

    [HttpDelete("DeleteUser/{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DTOGeneralResponse?>> Delete(int Id)
    {

        clsUser? User;

        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {


            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }

            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }


        if ((User.Role != DTOUser.enRole.Admine && !(User.IsAthorizedUser(DTOUser.enAtherizations.DeleteUsers))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }

        if (!int.TryParse(Id.ToString(), out int ID))
        {
            return BadRequest(new DTOGeneralResponse("Send a valid Params", 400, "Validation and Parsing data"));

        }

        clsUser? deletedUser = await clsUser.Find(ID);


        if (deletedUser == null)
        {
            return BadRequest(new DTOGeneralResponse("Send a valid Params", 400, "serch failuer"));

        }

        if (deletedUser.UserName == "A1111")
        {
            return BadRequest(new DTOGeneralResponse("You can not Delete  the Default Admmine  ", 400, "System breaking"));
        }

        if (deletedUser.Role==DTOUser.enRole.Admine&& (await clsUser.GetNumberOfAdmines()) == 1)
        {
            return BadRequest(new DTOGeneralResponse("You can not delete the last Admmine", 400, "System breaking"));
        }


        clsTransaction? TransactionForUser = await clsTransaction.FindFirst(ID);
        if (TransactionForUser!=null)
        {
            return StatusCode(500, new DTOGeneralResponse("Could Not Delete the User, thier are dependencies  related to this Customer (you need to delete the transactions related to this user first )", 400, "Delete failuer"));

        }

        bool DeletingResult = await clsUser.Delete(ID);
        if (!DeletingResult)
        {
            return StatusCode(500, new DTOGeneralResponse("Could Not Delete the User and and  it's dependencies", 400, "Delete failuer"));

        }

   

        DeletingResult = await clsValidatingEmail.Delete(deletedUser.PersonID);

        if (!DeletingResult&& await clsValidatingEmail.Find(deletedUser.PersonID)!=null)
        {
            return StatusCode(500, new DTOGeneralResponse("Could Not Delete the Unverfied Email dependency", 400, "Delete failuer"));


        }

        DeletingResult = await clsPerson.Delete(deletedUser.PersonID);
     
        if (!DeletingResult)
        {
            return StatusCode(500, new DTOGeneralResponse("Could Not Delete the Person dependency", 400, "Delete failuer"));


        }       return Ok(new DTOGeneralResponse("the User deleted with all its dependencies ", 200, "None"));

    }

    //Show Users Lists 

    [HttpGet("GetAllUsers",Name ="GetAllUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<List<DTOUser>?>> GetAllUser()
    {

        clsUser? User;


        if (Request.Cookies.TryGetValue("Authentication", out string token1))
        {

            int? UserID = clsGlobale.ExtractUserIdFromToken(token1);

            if (UserID == null)
            {

                return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));
            }

            else
            {
                User = await clsUser.Find(UserID.Value);
                if (User == null)
                {
                    return StatusCode(500, new DTOGeneralResponse("An unexpected server error occurred.", 500, "null requests"));


                }



            }
        }
        else
        {
            return BadRequest(new DTOGeneralResponse("you need to log in again", 400, "Validation Error"));


        }

        if ((User.Role != DTOUser.enRole.Admine && !(User.IsAthorizedUser(DTOUser.enAtherizations.ShowUsersList))) || User.Role == DTOUser.enRole.Customer)
        {
            return BadRequest(new DTOGeneralResponse("Your not athorized", 400, "UnAthorized request"));
        }
        return await clsUser.GetAll();


    }

    //Show User  Details {personal inf ,User Inf } , show User Transactions

    [HttpGet("GetAthorizationOptions")]
    public ActionResult AthoriztionList()
    {
        List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
        enAtherizations[] allDays = Enum.GetValues(typeof(DTOUser.enAtherizations))
        .Cast<enAtherizations>()
                              .ToArray();

        foreach (var e in allDays)
        {
            list.Add(new KeyValuePair<int, string>((int)e, e.ToString()));
        }
        return Ok(list);
    }

    [HttpGet("GetUserRoles")]
    public ActionResult GetUsersRoles()
    {
        List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
        enRole[] allDays = Enum.GetValues(typeof(DTOUser.enRole))
        .Cast<enRole>()
                              .ToArray();

        foreach (var e in allDays)
        {
            list.Add(new KeyValuePair<int, string>((int)e, e.ToString()));
        }
        return Ok(list);
    }


}