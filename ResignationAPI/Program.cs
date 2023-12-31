using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResignationAPI.Mappings;
using ResignationAPI.Models;
using ResignationAPI.Repository;
using ResignationAPI.Repository.IRepository;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.AddScoped<IResignationRepository, ResignationRepository>();
builder.Services.AddScoped<ILoggingRepository, LoggingRepository>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromMinutes(1),
        RequireExpirationTime = false
    };
});

builder.Services.AddControllers();
//builder.Services.AddControllers()
//        .AddNewtonsoftJson(options => options.UseMemberCasing());



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorizationn header using the Bearer Scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options => options.AddPolicy(name: "CorsPolicy",
    policy =>
    {
        policy.WithOrigins().AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    }
));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
