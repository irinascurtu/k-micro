
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Data.Repositories;
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
                //  options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                // options.ModelValidatorProviders.Clear();
                //options.RespectBrowserAcceptHeader = true;

            });


            //builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; })
            //  .AddXmlDataContractSerializerFormatters();
            //builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            //{
            //    options.SuppressMapClientErrors = true;
            //    //options.SuppressUseValidationProblemDetailsForInvalidModelStateResponses.Value = true;
            //    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";

            //    options.InvalidModelStateResponseFactory = context =>
            //    {
            //        var result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(context.ModelState);
            //        result.ContentTypes.Add("application/problem+json");
            //        result.ContentTypes.Add("application/problem+xml");
            //        return result;
            //    };

            //    //options.InvalidModelStateResponseFactory = context =>
            //    //{
            //    //    var problemDetails = new ValidationProblemDetails(context.ModelState)
            //    //    {
            //    //        Type = "https://conference-api.com/modelvalidationproblem",
            //    //        Title = "One or more model validation errors occurred.",
            //    //        Status = StatusCodes.Status422UnprocessableEntity,
            //    //        Detail = "See the errors property for details.",
            //    //        Instance = context.HttpContext.Request.Path
            //    //    };

            //    //    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
            //    //    ///  can be replaced with BadRequestObjectResult

            //    //    return new UnprocessableEntityObjectResult(problemDetails)
            //    //    {
            //    //        ContentTypes = { "application/problem+json" }
            //    //    };
            //    //};


            //    options.SuppressModelStateInvalidFilter = true;
            //});


            builder.Services.AddDbContext<ProductContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            #region Versioning

            // services.AddApiVersioning(o => o.ApiVersionReader = new UrlSegmentApiVersionReader());
            // services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));

            builder.Services.AddApiVersioning(o => o.ApiVersionReader = new MediaTypeApiVersionReader("v"));

            //builder.Services.AddApiVersioning(o => o.ApiVersionReader = 
            //    ApiVersionReader.Combine(new QueryStringApiVersionReader(),
            //        new HeaderApiVersionReader("api-version"),
            //    new MediaTypeApiVersionReader("v")));


            //defaults to api-version
            //builder.Services.AddApiVersioning( 
            //    options => options.ApiVersionReader = new QueryStringApiVersionReader());

            //?v=2.0
            //builder.Services.AddApiVersioning(
            //    options => options.ApiVersionReader = new QueryStringApiVersionReader("v"));

         

            builder.Services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(2, 0);
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
                //o.ErrorResponses = new IErrorResponseProvider[]
                //{
                //    new Api(){}
                //};
            });

            #endregion


            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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


            app.MapControllers();

            app.Run();
        }
    }
}
