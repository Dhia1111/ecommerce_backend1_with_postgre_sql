using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionLayer;

  namespace BusinessLayer

{
    public interface ILocationService
    {


        public Task<string?> GetCountryCode(string CountryName);

        public Task<List<string>?> GetAllCountries();

        public string ExtractPostCodeFromPostCodeAndLocation(string PostCodeAndLocation);

        public Task<List<string>?> PostCodes(string CountryName, string CityName);
        public Task<List<string>?> GetAllCities(string CountryName);
    }



    public class clsLocation:ILocationService
    {
         private ILocationRepo _LocationRepo;
         private ILocationProvider _locationProvider;

         
        public clsLocation(ILocationRepo locationRepo,ILocationProvider locationProvider) {
        
            _LocationRepo = locationRepo;
            _locationProvider = locationProvider;

        }

        public  async Task<string?> GetCountryCode(string CountryName) {

            return await _LocationRepo.GetCountryCode(CountryName);
        }

        public async  Task<List<string>?> GetAllCountries()
        {
            var  data= await  _LocationRepo.GetAllCountries();
            return data?.Select(c => c.CountryName).ToList();
        }

        public  string ExtractPostCodeFromPostCodeAndLocation(string PostCodeAndLocation)
        {
            int Index = PostCodeAndLocation.IndexOf("//");



            if (Index > 0) return PostCodeAndLocation.Substring(0, Index);
            else
            {
                return PostCodeAndLocation;
            }
        }

        public async Task<List<string>?> PostCodes(string CountryName,string CityName)
        {
            string? CountryCode = await _LocationRepo.GetCountryCode(CountryName);
            if (string.IsNullOrEmpty(CountryCode))
            {
                return null;
            }

         List<string>? PostCodes=  await  _locationProvider.PostCodes(CountryCode, CityName);

            return PostCodes;


        }

        public async Task<List<string>?> GetAllCities(string CountryName)
        {
            string ? CountryCode=await _LocationRepo.GetCountryCode(CountryName);

            if (string.IsNullOrEmpty(CountryCode)) {
                return null; 
            }

            return await _locationProvider.AllCities(CountryCode);
        }


    }
}
