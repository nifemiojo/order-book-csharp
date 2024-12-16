using Core.Assets;
using Core.Orders;

namespace Core;

public class OrderBook
{
    public Asset Asset { get; private set; }
    public List<LimitOrder> Bids { get; private set; }
    public List<LimitOrder> Asks { get; private set; }

    private OrderBook(Asset asset)
    {
        Bids = [];
        Asks = [];
        Asset = asset;
    }

    public static OrderBook Create(Asset asset)
    {
        return new OrderBook(asset);
    }

    public void AddOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
        {
            Bids.Add(order);
            Bids.Sort((a, b) => b.Price.CompareTo(a.Price));
        }
        else if (order.Side == Side.Sell)
        {
            Asks.Add(order);
            Asks.Sort((a, b) => a.Price.CompareTo(b.Price));
        }
    }

    public void RemoveOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
            Bids.Remove(order);
        else if (order.Side == Side.Sell)
            Asks.Remove(order);
    }

    internal List<LimitOrder> GetCounterOrders(Side side)
    {
        return side == Side.Buy ? Asks : Bids;
    }
}
