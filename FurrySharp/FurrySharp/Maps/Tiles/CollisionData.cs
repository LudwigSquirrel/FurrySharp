using System.Text.Json.Serialization;
using FurrySharp.Entities.Base;

namespace FurrySharp.Maps.Tiles;

public class CollisionData
{
    public int Start { get; set; }

    public int? End { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Touching Allowed { get; set; }
}