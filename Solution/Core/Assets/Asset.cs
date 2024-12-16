namespace Core.Assets;

public class Asset
{
    /// <summary>
    /// Unique identifier, e.g., "AAPL" or "BTC-USD"
    /// </summary>
    public string AssetId { get; set; }

    /// <summary>
    /// Descriptive name, e.g., "Apple Inc." or "Bitcoin/USD"
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Type of asset, e.g., "Stock", "Crypto", "Forex"
    /// </summary>
    public AssetType AssetType { get; set; }

    private Asset(string assetId, string name, AssetType assetType)
    {
        AssetId = assetId;
        Name = name;
        AssetType = assetType;
    }

    public static Asset Create(string assetId, string name)
    {
        return new(assetId, name, AssetType.Stock);
    }

    public override string ToString()
    {
        return $"{Name} ({AssetId}), Type: {AssetType}";
    }
}

