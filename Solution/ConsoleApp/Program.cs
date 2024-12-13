using Core;

var appleStock = Asset.Create("AAPL", "Apple Inc.", AssetType.Stock, "NASDAQ", "Shares");
var order = Order.Create(OrderAction.Buy, 100, appleStock);

Console.WriteLine(appleStock);
Console.WriteLine(order);