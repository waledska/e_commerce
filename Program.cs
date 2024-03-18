using e_commerce.e_commerceData;
using e_commerce.Helpers;
using e_commerce.IdentityData;
using e_commerce.IdentityData.Models;
using e_commerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Http.Features;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// configuration to map the data from appsetings to class JWT
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// configuration to map the data from appsetings to class JWT
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

// make the mapping between IdentityContext Class and our DB by the connectionString
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<identityContext>(options => options.UseSqlServer(connectionString));
// configure e_commerce context 
builder.Services.AddDbContext<businessContext>(options => options.UseSqlServer(connectionString));
// configure the relation between interface IAuthService and class AuthService and that is for dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
// configure store images service 
builder.Services.AddScoped<ITransferPhotosToPathWithStoreService, transferPhotoToPathWithStoreService>();
// configure the relation between interface IProductService and class ProductService and that is for dependency injection
builder.Services.AddScoped<IProductService, ProductService>();
// configure the relation between interface IEmailSender and class EmailSender and that is for dependency injection
builder.Services.AddTransient<IEmailSender, EmailSender>();
// configure the relation between interface ISMSService and class SMSService and that is for dependency injection
builder.Services.AddTransient<ISMSService, SMSService>();

// configure JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(o =>
   {
       o.RequireHttpsMetadata = false;
       o.SaveToken = false;
       o.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ValidAudience = builder.Configuration["JWT:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
       };
   });

// configure json serialization as when you call nested object desn't make like nested loop
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// to determine the time of lifespan of the OTP
builder.Services.Configure<DataProtectionTokenProviderOptions>(op => 
{
    // Token valid for 1 hours
    op.TokenLifespan = TimeSpan.FromHours(1);
});

// configure our identity(tells our system that which context is for the identity)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<identityContext>()
    .AddDefaultTokenProviders();

// for uploading images 
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

builder.Services.AddControllers();
// Add HttpContextAccessor service this is for make func getUserId...
// AddHttpContextAccessor => and this to could us get the token from the header of the API
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// enable token bearer functionality in Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the Bearer Authentication scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // Make sure the bearer token is applied to all requests
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
