namespace Core.Orders;

public record class LimitPrice
{
    public decimal Amount { get; set; }

    private LimitPrice(decimal amount)
    {
        Amount = amount;
    }

    public static LimitPrice Create(decimal amount) => new(amount);
}
