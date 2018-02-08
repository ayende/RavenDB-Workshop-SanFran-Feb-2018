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
                    var emps = session.Advanced.RawQuery<Employee>("from Employees where Address.City = $city")
                        .AddParameter("$city", "London")
                        .Statistics(out var stats)
                        .Take(2)
                        .ToList();

                    foreach (var item in emps)
                    {
                        Console.WriteLine(item.LastName);
                    }
                }
            }
        }
    }
}
