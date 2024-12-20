using System.Collections.Concurrent;
using Core.Orders;

namespace Core;

public class OrderQueue
{
    private readonly OrderBook _orderBook;
    private readonly ConcurrentQueue<Order> _orders = new();
    private readonly object workerLock = new();
    private Task? workerTask = null;

    private OrderQueue(OrderBook orderBook)
    {
        _orderBook = orderBook;
    }

    public static OrderQueue Create(OrderBook orderBook) => new(orderBook);

    public void Place(Order order)
    {
        _orders.Enqueue(order);

        Console.WriteLine($"Order {order.Id} enqueued at {DateTime.UtcNow} by thread {Thread.CurrentThread.ManagedThreadId}.");
    }

    public Task StartProcessingOrders()
    {
        lock (workerLock)
        {
            if (workerTask == null)
                return Task.Run(() => ProcessOrders(_orderBook));
        }

        return Task.CompletedTask;
    }

    private void ProcessOrders(OrderBook orderBook)
    {
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} has started processing orders at {DateTime.UtcNow}.");

        while (_orders.TryDequeue(out var order))
        {
            Console.WriteLine($"Order {order.Id} dequeued at {DateTime.UtcNow}");
            
            if (order is MarketOrder marketOrder)
            {
                MatchingService.PlaceOrder(marketOrder, orderBook);
            }
            else if (order is LimitOrder limitOrder)
            {
                MatchingService.PlaceOrder(limitOrder, orderBook);
            }
        }
    }
}
