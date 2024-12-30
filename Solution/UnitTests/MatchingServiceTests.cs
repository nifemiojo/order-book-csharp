using FluentAssertions;
using Moq;
using Core;
using Core.Orders;
using Core.Assets;

namespace UnitTests;

public class MatchingServiceTests
{
    private readonly Asset _asset;

    public MatchingServiceTests()
    {
        _asset = Asset.Create("AAPL", "Apple Inc.");
    }

    [Fact]
    public void PlaceMarketOrder_ShouldFillCompletely_WhenLiquidityAvailable()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        var limitOrder = LimitOrder.Create(Side.Sell, 50, _asset, LimitPrice.Create(100));
        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, null)).Returns(limitOrder);

        var marketOrder = MarketOrder.Create(Side.Buy, 50, _asset);

        MatchingService.PlaceOrder(marketOrder, mockOrderBook.Object);

        marketOrder.Status.Should().Be(OrderStatus.Filled);
        mockOrderBook.Verify(o => o.RemoveOrder(limitOrder), Times.Once);
    }

    [Fact]
    public void PlaceMarketOrder_ShouldPartiallyFill_WhenNotEnoughLiquidity()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        var limitOrder = LimitOrder.Create(Side.Sell, 30, _asset, LimitPrice.Create(100));
        var queue = new Queue<LimitOrder>();
        queue.Enqueue(limitOrder);
        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, null)).Returns(new Func<LimitOrder?>(() => queue.Count == 0 ? null : queue.Dequeue()));

        var marketOrder = MarketOrder.Create(Side.Buy, 50, _asset);

        MatchingService.PlaceOrder(marketOrder, mockOrderBook.Object);

        marketOrder.Status.Should().Be(OrderStatus.PartiallyFilled);
        mockOrderBook.Verify(o => o.RemoveOrder(limitOrder), Times.Once);
    }
    
    [Fact]
    public void PlaceMarketOrder_ShouldUpdateOrderStatus_WhenNoLiquidity()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, null)).Returns((LimitOrder)null);

        var marketOrder = MarketOrder.Create(Side.Buy, 50, _asset);

        MatchingService.PlaceOrder(marketOrder, mockOrderBook.Object);

        marketOrder.Status.Should().Be(OrderStatus.NoLiquidity);
        mockOrderBook.Verify(o => o.GetBestOrder(Side.Sell, null), Times.Once);
    }
    
    [Fact]
    public void PlaceMarketOrder_ShouldFillFromMultipleOrders_WhenBestOrderHasInsufficientQuantity()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        var limitOrder1 = LimitOrder.Create(Side.Sell, 25, _asset, LimitPrice.Create(100));
        var limitOrder2 = LimitOrder.Create(Side.Sell, 25, _asset, LimitPrice.Create(100));
        var queue = new Queue<LimitOrder>();
        queue.Enqueue(limitOrder1);
        queue.Enqueue(limitOrder2);
        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, null)).Returns(new Func<LimitOrder?>(() => queue.Count == 0 ? null : queue.Dequeue()));

        var marketOrder = MarketOrder.Create(Side.Buy, 50, _asset);

        MatchingService.PlaceOrder(marketOrder, mockOrderBook.Object);

        marketOrder.Status.Should().Be(OrderStatus.Filled);
        mockOrderBook.Verify(o => o.GetBestOrder(Side.Sell, null), Times.Exactly(2));
    }

    [Fact]
    public void PlaceLimitOrder_ShouldAddToOrderBook_WhenNoMatchingOrders()
    {
        // Arrange
        var mockOrderBook = new Mock<IOrderBook>();
        var limitOrder = LimitOrder.Create(Side.Buy, 50, _asset, LimitPrice.Create(100));
        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, It.IsAny<LimitPrice>())).Returns((LimitOrder)null);

        // Act
        MatchingService.PlaceOrder(limitOrder, mockOrderBook.Object);

        // Assert
        limitOrder.Status.Should().Be(OrderStatus.Open);
        mockOrderBook.Verify(o => o.AddOrder(limitOrder), Times.Once);
    }

    [Fact]
    public void PlaceLimitOrder_ShouldPartiallyFillAndAddRemainderToOrderBook_WhenExistingOrdersHaveInsufficientQuantity()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        var existingOrder = LimitOrder.Create(Side.Buy, 30, _asset, LimitPrice.Create(100));
        var limitOrder = LimitOrder.Create(Side.Buy, 50, _asset, LimitPrice.Create(100));

        mockOrderBook.SetupSequence(o => o.GetBestOrder(Side.Sell, It.IsAny<LimitPrice>()))
            .Returns(existingOrder)
            .Returns((LimitOrder)null);

        MatchingService.PlaceOrder(limitOrder, mockOrderBook.Object);

        limitOrder.Status.Should().Be(OrderStatus.Open);
        mockOrderBook.Verify(o => o.RemoveOrder(existingOrder), Times.Once);
        mockOrderBook.Verify(o => o.AddOrder(It.Is<LimitOrder>(o => o.Quantity == 20)), Times.Once);
    }

    [Fact]
    public void PlaceLimitOrder_Should_FillCompletely_WhenMatchingOrdersAvailable()
    {
        var mockOrderBook = new Mock<IOrderBook>();
        var existingOrder = LimitOrder.Create(Side.Sell, 50, _asset, LimitPrice.Create(100));
        var limitOrder = LimitOrder.Create(Side.Buy, 50, _asset, LimitPrice.Create(100));

        mockOrderBook.Setup(o => o.GetBestOrder(Side.Sell, It.IsAny<LimitPrice>())).Returns(existingOrder);

        MatchingService.PlaceOrder(limitOrder, mockOrderBook.Object);

        limitOrder.Status.Should().Be(OrderStatus.Filled);
        mockOrderBook.Verify(o => o.RemoveOrder(existingOrder), Times.Once);
    } 
}
