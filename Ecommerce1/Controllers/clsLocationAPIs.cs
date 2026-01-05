using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe.V2;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using System.Net;
using System.Net.Http;
using System.Diagnostics.Metrics;


[Route("api/Ecommerce/clsLocationAPIs")]
[ApiController]


public class clsLocationAPIs : ControllerBase
{


    private ILocationService _LocationService;
    private IIP2Location _IP2Location;
    public clsLocationAPIs( ILocationService LocationService, IIP2Location iP2Location)
    {
        _LocationService = LocationService;
        _IP2Location = iP2Location;
    }


    [HttpGet("GetCounties")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<string>?>> GetAllCounties()
    {
        List<string>? counties = await _LocationService.GetAllCountries();
        if (counties == null)
        {
            return StatusCode(500, new DTOGeneralResponse("We could not respond due to server error", 500, "NotSet"));
        }

        return Ok(counties);
    }


    [HttpGet("GetCityies/{CountryName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<ActionResult<List<string>?>> GetAllCities(string CountryName)
    {

        if (string.IsNullOrEmpty(CountryName))
        {
            return BadRequest(new DTOGeneralResponse("You should Provid the country name", 400, "Validation Error"));

            
        }

        List<string>? Cyties = await _LocationService.GetAllCities(CountryName);

        if (Cyties == null)
            return StatusCode(500, new DTOGeneralResponse("Could not retrieve cities", 500, "NotSet"));

        return Ok(Cyties);


    }


    [HttpGet("GetPostCodes/{CountryName}/{CityName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<string>?>> GetPostCodes(string CountryName, string CityName)
    {
        if (string.IsNullOrEmpty(CountryName))
        {
            return BadRequest(new DTOGeneralResponse("You should Provid the country name ", 400, "Validation Error"));


        }
        if ( string.IsNullOrEmpty(CityName))
        {
            return BadRequest(new DTOGeneralResponse("You should Provid the city name ", 400, "Validation Error"));


        }
        var PostCodes= await _LocationService.PostCodes(CountryName,CityName);
        if (PostCodes == null)
        {
            return StatusCode(500, new DTOGeneralResponse("the server is experincing a faileur pleas try later", 500, "")); 
        }
        return Ok(PostCodes);
    }


    [HttpGet("AddressDataByIpAddress")]
    public async Task<ActionResult> GetLocation()
    {
        long Ip = 34640896;
        var Address = Request.HttpContext.Connection.RemoteIpAddress?.ToString();


    
        {


            var location = await _IP2Location.FindByIpAddress(Ip);


            return Ok(location);
        }


    }


}

