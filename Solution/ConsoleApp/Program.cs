using Core;
using Core.Assets;
using Core.Orders;

// Welcome to Femi's exchange! We're trading Apple stocks today.

// Asset
var appleShares = Asset.Create("AAPL", "Apple Inc.");

// Order Book
var appleOrderBook = OrderBook.Create(appleShares);

// Order Queue
var appleOrderQueue = OrderQueue.Create(appleOrderBook);

// Process orders
var orderProcessingTask = appleOrderQueue.StartProcessingOrders();

// Existing order
appleOrderQueue.Place(LimitOrder.Create(Side.Sell, 100, appleShares, LimitPrice.Create(410)));

// Simultaneous orders
var launchTasks = new List<Task>
{
    Task.Run(() => appleOrderQueue.Place(
    LimitOrder.Create(Side.Buy, 100, appleShares, LimitPrice.Create(415)))),
    
    Task.Run(() => appleOrderQueue.Place(
    MarketOrder.Create(Side.Buy, 100, appleShares))),
    
    Task.Run(() => appleOrderQueue.Place(
    LimitOrder.Create(Side.Buy, 100, appleShares, LimitPrice.Create(415)))),

    orderProcessingTask
};

await Task.WhenAll(launchTasks);
