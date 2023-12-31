
using ASPNETCore_WebAPI_JWT_RefreshToken.Models;
using ASPNETCore_WebAPI_JWT_RefreshToken.Repositories;
using ASPNETCore_WebAPI_JWT_RefreshToken.Service;
using ASPNETCore_WebAPI_JWT_RefreshToken.ServiceInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_WebAPI_JWT_RefreshToken
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var congiguration = builder.Configuration;

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            congiguration.
                AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", true, true);

            // Add services to the container.
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("EnableCORS", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                });
            });

            builder.Services.AddDbContext<RepositoryContext>(option =>
            {
                option.UseSqlServer(congiguration.GetConnectionString("default"));
            });

            builder.Services.AddIdentity<User, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || env == "Local")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("EnableCORS");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}