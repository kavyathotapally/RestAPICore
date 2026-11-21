
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RestAPICore.DB
{
    public interface IRedisConnectionFactory : IDisposable
    {
        IDatabase GetDatabase(
            REDIS.RedisServer redisServer = REDIS.RedisServer.GDS,
            REDIS.SlaveOrMaster slaveOrMaster = REDIS.SlaveOrMaster.SLAVE1);

        IConnectionMultiplexer GetMultiplexer(
            REDIS.RedisServer redisServer = REDIS.RedisServer.GDS,
            REDIS.SlaveOrMaster slaveOrMaster = REDIS.SlaveOrMaster.SLAVE1);
    }

    public sealed class REDIS : IRedisConnectionFactory
    {
        public enum RedisServer
        {
            GDS = 0,
            DIST_IT = 2,
            PROV_DB = 3,
            FEED = 4,
            SEO = 5,
            BRIDGE = 6
        }

        public enum SlaveOrMaster
        {
            MASTER = 0,
            SLAVE1 = 1
        }

        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, IConnectionMultiplexer> _multiplexers = new();
        private readonly object _syncRoot = new();

        public REDIS(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IDatabase GetDatabase(
            RedisServer redisServer = RedisServer.GDS,
            SlaveOrMaster slaveOrMaster = SlaveOrMaster.SLAVE1)
        {
            return GetMultiplexer(redisServer, slaveOrMaster).GetDatabase();
        }

        public IConnectionMultiplexer GetMultiplexer(
            RedisServer redisServer = RedisServer.GDS,
            SlaveOrMaster slaveOrMaster = SlaveOrMaster.SLAVE1)
        {
            string settingKey = GetSettingKey(redisServer, slaveOrMaster);

            lock (_syncRoot)
            {
                if (!_multiplexers.TryGetValue(settingKey, out IConnectionMultiplexer? multiplexer))
                {
                    multiplexer = InitMultiplexer(settingKey);
                    _multiplexers.Add(settingKey, multiplexer);
                }

                return multiplexer;
            }
        }

        private string GetSettingKey(RedisServer redisServer, SlaveOrMaster slaveOrMaster)
        {
            return redisServer switch
            {
                RedisServer.GDS when slaveOrMaster == SlaveOrMaster.MASTER => "SE_REDIS_GDS_MASTER",
                RedisServer.GDS => "SE_REDIS_GDS_SLAVE1",
                RedisServer.DIST_IT => "SE_REDIS_DIST_IT",
                RedisServer.PROV_DB => "SE_REDIS_PROV_DB",
                RedisServer.FEED => "SE_REDIS_FEED",
                RedisServer.SEO => "SE_REDIS_SEO",
                RedisServer.BRIDGE => "SE_REDIS_BRIDGE",
                _ => throw new ArgumentException("Unsupported Redis server: " + redisServer, nameof(redisServer))
            };
        }

        private IConnectionMultiplexer InitMultiplexer(string settingKey)
        {
            string? connString = _configuration[$"Redis:{settingKey}"];
            if (string.IsNullOrWhiteSpace(connString))
            {
                connString = _configuration.GetConnectionString(settingKey);
            }

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new InvalidOperationException(
                    $"Redis configuration '{settingKey}' not found.");
            }

            ConfigurationOptions options = ConfigurationOptions.Parse(connString);
            options.ClientName = GetClientName();
            options.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(options);
        }

        private string GetClientName()
        {
            string clientName = _configuration["REDIS_CLIENT_NAME"];

            if (string.IsNullOrWhiteSpace(clientName))
            {
                clientName = "DEFAULT";
            }

            string pid = Process.GetCurrentProcess().Id.ToString();
            string randomId = Guid.NewGuid().ToString("N")[..8];

            return $"se-{clientName}-pid{pid}-rnd{randomId}";
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                foreach (IConnectionMultiplexer multiplexer in _multiplexers.Values)
                {
                    multiplexer.Dispose();
                }

                _multiplexers.Clear();
            }
        }
    }
}
