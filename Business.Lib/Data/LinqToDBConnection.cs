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

        public LinqToDBConnection(LinqToDB.DataProvider.IDataProvider provider, string conString)
            : base(provider, conString) { }

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

        public void BulkCopy<T>(System.Collections.Generic.IEnumerable<T> source)
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
