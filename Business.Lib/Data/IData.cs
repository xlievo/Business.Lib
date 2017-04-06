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
    public interface ITransaction : System.IDisposable
    {
        System.Data.IDbTransaction Transaction { get; }

        void BeginTransaction();

        void BeginTransaction(System.Data.IsolationLevel isolationLevel);

        void Commit();

        void Rollback();
    }

    public interface IConnection : System.IDisposable, ITransaction, IData2
    {
        System.Data.IDbCommand CreateCommand();

        IEntity Entity
        {
            get;
        }
    }

    public interface IData2
    {
        int Save<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Save<T>(T obj);

        int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj);

        int SaveOrUpdate<T>(T obj);

        int Update<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Update<T>(T obj);

        int Delete<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Delete<T>(T obj);
    }

    public interface IData : IData2
    {
        IConnection GetConnection();
    }

    public interface IEntity
    {
        System.Linq.IQueryable<T> Get<T>() where T : class, new();
    }
}
