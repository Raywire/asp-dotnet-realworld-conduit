using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conduit.Data;
using Conduit.DTOs.Responses;
using Conduit.Extensions;
using Conduit.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Npgsql;

namespace Conduit
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
            var jwtSecret = Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET");

            // MSSQL Connection
            var connection = Configuration.GetConnectionString("ConduitConnection");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection);
            builder.DataSource = Configuration["DataSource"] ?? Environment.GetEnvironmentVariable("MSSQL_DATASOURCE");
            builder.InitialCatalog = Configuration["InitialCatalog"] ?? Environment.GetEnvironmentVariable("MSSQL_DB");
            builder.UserID = Configuration["UserID"] ?? Environment.GetEnvironmentVariable("MSSQL_USER");
            builder.Password = Configuration["Password"] ?? Environment.GetEnvironmentVariable("MSSQL_PASSWORD");
            builder.TrustServerCertificate = true;

            // services.AddDbContext<ConduitContext>(options => options.UseSqlServer(builder.ConnectionString));

            // Postgres Connection
            var postgresConnection = Configuration.GetConnectionString("ConduitConnection");
            NpgsqlConnectionStringBuilder postgresBuilder = new NpgsqlConnectionStringBuilder(postgresConnection);
            postgresBuilder.Host = Configuration["PostgresDataSource"] ?? Environment.GetEnvironmentVariable("POSTGRES_DATASOURCE");
            postgresBuilder.Username = Configuration["PostgresUserID"] ?? Environment.GetEnvironmentVariable("POSTGRES_USER");
            postgresBuilder.Password = Configuration["PostgresPassword"] ?? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            postgresBuilder.Database = Configuration["PostgresDatabase"] ?? Environment.GetEnvironmentVariable("POSTGRES_DB");
            postgresBuilder.Port = 5432;
            postgresBuilder.SslMode = SslMode.Prefer;
            postgresBuilder.TrustServerCertificate = true;
            postgresBuilder.Pooling = true;

            // services.AddDbContext<ConduitContext>(options => options.UseNpgsql(postgresBuilder.ConnectionString));

            var provider = Configuration["DbProvider"] ?? Environment.GetEnvironmentVariable("DB_PROVIDER") ?? "Postgres";

            services.AddDbContext<ConduitContext>(
                options => _ = provider switch
                {
                    "Postgres" => options.UseNpgsql(
                        postgresBuilder.ConnectionString,
                        x => x.MigrationsAssembly("Migrations.Postgres")),

                    "SqlServer" => options.UseSqlServer(
                        builder.ConnectionString,
                        x => x.MigrationsAssembly("Migrations.SqlServer")),

                    _ => throw new Exception($"Unsupported provider: {provider}")
                });

            services.ConfigureCors();

            services.AddControllers()
            .AddNewtonsoftJson(s => {
                s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    // add additional info not added by default
                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all arguments were correctly
                    // found/parsed we're dealing with validation errors
                    if ((context.ModelState.ErrorCount > 0) && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        return new UnprocessableEntityObjectResult(new ValidationErrorResponse()
                        {
                            Errors = problemDetails.Errors.ToDictionary(x => x.Key.Split(".").LastOrDefault(), x => x.Value),
                            Success = false
                        });
                    }

                    // if one of the arguments wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparseable input
                    return new BadRequestObjectResult(new ValidationErrorResponse()
                    {
                        Errors = problemDetails.Errors.ToDictionary(x => x.Key.Split(".").LastOrDefault(), x => x.Value),
                        Success = false
                    });
                };
            });

            services.AddTransient<IMailService, CloudMailService>();

            services.AddScoped<IConduitRepository, ConduitRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew = TimeSpan.FromMinutes(0)
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ASP DotNet Realworld Conduit", Version = "v1" });
                // To Enable authorization using Swagger (JWT)  
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
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

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Run migrations in code
                // DatabaseManagementService.MigrationInitialisation(app);
            }


            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP DotNet Realworld Conduit v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCustomExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
