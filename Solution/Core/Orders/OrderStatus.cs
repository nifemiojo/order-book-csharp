namespace Core.Orders;

public enum OrderStatus
{
    /// <summary>
    /// The order has been submitted but not yet processed.
    /// </summary>
    Pending,

    /// <summary>
    /// 
    /// </summary>
    Open,

    /// <summary>
    /// Only part of the order quantity is matched.
    /// </summary>
    PartiallyFilled,

    /// <summary>
    /// The order is completely matched and executed.
    /// </summary>
    Filled,

    /// <summary>
    /// There is no liquidity available to execute the order.
    /// </summary>
    NoLiquidity,

    /// <summary>
    /// The order is canceled before execution.
    /// </summary>
    Cancelled
}
