using AMiON.Helper.ConfigurationManagerHelper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AMiON.Helper.CosmosDB
{
    public static class CosmosDB<T> where T : class, IEntityDAO
    {

        private static DocumentClient GetInstance()
        {
            var cosmosURL = AppSetting.CosmosURL;
            var cosmosAuthKey = AppSetting.CosmosAuthKey;

            var connectionPolicy = new ConnectionPolicy
            {
                ConnectionProtocol = Protocol.Tcp,
                ConnectionMode = ConnectionMode.Direct,
                MaxConnectionLimit = 1000,
                RequestTimeout = new TimeSpan(1, 0, 0),
                RetryOptions = new RetryOptions { MaxRetryWaitTimeInSeconds = 60, MaxRetryAttemptsOnThrottledRequests = 10 }
            };

            return new DocumentClient(new Uri(cosmosURL), cosmosAuthKey, connectionPolicy);

        }

        public static string InsertOrUpdateCollection(string databaseName, string collectionName, object item)
        {
            try
            {
                using (var documentClient = GetInstance())
                {
                    Document doc = documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), item).GetAwaiter().GetResult();
                    return doc.Id;
                }

            }
            catch (DocumentClientException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static T GetItemAsync(Expression<Func<T, bool>> where, string databaseName, string collectionName)
        {
            try
            {
                using (var documentClient = GetInstance())
                {
                    IDocumentQuery<T> query = documentClient.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(where).AsDocumentQuery();

                    return query.ExecuteNextAsync<T>().GetAwaiter().GetResult().FirstOrDefault();
                }
            }
            catch (DocumentClientException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
