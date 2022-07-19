using A2TestTask.Models.Database;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A2TestTask.Services
{
    public class DatabaseService : IDisposable
    {
        private bool disposedValue;

        private readonly SqlConnection _connection;

        public DatabaseService(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public Task OpenConnectionAsync()
        {
            return _connection.OpenAsync();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        public async Task<string> CreateWoodDealAsync(WoodDeal woodDeal, bool applyValidation = false)
        {
            if (applyValidation) // In some cases we may ignore validation
                woodDeal.Validate();

            var query = @"
                INSERT INTO a2team.dbo.wooddeals(
                    dealNumber,
                    dealDate,
                    sellerName,
                    sellerInn,
                    buyerName,
                    buyerInn,
                    woodVolumeSeller,
                    woodVolumeBuyer
                )
                VALUES (
                    @DealNumber,
                    @DealDate,
                    @SellerName,
                    @SellerInn,
                    @BuyerName,
                    @BuyerInn,
                    @WoodVolumeSeller,
                    @WoodVolumeBuyer
            );";

            await _connection.ExecuteAsync(query, woodDeal);

            return woodDeal.DealNumber;
        }

        public async Task<(int, int, int)> CreateWoodDealListAsync(List<WoodDeal> woodDeals)
        {
            var woodDealsFiltered = new List<WoodDeal>();
            foreach (var woodDeal in woodDeals)
            {
                try
                {
                    woodDeal.Validate();

                    woodDealsFiltered.Add(woodDeal);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            var query = @"
                INSERT INTO a2team.dbo.wooddeals
                    (sellerName, sellerInn, buyerName, buyerInn, dealDate, dealNumber, woodVolumeBuyer, woodVolumeSeller)
                    SELECT
                        ttm.sellerName, ttm.sellerInn, ttm.buyerName, ttm.buyerInn, ttm.dealDate, ttm.dealNumber, ttm.woodVolumeBuyer, ttm.woodVolumeSeller
                    FROM
                        a2team.dbo.wooddeals rjwn RIGHT JOIN ( VALUES
                            (@SellerName, @SellerInn, @BuyerName, @BuyerInn, @DealDate, @DealNumber, @WoodVolumeBuyer, @WoodVolumeSeller)
                        ) AS ttm (sellerName, sellerInn, buyerName, buyerInn, dealDate, dealNumber, woodVolumeBuyer, woodVolumeSeller)
                        ON rjwn.dealNumber = ttm.dealNumber
                    WHERE
                        rjwn.dealNumber IS NULL;";

            var successInserts = 0;

            //using (var transaction = _connection.BeginTransaction())
            //{
                successInserts = await _connection.ExecuteAsync(query, woodDealsFiltered);

            //    transaction.Commit();
            //}
            var nonValidatedRecords = woodDeals.Count - woodDealsFiltered.Count;
            var duplicateRecords = woodDealsFiltered.Count - successInserts;

            return (successInserts, duplicateRecords, nonValidatedRecords);
        }

        public WoodDeal ReadWoodDeal(string id)
        {
            throw new NotImplementedException();
        }

        public WoodDeal UpdateWoodDeal(WoodDeal woodDeal)
        {
            throw new NotImplementedException();
        }

        public string DeleteWoodDeal(string id)
        {
            throw new NotImplementedException();
        }

        #region IDisposable implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _connection?.Close();
                    _connection?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DatabaseService()
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
