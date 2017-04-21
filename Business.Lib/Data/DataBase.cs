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

    public struct DataParameter
    {
        public DataParameter(string name, object value) { this.name = name; this.value = value; }

        readonly string name;
        readonly object value;

        public string Name { get { return name; } }

        public object Value { get { return value; } }
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
            foreach (var item in parameter)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = item.Name;
                p.Value = System.DateTime.MinValue.Equals(item.Value) ? System.DBNull.Value : item.Value ?? System.DBNull.Value;
                cmd.Parameters.Add(p);
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

        public static T ExecuteScalar<T>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack<T>(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    return (T)cmd.ExecuteScalar();
                }
            }, minusOneExcep: false);
        }

        public static System.Collections.Generic.IEnumerable<TEntity> Execute<TEntity>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack<System.Collections.Generic.IEnumerable<TEntity>>(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    var reader = cmd.ExecuteReader();

                    return new Utils.LightDataAccess.DataReaderToObjectMapper<TEntity>().ReadCollection(reader);
                }
            }, minusOneExcep: false);
        }

        public static TEntity ExecuteSingle<TEntity>(this IConnection connection, string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return connection.ExecutePack<TEntity>(() =>
            {
                using (var cmd = connection.GetCommand(commandText, connection.Transaction, commandType, parameter))
                {
                    var reader = cmd.ExecuteReader();

                    return new Utils.LightDataAccess.DataReaderToObjectMapper<TEntity>().ReadSingle(reader);
                }
            }, minusOneExcep: false);
        }

        internal static Result ExecutePack<Result>(this IConnection connection, System.Func<Result> func, bool minusOneExcep = true)
        {
            bool isCreateTransaction = false;
            if (null == connection.Transaction) { connection.BeginTransaction(); isCreateTransaction = !isCreateTransaction; }

            try
            {
                var result = func.Invoke();

                if (minusOneExcep && typeof(Result).Equals(typeof(System.Int32)))
                {
                    var count = System.Convert.ToInt32(result);
                    if (-1 == count)
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
    }

    public abstract class DataBase<IConnection> : IData
        where IConnection : class, Data.IConnection
    {
        public abstract IConnection GetConnection();

        Data.IConnection IData.GetConnection()
        {
            return GetConnection();
        }

        static T UseConnection<T>(System.Func<Data.IConnection> getConnection, System.Func<Data.IConnection, T> func)
        {
            using (var con = getConnection.Invoke()) { return func.Invoke(con); }
        }

        public int Save<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Save(obj); });
        }

        public int Save<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Save(obj); });
        }

        public int SaveOrUpdate<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.SaveOrUpdate(obj); });
        }

        public int SaveOrUpdate<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.SaveOrUpdate(obj); });
        }

        public int Update<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Update(obj); });
        }

        public int Update<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Update(obj); });
        }

        public int Delete<T>(System.Collections.Generic.IEnumerable<T> obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Delete(obj); });
        }

        public int Delete<T>(T obj)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.Delete(obj); });
        }

        public int ExecuteNonQuery(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return UseConnection<int>(GetConnection, (con) => { return con.ExecuteNonQuery(commandText, commandType, parameter); });
        }

        public T ExecuteScalar<T>(string commandText, System.Data.CommandType commandType = System.Data.CommandType.Text, params DataParameter[] parameter)
        {
            return UseConnection<T>(GetConnection, (con) => { return con.ExecuteScalar<T>(commandText, commandType, parameter); });
        }
    }

    public abstract class Entitys : System.MarshalByRefObject, IEntity
    {
        public abstract System.Linq.IQueryable<T> Get<T>()
            where T : class, new();
    }
}