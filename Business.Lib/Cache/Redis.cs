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
    public class Redis
    {
        #region Init

        static Redis()
        {
            System.Threading.ThreadPool.SetMinThreads(300, 300);
        }

        public static StackExchange.Redis.IDatabase Init(string host, int port = 6379, string password = null, int keepAlive = 180, int connectTimeout = 5000, bool allowAdmin = true, bool abortOnConnectFail = false, int db = -1, object asyncState = null)
        {
            ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions { EndPoints = { { host, port } }, Password = password, KeepAlive = keepAlive, ConnectTimeout = connectTimeout, AllowAdmin = allowAdmin, AbortOnConnectFail = abortOnConnectFail };

            return Connection.GetDatabase(db, asyncState);
        }

        public static void Init(StackExchange.Redis.ConfigurationOptions cfg)
        {
            ConfigurationOptions = cfg;
        }

        public static StackExchange.Redis.ConfigurationOptions ConfigurationOptions;

        static readonly System.Lazy<StackExchange.Redis.ConnectionMultiplexer> lazyConnection = new System.Lazy<StackExchange.Redis.ConnectionMultiplexer>(() => { return StackExchange.Redis.ConnectionMultiplexer.Connect(ConfigurationOptions); });

        public static StackExchange.Redis.ConnectionMultiplexer Connection { get { return lazyConnection.Value; } }

        public static StackExchange.Redis.IDatabase Cache { get { return Connection.GetDatabase(); } }

        public static StackExchange.Redis.IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            return Connection.GetDatabase(db, asyncState);
        }

        #endregion

        public static StackExchange.Redis.RedisKey[] RedisKey(string[] key)
        {
            return key.Where(c => !System.String.IsNullOrEmpty(c)).Distinct(System.StringComparer.InvariantCultureIgnoreCase).Select<string, StackExchange.Redis.RedisKey>(c => c).ToArray();
        }

        public static StackExchange.Redis.RedisValue RedisValue(dynamic value)
        {
            if (System.Object.Equals(null, value)) { return StackExchange.Redis.RedisValue.EmptyString; }
            return (StackExchange.Redis.RedisValue)value;
        }

        public static StackExchange.Redis.HashEntry HashEntry(StackExchange.Redis.RedisValue name, dynamic value)
        {
            return new StackExchange.Redis.HashEntry(name, RedisValue(value));
        }
    }
}
