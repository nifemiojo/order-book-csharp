namespace Core;

public class Asset
{
    public string AssetId { get; set; } // Unique identifier, e.g., "AAPL" or "BTC-USD"
    public string Name { get; set; } // Descriptive name, e.g., "Apple Inc." or "Bitcoin/USD"
    public AssetType AssetType { get; set; } // Type of asset, e.g., "Stock", "Crypto", "Forex"
    public string Exchange { get; set; } // Exchange or market where the asset is traded
    public string Unit { get; set; } // Unit of trade, e.g., "Shares", "BTC", "Lots"

    private Asset(string assetId, string name, AssetType assetType, string exchange, string unit)
    {
        AssetId = assetId;
        Name = name;
        AssetType = assetType;
        Exchange = exchange;
        Unit = unit;
    }

    public static Asset Create(string assetId, string name, AssetType assetType, string exchange, string unit)
    {
        return new(assetId, name, assetType, exchange, unit);
    }

    public override string ToString()
    {
        return $"{Name} ({AssetId}), Type: {AssetType}, Exchange: {Exchange}, Unit: {Unit}";
    }
}

