
namespace Business.Utils
{
    public static class Help2
    {
        public static int GetValue(int? value, int defaultValue = default) => null == value || !value.HasValue ? defaultValue : value.Value;
        public static double GetValue(double? value, double defaultValue = default) => null == value || !value.HasValue ? defaultValue : value.Value;

        #region Json

        public static Type TryJsonDeserialize<Type>(string value, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value, settings);
            }
            catch (System.Exception)
            {
                return default;
            }
        }
        public static Type TryJsonDeserialize<Type>(string value, params Newtonsoft.Json.JsonConverter[] converters)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value, converters);
            }
            catch
            {
                return default;
            }
        }

        public static Type TryJsonDeserialize<Type>(string value, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value);
            }
            catch (System.Exception ex)
            {
                error = System.Convert.ToString(ex);
                return default;
            }
        }
        public static bool TryJsonDeserialize<Type>(string value, System.Type type, out Type result)
        {
            try
            {
                result = (Type)Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        public static bool TryJsonDeserialize<Type>(string value, out Type result)
        {
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<Type>(value);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        public static object TryJsonDeserialize(string value, System.Type type, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

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
        public static string JsonSerialize<Type>(Type value, params Newtonsoft.Json.JsonConverter[] converters)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, converters);
        }
        public static string JsonSerialize<Type>(Type value, Newtonsoft.Json.JsonSerializerSettings settings)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);
        }

        #endregion

        #region Binary Serialize

        public static Type TryBinaryDeserialize<Type>(this byte[] source)
        {
            if (source == null)
            {
                return default;
            }

            try
            {
                return MessagePack.MessagePackSerializer.Deserialize<Type>(source);
            }
            catch { return default; }
        }
        //public static bool TryProtoBufDeserialize<Type>(System.Byte[] source, System.Type type, out Type result)
        //{
        //    try
        //    {
        //        using (var stream = new System.IO.MemoryStream(source))
        //        {
        //            result = MessagePack.MessagePackSerializer.Deserialize<Type>(source);
        //            //result = (Type)ProtoBuf.Serializer.Deserialize(type, stream);
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        result = default(Type);
        //        return false;
        //    }
        //}
        public static bool TryBinaryDeserialize<Type>(this byte[] source, out Type result)
        {
            try
            {
                result = MessagePack.MessagePackSerializer.Deserialize<Type>(source);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static byte[] BinarySerialize<Type>(this Type source) => MessagePack.MessagePackSerializer.Serialize(source);

        //public static object ProtoBufDeserialize(byte[] source, System.Type type)
        //{
        //    using (var stream = new System.IO.MemoryStream(source))
        //    {
        //        return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
        //    }
        //}
        //public static object TryProtoBufDeserialize(System.Byte[] source, System.Type type)
        //{
        //    try
        //    {
        //        using (var stream = new System.IO.MemoryStream(source))
        //        {
        //            return ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
        //        }
        //    }
        //    catch { return null; }
        //}

        #endregion

        internal static Type ChangeType<Type>(object value)
        {
            return (Type)ChangeType(value, typeof(Type));
        }

        internal static object ChangeType(object value, System.Type type)
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
        internal static int Random(int maxValue)
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
        #region Encrypt

        public struct AESValue { public string value; public string salt; }

        public static AESValue AESEncrypt(string data, string password, AESKeySize keySize = AESKeySize.AES_256)
        {
            var random = new Org.BouncyCastle.Security.SecureRandom();
            var salt = new byte[Org.AlbertSchmitt.Crypto.AESService.SALT_SIZE];
            random.NextBytes(salt);

            return new AESValue { value = AESEncrypt(data, password, salt, keySize), salt = System.Convert.ToBase64String(salt) };
        }

        public enum AESKeySize
        {
            AES_128 = 128,
            AES_256 = 256
        }

        public static string AESEncrypt(string data, string password, string salt, AESKeySize keySize = AESKeySize.AES_256) => AESEncrypt(data, password, System.Convert.FromBase64String(salt), keySize);
        public static string AESEncrypt(string data, string password, byte[] salt, AESKeySize keySize = AESKeySize.AES_256)
        {
            var aes = new Org.AlbertSchmitt.Crypto.AESService();

            // Create the AES Key using password and salt.
            aes.GenerateKey(password, salt, (int)keySize);

            // Encode and Decode a string then compare to verify they are the same.
            var enc_bytes = aes.Encode(data);

            return Org.AlbertSchmitt.Crypto.Hex.Encode(enc_bytes);
        }

        public static string AESDecrypt(string data, string key, string salt, AESKeySize keySize = AESKeySize.AES_256)
        {
            var aes = new Org.AlbertSchmitt.Crypto.AESService();
            aes.GenerateKey(key, System.Convert.FromBase64String(salt), (int)keySize);
            return System.Text.UTF8Encoding.UTF8.GetString(aes.Decode(data));
        }

        #endregion

        public static void WriteKey(string privateKeyfile, string publicKeyfile)
        {
            var rsa = new Org.AlbertSchmitt.Crypto.RSAService(Org.AlbertSchmitt.Crypto.RSAService.KEYSIZE.RSA_4K);

            using (var privateKey = new System.IO.FileStream(privateKeyfile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                using (var publicKey = new System.IO.FileStream(publicKeyfile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    rsa.GenerateKey(privateKey, publicKey);
                }
            }
        }
        */
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
