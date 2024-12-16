using Core;
using Core.Assets;
using Core.Orders;

// Welcome to Femi's exchange! We're trading Apple stocks today.

// Asset
var appleShares = Asset.Create("AAPL", "Apple Inc.");

// Order Book
var appleOrderBook = OrderBook.Create(appleShares);

// Place orders with matching service
MatchingService.PlaceOrder(
    LimitOrder.Create(Side.Sell, 100, appleShares, LimitPrice.Create(420)),
    appleOrderBook);

MatchingService.PlaceOrder(
    LimitOrder.Create(Side.Sell, 50, appleShares, LimitPrice.Create(415)),
    appleOrderBook);

MatchingService.PlaceOrder(
    LimitOrder.Create(Side.Sell, 100, appleShares, LimitPrice.Create(415)),
    appleOrderBook);

MatchingService.PlaceOrder(
    MarketOrder.Create(Side.Buy, 100, appleShares),
    appleOrderBook);

MatchingService.PlaceOrder(
    LimitOrder.Create(Side.Buy, 100, appleShares, LimitPrice.Create(415)),
    appleOrderBook);