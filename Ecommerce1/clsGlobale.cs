﻿using BusinessLayer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Ecommerce1
{
    public static class clsGlobale
    {
        private static readonly HttpClient client = new HttpClient();

        private static readonly IConfigurationRoot _configuration;

        // Static constructor to initialize the configuration
        static clsGlobale()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true).AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        // Method to retrieve values
        public static string GetJwtSecret()
        {
            return _configuration["JWT_SECRET"];
        }
        public static string GetTheBestImageExtention()
        {
            return _configuration["BestImageExtension"];
        }
        public static string GetStripSecret()
        {
            return _configuration["stripeSecretKey"];
        }

        public static string SetImageURL(string ImageName)
        {
            return _configuration["ImagesUrl"].ToString()+ImageName;
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

        public static string? GenerateJwtToken(DTOUser user)
        {
            if (string.IsNullOrEmpty(GetJwtSecret())) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string? GenerateJwtToken(Guid GuidID)
        {
            if (string.IsNullOrEmpty(GetJwtSecret())) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, GuidID.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // **New method to extract UserID from a token**

        public static int? ExtractUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(GetJwtSecret()))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
            }
            catch
            {
                return null; // Invalid token
            }
        }

        public static Guid? ExtractGuidIDFromToken(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(GetJwtSecret()))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                var GuidIDClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

                return GuidIDClaim != null ? Guid.Parse(GuidIDClaim.Value) : null;
            }
            catch
            {
                return null; // Invalid token
            }
        }


        public  static async Task<bool>SendEmail(clsUser User,string emailbody,string Subject,bool IsHtml)
        {
            try
            {
                

                using SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new System.Net.NetworkCredential(clsGlobale.GetEmail(), clsGlobale.GetEmailPassWord()),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(clsGlobale.GetEmail()),
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


       static string _GetTextUntilTheFirstSpace(string Text)
        {
            string text = "";
            foreach (char c in Text)
            {
                if (char.IsWhiteSpace(c)) return text;
                text += c;
            }
            return text;
        }


        public static async Task<bool> ValidateLocationAsync(string postalCode, string city, string countryCode)
        {
            string username = _configuration["geonamesUserName"];
            var url = $"http://api.geonames.org/postalCodeSearchJSON?postalcode={postalCode}&placename={city}&country={countryCode}&username={username}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return false;

            var content = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("postalCodes", out JsonElement postalCodes) && postalCodes.GetArrayLength() > 0)
            {
                foreach (var place in postalCodes.EnumerateArray())
                {
                    if (place.TryGetProperty("placeName", out JsonElement placeName) &&
                    _GetTextUntilTheFirstSpace(placeName.GetString())?.Equals(city, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return true; // Match found
                    }
                }
            }

            return false; // No match
        }
    }


}

