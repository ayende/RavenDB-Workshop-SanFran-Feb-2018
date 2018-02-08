using System;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands.Batches;
using Workshop.SanFran.Orders;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Workshop.SanFran
{
    class Program
    {
        static void Main(string[] args)
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

                using (var session = store.OpenSession())
                {
                    var totals = from o in session.Query<Order>()
                               group o by o.ShipTo.Country into g
                               select new
                               {
                                   Country = g.Key,
                                   Count = g.Count()
                               };


                    foreach (var item in totals)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }
    }
}
