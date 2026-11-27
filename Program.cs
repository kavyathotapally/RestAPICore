
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi;
using RestAPICore.Authentication;
using RestAPICore.DB;
using RestAPICore.Repositories;
using RestAPICore.Repositories.AuthRepo;
using RestAPICore.Repositories.CityRepo;
using RestAPICore.Services;
using RestAPICore.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

RestAPICore.Common.AccessToken.Configure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<MySqlGDSDB>();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddScoped<ICityRepository, CityRepository>();
//builder.Services.AddScoped<ICityRepo, CityRepo>();

var masterConnection = builder.Configuration.GetConnectionString("SE_REDIS_GDS_MASTER");
if (string.IsNullOrWhiteSpace(masterConnection))
{
    throw new InvalidOperationException("Redis configuration 'SE_REDIS_GDS_MASTER' not found.");
}

builder.Services.AddSingleton<IRedisConnectionFactory, REDIS>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => sp.GetRequiredService<IRedisConnectionFactory>().GetMultiplexer(
        REDIS.RedisServer.GDS,
        REDIS.SlaveOrMaster.MASTER));

// Register Redis Cache
// Uses the master connection for cache operations
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = masterConnection;
    options.InstanceName = "RestAPICore:";
});
builder.Services.AddAuthentication("Custom")
    .AddScheme<AuthenticationSchemeOptions,
        CustomAuthenticationHandler>(
            "Custom", options => { });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("AccessToken",
        new OpenApiSecurityScheme
        {
            Name = "access-token",
            Description = "Enter your access token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "AccessToken", document)] = new List<string>()
        });
});


builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
