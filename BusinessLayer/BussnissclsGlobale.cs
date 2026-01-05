using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{

    public static class BussnissclsGlobale
    {

        private static readonly IConfiguration _configuration;

        // Static constructor to initialize the configuration
        static BussnissclsGlobale()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public static string GetJwtSecret()
        {
            return _configuration["JWT_SECRET"];
        }
        public static string GetCurrencyExChangeAPI()
        {
            return _configuration["CurrencyExChangeAPI"];
        }


        public static string GetTheBestImageExtention()
        {
            return _configuration["BestImageExtension"];
        }
        public static string GetStripSecret()
        {
            return _configuration["stripeSecretKey"];
        }

        public static string StripeWebhookSecret()
        {
            return _configuration["StripeWebHook"];
        }
        public static string GetStripCLIAcountNumber()
        {
            return _configuration["StripAccountCLI"];
        }
        public static string SetImageURL(string ImageName)
        {
            return _configuration["ImagesUrl"].ToString() + ImageName;
        }

        public static string GetEmail()
        {
            return _configuration["Email"];
        }

        public static string GetVerifyEmailLink()
        {
            return _configuration["GetVerifyEmailLink"];

        }
        public static string GetEmailPassWord()
        {
            return _configuration["EmailPassWord"];
        }

        public static string GetLoadDiractory()
        {
            return _configuration["UploadDiractory"];
        }
        public static string? ConectionString()
        {
            return _configuration["ConnectionString"];
        }

        public static async Task<bool> SendEmail(clsUser User, string emailbody, string Subject, bool IsHtml)
        {
            try
            {


                using SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(BussnissclsGlobale.GetEmail(), BussnissclsGlobale.GetEmailPassWord()),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(BussnissclsGlobale.GetEmail()),
                    Subject = Subject,

                    // make sure to create a valiad VerificationLink based on yor front end 

                    Body = emailbody,

                    IsBodyHtml = IsHtml,
                };

                mailMessage.To.Add(User.Person.Email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nError :\n" + ex);
                return false;

            }
            return true;
        }
 
        public static string BaseGeoNameUrl()
        {
            return _configuration["baseGeoNameUrl"];
        }
        public static string GetgeonamesUserName() { return _configuration["geonamesUserName"]; }

        public static string GetAPI_KEY()
        {
            return _configuration["API_KEY"];
        }

        public static string GetCloudName()
        {
            return _configuration["CloudName"];
        }

        public static string GetAPI_SECRET()
        {
            return _configuration["API_SECRET"];
        }





    }
}
