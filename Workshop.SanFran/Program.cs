using Raven.Client.Documents;
using System;

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
                    "http://localhost:8080"
                }
            })
            {
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    var nancy = session.Load<dynamic>("employees/1-A");
                    Console.WriteLine(nancy.FirstName + " " + nancy.LastName);
                }
            }
        }
    }
}
