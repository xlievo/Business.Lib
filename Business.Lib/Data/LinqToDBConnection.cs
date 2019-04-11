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

namespace Business.Data
{
    using LinqToDB;

    /// <summary>
    /// the a LinqToDBConnection
    /// </summary>
    /// <typeparam name="IEntity"></typeparam>
    public abstract class LinqToDBConnection<IEntity> : LinqToDB.Data.DataConnection, IConnection
        where IEntity : class, Data.IEntity
    {
        static int ForEach<T>(System.Collections.Generic.IEnumerable<T> obj, System.Func<T, int> func)
        {
            var count = 0;

            foreach (var item in obj)
            {
                var result = func(item);

                if (-1 == result)
                {
                    count = result;
                    break;
                }
                else
                {
                    count += result;
                }
            }

            return count;
        }

        public LinqToDBConnection() { }

        public LinqToDBConnection(string configuration) : base(configuration) { }

        public LinqToDBConnection(string providerName, string connectionString) : base(providerName, connectionString) { }

        public LinqToDBConnection(LinqToDB.DataProvider.IDataProvider provider, string conString) : base(provider, conString) { }

        public abstract IEntity Entity { get; }

        Data.IEntity IConnection.Entity
        {
            get { return Entity; }
        }

        public new void BeginTransaction()
        {
            base.BeginTransaction();
        }

        public new void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            base.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            base.CommitTransaction();
        }

        public void Rollback()
        {
            base.RollbackTransaction();
        }

        public int Save<T>(System.Collections.Generic.IEnumerable<T> obj) => this.ExecutePack(() => ForEach(obj, item => DataExtensions.Insert(this, item)));

        public int Save<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.Insert(this, obj));
        }

        public int SaveWithInt32Identity<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.InsertWithInt32Identity(this, obj));
        }

        public long SaveWithInt64Identity<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.InsertWithInt64Identity(this, obj));
        }

        public int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return this.ExecutePack(() => ForEach(obj, item => DataExtensions.InsertOrReplace(this, item)));
        }

        public int SaveOrUpdate<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.InsertOrReplace(this, obj));
        }

        public int Update<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return this.ExecutePack(() => ForEach(obj, item => DataExtensions.Update(this, item)));
        }

        public int Update<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.Update(this, obj));
        }

        public int Delete<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return this.ExecutePack(() => ForEach(obj, item => DataExtensions.Delete(this, item)));
        }

        public int Delete<T>(T obj)
        {
            return this.ExecutePack(() => DataExtensions.Delete(this, obj));
        }

        public void BulkCopy<T>(System.Collections.Generic.IEnumerable<T> source) where T : class
        {
            LinqToDB.Data.DataConnectionExtensions.BulkCopy(this, source);
        }

        public new void Dispose()
        {
            base.DisposeCommand();
            base.Dispose();
            if (null != base.Transaction) { base.Transaction.Dispose(); }
            if (null != base.Connection) { base.Connection.Dispose(); }
        }
    }
}

namespace LinqToDB
{
    using System.Linq;

    public abstract class Entitys : LinqToDB.Data.DataConnection, Business.Data.IEntity
    {
        public Entitys() { }

        public Entitys(string configuration) : base(configuration) { }

        public Entitys(string providerName, string connectionString) : base(providerName, connectionString) { }

        public Entitys(DataProvider.IDataProvider dataProvider, string connectionString) : base(dataProvider, connectionString) { }

        public virtual System.Linq.IQueryable<T> Get<T>() where T : class, new() => this.GetTable<T>();
    }

    public class ConnectionStringSettings : LinqToDB.Configuration.IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// LinqToDB.ProviderName
        /// </summary>
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    /// <summary>
    /// T4: NamespaceName = "DataModels"; DataContextName = "Model"; BaseDataContextClass = "LinqToDB.Entitys"; PluralizeDataContextPropertyNames = false;
    /// </summary>
    public class LinqToDBSection : LinqToDB.Configuration.ILinqToDBSettings
    {
        readonly System.Collections.Generic.IEnumerable<ConnectionStringSettings> connectionStringSettings;

        /// <summary>
        /// NamespaceName = "DataModels"; DataContextName = "Model"; BaseDataContextClass = "LinqToDB.Entitys"; PluralizeDataContextPropertyNames = false;
        /// </summary>
        /// <param name="connectionStringSettings"></param>
        /// <param name="defaultConfiguration"></param>
        public LinqToDBSection(System.Collections.Generic.IEnumerable<ConnectionStringSettings> connectionStringSettings, string defaultConfiguration = null)
        {
            this.connectionStringSettings = connectionStringSettings;
            var first = connectionStringSettings?.FirstOrDefault();
            DefaultConfiguration = defaultConfiguration ?? first?.Name;
            DefaultDataProvider = first?.ProviderName;
        }

        public System.Collections.Generic.IEnumerable<LinqToDB.Configuration.IDataProviderSettings> DataProviders => System.Linq.Enumerable.Empty<LinqToDB.Configuration.IDataProviderSettings>();
        /// <summary>
        /// Key
        /// </summary>
        public string DefaultConfiguration { get; private set; }
        /// <summary>
        /// LinqToDB.ProviderName
        /// </summary>
        public string DefaultDataProvider { get; private set; }

        public System.Collections.Generic.IEnumerable<LinqToDB.Configuration.IConnectionStringSettings> ConnectionStrings
        {
            get { foreach (var item in connectionStringSettings) { yield return item; } }
        }
    }

    //public class Data2<IConnection> : Business.Data.DataBase<Business.Data.IConnection>
    //    where IConnection : class, Business.Data.IConnection
    //{
    //    readonly System.Func<Business.Data.IConnection> creat;

    //    public Data2(System.Func<Business.Data.IConnection> creat) => this.creat = creat;

    //    static Data2() => LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

    //    public override Business.Data.IConnection GetConnection() => creat();
    //}

    //public class DataConnection : Business.Data.LinqToDBConnection<DataModels.Model>
    //{
    //    public DataConnection(string configuration) : base(configuration) { }
    //    public override DataModels.Model Entity { get => new DataModels.Model(this.ConfigurationString); }
    //}
}