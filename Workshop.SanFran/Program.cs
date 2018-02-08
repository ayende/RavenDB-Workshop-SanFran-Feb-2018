using System;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands.Batches;
using Workshop.SanFran.Orders;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Raven.Client.Documents.Queries;

namespace Workshop.SanFran
{
    class Program
    {
        static void Main(string[] args)
        {
            Main().GetAwaiter().GetResult();
        }

        static async Task Main()
        {
            using(var store = new DocumentStore
            {
                Database = "Bio",
                Urls = new[] 
                {
                    "https://a.worksop.dbs.local.ravendb.net",
                    "https://b.worksop.dbs.local.ravendb.net",
                    "https://c.worksop.dbs.local.ravendb.net",
                },
                Certificate = new X509Certificate2(@"C:\Users\ayende\Downloads\admin.client.certificate.worksop.pfx")
            })
            {
                store.Initialize();

                using (var session = store.OpenAsyncSession())
                {
                    
                    var totals = from o in session.Query<Order>()
                                 where o.ShipTo.City == "London"
                                 let e = RavenQuery.Load<Employee>(o.Employee)
                                 let manager = RavenQuery.Load<Employee>(e.ReportsTo)
                                 let name = (Func<Employee, string>)(e => e.FirstName + " " + e.LastName)
                                 select new
                                 {
                                     o.Company,
                                     EmpName = name(e),
                                     ManagerName = name(manager)
                                 };

                    Console.WriteLine(totals);

                    var results = await totals.Take(2).ToListAsync();

                    foreach (var item in results)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }
    }
}
