/*==================================
             ########
            ##########

             ########
            ##########
          ##############
         #######  #######
        ######      ######
        #####        #####
        ####          ####
        ####   ####   ####
        #####  ####  #####
         ################
          ##############
==================================*/

using System.Linq;

namespace Business.Cache
{
    public class StackExchangeEx
    {
        #region Init

        //static Redis() => System.Threading.ThreadPool.SetMinThreads(300, 300);

        public static StackExchange.Redis.IDatabase Init(string host, int port = 6379, string password = null, int keepAlive = 60, int connectTimeout = 5000, bool allowAdmin = true, bool abortOnConnectFail = false, int db = -1, object asyncState = null)
        {
            ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions { EndPoints = { { host, port } }, Password = password, KeepAlive = keepAlive, ConnectTimeout = connectTimeout, AllowAdmin = allowAdmin, AbortOnConnectFail = abortOnConnectFail };

            return Connection.GetDatabase(db, asyncState);
        }

        public static void Init(StackExchange.Redis.ConfigurationOptions cfg) => ConfigurationOptions = cfg;

        public static StackExchange.Redis.ConfigurationOptions ConfigurationOptions;

        static readonly System.Lazy<StackExchange.Redis.ConnectionMultiplexer> lazyConnection = new System.Lazy<StackExchange.Redis.ConnectionMultiplexer>(() => StackExchange.Redis.ConnectionMultiplexer.Connect(ConfigurationOptions));

        public static StackExchange.Redis.ConnectionMultiplexer Connection { get => lazyConnection.Value; }

        public static StackExchange.Redis.IDatabase Cache { get => Connection.GetDatabase(); }

        public static StackExchange.Redis.IDatabase GetDatabase(int db = -1, object asyncState = null) => Connection.GetDatabase(db, asyncState);

        #endregion

        public static StackExchange.Redis.RedisKey[] RedisKey(string[] key)
        {
            return key.Where(c => !string.IsNullOrEmpty(c)).Distinct(System.StringComparer.InvariantCultureIgnoreCase).Select<string, StackExchange.Redis.RedisKey>(c => c).ToArray();
        }

        public static StackExchange.Redis.RedisValue RedisValue(dynamic value)
        {
            if (object.Equals(null, value)) { return StackExchange.Redis.RedisValue.EmptyString; }
            return (StackExchange.Redis.RedisValue)value;
        }

        public static StackExchange.Redis.HashEntry HashEntry(StackExchange.Redis.RedisValue name, dynamic value)
        {
            return new StackExchange.Redis.HashEntry(name, RedisValue(value));
        }
    }

    //public class CSRedisEx
    //{
    //    /// <summary>
    //    ///"127.0.0.1:6379,password=123,defaultDatabase=13,poolsize=50,ssl=false,writeBuffer=10240,prefix=key前辍"
    //    /// </summary>
    //    /// <param name="connectionString"></param>
    //    public static void Init(string connectionString) => RedisHelper.Initialization(new CSRedis.CSRedisClient(connectionString));

    //    //public static bool KeyExpire(string key, System.TimeSpan? expiry) => RedisHelper.Expire(key, (int)Utils.Help2.GetValue(expiry?.TotalSeconds, -1));

    //    //public static bool KeyExists(string key) => RedisHelper.Exists(key);

    //    //public static long KeyDelete(string key) => RedisHelper.Del(key);

    //    //public static bool StringSet(string key, dynamic value, System.TimeSpan? expiry = null, CSRedis.RedisExistence? when = null) => RedisHelper.Set(key, value, (int)Utils.Help2.GetValue(expiry?.TotalSeconds, -1), when);

    //    //public static T StringGet<T>(string key) => RedisHelper.Get<T>(key);
    //}
}
