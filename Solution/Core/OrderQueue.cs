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

    public int Count => _orders.Count;

    public void Place(Order order)
    {
        _orders.Enqueue(order);

        //Console.WriteLine($"Order {order.Id} enqueued at {DateTime.UtcNow} by thread {Thread.CurrentThread.ManagedThreadId}.");
    }

    public void Clear()
    {
        _orders.Clear();
    }

    public Task StartProcessingOrdersAsync(bool keepAlive = false, CancellationTokenSource? cts = null)
    {
        // Console.WriteLine("Starting order queue processing.");
        // Minimising lock granularity
        lock (workerLock)
        {
            if (workerTask == null)
            {
                //Console.WriteLine("Starting order queue worker.");

                if (keepAlive)
                {
                    cts ??= new CancellationTokenSource();

                    workerTask = Task.Run(
                        () => ProcessOrdersOngoing(_orderBook, cts.Token));
                }
                else
                {
                    workerTask = Task.Run(
                        () => ProcessOrdersQueue(_orderBook));
                }

                return workerTask;
            }
        }

        return Task.CompletedTask;
    }

    public void StartProcessingOrders()
    {
        lock (workerLock)
        {
            ProcessOrdersQueue(_orderBook);
        }
    }

    private void ProcessOrdersOngoing(OrderBook orderBook, CancellationToken token)
    {
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} has started processing orders at {DateTime.UtcNow}.");

        while (!token.IsCancellationRequested)
        {
            if (_orders.TryDequeue(out var order))
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

    private void ProcessOrdersQueue(OrderBook orderBook)
    {
        // Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} has started processing orders at {DateTime.UtcNow}.");

        while (_orders.TryDequeue(out var order))
        {
            //Console.WriteLine($"Order {order.Id} dequeued at {DateTime.UtcNow}");
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
