using Core;
using Core.Assets;
using Core.Orders;
using FluentAssertions;

namespace UnitTests;

public class OrderBookBSTTests
{
    private readonly Asset _asset;
    private readonly OrderBookBST _sut;

    public OrderBookBSTTests()
    {
        _asset = Asset.Create("AAPL", "Apple Inc.");
        _sut = OrderBookBST.Create(_asset);
    }

    [Fact]
    public void Clear_ShouldClearTheOrderBook()
    {
        _sut.AddOrder(
            LimitOrder.Create(Side.Buy, 99, _asset, LimitPrice.Create(410)));
        _sut.AddOrder(
            LimitOrder.Create(Side.Sell, 101, _asset, LimitPrice.Create(415)));

        _sut.Clear();

        _sut.GetBidsSnapshot().Count.Should().Be(0);
        _sut.GetAsksSnapshot().Count.Should().Be(0);
    }

    [Fact]
    public void AddOrder_ShouldAddBuyOrderToBidsInDescendingPriceOrder()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBidsSnapshot().Keys.Should().ContainInOrder(LimitPrice.Create(412), LimitPrice.Create(321), LimitPrice.Create(103));
        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(412))!.Peek().Should().Be(order2);
        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(321))!.Peek().Should().Be(order3);
        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(103))!.Peek().Should().Be(order1);
    }

    [Fact]
    public void AddOrder_ShouldAddSellOrderToAsksInAscendingPriceOrder()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Sell, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetAsksSnapshot().Keys.Should().ContainInOrder(LimitPrice.Create(103), LimitPrice.Create(321), LimitPrice.Create(412));
        _sut.GetAsksSnapshot().GetValueOrDefault(LimitPrice.Create(103))!.Peek().Should().Be(order1);
        _sut.GetAsksSnapshot().GetValueOrDefault(LimitPrice.Create(321))!.Peek().Should().Be(order3);
        _sut.GetAsksSnapshot().GetValueOrDefault(LimitPrice.Create(412))!.Peek().Should().Be(order2);
    }
    
    [Fact]
    public void AddOrder_ShouldAddOrdersAtTheSamePriceInTheOrderReceived()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(103));
        var order3 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(110));
        var order4 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(103));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);
        _sut.AddOrder(order4);

        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(103))!.Dequeue().Should().Be(order1);        
        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(103))!.Dequeue().Should().Be(order2);        
        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(103))!.Dequeue().Should().Be(order4);        
    }

    [Fact]
    public void RemoveOrder_ShouldRemoveBuyOrder()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.RemoveOrder(order2);

        _sut.GetBidsSnapshot().GetValueOrDefault(LimitPrice.Create(412))!.Should().BeEmpty();
    }
    
    [Fact]
    public void RemoveOrder_ShouldRemoveSellOrder()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Sell, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.RemoveOrder(order1);

        _sut.GetAsksSnapshot().GetValueOrDefault(order1.Price)!.Should().BeEmpty();
    }

    [Theory]
    [InlineData(Side.Buy)]
    [InlineData(Side.Sell)]
    public void GetBestOrder_ShouldReturnNull_WhenOrderBookIsEmpty(Side side)
    {
        _sut.GetBestOrder(side).Should().BeNull();
    }

    [Fact]
    public void GetBestOrder_ShouldReturnHighestOldestBuyOrder_WhenSideIsBuy()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(412));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Buy).Should().Be(order2);
    }

    [Fact]
    public void GetBestOrder_ShouldReturnLowestOldestSellOrder_WhenSideIsSell()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(103));
        var order3 = LimitOrder.Create(Side.Sell, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Sell).Should().Be(order1);
    }

    [Fact]
    public void GetBestOrder_ShouldReturnNull_WhenSideIsBuyAndAllOrdersAreBelowLimit()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Buy, LimitPrice.Create(500)).Should().BeNull();
    }

    [Fact]
    public void GetBestOrder_ShouldReturnNull_WhenSideIsSellAndAllOrdersAreAboveLimit()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Sell, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Sell, LimitPrice.Create(50)).Should().BeNull();
    }

    [Fact]
    public void GetBestOrder_ShouldReturnHighestBuyOrder_WhenSideIsBuyAndThereAreOrdersAboveLimit()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Buy, LimitPrice.Create(250)).Should().Be(order2);
    }

    [Fact]
    public void GetBestOrder_ShouldReturnLowestSellOrder_WhenSideIsSellAndThereAreOrdersBelowLimit()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Sell, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Sell, LimitPrice.Create(400)).Should().Be(order1);
    }
}
