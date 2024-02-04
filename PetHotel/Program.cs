using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetHotel;
using PetHotel.IServices;
using PetHotel.Models;
using PetHotel.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
            }, 
            new string[] { }
        }
    
    });
});

builder.Services.AddAutoMapper(typeof(MappingConfig));  // useful when there are different DTO types

builder.Services.AddDbContext<PetHotelDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnString"));

});

builder.Services.AddScoped<IRepository<Pet>, Repository<Pet>>();
builder.Services.AddScoped<IRepository<Reservation>, Repository<Reservation>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(c => {
        c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
            //IssuerSigningKey = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);
    };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();
