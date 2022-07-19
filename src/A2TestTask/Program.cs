using A2TestTask.Models.GraphQL;
using A2TestTask.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace A2TestTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {            
            do
            {
                var size = 1000;
                var number = 0;

                List<Content> content;

                using (var client = new GraphQLService("https://www.lesegais.ru/open-area/graphql"))
                {
                    using (var db = new DatabaseService("Server=localhost,1433;User=sa;Password=b4gtkMfy;Database=a2team;"))
                    {
                        await db.OpenConnectionAsync();

                        do
                        {
                            Console.WriteLine($"Requesting page {number} for {size} records...");

                            var searchReportWoodDealResponse = await client.GetContentsAsync(size, number);

                            content = searchReportWoodDealResponse?.SearchReportWoodDeal?.Content;

                            if (content == null)
                            {
                                //TODO: add check.
                            }

                            Console.WriteLine($"Recieved {content.Count} records.");

                            Console.WriteLine("Storing records to DB...");
#if DEBUG
                            var watch = System.Diagnostics.Stopwatch.StartNew();
#endif
                            var successInserts = 0;
                            var duplicateRecords = 0;
                            var nonValidatedRecords = 0;

                            var dbList = content.Select(x => x.ToData()).ToList();

                            //(successInserts, duplicateRecords, nonValidatedRecords) =
                            //    await db.CreateWoodDealListAsync(dbList);

                            // Inserting one-by-one

                            foreach (var dbItem in dbList)
                            {
                                try
                                {
                                    await db.CreateWoodDealAsync(dbItem, true);

                                    successInserts++;
                                }
                                catch (FormatException ex)
                                {
                                    nonValidatedRecords++;

                                    Console.WriteLine($"Skipping {dbItem}: {ex.Message}");

                                    continue;
                                }
                                catch (SqlException ex)
                                {
                                    switch (ex.Class)
                                    {
                                        case 14:
                                            duplicateRecords++;
                                            break;
                                        default:
                                            Console.WriteLine(ex.Message);
                                            break;
                                    }
                                }
                            }

                            Console.WriteLine($"Stored: {successInserts}, failed validation: {nonValidatedRecords}, duplicates: {duplicateRecords}");
#if DEBUG
                            watch.Stop();

                            Console.WriteLine($"Time: {(watch.ElapsedMilliseconds / 1000.0):0.#} seconds!");
                            Console.WriteLine();
#endif
                            number++;
                        } while (content.Count == size);
                    }
                }

                var timeout = 60000;
                
                Console.WriteLine($"Next pass in {timeout / 1000} seconds...");
                Console.WriteLine();

                await Task.Delay(timeout);

            } while (true);
        }
    }
}
