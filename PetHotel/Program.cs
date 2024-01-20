using Microsoft.EntityFrameworkCore;
using PetHotel;
using PetHotel.IServices;
using PetHotel.Models;
using PetHotel.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingConfig));  // useful when there are different DTO types

builder.Services.AddDbContext<PetHotelDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnString"));

});

builder.Services.AddScoped<IRepository<Pet>, Repository<Pet>>();
builder.Services.AddScoped<IRepository<Reservation>, Repository<Reservation>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
