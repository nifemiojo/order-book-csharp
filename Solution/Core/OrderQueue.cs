using System.Collections.Concurrent;
using Core.Orders;

namespace Core;

public class OrderQueue
{
    private readonly ConcurrentQueue<Order> _orders = new();
    private readonly object _processingLock = new();
    private readonly OrderBook _orderBook;

    private OrderQueue(OrderBook orderBook)
    {
        _orderBook = orderBook;
    }

    public static OrderQueue Create(OrderBook orderBook) => new(orderBook);

    public void Place(Order order)
    {
        _orders.Enqueue(order);
        Console.WriteLine($"Order {order.Id} enqueued at {DateTime.UtcNow} by thread {Thread.CurrentThread.ManagedThreadId}.");

        ProcessOrders(_orderBook);
    }

    private void ProcessOrders(OrderBook orderBook)
    {
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is attempting to acquire the lock at {DateTime.Now}");

        lock (_processingLock)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} acquired the lock at {DateTime.Now}");

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
}
