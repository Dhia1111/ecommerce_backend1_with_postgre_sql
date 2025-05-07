
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.Extensions.FileProviders;
using Ecommerce1;
using Stripe;



var builder = WebApplication.CreateBuilder(args);

string SecretKey = clsGlobale.GetJwtSecret();
if (SecretKey == null) throw new Exception("Could not Create services due to Invaliad information ");
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


//set TSL configuration 
builder.Services.AddHttpClient("GeoClient", client => { 
    client.BaseAddress = new Uri(clsGlobale.BaseGeoNameUrl());
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");

})
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                      | System.Security.Authentication.SslProtocols.Tls13,
        // For testing ONLY - remove in production
        ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
    });



//builder.Services.AddAuthentication((option) =>
//{

//    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//}).
//AddCookie((options) =>{
//    options.Cookie.Name = "Authentication";  // Set cookie name
//    options.Cookie.HttpOnly = true;  // Cookie is HTTP only (can't be accessed by JavaScript)
//    options.SlidingExpiration = true;  // Allow cookie expiration on every request
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  // Set the expiration time
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;


//}).

//AddJwtBearer(options =>
//{

//    options.RequireHttpsMetadata = false; // Set true in production
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidIssuer = "APIS_TEST", // Set your actual issuer
//        ValidateAudience = true,
//        ValidAudience = "REACT_TEST", // Set your actual audience
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey))
//    };
//});



//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp", policy =>
//    {
//        policy.WithOrigins("https://firstecommerceforntend.netlify.app")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();

//    });
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

    });
});



builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

var factory = app.Services.GetRequiredService<IHttpClientFactory>();
clsValidation.Insilaze(factory);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

// Serve static files from your custom "images" directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "images")), // Adjust path as needed
    RequestPath = "/images"
});
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowReactApp");
app.MapControllers();
app.Run();
