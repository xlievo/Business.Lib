using System.Linq;

namespace Business.Utils
{
    #region Paging Object

    [ProtoBuf.ProtoContract(SkipConstructor = true)]
    public struct Paging<T>
    {
        public static implicit operator Paging<T>(string value)
        {
            return Help2.TryJsonDeserialize<Paging<T>>(value);
        }
        public static implicit operator Paging<T>(byte[] value)
        {
            return Help2.TryProtoBufDeserialize<Paging<T>>(value);
        }

        [ProtoBuf.ProtoMember(1)]
        public System.Collections.Generic.List<T> Data { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Length { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int CurrentPage { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int Count { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int CountPage { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public byte[] ToBytes()
        {
            return Help2.ProtoBufSerialize(this);
        }
    }

    public struct PagingInfo
    {
        public int Skip;
        public int Take;
        public int CurrentPage;
        public int CountPage;
    }

    #endregion

    public static class Help2
    {
        #region Paging

        public static PagingInfo GetPaging(int count, int currentPage, int pageSize, int pageSizeMax = 50)
        {
            if (0 == count) { return default(PagingInfo); }

            var _pageSize = System.Math.Min(pageSize, pageSizeMax);
            var _countPage = System.Convert.ToDouble(count) / System.Convert.ToDouble(_pageSize);
            var countPage = (double.IsNaN(_countPage) || double.IsPositiveInfinity(_countPage) || double.IsNegativeInfinity(_countPage)) ? 1 : System.Convert.ToInt32(System.Math.Ceiling(_countPage));

            currentPage = currentPage < 0 ? 0 : currentPage > countPage ? countPage : currentPage;
            if (currentPage <= 0 && countPage > 0) { currentPage = 1; }

            return new PagingInfo { Skip = _pageSize * (currentPage - 1), Take = _pageSize, CurrentPage = currentPage, CountPage = countPage };
        }

        public static Paging<T> GetPaging<T>(this IQueryable<T> query, int currentPage, int pageSize, int pageSizeMax = 50)
        {
            var count = query.Count();
            if (0 == count) { return new Paging<T> { Data = new System.Collections.Generic.List<T>() }; }

            var p = GetPaging(count, currentPage, pageSize, pageSizeMax);

            var data = query.Skip(p.Skip).Take(p.Take).ToList();

            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = p.CurrentPage, Count = count, CountPage = p.CountPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, int currentPage = 0, int count = 0, int countPage = 0)
        {
            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = currentPage, Count = count, CountPage = countPage };
        }

        public static Paging<T> ToPaging<T>(this System.Collections.Generic.List<T> data, dynamic pagingObj)
        {
            return new Paging<T> { Data = data, Length = data.Count, CurrentPage = pagingObj.CurrentPage, Count = pagingObj.Count, CountPage = pagingObj.CountPage };
        }

        #endregion

        #region Json

        public static Type TryJsonDeserialize<Type>(this string value)
        {
            try { return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value); }
            catch { return default(Type); }
        }

        public static Type TryJsonDeserialize<Type>(this string value, out string error)
        {
            error = null;

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value);
            }
            catch (System.Exception ex)
            {
                error = System.Convert.ToString(ex);
                return default(Type);
            }
        }
        public static object TryJsonDeserialize(this string value, System.Type type, out string error)
        {
            error = null;

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
            }
            catch (System.Exception ex)
            {
                error = System.Convert.ToString(ex);
                return null;
            }
        }
        public static string JsonSerialize<Type>(this Type value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        #endregion

        #region ProtoBuf Serialize
        public static Type TryProtoBufDeserialize<Type>(this System.Byte[] source)
        {
            try
            {
                using (var stream = new System.IO.MemoryStream(source))
                {
                    return ProtoBuf.Serializer.Deserialize<Type>(stream);
                }
            }
            catch { return default(Type); }
        }
        public static System.Byte[] ProtoBufSerialize<Type>(this Type instance)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }
        public static object ProtoBufDeserialize(this byte[] source, System.Type type)
        {
            using (var stream = new System.IO.MemoryStream(source))
            {
                return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
            }
        }

        #endregion

        internal static Type ChangeType<Type>(this object value)
        {
            return (Type)ChangeType(value, typeof(Type));
        }

        internal static object ChangeType(this object value, System.Type type)
        {
            if (null == value) { return System.Activator.CreateInstance(type); }

            try
            {
                return System.Convert.ChangeType(value, type);
            }
            catch { return System.Activator.CreateInstance(type); }
        }

        internal static int Random(int minValue, int maxValue)
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return new System.Random(System.BitConverter.ToInt32(bytes, 0)).Next(minValue, maxValue);
            }
        }
        internal static int Random(this int maxValue)
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return new System.Random(System.BitConverter.ToInt32(bytes, 0)).Next(maxValue);
            }
        }
        internal static double Random()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                return new System.Random(System.BitConverter.ToInt32(bytes, 0)).NextDouble();
            }
        }

        /*
        public static long ConvertTime(this System.DateTime time)
        {
            return (time.Ticks - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks) / 10000;//除10000调整为13位
            //return (time.ToUniversalTime().Ticks - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks) / 10000;
        }
        public static System.DateTime ConvertTime(this long time)
        {
            //return new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(time);
            //10位处理 补毫秒000
            //(time + 8 * 60 * 60) * 10000000 + System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;
            return new System.DateTime(System.Convert.ToInt64(time.ToString().PadRight(13, '0')) * 10000 + System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks);
        }
        */
    }
}
