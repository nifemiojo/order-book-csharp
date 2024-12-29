using Core.Orders;

namespace Core;

public interface IOrderBook
{
    public void Clear();

    public void AddOrder(LimitOrder order);

    public void RemoveOrder(LimitOrder order);

    public LimitOrder? GetBestOrder(Side side, LimitPrice? limitPrice = null);
}
