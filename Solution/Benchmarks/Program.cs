using BenchmarkDotNet.Running;
using Benchmarking;
using Core;
using Core.Assets;
using Core.Orders;
using System.Collections.Generic;
using System.Diagnostics;

//BenchmarkRunner.Run<OrderQueueOptV1Benchmarks>();

// Asset
var appleShares = Asset.Create("AAPL", "Apple Inc.");

// Order Book
var appleOrderBook = OrderBookRBTree.Create(appleShares);

// Order Queue
var appleOrderQueue = OrderQueue.Create(appleOrderBook);

var orderCount = 25000;
var random = new Random();
for (int i = 0; i < orderCount; i++)
{
    var price = random.NextDouble() + 400;
    var quantity = random.Next(1, 100);
    var side = random.Next(0, 2) == 0 ? Side.Buy : Side.Sell;
    appleOrderQueue.Place(LimitOrder.Create(side, quantity, appleShares, LimitPrice.Create(decimal.Round((decimal)price, 1))));
}

Thread.Sleep(5000);

// Process orders
await appleOrderQueue.StartProcessingOrdersAsync();