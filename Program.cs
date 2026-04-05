using Fashion.Api.Core.Interfaces;
using Fashion.Api.Helpers;
using Fashion.Api.Infrastructure.Configurations;
using Fashion.Api.Infrastructure.Data;
using Fashion.Api.Infrastructure.Services;
using Fashion.Api.Middlewares;
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
            // CORS
            // =========================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("NextJs", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // =========================
            // SWAGGER
            // =========================

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Fashion API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            });

            // =========================
            // 🔥 FIREBASE (SAFE)
            // =========================

            if (builder.Environment.IsDevelopment())
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var firebasePath = Path.Combine(
                        builder.Environment.ContentRootPath,
                        "firebase-service-account.json"
                    );

                    if (File.Exists(firebasePath))
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            Credential = GoogleCredential.FromFile(firebasePath)
                        });
                    }
                }
            }

            // =========================
            // 🔥 DATABASE (RENDER READY)
            // =========================

            var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(dbUrl))
                {
                    options.UseNpgsql(dbUrl + "?sslmode=require");
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
                            Encoding.UTF8.GetBytes(jwt["Key"]!)
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
            // MIDDLEWARE
            // =========================

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseOutputCache();

            app.UseHttpsRedirection();

            app.UseCors("NextJs");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}