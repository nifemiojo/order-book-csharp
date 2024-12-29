using Core;
using Core.Assets;
using Core.Orders;
using FluentAssertions;

namespace UnitTests;

public class OrderBookTests
{
    private readonly Asset _asset;
    private readonly OrderBook _sut;

    public OrderBookTests()
    {
        _asset = Asset.Create("AAPL", "Apple Inc.");
        _sut = OrderBook.Create(_asset);
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

        _sut.GetBidsSnapshot().Should().ContainInOrder(order2, order3, order1);
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

        _sut.GetAsksSnapshot().Should().ContainInOrder(order1, order3, order2);
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

        _sut.GetBidsSnapshot().Count.Should().Be(2);
        _sut.GetBidsSnapshot().Should().ContainInOrder(order3, order1); 
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

        _sut.RemoveOrder(order2);

        _sut.GetAsksSnapshot().Count.Should().Be(2);
        _sut.GetAsksSnapshot().Should().ContainInOrder(order1, order3); 
    }

    [Theory]
    [InlineData(Side.Buy)]
    [InlineData(Side.Sell)]
    public void GetBestOrder_ShouldReturnNull_WhenOrderBookIsEmpty(Side side)
    {
        _sut.GetBestOrder(side).Should().BeNull();
    }

    [Fact]
    public void GetBestOrder_ShouldReturnHighestBuyOrder_WhenSideIsBuy()
    {
        var order1 = LimitOrder.Create(Side.Buy, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Buy, 105, _asset, LimitPrice.Create(412));
        var order3 = LimitOrder.Create(Side.Buy, 108, _asset, LimitPrice.Create(321));

        _sut.AddOrder(order1);
        _sut.AddOrder(order2);
        _sut.AddOrder(order3);

        _sut.GetBestOrder(Side.Buy).Should().Be(order2);
    }

    [Fact]
    public void GetBestOrder_ShouldReturnLowestSellOrder_WhenSideIsSell()
    {
        var order1 = LimitOrder.Create(Side.Sell, 98, _asset, LimitPrice.Create(103));
        var order2 = LimitOrder.Create(Side.Sell, 105, _asset, LimitPrice.Create(412));
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
