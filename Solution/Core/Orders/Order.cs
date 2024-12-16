using Core.Assets;

namespace Core.Orders;

public abstract class Order
{
    public Guid Id { get; set; }
    public Side Side { get; set; }
    public int Quantity { get; set; }
    public Asset Asset { get; set; }
    public DateTime Timestamp { get; set; }
    public OrderStatus Status { get; set; }

    protected Order(Guid id, Side side, int quantity, Asset asset, DateTime timestamp, OrderStatus status)
    {
        Id = id;
        Side = side;
        Quantity = quantity;
        Asset = asset;
        Timestamp = timestamp;
        Status = status;
    }

    public override string ToString()
    {
        return $"Order #{Id}: {Side} {Quantity} of {Asset.Name}. Placed at {Timestamp}, Status: {Status}";
    }
}

