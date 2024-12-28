using BenchmarkDotNet.Attributes;
using Core;
using Core.Assets;
using Core.Orders;

namespace Benchmarking;

[SimpleJob(iterationCount: 10, invocationCount: 1)]
public class OrderQueueBenchmarks
{
    private OrderQueue _orderQueue;
    private Asset _asset;
    private OrderBook _orderBook;

    [Params(1000, 10000, 25000, 50000)]
    public int _orderCount;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _asset = Asset.Create("AAPL", "Apple Inc.");
        _orderBook = OrderBook.Create(_asset);
        _orderQueue = OrderQueue.Create(_orderBook);
    }

    [IterationSetup(Target = nameof(ProcessOrderQueueBatch))]
    public void Setup()
    {
        _orderQueue.Clear();
        _orderBook.Clear();

        var random = new Random();

        for (int i = 0; i < _orderCount; i++)
        {
            var price = random.NextDouble() + 400;
            var quantity = random.Next(1, 100);
            var side = random.Next(0, 2) == 0 ? Side.Buy : Side.Sell;
            _orderQueue.Place(LimitOrder.Create(side, quantity, _asset, LimitPrice.Create(decimal.Round((decimal)price, 1))));
        }
    }

    [Benchmark]
    public void ProcessOrderQueueBatch()
    {
        _orderQueue.StartProcessingOrders();
    }
}
