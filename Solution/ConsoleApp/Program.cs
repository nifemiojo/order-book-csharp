using Core;
using Core.Assets;
using Core.Orders;

// Welcome to Femi's exchange! We're trading Apple stocks today.

// Asset
var appleShares = Asset.Create("AAPL", "Apple Inc.");

// Order Book - Existing orders
var appleOrderBook = OrderBook.Create(appleShares);
appleOrderBook.AddOrder(LimitOrder.Create(Side.Sell, 100, appleShares, 420));
appleOrderBook.AddOrder(LimitOrder.Create(Side.Sell, 50, appleShares, 415));
appleOrderBook.AddOrder(LimitOrder.Create(Side.Sell, 100, appleShares, 415));

// New orders
var marketOrderOne = MarketOrder.Create(Side.Buy, 100, appleShares);

// Place orders with matching engine
var matchingEngine = MatchingService.Create();
matchingEngine.PlaceOrder(marketOrderOne, appleOrderBook);