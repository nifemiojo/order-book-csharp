using Core.Assets;
using Core.Orders;

namespace Core;

public class OrderBook
{
    public Asset Asset { get; private set; }
    public List<LimitOrder> Bids { get; private set; }
    public List<LimitOrder> Asks { get; private set; }

    protected OrderBook(Asset asset)
    {
        Bids = [];
        Asks = [];
        Asset = asset;
    }

    public static OrderBook Create(Asset asset)
    {
        return new OrderBook(asset);
    }

    public virtual void Clear()
    {
        Bids.Clear();
        Asks.Clear();
    }

    public virtual void AddOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
        {
            Bids.Add(order);
            Bids.Sort((a, b) => b.Price.Amount.CompareTo(a.Price.Amount));
        }
        else if (order.Side == Side.Sell)
        {
            Asks.Add(order);
            Asks.Sort((a, b) => a.Price.Amount.CompareTo(b.Price.Amount));
        }
    }

    public virtual void RemoveOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
            Bids.Remove(order);
        else if (order.Side == Side.Sell)
            Asks.Remove(order);
    }

    public virtual List<LimitOrder> GetCounterOrders(Side side, LimitPrice? limitPrice = null)
    {
        if (side == Side.Buy)
            return limitPrice == null ? Asks : Asks.Where(order => order.Price.Amount <= limitPrice.Amount).ToList();
        else if (side == Side.Sell)
            return limitPrice == null ? Bids : Bids.Where(order => order.Price.Amount >= limitPrice.Amount).ToList();
        else
            throw new ArgumentException("Invalid side");
    }
}
