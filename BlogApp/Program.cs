using System.Text;
using System.Text.Json.Serialization;
using BlogApp.Application.IRepositories;
using BlogApp.Application.IServices;
using BlogApp.Application.Mapper;
using BlogApp.Application.MiddleWare;
using BlogApp.Application.Service;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models;
using BlogApp.Infrastructure.ExternalServices;
using BlogApp.Infrastructure.ExternalServices.Impl;
using BlogApp.Infrastructure.ExternalServices.Interface;
using BlogApp.Infrastructure.ExternalServices.Kafka;
using BlogApp.Infrastructure.ExternalServices.Minio;
using BlogApp.Infrastructure.Persistence;
using BlogApp.Infrastructure.Repositories;
using CloudinaryDotNet;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var minioConfig = builder.Configuration.GetSection("Minio");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:3000"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // nếu có cookie / auth
        });
});

// Logging serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IBlogSuggestService, BlogSuggestService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHostedService<MinioStartupService>();


// Repo
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Dem nay e dep lam",
        Version = "v1",
        Description = "API demo project"
    });
    // Thêm security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer {token}'"
    });

    // Thêm requirement cho endpoint
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
    
});

// Đăng ký AutoMapper, truyền vào kiểu bất kỳ từ profile của bạn
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Cấu hình connection string
/*
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
*/

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 2)) // version MySQL của bạn
    )
);

// Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

// Mail
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    var account = new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret
    );

    return new Cloudinary(account);
});

// security
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

/*builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5230); // HTTP
    options.ListenAnyIP(7184, listenOptions => listenOptions.UseHttps()); // HTTPS
});*/

// Elasticsearch
builder.Services.AddSingleton(new ElasticsearchClient(
    new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("blogs")
));

builder.Services.AddSingleton<BlogIndexInitializer>();

// Minio
builder.Services.AddSingleton<IMinioClient>(_ =>
{
    return new MinioClient()
        .WithEndpoint(minioConfig["Endpoint"])
        .WithCredentials(
            minioConfig["AccessKey"],
            minioConfig["SecretKey"]
        )
        .WithSSL(minioConfig.GetValue<bool>("UseSSL"))
        .Build();
});
builder.Services.Configure<MinioOptions>(minioConfig);

// kafka
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<BlogCreatedConsumer>();

// builder.Services.AddSingleton<ElasticsearchService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    var initializer = scope.ServiceProvider
        .GetRequiredService<BlogIndexInitializer>();

    await initializer.InitAsync();
    db.Database.Migrate();

    // SEED ADMIN
    if (!db.Users.Any(u => u.Role == UserRole.Admin))
    {
        var admin = new User
        {
            UserName = "admin",
            Email = "tqdinhtt@gmail.com",
            FirstName = "System",
            LastName = "Admin",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        admin.Password = passwordHasher.HashPassword(admin, "123456");

        db.Users.Add(admin);
        db.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
    
app.Run();
