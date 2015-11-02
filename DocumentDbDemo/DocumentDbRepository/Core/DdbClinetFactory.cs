using System;
using DocumentDbDemo.Utils;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using static System.Configuration.ConfigurationManager;

namespace DocumentDbDemo.DocumentDbRepository.Core
{
    static class DdbClinetFactory
    {
        #region variables & construcotrs

        static readonly ConnectionMode ConnectionMode = ConnectionMode.Direct;
        static readonly Protocol Protocol = Protocol.Tcp;

        static readonly string EndpointUri;
        static readonly string AuthorizationKey;
        static readonly int RetryCount;
        static readonly TimeSpan RetryInterval;
        static readonly string DatabaseName;
        static readonly int FeedOptionMaxItemCount;

        static DdbClinetFactory()
        {
            EndpointUri = AppSettings["DdbConfig.endpointUri"];
            AuthorizationKey = AppSettings["DdbConfig.authorizationKey"];
            RetryCount = int.Parse(AppSettings["DdbConfig.retryCount"]);
            RetryInterval = TimeSpan.FromSeconds(int.Parse(AppSettings["DdbConfig.retryInterval"]));
            FeedOptionMaxItemCount = int.Parse(AppSettings["DdbConfig.feedOptionMaxItemCount"]);
            DatabaseName = AppSettings["DdbConfig.databaseName"];
        }

        #endregion

        public static DdbClinet GetInstance()
        {
            PreRequisite.NotNullOrWhiteSpace(EndpointUri);
            PreRequisite.NotNullOrWhiteSpace(AuthorizationKey);
            PreRequisite.NotNullOrWhiteSpace(DatabaseName);

            var policy = CreateConnectionPolicy(ConnectionMode, Protocol);
            var documentClinet = CreateDocumentClient(EndpointUri, AuthorizationKey, policy);
            var strategy = GetRetryStrategy(null, RetryCount, RetryInterval, false);
            return new DdbClinet(documentClinet.AsReliable(strategy), DatabaseName, FeedOptionMaxItemCount);

        }

        #region private 

        static ConnectionPolicy CreateConnectionPolicy(ConnectionMode connectionMode, Protocol protocol)
        {
            return new ConnectionPolicy
            {
                ConnectionMode = connectionMode,
                ConnectionProtocol = protocol
            };
        }
        static DocumentClient CreateDocumentClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy)
                => new DocumentClient(new Uri(endpointUrl), authorizationKey, connectionPolicy);

        static FixedInterval GetRetryStrategy(string name, int retryCount, TimeSpan retryInterval, bool firstFastRetry)
                => new FixedInterval(name, retryCount, retryInterval, firstFastRetry);

        #endregion
    }
}
