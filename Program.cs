using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServicePortal.Application.Services;
using ServicePortal.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using ServicePortal.Modules.User.Services;
using ServicePortal.Modules.Auth.Services;
using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Enums;
using ServicePortal.Modules.Auth.Interfaces;
using ServicePortal.Modules.User.Interfaces;
using ServicePortal.Modules.Role.Interfaces;
using ServicePortal.Modules.Role.Services;
using ServicePortal.Modules.Deparment.Interfaces;
using ServicePortal.Modules.Deparment.Services;
using ServicePortal.Common.Middleware;
using Serilog;
using ServicePortal.Infrastructure.BackgroundServices;
using ServicePortal.Modules.LeaveRequest.Interfaces;
using ServicePortal.Modules.LeaveRequest.Services;
using ServicePortal.Modules.LeaveRequestStep.Services;
using ServicePortal.Modules.LeaveRequestStep.Interfaces;

namespace ServicePortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Config Serilog

            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", DateTimeOffset.UtcNow.ToString("yyyy"), DateTimeOffset.UtcNow.ToString("MM"));

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            var logFile = Path.Combine(logDir, "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: logFile,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ===========> {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    shared: true
                )
                .CreateLogger();

            #endregion

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks();

            builder.Host.UseSerilog();

            #region Config SqlServer


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("StringConnectionDb"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            #endregion

            #region DI

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IRoleService, RoleService>();

            //builder.Services.AddScoped<IPositionService, PositionService>();

            builder.Services.AddScoped<IDepartmentService, DepartmentService>();

            //builder.Services.AddScoped<ITeamService, Teamservice>();

            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

            builder.Services.AddScoped<ILeaveRequestStepService, LeaveRequestStepService>();

            builder.Services.AddScoped<JwtService>();

            #endregion

            // Add services to the container.
            builder.Services.AddControllers();

            #region Multiple language

            builder.Services.AddLocalization(options => options.ResourcesPath = "Application/Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("vi"),
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            #endregion

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Cors

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowPrivate",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins ?? ["http://localhost:5173"])
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });

            #endregion

            #region JWT

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        FileHelper.WriteLog(TypeErrorEnum.ERROR,$"JWT lỗi: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },

                    OnMessageReceived = context =>
                    {
                        var token = context.HttpContext.Request.Cookies["access_token"];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            #endregion

            #region Configure validation

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            Errors = e.Value!.Errors.Select(e => e.ErrorMessage)
                        });

                    return new UnprocessableEntityObjectResult(new
                    {
                        status = 422,
                        message = "Validation failed",
                        errors = errors
                    });
                };
            });

            #endregion

            builder.Services.AddHostedService<LogCleanupService>();

            var app = builder.Build();

            app.MapHealthChecks("/health");

            //when run app, excute class db seeder
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                DbSeeder.SeedAsync(dbContext).GetAwaiter().GetResult();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowPrivate");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Add localization middleware
            app.UseRequestLocalization();

            //add global exception
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.MapControllers();

            //default redirect to swagger
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger/index.html", permanent: false);
                    return;
                }
                await next();
            });

            app.Run();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    FileHelper.WriteLog(TypeErrorEnum.ERROR, $"[UnhandledException] {ex.Message}\n{ex.StackTrace}");
                }
            };

            FileHelper.CleanupOldLogs();
        }
    }
}
