﻿using StudioCore.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Memory;

public class GameOffsetsEntry
{
    internal static Dictionary<ProjectType, GameOffsetsEntry> GameOffsetBank = new();

    internal string exeName;
    internal bool Is64Bit;
    internal Dictionary<string, int> itemGibOffsets;

    // Hard offset for param base. Unused if ParamBase AOB is set.
    internal int ParamBaseOffset = 0;

    // AOB for param base offset. If null, ParamBaseOffset will be used instead.
    internal string ParamBaseAobPattern;
    internal List<(int, int)> ParamBaseAobRelativeOffsets = new();

    internal int paramCountOffset;
    internal int paramDataOffset;
    internal int[] paramInnerPath;
    internal Dictionary<string, int> paramOffsets;
    internal int rowHeaderSize;
    internal int rowPointerOffset;
    internal ProjectType type;

    internal GameOffsetsEntry(ProjectEntry project)
    {
        var data = project.ParamMemoryOffsets.list[CFG.Current.SelectedGameOffsetData];

        paramOffsets = new();
        itemGibOffsets = new();

        exeName = project.ParamMemoryOffsets.exeName;

        if (!string.IsNullOrEmpty(data.paramBase))
        {
            ParamBaseOffset = Utils.ParseHexFromString(data.paramBase);
        }

        if (!string.IsNullOrEmpty(data.paramBaseAob))
        {
            ParamBaseAobPattern = data.paramBaseAob;
        }

        if (!string.IsNullOrEmpty(data.paramBaseAobRelativeOffset))
        {
            foreach (var relativeOffset in data.paramBaseAobRelativeOffset.Split(','))
            {
                var split = relativeOffset.Split('/');
                ParamBaseAobRelativeOffsets.Add(new(Utils.ParseHexFromString(split[0]), Utils.ParseHexFromString(split[1])));
            }
        }

        if (!string.IsNullOrEmpty(data.paramInnerPath))
        {
            var innerpath = data.paramInnerPath.Split("/");
            paramInnerPath = new int[innerpath.Length];

            for (var i = 0; i < innerpath.Length; i++)
            {
                paramInnerPath[i] = Utils.ParseHexFromString(innerpath[i]);
            }
        }

        if (!string.IsNullOrEmpty(data.paramCountOffset))
        {
            paramCountOffset = Utils.ParseHexFromString(data.paramCountOffset);
        }

        if (!string.IsNullOrEmpty(data.paramDataOffset))
        {
            paramDataOffset = Utils.ParseHexFromString(data.paramDataOffset);
        }

        if (!string.IsNullOrEmpty(data.rowPointerOffset))
        {
            rowPointerOffset = Utils.ParseHexFromString(data.rowPointerOffset);
        }

        if (!string.IsNullOrEmpty(data.rowHeaderSize))
        {
            rowHeaderSize = Utils.ParseHexFromString(data.rowHeaderSize);
        }

        foreach (var entry in data.paramOffsets)
        {
            var name = entry.Split(':')[0];
            var address = entry.Split(':')[1];

            paramOffsets.Add(name, Utils.ParseHexFromString(address));
        }

        foreach (var entry in data.itemGibOffsets)
        {
            var name = entry.Split(':')[0];
            var address = entry.Split(':')[1];

            itemGibOffsets.Add(name, Utils.ParseHexFromString(address));
        }

        Is64Bit = type != ProjectType.DS1;
        type = project.ProjectType;
    }

    internal GameOffsetsEntry()
    { }

}
