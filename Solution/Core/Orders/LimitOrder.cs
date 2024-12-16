using Core.Assets;

namespace Core.Orders;

public class LimitOrder : Order
{
    public decimal Price { get; set; }

    protected LimitOrder(Guid id, Side side, int quantity, Asset asset, decimal price, DateTime timestamp, OrderStatus status)
        : base(id, side, quantity, asset, timestamp, status)
    {
        Price = price;
    }

    public static LimitOrder Create(Side side, int quantity, Asset asset, decimal price)
    {
        return new(Guid.NewGuid(), side, quantity, asset, price, DateTime.UtcNow, OrderStatus.Pending);
    }

    public override string ToString()
    {
        return $"Limit {base.ToString()}, Price: {Price:C}";
    }

}
