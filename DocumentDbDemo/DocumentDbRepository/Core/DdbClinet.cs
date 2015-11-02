using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocumentDbDemo.Utils;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;

namespace DocumentDbDemo.DocumentDbRepository.Core
{
    class DdbClinet : IDisposable
    {
        #region variables & constructors

        private IReliableReadWriteDocumentClient _client;
        private readonly FeedOptions _feedOptions;
        private readonly string _databaseName;
        private Lazy<Database> _databaseInstance;
        private Database DatabaseInstance => _databaseInstance.Value;

        public DdbClinet(IReliableReadWriteDocumentClient client, string databaseName, int feedOptionMaxItemCount)
        {
            _client = client;
            _databaseName = databaseName;
            _feedOptions = new FeedOptions { MaxItemCount = feedOptionMaxItemCount };
            _databaseInstance = new Lazy<Database>(() => GetDatabaseIfNotExistsCreate(_databaseName));
        }

        #endregion

        #region database control

        Database GetDatabaseIfNotExistsCreate(string name)
        {
            var database = TryGetDatabase(name) ?? CreateDatabaseAsync(name).Result;
            if (database == null) throw new InvalidOperationException($"データベースの生成に異常ありんご！({name})");

            return database;
        }

        Database TryGetDatabase(string name)
        {
            return _client.CreateDatabaseQuery().Where(d => d.Id == name).AsEnumerable().FirstOrDefault();
        }

        async Task<Database> CreateDatabaseAsync(string name)
        {
            return await _client.CreateDatabaseAsync(new Database { Id = name });
        }

        #endregion

        #region collection control

        public DocumentCollection GetCollectionIfNotExistsCreate(string collectionName, CollectionOfferType offerType = CollectionOfferType.S1)
        {
            var collection = TryGetCollection(DatabaseInstance, collectionName) ??
                                CreateCollectionAsync(DatabaseInstance.CollectionsLink, collectionName, offerType).Result;
            if (collection == null) throw new InvalidOperationException($"コレクションの生成に異常ありんご！({collectionName})");

            return collection;
        }

        DocumentCollection TryGetCollection(Database database, string collectionName)
        {

            return _client.CreateDocumentCollectionQuery(database.CollectionsLink)
                        .Where(c => c.Id == collectionName).AsEnumerable().FirstOrDefault();
        }

        async Task<DocumentCollection> CreateCollectionAsync(string databaseLink, string collectionName, CollectionOfferType offerType)
        {
            var collection = new DocumentCollection() { Id = collectionName };
            var requestOptions = new RequestOptions() { OfferType = ComvertOfferTypeToString(offerType) };
            return await _client.CreateDocumentCollectionAsync(databaseLink, collection, requestOptions);

        }

        static string ComvertOfferTypeToString(CollectionOfferType offerType) => Enum.GetName(typeof(CollectionOfferType), offerType).ToUpperInvariant();

        #endregion

        #region document control

        public async Task<dynamic> InsertDocumentAsync<TEntity>(string collectionSelfLink, TEntity entity, bool configureAwait)
        {
            return await _client.CreateDocumentAsync(collectionSelfLink, entity).ConfigureAwait(configureAwait);
        }

        public IEnumerable<TEntity> GetDocumentsByPredicate<TEntity>(string collectionSelfLink, Expression<Func<TEntity, bool>> predicate)
        {

            return _client.CreateDocumentQuery<TEntity>(collectionSelfLink, _feedOptions).Where(predicate).AsEnumerable();
        }


        public async Task DeleteDocumentsByQueryAsync(string collectionSelfLink, string query, bool configureAwait)
        {
            var documents = _client.CreateDocumentQuery<Document>(collectionSelfLink, query).AsEnumerable().ToArray();
            if (documents.Any())
            {
                foreach (var doc in documents) await DeleteDocumentAsunc(doc, configureAwait);
            }
        }

        public async Task DeleteDocumentAsunc(Document document, bool cofigureAwait = true)
        {
            await _client.DeleteDocumentAsync(document.SelfLink).ConfigureAwait(cofigureAwait);
        }

        #endregion

        #region GC

        public void Dispose() => Gc.Throw(ref _client);

        #endregion

    }
}
