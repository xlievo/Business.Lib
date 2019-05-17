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

    public interface IConnection : System.IDisposable, ITransaction
    {
        System.Data.IDbCommand CreateCommand();

        int Save<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Save<T>(T obj);

        int SaveWithInt32Identity<T>(T obj);

        long SaveWithInt64Identity<T>(T obj);

        int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj);

        int SaveOrUpdate<T>(T obj);

        int Update<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Update<T>(T obj);

        int Delete<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Delete<T>(T obj);
    }

    public interface IEntitys
    {
        IEntity Entity { get; }
    }

    /*
    public interface IData2
    {
        int Save<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Save<T>(T obj);

        int SaveWithInt32Identity<T>(T obj);

        long SaveWithInt64Identity<T>(T obj);

        int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj);

        int SaveOrUpdate<T>(T obj);

        int Update<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Update<T>(T obj);

        int Delete<T>(System.Collections.Generic.IEnumerable<T> obj);

        int Delete<T>(T obj);
    }
    */

    public interface IData
    {
        IConnection GetConnection([System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Save<T>(System.Collections.Generic.IEnumerable<T> obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Save<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int SaveWithInt32Identity<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        long SaveWithInt64Identity<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int SaveOrUpdate<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Update<T>(System.Collections.Generic.IEnumerable<T> obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Update<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Delete<T>(System.Collections.Generic.IEnumerable<T> obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);

        int Delete<T>(T obj, [System.Runtime.CompilerServices.CallerMemberName] string callMethod = null);
    }

    public interface IEntity
    {
        System.Linq.IQueryable<T> Get<T>() where T : class, new();
    }
}
