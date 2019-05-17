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

using Business.Utils;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Business.Data
{
    /*
    public enum DataType
    {
        Undefined = 0,
        Char = 1,
        VarChar = 2,
        Text = 3,
        NChar = 4,
        NVarChar = 5,
        NText = 6,
        Binary = 7,
        VarBinary = 8,
        Blob = 9,
        Image = 10,
        Boolean = 11,
        Guid = 12,
        SByte = 13,
        Int16 = 14,
        Int32 = 15,
        Int64 = 16,
        Byte = 17,
        UInt16 = 18,
        UInt32 = 19,
        UInt64 = 20,
        Single = 21,
        Double = 22,
        Decimal = 23,
        Money = 24,
        SmallMoney = 25,
        Date = 26,
        Time = 27,
        DateTime = 28,
        DateTime2 = 29,
        SmallDateTime = 30,
        DateTimeOffset = 31,
        Timestamp = 32,
        Xml = 33,
        Variant = 34,
        VarNumeric = 35,
        Udt = 36,
    }
    */

    #region Paging Object

    public interface IPaging
    {
        dynamic Data { get; set; }

        int Length { get; set; }

        int CurrentPage { get; set; }

        int Count { get; set; }

        int CountPage { get; set; }
    }

    public struct Paging<T> : IPaging
    {
        public static implicit operator Paging<T>(string value) => Help2.TryJsonDeserialize<Paging<T>>(value);

        public static implicit operator Paging<T>(byte[] value) => Help2.TryBinaryDeserialize<Paging<T>>(value);

        public System.Collections.Generic.List<T> Data { get; set; }

        public int Length { get; set; }

        public int CurrentPage { get; set; }

        public int Count { get; set; }

        public int CountPage { get; set; }

        dynamic IPaging.Data { get => Data; set => Data = value; }

        public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(this);

        public byte[] ToBytes() => Help2.BinarySerialize(this);
    }

    public struct PagingInfo
    {
        public int Skip;
        public int Take;
        public int CurrentPage;
        public int CountPage;
    }

    public enum Order
    {
        Ascending = 1,
        Descending = 2
    }

    #endregion

    public struct DataParameter
    {
        public DataParameter(string name, object value) { this.Name = name; this.Value = value; }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }

    //[ProtoBuf.ProtoContract(SkipConstructor = true)]
    //public struct Paging<T>
    //{
    //    public static implicit operator Paging<T>(string value)
    //    {
    //        return Help2.JsonDeserialize<Paging<T>>(value);
    //    }
    //    public static implicit operator Paging<T>(byte[] value)
    //    {
    //        return Help2.ProtoBufDeserialize<Paging<T>>(value);
    //    }

    //    [ProtoBuf.ProtoMember(1, Name = "D")]
    //    [Newtonsoft.Json.JsonProperty(PropertyName = "D")]
    //    public System.Collections.Generic.IList<T> Data { get; set; }

    //    [ProtoBuf.ProtoMember(2, Name = "P")]
    //    [Newtonsoft.Json.JsonProperty(PropertyName = "P")]
    //    public int CurrentPage { get; set; }

    //    [ProtoBuf.ProtoMember(3, Name = "C")]
    //    [Newtonsoft.Json.JsonProperty(PropertyName = "C")]
    //    public int Count { get; set; }

    //    public override string ToString()
    //    {
    //        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    //    }

    //    public byte[] ToBytes()
    //    {
    //        return Help2.ProtoBufSerialize(this);
    //    }
    //}

    public static class DataConnectionEx
    {
        public static System.Data.IDbCommand GetCommand(this IConnection connection, string commandText, System.Data.IDbTransaction t = null, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;

            if (null != parameter)
            {
                foreach (var item in parameter)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = item.Name;
                    //p.Value = System.DateTime.MinValue.Equals(item.Value) ? System.DBNull.Value : item.Value ?? System.DBNull.Value;
                    p.Value = item.Value ?? System.DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }

            cmd.Transaction = t;
            return cmd;
        }

        public static int ExecuteNonQuery(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    return cmd.ExecuteNonQuery();
                }
            });
        }

        public static Result ExecuteScalar<Result>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    return (Result)cmd.ExecuteScalar();
                }
            }, minusOneExcep: false);
        }
        /*
        public static System.Collections.Generic.IList<TEntity> Execute<TEntity>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack<System.Collections.Generic.IList<TEntity>>(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        return new Utils.LightDataAccess.DataReaderToObjectMapper<TEntity>().ReadCollection(reader).ToList();
                    }
                }
            }, minusOneExcep: false);
        }

       public static TEntity ExecuteSingle<TEntity>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
       {
           return connection.ExecutePack<TEntity>(() =>
           {
               using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
               {
                   using (var reader = cmd.ExecuteReader())
                   {
                       return new Utils.LightDataAccess.DataReaderToObjectMapper<TEntity>().ReadSingle(reader);
                   }
               }
           }, minusOneExcep: false);
       }
        */

        public static System.Collections.Generic.IList<TEntity> Execute<TEntity>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.ToEntity<TEntity>();
                    }
                }
            }, minusOneExcep: false);
        }

        public static System.Collections.Generic.IList<TEntity> ToEntity<TEntity>(this System.Data.IDataReader reader) => new AutoMapper.MapperConfiguration(cfg =>
        {
            AutoMapper.Data.ConfigurationExtensions.AddDataReaderMapping(cfg);
            cfg.CreateMap<System.Data.IDataReader, TEntity>();
        }).CreateMapper().Map<System.Collections.Generic.IList<TEntity>>(reader);

        internal static Result ExecutePack<Result>(this IConnection connection, System.Func<Result> func, bool minusOneExcep = true)
        {
            bool isCreateTransaction = false;
            if (null == connection.Transaction) { connection.BeginTransaction(); isCreateTransaction = !isCreateTransaction; }

            try
            {
                var result = func();

                if (minusOneExcep && (typeof(Result).Equals(typeof(int)) || typeof(Result).Equals(typeof(long))))
                {
                    //var count = System.Convert.ToInt32(result);
                    if (object.Equals(-1, result))
                    {
                        connection.Rollback();
                        throw new System.Exception("Affected the number of records -1");
                    }
                }

                if (isCreateTransaction) { connection.Commit(); }
                return result;
            }
            catch (System.Exception ex) { if (null != connection.Transaction) { connection.Rollback(); } throw ex; }
            finally { if (isCreateTransaction && null != connection.Transaction) { connection.Transaction.Dispose(); } }
        }

        //public static Paging<T> GetPaging<T>(this IQueryable<T> query, int currentPage, int pageSize, int pageSizeMax = 30)
        //{
        //    var count = query.Count();
        //    if (0 == count) { return new Paging<T> { Data = new System.Collections.Generic.List<T>() }; }

        //    var p = Help2.GetPaging(count, currentPage, pageSize, pageSizeMax);

        //    var data = query.Skip(p.Skip).Take(p.Take).ToList();

        //    return new Paging<T> { Data = data, DataLength = data.Count, CurrentPage = p.CurrentPage, Count = count, CountPage = p.CountPage };
        //    //var _pageSize = System.Math.Min(pageSize, pageSizeMax);

        //    //var countPage = System.Convert.ToInt32(System.Math.Ceiling(System.Convert.ToDouble(count) / System.Convert.ToDouble(_pageSize)));

        //    //currentPage = currentPage < 0 ? 0 : currentPage > countPage ? countPage : currentPage;
        //    //if (currentPage <= 0 && countPage > 0) { currentPage = 1; }

        //    //return new Paging<T> { Data = query.Skip(_pageSize * (currentPage - 1)).Take(_pageSize).ToList(), CurrentPage = currentPage, Count = count };
        //}

        //public static Help2.Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, int currentPage, int count)
        //{
        //    return new Help2.Paging<T> { Data = data, CurrentPage = currentPage, Count = count };
        //}

        public static IQueryable<T> SkipRandom<T>(this IQueryable<T> query, int take = 0)
        {
            if (0 < take)
            {
                query = query.Skip(Help2.Random(query.Count() - take)).Take(take);
            }
            else
            {
                query = query.Skip(Help2.Random(query.Count()));
            }
            return query;
        }

        public static int InsertOrUpdate<T>(this IQueryable<T> target, System.Linq.Expressions.Expression<System.Func<T>> insertSetter, System.Linq.Expressions.Expression<System.Func<T, T>> onDuplicateKeyUpdateSetter, System.Linq.Expressions.Expression<System.Func<T>> keySelector) where T : class => LinqToDB.LinqExtensions.InsertOrUpdate(target as LinqToDB.ITable<T>, insertSetter, onDuplicateKeyUpdateSetter, keySelector);

        public static int InsertOrUpdate<T>(this IQueryable<T> target, System.Linq.Expressions.Expression<System.Func<T>> insertSetter, System.Linq.Expressions.Expression<System.Func<T>> keySelector) where T : class, new() => InsertOrUpdate(target as LinqToDB.ITable<T>, insertSetter, c => new T { }, keySelector);

        public static LinqToDB.ITable<T> AsTable<T>(this IQueryable<T> queryable) => queryable as LinqToDB.ITable<T>;

        #region Paging

        public static PagingInfo GetPaging(int count, int currentPage, int pageSize, int pageSizeMax = 50)
        {
            if (0 == count) { return default; }

            var _pageSize = System.Math.Min(pageSize, pageSizeMax);
            var _countPage = System.Convert.ToDouble(count) / System.Convert.ToDouble(_pageSize);
            var countPage = (double.IsNaN(_countPage) || double.IsPositiveInfinity(_countPage) || double.IsNegativeInfinity(_countPage)) ? 1 : System.Convert.ToInt32(System.Math.Ceiling(_countPage));

            currentPage = currentPage < 0 ? 0 : currentPage > countPage ? countPage : currentPage;
            if (currentPage <= 0 && countPage > 0) { currentPage = 1; }

            return new PagingInfo { Skip = _pageSize * (currentPage - 1), Take = _pageSize, CurrentPage = currentPage, CountPage = countPage };
        }

        public static Paging<T> GetPaging<T>(this IQueryable<T> query, int currentPage, int pageSize, int pageSizeMax = 50)
        {
            if (null == query) { throw new System.ArgumentNullException(nameof(query)); }

            var count = query.Count();
            if (0 == count) { return new Paging<T> { Data = new System.Collections.Generic.List<T>() }; }

            var p = GetPaging(count, currentPage, pageSize, pageSizeMax);

            var data = query.Skip(p.Skip).Take(p.Take).ToList();

            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = p.CurrentPage, Count = count, CountPage = p.CountPage };
        }

        public static Paging<T> GetPagingOrderBy<T, TKey>(this IQueryable<T> query, int currentPage, int pageSize, System.Linq.Expressions.Expression<System.Func<T, TKey>> keySelector, Order order = Order.Ascending, int pageSizeMax = 50)
        {
            if (null == query) { throw new System.ArgumentNullException(nameof(query)); }

            var count = query.Count();
            if (0 == count) { return new Paging<T> { Data = new System.Collections.Generic.List<T>() }; }

            var p = GetPaging(count, currentPage, pageSize, pageSizeMax);

            System.Collections.Generic.List<T> data = null;

            switch (order)
            {
                case Order.Ascending:
                    data = query.Skip(p.Skip).Take(p.Take).OrderBy(keySelector).ToList();
                    break;
                case Order.Descending:
                    data = query.Skip(p.Skip).Take(p.Take).OrderByDescending(keySelector).ToList();
                    break;
            }

            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = p.CurrentPage, Count = count, CountPage = p.CountPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, int currentPage = 0, int count = 0, int countPage = 0)
        {
            if (null == data) { throw new System.ArgumentNullException(nameof(data)); }

            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = currentPage, Count = count, CountPage = countPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, IPaging pagingObj)
        {
            if (null == data) { throw new System.ArgumentNullException(nameof(data)); }

            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = pagingObj.CurrentPage, Count = pagingObj.Count, CountPage = pagingObj.CountPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.IEnumerable<T> data, int currentPage = 0, int count = 0, int countPage = 0)
        {
            if (null == data) { throw new System.ArgumentNullException(nameof(data)); }

            var data2 = data.ToList();
            return new Paging<T> { Data = data2, Length = data2.Count, CurrentPage = currentPage, Count = count, CountPage = countPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.IEnumerable<T> data, IPaging pagingObj)
        {
            if (null == data) { throw new System.ArgumentNullException(nameof(data)); }

            var data2 = data.ToList();
            return new Paging<T> { Data = data2.ToList(), Length = data2.Count, CurrentPage = pagingObj.CurrentPage, Count = pagingObj.Count, CountPage = pagingObj.CountPage };
        }

        #endregion
    }

    public abstract class DataBase<IConnection> : IData
        where IConnection : class, Data.IConnection
    {
        static DataBase() => LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

        public abstract IConnection GetConnection([CallerMemberName] string callMethod = null);

        Data.IConnection IData.GetConnection([CallerMemberName] string callMethod = null) => GetConnection(callMethod);

        static Result UseConnection<Result>(System.Func<string, Data.IConnection> getConnection, System.Func<Data.IConnection, Result> func, string callMethod)
        {
            using (var con = getConnection(callMethod)) { return func(con); }
        }

        public int Save<T>(System.Collections.Generic.IEnumerable<T> obj, [CallerMemberName] string callMethod = null)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }

            return UseConnection(GetConnection, con => con.Save(obj), callMethod);
        }

        public int Save<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.Save(obj), callMethod);
        }

        public int SaveWithInt32Identity<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.SaveWithInt32Identity(obj), callMethod);
        }

        public long SaveWithInt64Identity<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.SaveWithInt64Identity(obj), callMethod);
        }

        public int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj, [CallerMemberName] string callMethod = null)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }

            return UseConnection(GetConnection, con => { return con.SaveOrUpdate(obj); }, callMethod);
        }

        public int SaveOrUpdate<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => { return con.SaveOrUpdate(obj); }, callMethod);
        }

        public int Update<T>(System.Collections.Generic.IEnumerable<T> obj, [CallerMemberName] string callMethod = null)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }

            return UseConnection(GetConnection, con => con.Update(obj), callMethod);
        }

        public int Update<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.Update(obj), callMethod);
        }

        public int Delete<T>(System.Collections.Generic.IEnumerable<T> obj, [CallerMemberName] string callMethod = null)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }

            return UseConnection(GetConnection, con => con.Delete(obj), callMethod);
        }

        public int Delete<T>(T obj, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.Delete(obj), callMethod);
        }

        public int ExecuteNonQuery(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, DataParameter[] parameter = null, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.ExecuteNonQuery(commandText, commandType, parameter), callMethod);
        }

        public Result ExecuteScalar<Result>(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, DataParameter[] parameter = null, [CallerMemberName] string callMethod = null)
        {
            return UseConnection(GetConnection, con => con.ExecuteScalar<Result>(commandText, commandType, parameter), callMethod);
        }
    }

    //public class DB<IConnection> : DataBase<IConnection>
    //    where IConnection : class, Data.IConnection
    //{
    //    readonly System.Func<IConnection> creat;

    //    public DB(System.Func<IConnection> creat) => this.creat = creat;

    //    public override IConnection GetConnection() => creat();

    //    public static DB<IConnection> Creat(System.Func<IConnection> creat) => new DB<IConnection>(creat);
    //}

    //public abstract class EntitysBase : System.MarshalByRefObject, IEntity
    //{
    //    public abstract IQueryable<T> Get<T>() where T : class, new();
    //}
}