namespace Core;

public class Order
{
    public Guid Id { get; set; }
    public OrderAction Action { get; set; }
    public int Quantity { get; set; }
    public Asset Asset { get; set; }
    public DateTime Timestamp { get; set; }
    public OrderStatus Status { get; set; }

    private Order(Guid id, OrderAction action, int quantity, Asset asset, DateTime timestamp, OrderStatus status)
    {
        Id = id;
        Action = action;
        Quantity = quantity;
        Asset = asset;
        Timestamp = timestamp;
        Status = status;
    }

    public static Order Create(OrderAction action, int quantity, Asset asset)
    {
        return new(Guid.NewGuid(), action, quantity, asset, DateTime.UtcNow, OrderStatus.Pending);
    }

    public override string ToString()
    {
        return $"Order #{Id}: {Action} {Quantity} {Asset.Unit} of {Asset.Name}. Placed at {Timestamp}, Status: {Status}";
    }
}

