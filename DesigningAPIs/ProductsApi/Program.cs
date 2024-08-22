
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductsApi.Data;
using ProductsApi.Data.Repositories;
using ProductsApi.Infrastructure;
using ProductsApi.Service;

namespace ProductsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.AllowEmptyInputInBodyModelBinding = true;
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));


            });

            builder.Services.AddAutoMapper(typeof(ProductProfile));

            builder.Services.AddDbContext<ProductContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
            .EnableSensitiveDataLogging()); // Optional: shows parameter values in SQL logs);

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
                   new MemoryCacheOptions
                   {
                       TrackStatistics = true,
                       //SizeLimit = 50 // Products.
                   }));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddResponseCaching();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseDeveloperExceptionPage();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ProductContext>().Database.EnsureCreated();
                    //serviceScope.ServiceProvider.GetService<ProductContext>().EnsureSeeded();
                }
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseResponseCaching();

            app.MapControllers();

            app.Run();
        }
    }
}
