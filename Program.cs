
using RestAPICore.DB;
using RestAPICore.Repositories;
using RestAPICore.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

RestAPICore.Auth.AccessToken.Configure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthRepository,AuthRepository>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<MySqlGDSDB>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
