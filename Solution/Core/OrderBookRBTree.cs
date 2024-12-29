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

    public override LimitOrder? GetBestOrder(Side side, LimitPrice? limitPrice = null)
    {
        var counterSideBook = side == Side.Buy ? _asks : _bids;

        if (counterSideBook.Count == 0)
            return null;

        foreach (var kvp in counterSideBook)
        {
            var counterPrice = kvp.Key;
            var queue = kvp.Value;

            if (limitPrice != null)
            {
                var priceSatisfiesLimit = side == Side.Buy
                    ? counterPrice.Amount <= limitPrice.Amount
                    : counterPrice.Amount >= limitPrice.Amount;

                if (!priceSatisfiesLimit)
                    return null;
            }

            // Deciding whether to keep empty queues or not depends on the use case
            // High Activity at Reused Price Levels (Frequent Updates) - Keep Empty Queues (In this impl)
            // Wide Price Ranges with Sparse Activity - Don't Keep Empty Queues
            if (queue.Count == 0)
                continue;

            return queue.Peek();
        }

        return null;
    }
}
