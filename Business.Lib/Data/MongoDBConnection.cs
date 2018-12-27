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

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Data
{
    public abstract class EntitysMongoDB : IEntity
    {
        readonly IMongoClient server;
        readonly string dbName;

        public EntitysMongoDB(IMongoClient server, string dbName)
        {
            this.server = server;
            this.dbName = dbName;
        }

        public IQueryable<T> Get<T>() where T : class, new()
        {
            return server.GetDatabase(dbName).GetCollection<T>(typeof(T).Name).AsQueryable();
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return server.GetDatabase(dbName).GetCollection<T>(typeof(T).Name);
        }

        public IMongoDatabase GetDatabase(MongoDatabaseSettings settings = null)
        {
            return server.GetDatabase(dbName, settings);
        }
    }

    [BsonIgnoreExtraElements]
    public class MongoDBEntity
    {
        ObjectId id = ObjectId.GenerateNewId();

        //[BsonIgnore]
        public virtual ObjectId _id { get { return id; } set { id = value; } }
    }

    public abstract class MongoDBConnection<IEntity> : IConnection
        where IEntity : class, Data.IEntity
    {
        IMongoClient server;
        readonly string dbName;

        static IMongoCollection<T> GetCollection<T>(IMongoClient server, string dbName, string name = null)
        {
            return server.GetDatabase(dbName).GetCollection<T>(name ?? typeof(T).Name);
        }

        public MongoDBConnection(IMongoClient server, string dbName)
        {
            this.server = server;
            this.dbName = dbName;
        }

        public System.Data.IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public abstract IEntity Entity { get; }

        Data.IEntity IConnection.Entity
        {
            get { return Entity; }
        }

        public void Dispose()
        {
            server = null;
        }

        public System.Data.IDbTransaction Transaction
        {
            get { throw new NotImplementedException(); }
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public int Save<T>(IEnumerable<T> obj)
        {
            var collection = GetCollection<T>(this.server, dbName);

            collection.InsertMany(obj);

            return obj.Count();
        }

        public int Save<T>(T obj)
        {
            var collection = GetCollection<T>(this.server, dbName);

            collection.InsertOne(obj);

            return 1;
        }

        public int SaveWithInt32Identity<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public long SaveWithInt64Identity<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public int SaveOrUpdate<T>(IEnumerable<T> obj)
        {
            return SaveOrUpdate<T>(obj as IEnumerable<MongoDBEntity>);
        }
        int SaveOrUpdate<T>(IEnumerable<MongoDBEntity> obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            var count = 0;

            foreach (var item in obj)
            {
                count += (int)collection.ReplaceOne(c => c._id == item._id, item, new UpdateOptions { IsUpsert = true }).MatchedCount;
            }

            return count;
        }

        public int SaveOrUpdate<T>(T obj)
        {
            return SaveOrUpdate<T>(obj as MongoDBEntity);
        }
        int SaveOrUpdate<T>(MongoDBEntity obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            return (int)collection.ReplaceOne(c => c._id == obj._id, obj, new UpdateOptions { IsUpsert = true }).MatchedCount;
        }

        public int Update<T>(IEnumerable<T> obj)
        {
            return Update<T>(obj as IEnumerable<MongoDBEntity>);
        }
        int Update<T>(IEnumerable<MongoDBEntity> obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            var count = 0;

            foreach (var item in obj)
            {
                count += (int)collection.ReplaceOne(c => c._id == item._id, item).MatchedCount;
            }

            return count;
        }

        public int Update<T>(T obj)
        {
            return Update<T>(obj as MongoDBEntity);
        }
        int Update<T>(MongoDBEntity obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            return (int)collection.ReplaceOne(c => c._id == obj._id, obj).MatchedCount;
        }

        public int Delete<T>(IEnumerable<T> obj)
        {
            return Delete<T>(obj as IEnumerable<MongoDBEntity>);
        }

        int Delete<T>(IEnumerable<MongoDBEntity> obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            var count = 0;

            foreach (var item in obj)
            {
                count += (int)collection.DeleteOne(c => c._id == item._id).DeletedCount;
            }

            return count;
        }

        public int Delete<T>(T obj)
        {
            return Delete<T>(obj as MongoDBEntity);
        }

        int Delete<T>(MongoDBEntity obj)
        {
            var collection = GetCollection<MongoDBEntity>(this.server, dbName, typeof(T).Name);

            return (int)collection.DeleteOne(c => c._id == obj._id).DeletedCount;
        }
    }
}
