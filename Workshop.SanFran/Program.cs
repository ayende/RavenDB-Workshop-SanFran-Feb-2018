using System;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands.Batches;
using Workshop.SanFran.Orders;

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
                    session.Advanced.UseOptimisticConcurrency = true;

                    var line = new OrderLine
                    {
                        Product = "products/1-A",
                        Discount = 0,
                        PricePerUnit = 3,
                        ProductName = "Milk",
                        Quantity = 3
                    };

                    session.Advanced.Defer(new PatchCommandData("orders/830-A", null, new Raven.Client.Documents.Operations.PatchRequest
                    {
                        Script = @"
for(var i = 0;  i < this.Lines.length; i ++ )
{
    if(this.Lines[i].Product == $line.Product)
    {
        this.Lines[i].Quantity += $line.Quantity;
        return;
    }
}
this.Lines.push($line);
",
                        Values = {["line"] = line}
                    }, patchIfMissing: null));

                    session.SaveChanges();
                }
            }
        }
    }
}
