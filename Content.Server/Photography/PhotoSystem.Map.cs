using System.Numerics;
using Content.Shared.GameTicking;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Photography;

public sealed partial class PhotoSystem
{
    /// <summary>
    ///     Separation between tabletops in the tabletop map.
    /// </summary>
    private const int PhotoSeparation = 100;

    /// <summary>
    ///     Map where all tabletops reside.
    /// </summary>
    public MapId PhotoMap { get; private set; } = MapId.Nullspace;

    /// <summary>
    ///     The number of tabletops created in the map.
    ///     Used for calculating the position of the next one.
    /// </summary>
    private int _photos = 0;

    /// <summary>
    ///     Despite the name, this method is only used to subscribe to events.
    /// </summary>
    private void InitializeMap()
    {
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
    }

    /// <summary>
    ///     Gets the next available position for a tabletop, and increments the tabletop count.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetNextTabletopPosition()
    {
        return UlamSpiral(_photos++) * PhotoSeparation;
    }

    private Vector2i UlamSpiral(int n)
    {
        var k = (int) MathF.Ceiling(MathF.Sqrt(n) - 1) / 2;
        var t = 2 * k + 1;
        var m = (int) MathF.Pow(t, 2);
        t--;

        if (n >= m - t)
            return new Vector2i(k - (m - n), -k);

        m -= t;

        if (n >= m - t)
            return new Vector2i(-k, -k + (m - n));

        m -= t;

        if (n >= m - t)
            return new Vector2i(-k + (m - n), k);

        return new Vector2i(k, k - (m - n - t));
    }

    /// <summary>
    ///     Ensures that the tabletop map exists. Creates it if it doesn't.
    /// </summary>
    private void EnsurePhotoMap()
    {
        if (PhotoMap != MapId.Nullspace && _mapManager.MapExists(PhotoMap))
            return;

        PhotoMap = _mapManager.CreateMap();
        _photos = 0;
        var mapUid = _mapManager.GetMapEntityId(PhotoMap);

        var mapComp = EntityManager.GetComponent<MapComponent>(mapUid);

        // Lighting is always disabled in tabletop world.
        mapComp.LightingEnabled = true;
        Dirty(mapUid, mapComp);
    }

    private void OnRoundRestart(RoundRestartCleanupEvent _)
    {
        if (PhotoMap == MapId.Nullspace || !_mapManager.MapExists(PhotoMap))
            return;

        // This will usually *not* be the case, but better make sure.
        _mapManager.DeleteMap(PhotoMap);

        // Reset tabletop count.
        _photos = 0;
    }
}
