
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
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

            builder.Services.AddRepositories();
            // builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.RegisterServices();
            //  builder.Services.AddScoped<IProductService, ProductService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
                   new MemoryCacheOptions
                   {
                       TrackStatistics = true,
                       //SizeLimit = 50 // Products.
                   }));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddResponseCaching();

            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.ApiVersionReader = new Asp.Versioning.QueryStringApiVersionReader("v");
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(2, 0);
            //    options.ReportApiVersions = true;
            //}).AddMvc();


            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.ApiVersionReader = new Asp.Versioning.HeaderApiVersionReader("v");
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(2, 0);
            //    options.ReportApiVersions = true;
            //}).AddMvc();

            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(2, 0);
            //    options.ReportApiVersions = true;
            //}).AddMvc();

            //builder.Services.AddApiVersioning(options =>
            //{
            //    options.ApiVersionReader = new Asp.Versioning.MediaTypeApiVersionReader("api-version");
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(2, 0);
            //    options.ReportApiVersions = true;
            //}).AddMvc();

            builder.Services.AddApiVersioning(options =>
            {

                options.ApiVersionReader = new MediaTypeApiVersionReaderBuilder().Template("application/vnd.example.v{api-version}+json").Build();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.ReportApiVersions = true;
            }).AddMvc().AddApiExplorer(
                    options =>
                    {
                        // the default is ToString(), but we want "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";
                    }); ;



            var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            builder.Services.AddSwaggerGen(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"My API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    // Add a Swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }

                });

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
