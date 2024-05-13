using System.Collections.Generic;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading.Tasks;
//Copyright © 2023 Mandala Consulting, LLC MIT License
//Created by Alexander Fields
using MongoDB.Bson;
using MongoDB.Driver;
using Optimization.Logging;

namespace Optimization.Repository
{
    /// <summary>
    /// Make sure you set the MongoDB Instance before calling the classes in this
    /// </summary>
    public class MongoHelper : IMongoHelper
    {
        private static List<LogMessage> mongoLogs = new List<LogMessage>();
        public static event System.EventHandler<LogMessageEventArgs> LogAdded;
        public static event System.EventHandler LogCleared;

        public IMongoDatabase database { get; set; }

        public string dbName { get; set; }

        /// <summary>
        /// default Constrcutor
        /// </summary>
        public MongoHelper() { }

        /// <summary>
        /// Constructor that will get Database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionString"></param>
        public MongoHelper(string dbName, string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(dbName);
            database = db;
        }

        /// <summary>
        /// Helper for connecting to database
        /// </summary>
        /// <param name="mongoHelper">instance of the mongo helper that will connect</param>
        /// <param name="dbName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cluster"></param>
        /// <param name="region">I have no idea if it's actually the region its just an assumption but its different on like all my databases</param>
        /// <returns>mongo helper</returns>
        public static IMongoHelper MongoHelperConnector(
            IMongoHelper mongoHelper,
            string dbName,
            string username,
            string password,
            string cluster,
            string region
        )
        {
            string connectionString = ConnectionStringBuilder(username, password, cluster, region);

            mongoHelper.database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);
            try
            {
                List<string> collections = mongoHelper.TestConnection();
            }
            catch (System.Exception theseHands)
            {
                AddLog(LogMessage.Error(theseHands.Message));
            }

            mongoHelper.dbName = dbName;

            return mongoHelper;
        }

        /// <summary>
        /// Connection string builder
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="cluster"></param>
        /// <param name="region">I have no idea if it's actually the region its just an assumption but its different on like all my databases</param>
        /// <returns></returns>
        public static string ConnectionStringBuilder(
            string username,
            string password,
            string cluster,
            string region
        )
        {
            string encodedPassword = System.Net.WebUtility.UrlEncode(password);

            //string connectionString = $"mongodb+srv://{username}:{encodedPassword}@{cluster}.vc4onns.mongodb.net/?retryWrites=true&w=majority";
            string connectionString =
                $"mongodb+srv://{username}:{encodedPassword}@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority";
            return connectionString;
        }

        /// <summary>
        /// For if already constructed to get Database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public IMongoDatabase CreateMongoDbInstance(string dbName, string connectionString)
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase db = client.GetDatabase(dbName);
            return db;
        }

        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        /// <returns>Will return a List of Collection Names if it worked otherwise returns null</returns>
        public List<string> TestConnection()
        {
            try
            {
                List<string> collectionNames = database.ListCollectionNames().ToList();
                return collectionNames;
            }
            catch (System.Exception e)
            {
                AddLog(
                    LogMessage.Error(
                        "Was unable to properly connect to the database!" + e.ToString()
                    )
                );
                return null;
            }
        }

        public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionName)
        {
            IMongoCollection<T> collection = GetCollection<T>(collectionName);
            return await collection.Find(x => true).ToListAsync();
        }

        public async Task<List<T>> GetFilteredDocumentsAsync<T>(
            string collectionName,
            FilterDefinition<T> filter
        )
        {
            return await GetCollection<T>(collectionName).Find(filter).ToListAsync();
        }

        public async Task UpdateDocumentAsync<T>(
            string collectionName,
            FilterDefinition<T> filter,
            T document
        )
        {
            await GetCollection<T>(collectionName).ReplaceOneAsync(filter, document);
        }

        public async Task UpdateDocumentAsync<T>(
            string collectionName,
            FilterDefinition<T> filter,
            UpdateDefinition<T> document
        )
        {
            await GetCollection<T>(collectionName).UpdateOneAsync(filter, document);
        }

        public async Task CreateDocumentAsync<T>(string collectionName, T document)
        {
            await GetCollection<T>(collectionName).InsertOneAsync(document);
        }

        public async Task DeleteDocumentAsync<T>(string collectionName, FilterDefinition<T> filter)
        {
            await GetCollection<T>(collectionName).DeleteOneAsync(filter);
        }

        private IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        public static void AddLog(LogMessage logMessage)
        {
            mongoLogs.Add(logMessage);
            LogAdded?.Invoke(null, new LogMessageEventArgs(logMessage));
        }

        public static void ClearLogs()
        {
            mongoLogs.Clear();
            LogCleared?.Invoke(null, System.EventArgs.Empty);
        }

        /// <summary>
        /// Get the id for when you don't have an object class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>object</returns>
        public static object GetIdFromObj<T>(T obj)
        {
            System.Type typeInfo = typeof(T);
            PropertyInfo idProperty = typeInfo.GetProperty("_id");

            if (idProperty != null)
            {
                return idProperty.GetValue(obj, null);
            }

            return null;
        }

        /// <summary>
        /// Get the id for when you don't have an object class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>object</returns>
        public static string GetIdFromObjAsString<T>(T obj)
        {
            System.Type typeInfo = typeof(T);
            PropertyInfo idProperty = typeInfo.GetProperty("_id");

            if (idProperty != null)
            {
                string idValue = (string)idProperty.GetValue(obj, null);
                return idValue;
            }

            return null;
        }

        public static List<LogMessage> GetLogs()
        {
            return mongoLogs;
        }
    }
}
