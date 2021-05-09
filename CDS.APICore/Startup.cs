using System;
using System.Collections.Generic;

using CDS.APICore.Bussiness;
using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities.Enums;
using CDS.APICore.Helpers;
using CDS.APICore.Jobs;
using CDS.APICore.Middlewares;
using CDS.APICore.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Quartz;

namespace CDS.APICore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            _ = services.AddCors();
            _ = services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            _ = services.AddSwaggerGen(setup =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                      { jwtSecurityScheme, Array.Empty<string>() }
                });

            });


            _ = services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();

                var jobKey = new JobKey(nameof(DailyAgregationJob));

                _ = q.AddJob<DailyAgregationJob>(opts => opts.WithIdentity(jobKey));

                _ = q.AddTrigger(opts => opts
                    .ForJob(jobKey) // link to the DailyAgregationJob
                    .WithIdentity("DailyAgregationJob-Catering-trigger") // give the trigger a unique name
                    .WithCronSchedule("1 * * * * ?")
                    .UsingJobData(new JobDataMap(new Dictionary<string, AgregationBy>() { { "AgregationType", AgregationBy.Catering } })));

                //_ = q.AddTrigger(opts => opts
                //    .ForJob(jobKey) // link to the DailyAgregationJob
                //    .WithIdentity("DailyAgregationJob-Customer-trigger") // give the trigger a unique name
                //    .WithCronSchedule("0/5 * * * * ?")
                //    .UsingJobData(new JobDataMap(new Dictionary<string, AgregationBy>() { { "AgregationType", AgregationBy.Customer } }))); 

                //_ = q.AddTrigger(opts => opts
                //    .ForJob(jobKey) // link to the DailyAgregationJob
                //    .WithIdentity("DailyAgregationJob-CustomerCatering-trigger") // give the trigger a unique name
                //    .WithCronSchedule("0/5 * * * * ?")
                //    .UsingJobData(new JobDataMap(new Dictionary<string, AgregationBy>() { { "AgregationType", AgregationBy.CustomerCatering } }))); 
            });

            _ = services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            _ = services.Configure<AppSettings>(this.Configuration.GetSection("AppSettings"));

            _ = services.AddSingleton<IDbHelper>(x => new SqlDbHelper("Data Source = .; Initial Catalog = NwPrdb;Integrated Security= true"));

            _ = services.AddScoped<IAccountManager, AccountManager>();
            _ = services.AddScoped<ICateringManager, CateringManager>();
            _ = services.AddScoped<ILocationManager, LocationManager>();
            _ = services.AddScoped<ICustomerManager, CustomerManager>();
            _ = services.AddScoped<IOrderManager, OrderManager>();
            _ = services.AddScoped<ITimeManager, TimeManager>();
            _ = services.AddScoped<IReflectionHelper, ReflectionHelper>();
            _ = services.AddScoped<ITagManager, TagManager>();
            _ = services.AddScoped<IAgregationManager, AgregationManager>();

            _ = services.AddScoped<IHashService, HashService>();
            _ = services.AddScoped<IAccountService, AccountService>();
            _ = services.AddScoped<ICateringService, CateringService>();
            _ = services.AddScoped<ILocationService, LocationService>();
            _ = services.AddScoped<IOrderService, OrderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Catering Data Service API"));

            _ = app.UseRouting();

            _ = app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            // custom jwt auth middleware
            _ = app.UseMiddleware<JwtMiddleware>();
            _ = app.UseMiddleware<ErrorHandlerMiddleware>();

            _ = app.UseEndpoints(x => x.MapControllers());
        }
    }
}