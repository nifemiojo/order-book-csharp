using Core.Assets;
using Core.Orders;

namespace Core;

public class OrderBookRBTree : OrderBook
{
    private readonly Asset _asset;
    private readonly SortedDictionary<LimitPrice, Queue<LimitOrder>> _bids;
    private readonly SortedDictionary<LimitPrice, Queue<LimitOrder>> _asks;

    private OrderBookRBTree(Asset asset) : base(asset)
    {
        _asset = asset;
        _bids = new SortedDictionary<LimitPrice, Queue<LimitOrder>>(
            Comparer<LimitPrice>.Create((a, b) => b.Amount.CompareTo(a.Amount)));
        _asks = new SortedDictionary<LimitPrice, Queue<LimitOrder>>(
            Comparer<LimitPrice>.Create((a, b) => a.Amount.CompareTo(b.Amount)));
    }

    public new static OrderBookRBTree Create(Asset asset)
    {
        return new OrderBookRBTree(asset);
    }

    public override void Clear()
    {
        _bids.Clear();
        _asks.Clear();
    }

    public override void AddOrder(LimitOrder order)
    {
        var bookSide = order.Side == Side.Buy ? _bids : _asks;

        if (!bookSide.ContainsKey(order.Price))
            bookSide.Add(order.Price, new Queue<LimitOrder>());

        bookSide[order.Price].Enqueue(order);
    }

    public override void RemoveOrder(LimitOrder order)
    {
        var bookSide = order.Side == Side.Buy ? _bids : _asks;

        if (!bookSide.ContainsKey(order.Price))
            return;

        bookSide[order.Price].Dequeue();
    }

    public override List<LimitOrder> GetCounterOrders(Side side, LimitPrice? limitPrice = null)
    {
        var bookSide = side == Side.Buy ? _asks : _bids;

        if (bookSide.Count == 0) return [];

        if (limitPrice == null) return bookSide.Values.SelectMany(x => x).ToList();

        if (side == Side.Buy)
            return bookSide.Where(priceOrders => priceOrders.Key.Amount >= limitPrice.Amount).SelectMany(x => x.Value).ToList();
        else
            return bookSide.Where(priceOrders => priceOrders.Key.Amount <= limitPrice.Amount).SelectMany(x => x.Value).ToList();
    }
}
