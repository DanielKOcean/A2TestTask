using A2TestTask.Models.GraphQL;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using System;
using System.Threading.Tasks;

namespace A2TestTask.Services
{
    public class GraphQLService : IDisposable
    {
        class Variables
        {
            public int Size { get; set; }
            public int Number { get; set; }
            public object Filter { get; set; }
            public object Orders { get; set; }
        }

        private bool disposedValue;
        private readonly GraphQLHttpClient _graphQLClient;

        public GraphQLService(string url)
        {
            _graphQLClient = new GraphQLHttpClient(url, new SystemTextJsonSerializer());
        }

        public async Task<SearchReportWoodDealResponse> GetContentsAsync(int size, int number)
        {
            var searchReportWoodDealRequest = new GraphQLRequest
            {
                Query = @"
                query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {
                    searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {
                        content {
                            sellerName
                            sellerInn
                            buyerName
                            buyerInn
                            woodVolumeBuyer
                            woodVolumeSeller
                            dealDate
                            dealNumber
                            __typename
                        }
                        __typename
                    }
                }",
                OperationName = "SearchReportWoodDeal",
                Variables = new Variables
                {
                    Size = size,
                    Number = number,
                },
            };

            var response = await _graphQLClient.SendQueryAsync<SearchReportWoodDealResponse>(searchReportWoodDealRequest);

            return response.Data;
        }

        #region IDisposable implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _graphQLClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GraphQLService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
