
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using OrdersApi.Data;
using OrdersApi.Data.Repositories;
using OrdersApi.Infrastructure.Mappings;
using OrdersApi.Service.Clients;
using OrdersApi.Services;
using Polly;
using System.Threading.RateLimiting;

namespace OrdersApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddAutoMapper(typeof(OrderProfileMapping).Assembly);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>()
            //    .AddStandardResilienceHandler();
            builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>()
               .AddResilienceHandler("my-pipeline", builder =>
               {
                   // Refer to https://www.pollydocs.org/strategies/retry.html#defaults for retry defaults
                   //builder.AddRetry(new HttpRetryStrategyOptions
                   //{
                   //    MaxRetryAttempts = 4,
                   //    Delay = TimeSpan.FromSeconds(2),
                   //    BackoffType = DelayBackoffType.Exponential
                   //});

                   // Refer to https://www.pollydocs.org/strategies/timeout.html#defaults for timeout defaults
                   // builder.AddTimeout(TimeSpan.FromSeconds(5));
                   //builder.AddConcurrencyLimiter(1, 2);
            
                   //builder.AddRateLimiter(new SlidingWindowRateLimiter(
                //    new SlidingWindowRateLimiterOptions
                //    {
                //        QueueLimit = 10, //alows 100 requests in the queue
                //        SegmentsPerWindow =2,
                //        PermitLimit = 2,//alows 1 request per minute
                //        Window = TimeSpan.FromMinutes(1)
                //    }));


               });


            var app = builder.Build();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
