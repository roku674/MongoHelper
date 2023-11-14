using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimization.Repository
{
    /// <summary>
    /// Make sure you set the MongoDB Instance before calling the classes in this 
    /// </summary>

    public interface IMongoHelper
    {
        /// <summary>
        /// The Databaes
        /// </summary>
        IMongoDatabase database { get; set; }

        /// <summary>
        /// db name for easier eaccess
        /// </summary>
        public string dbName { get; set; }


        /// <summary>
        /// Add a Document to MongoDB
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        Task CreateDocumentAsync<T>(string dbName, string collectionName, T document);
        /// <summary>
        /// Create Instance of the Database
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        IMongoDatabase CreateMongoDbInstance(string dbName, string connectionString);
        /// <summary>
        /// Be Careful
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task DeleteDocumentAsync<T>(string dbName, string collectionName, FilterDefinition<T> filter);
        /// <summary>
        /// Get All documents in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        Task<List<T>> GetAllDocumentsAsync<T>(string dbName, string collectionName);
        /// <summary>
        /// Get a Mongo Document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<T>> GetFilteredDocumentsAsync<T>(string dbName, string collectionName, FilterDefinition<T> filter);
        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        /// <returns>Will return a List of Collection Names if it worked otherwise returns null</returns>
        List<string> TestConnection();
        /// <summary>
        /// Updates a document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="updateDefinition"></param>
        /// <returns></returns>
        Task UpdateDocumentAsync<T>(string dbName, string collectionName, FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition);

        /// <summary>
        /// Replaces a document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <param name="filter"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        Task UpdateDocumentAsync<T>(string dbName, string collectionName, FilterDefinition<T> filter, T document);
    }
}