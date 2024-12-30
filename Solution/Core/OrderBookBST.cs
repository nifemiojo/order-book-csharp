using Core.Assets;
using Core.Orders;

namespace Core;

/// <summary>
/// Price time priority order book.
/// An optimised implementation of the order book.
/// Using a binary search tree to store the orders.
/// </summary>
public class OrderBookBST : IOrderBook
{
    private readonly Asset _asset;
    private readonly SortedDictionary<LimitPrice, Queue<LimitOrder>> _bids;
    private readonly SortedDictionary<LimitPrice, Queue<LimitOrder>> _asks;

    private OrderBookBST(Asset asset)
    {
        _asset = asset;
        _bids = new SortedDictionary<LimitPrice, Queue<LimitOrder>>(
            Comparer<LimitPrice>.Create((a, b) => b.Amount.CompareTo(a.Amount)));
        _asks = new SortedDictionary<LimitPrice, Queue<LimitOrder>>(
            Comparer<LimitPrice>.Create((a, b) => a.Amount.CompareTo(b.Amount)));
    }

    public static OrderBookBST Create(Asset asset)
    {
        return new OrderBookBST(asset);
    }

    public void Clear()
    {
        _bids.Clear();
        _asks.Clear();
    }

    public void AddOrder(LimitOrder order)
    {
        var bookSide = order.Side == Side.Buy ? _bids : _asks;

        if (!bookSide.ContainsKey(order.Price))
            bookSide.Add(order.Price, new Queue<LimitOrder>());

        bookSide[order.Price].Enqueue(order);
    }

    public void RemoveOrder(LimitOrder order)
    {
        var bookSide = order.Side == Side.Buy ? _bids : _asks;

        if (!bookSide.ContainsKey(order.Price))
            return;

        bookSide[order.Price].Dequeue();
    }

    public LimitOrder? GetBestOrder(Side side, LimitPrice? limitPrice = null)
    {
        var counterSideBook = side == Side.Buy ? _bids : _asks;

        if (counterSideBook.Count == 0)
            return null;

        foreach (var kvp in counterSideBook)
        {
            var counterPrice = kvp.Key;
            var queue = kvp.Value;

            if (limitPrice != null)
            {
                var priceSatisfiesLimit = side == Side.Buy
                    ? counterPrice.Amount >= limitPrice.Amount
                    : counterPrice.Amount <= limitPrice.Amount;

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

    // Expose snapshots for testing
    // In reality we'd have methods for accessing the underlying data structures e.g. market depth but this is out of scope
    public IReadOnlyDictionary<LimitPrice, Queue<LimitOrder>> GetBidsSnapshot() => _bids;
    public IReadOnlyDictionary<LimitPrice, Queue<LimitOrder>> GetAsksSnapshot() => _asks;
}
