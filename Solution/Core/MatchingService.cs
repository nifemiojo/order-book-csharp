using Core.Orders;

namespace Core;

public class MatchingService
{
    private MatchingService()
    {}

    public static MatchingService Create()
    {
        return new MatchingService();
    }

    public void PlaceOrder(MarketOrder order, OrderBook orderBook)
    {
        Console.WriteLine($"Placing order: {order}");

        var counterOrders = orderBook.GetCounterOrders(order.Side);

        var remainingQuantity = order.Quantity;

        while (remainingQuantity > 0 && counterOrders.Count > 0)
        {
            var bestOrder = counterOrders.First();

            var matchQuantity = Math.Min(bestOrder.Quantity, remainingQuantity);

            remainingQuantity -= matchQuantity;
            bestOrder.Quantity -= matchQuantity;

            Console.WriteLine($"Matched {matchQuantity} units at {bestOrder.Price:C} with order #{bestOrder.Id}.");

            if (bestOrder.Quantity == 0)
            {
                orderBook.RemoveOrder(bestOrder);
            }
        }

        if (remainingQuantity == order.Quantity)
        {
            order.Status = OrderStatus.NoLiquidity;
            Console.WriteLine($"Market order #{order.Id} failed due to lack of liquidity.");
        }
        else if (remainingQuantity > 0)
        {
            order.Status = OrderStatus.PartiallyFilled;
            Console.WriteLine($"Market order partially filled. Unable to fill {remainingQuantity} units.");
        }
        else
        {
            order.Status = OrderStatus.Filled;
            Console.WriteLine($"{order.Quantity} units of {order.Asset.Name} fully filled for market order #{order.Id}.");
        }
    }
}
