using Core.Assets;
using Core.Orders;

namespace Core;

/// <summary>
/// Price priority order book. An unoptimised implementation.
/// </summary>
public class OrderBook : IOrderBook
{
    private readonly Asset _asset;
    private readonly List<LimitOrder> _bids;
    private readonly List<LimitOrder> _asks;

    private OrderBook(Asset asset)
    {
        _bids = [];
        _asks = [];
        _asset = asset;
    }

    public static OrderBook Create(Asset asset)
    {
        return new OrderBook(asset);
    }

    public void Clear()
    {
        _bids.Clear();
        _asks.Clear();
    }

    public void AddOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
        {
            _bids.Add(order);
            _bids.Sort((a, b) => b.Price.Amount.CompareTo(a.Price.Amount));
        }
        else if (order.Side == Side.Sell)
        {
            _asks.Add(order);
            _asks.Sort((a, b) => a.Price.Amount.CompareTo(b.Price.Amount));
        }
    }

    public void RemoveOrder(LimitOrder order)
    {
        if (order.Side == Side.Buy)
            _bids.Remove(order);
        else if (order.Side == Side.Sell)
            _asks.Remove(order);
    }

    public List<LimitOrder> GetCounterOrders(Side side, LimitPrice? limitPrice = null)
    {
        if (side == Side.Buy)
            return limitPrice == null ? _asks : _asks.Where(order => order.Price.Amount <= limitPrice.Amount).ToList();
        else if (side == Side.Sell)
            return limitPrice == null ? _bids : _bids.Where(order => order.Price.Amount >= limitPrice.Amount).ToList();
        else
            throw new ArgumentException("Invalid side");
    }

    public LimitOrder? GetBestOrder(Side side, LimitPrice? limitPrice = null)
    {
        var counterSideBook = side == Side.Buy ? _bids : _asks;

        if (counterSideBook.Count == 0)
            return null;

        var bestOrder = counterSideBook.First();

        if (limitPrice != null)
        {
            var priceSatisfiesLimit = side == Side.Buy
                ? bestOrder.Price.Amount >= limitPrice.Amount
                : bestOrder.Price.Amount <= limitPrice.Amount;

            if (!priceSatisfiesLimit)
                return null;
        }

        return bestOrder;
    }

    // Expose snapshots for testing
    public IReadOnlyList<LimitOrder> GetBidsSnapshot() => _bids.AsReadOnly();
    public IReadOnlyList<LimitOrder> GetAsksSnapshot() => _asks.AsReadOnly();
}
