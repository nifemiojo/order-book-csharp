using Core.Orders;

namespace Core;

public static class MatchingService
{
    public static void PlaceOrder(MarketOrder order, IOrderBook orderBook)
    {
        // Console.WriteLine($"Beginning to place order: {order}");  

        var remainingQuantity = order.Quantity;

        while (remainingQuantity > 0)
        {
            var bestOrder = orderBook.GetBestOrder(
                order.Side == Side.Buy ? Side.Sell : Side.Buy);

            if (bestOrder == null)
            {
                break;
            }

            var matchQuantity = Math.Min(bestOrder.Quantity, remainingQuantity);

            remainingQuantity -= matchQuantity;
            order.Quantity -= matchQuantity;
            bestOrder.Quantity -= matchQuantity;

            // Console.WriteLine($"Matched {matchQuantity} units at {bestOrder.Price.Amount:C} with order #{bestOrder.Id}.");

            if (bestOrder.Quantity == 0)
            {
                orderBook.RemoveOrder(bestOrder);
            }
        }

        if (remainingQuantity == order.Quantity)
        {
            order.Status = OrderStatus.NoLiquidity;
            // Console.WriteLine($"Market order #{order.Id} failed due to lack of liquidity.");
        }
        else if (remainingQuantity > 0)
        {
            order.Status = OrderStatus.PartiallyFilled;
            // Console.WriteLine($"Market order partially filled. Unable to fill {remainingQuantity} units.");
        }
        else
        {
            order.Status = OrderStatus.Filled;
            // Console.WriteLine($"{order.Quantity} units of {order.Asset.Name} fully filled for market order #{order.Id}.");
        }
    }

    public static void PlaceOrder(LimitOrder order, IOrderBook orderBook)
    {
        //Console.WriteLine($"Beginning to place order: {order}");

        var remainingQuantity = order.Quantity;

        while (remainingQuantity > 0)
        {
            var bestOrder = orderBook.GetBestOrder(
                order.Side == Side.Buy ? Side.Sell : Side.Buy,
                order.Price);

            if (bestOrder == null)
            {
                break;
            }

            var matchQuantity = Math.Min(bestOrder.Quantity, remainingQuantity);

            remainingQuantity -= matchQuantity;
            order.Quantity -= matchQuantity;
            bestOrder.Quantity -= matchQuantity;

            // Console.WriteLine($"Matched {matchQuantity} units at {bestOrder.Price.Amount:C} with order #{bestOrder.Id}.");

            if (bestOrder.Quantity == 0)
            {
                orderBook.RemoveOrder(bestOrder);
            }
        }

        if (remainingQuantity > 0)
        {
            orderBook.AddOrder(order);
            order.Status = OrderStatus.Open;
            if (remainingQuantity < order.Quantity)
            {
                // Console.WriteLine($"Limit order #{order.Id} partially filled {order.Quantity - remainingQuantity} units.");
            }
            // Console.WriteLine($"Limit order #{order.Id} opened on the order book with {remainingQuantity} units at {order.Price.Amount:C}.");
        }
        else
        {
            order.Status = OrderStatus.Filled;
            // Console.WriteLine($"{order.Quantity} units of {order.Asset.Name} fully filled for limit order #{order.Id} at {order.Price.Amount:C}.");
        }
    }
}
