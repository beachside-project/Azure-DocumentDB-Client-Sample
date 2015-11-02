using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDbDemo.DocumentDbRepository.Core;
using DocumentDbDemo.Models;
using Microsoft.Azure.Documents;
using System.Configuration;
using DocumentDbDemo.Utils;


namespace DocumentDbDemo.DocumentDbRepository
{
    public class DemoCollecitonManager
    {
        #region fields
        static readonly string DemoCollectionId = ConfigurationManager.AppSettings["DdbConfig.demoCollection"];
        static Lazy<DocumentCollection> _demoCollection;
        static Lazy<DdbClinet> _clinet;

        static DdbClinet Client => _clinet.Value;
        static DocumentCollection DemoCollection => _demoCollection.Value;

        static DemoCollecitonManager()
        {
            PreRequisite.NotNullOrWhiteSpace(DemoCollectionId);

            _clinet = new Lazy<DdbClinet>(DdbClinetFactory.GetInstance);
            _demoCollection = new Lazy<DocumentCollection>(() => Client.GetCollectionIfNotExistsCreate(DemoCollectionId));
        }
        #endregion

        #region Document control

        public static async Task InsertPersonDocument(Person model)
        {
            await Client.InsertDocumentAsync(DemoCollection.SelfLink, model, true);
        }

        public static IEnumerable<Person> GetPersonModelsById(string personId)
        {
            return Client.GetDocumentsByPredicate<Person>(DemoCollection.DocumentsLink, p => p.PersonId == personId);
        }

        public static async Task DeleteByPersonIdArrayAsync(string[] personIdArray)
        {
            var query = Queries.GetQueryForDeleteByPersonIdArray(personIdArray);
            await Client.DeleteDocumentsByQueryAsync(DemoCollection.SelfLink, query, false);
        }

        #endregion


    }
}
