using Fashion.Api.Core.Interfaces;
using Fashion.Api.Helpers;
using Fashion.Api.Infrastructure.Configurations;
using Fashion.Api.Infrastructure.Data;
using Fashion.Api.Infrastructure.Services;
using Fashion.Api.Middlewares;
using Fashion.Api.Services;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Fashion.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOutputCache();

            // =========================
            // CONTROLLERS
            // =========================

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                        .Add(new JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpClient();

            // =========================
            // CORS (PRODUCTION READY)
            // =========================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // =========================
            // SWAGGER
            // =========================

            builder.Services.AddSwaggerGen();

            // =========================
            // 🔥 FIREBASE (ENV BASED)
            // =========================

            var firebaseConfig = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

            if (!string.IsNullOrWhiteSpace(firebaseConfig))
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(firebaseConfig)
                    });
                }
            }

            // =========================
            // 🔥 DATABASE (RENDER READY)
            // =========================

var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(dbUrl))
    {
        var connString = dbUrl;

        // Fix SSL properly
        if (!connString.Contains("SSL Mode"))
        {
            connString += ";SSL Mode=Require;Trust Server Certificate=true";
        }

        options.UseNpgsql(connString);
    }
    else
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
        );
    }
});

            // =========================
            // CONFIG
            // =========================

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt")
            );

            builder.Services.Configure<StripeSettings>(
                builder.Configuration.GetSection("Stripe")
            );

            builder.Services.Configure<RazorpaySettings>(
                builder.Configuration.GetSection("Razorpay")
            );

            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("Cloudinary")
            );

            // =========================
            // SERVICES
            // =========================

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IStripeService, StripeService>();
            builder.Services.AddScoped<IRazorpayService, RazorpayService>();
            builder.Services.AddScoped<IImageService, CloudinaryService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IDropService, DropService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<JwtTokenGenerator>();

            // =========================
            // JWT AUTH
            // =========================

            var jwt = builder.Configuration.GetSection("Jwt");

            var jwtKey = jwt["Key"] ?? throw new Exception("JWT Key is missing");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)
                        ),

                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // =========================
            // BUILD
            // =========================

            var app = builder.Build();

            // =========================
            // 🔥 AUTO DB MIGRATION
            // =========================

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // =========================
            // MIDDLEWARE
            // =========================

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            // =========================
            // 🔥 RENDER PORT FIX
            // =========================

            var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
            app.Urls.Add($"http://0.0.0.0:{port}");

            app.UseRouting();
            app.UseOutputCache();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}