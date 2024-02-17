﻿using SoulsFormats;
using StudioCore.Editors.MapEditor;
using StudioCore.Formats;
using System.Collections.Generic;

namespace StudioCore.Formats;

/// <summary>
///     Navigation context for a map. Stores all the meta navigation information for a DS1
///     map (i.e. mcg and mcp stuff).
/// </summary>
public class NavigationContext
{
    public NavigationContext(MapObjectContainer map, MCP mcp, MCG mcg)
    {
        Map = map;

        foreach (MCP.Room r in mcp.Rooms)
        {
            Regions.Add(new NavigationRegion(Map, r));
        }
    }

    public MapObjectContainer Map { get; }

    public List<NavigationRegion> Regions { get; } = null;
}