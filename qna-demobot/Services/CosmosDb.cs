using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using System.Configuration;

namespace qna_demobot.Services
{

    [Serializable]
    public class CosmosDb
    {


        //Read config

        private static readonly string endpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string authorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static readonly string databaseId = ConfigurationManager.AppSettings["DatabaseId"];
        private static readonly string collectionId = ConfigurationManager.AppSettings["CollectionId"];
        private static readonly ConnectionPolicy connectionPolicy = new ConnectionPolicy { UserAgentSuffix = " samples-net/3" };

        //Reusable instance of DocumentClient which represents the connection to a DocumentDB endpoint

        public async Task CreateNewQuestion(Question question)
        {
            DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
            Document created = await client.CreateDocumentAsync(collectionLink, question);
            //return true;
        }

       /** public async Task<Question> retrieveQuestion(string id)
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
            Document created = await client.CreateDocumentAsync(collectionLink, question);
            var response = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id));
            return await Task.FromResult<Question>(null);
        }**/

    }

    public class Question
    {
        public string ID
        {
            get; set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Team
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string question
        {
            get;
            set;
        }

    }
}