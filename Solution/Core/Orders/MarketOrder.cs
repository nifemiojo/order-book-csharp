using Core.Assets;

namespace Core.Orders;

public class MarketOrder : Order
{
    private MarketOrder(Guid id, Side action, int quantity, Asset asset, DateTime timestamp, OrderStatus status) 
        : base(id, action, quantity, asset, timestamp, status)
    {
    }

    public static MarketOrder Create(Side action, int quantity, Asset asset)
    {
        return new(Guid.NewGuid(), action, quantity, asset, DateTime.UtcNow, OrderStatus.Pending);
    }

    public override string ToString()
    {
        return $"Market {base.ToString()}";
    }
}
