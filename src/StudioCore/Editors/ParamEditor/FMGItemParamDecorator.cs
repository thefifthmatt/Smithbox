﻿using ImGuiNET;
using SoulsFormats;
using StudioCore.Editors.TextEditor.Utils;
using StudioCore.Editors.TextEditor;
using StudioCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andre.Formats;
using StudioCore.Editor;
using HKLib.hk2018.hkWeakPtrTest;
using Silk.NET.OpenGL;

namespace StudioCore.Editors.ParamEditor;

/// <summary>
///     Interface for decorating param rows with additional information (such as english
///     strings sourced from FMG files)
/// </summary>
public interface IParamDecorator
{
    public void DecorateParam(Param.Row row);

    public void DecorateContextMenuItems(Param.Row row);

    public void ClearDecoratorCache();
}

public class FMGItemParamDecorator : IParamDecorator
{
    private readonly Dictionary<int, FMG.Entry> _entryCache = new();

    private string ParamName;
    private string CommandLine;

    public FMGItemParamDecorator(string paramName)
    {
        ParamName = paramName;
        CommandLine = "";
    }

    public void ClearDecoratorCache()
    {
        _entryCache.Clear();
    }


    public void DecorateParam(Param.Row row)
    {
        PopulateDecorator();
        FMG.Entry entry = null;
        _entryCache.TryGetValue(row.ID, out entry);

        if (entry != null)
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, UI.Current.ImGui_FmgLink_Text);
            ImGui.TextUnformatted($@" <{entry.Text}>");
            ImGui.PopStyleColor();
        }
    }

    public void DecorateContextMenuItems(Param.Row row)
    {
        PopulateDecorator();
        if (!_entryCache.ContainsKey(row.ID))
        {
            return;
        }

        if (CommandLine == "")
        {
            var category = CFG.Current.TextEditor_PrimaryCategory.ToString();
            if (_entryCache.Values.Count > 0)
            {
                var cachedEntry = _entryCache.Values.Where(e => e.ID == row.ID).FirstOrDefault();

                var containerName = "";
                var fmg = cachedEntry.Parent;
                var fmgName = fmg.Name;

                foreach (var (path, entry) in TextBank.FmgBank)
                {
                    if (entry.ContainerDisplayCategory == CFG.Current.TextEditor_PrimaryCategory)
                    {
                        foreach(var fmgInfo in entry.FmgWrappers)
                        {
                            if(fmgInfo.Name == fmgName)
                            {
                                containerName = entry.Filename;
                            }
                        }
                    }
                }

                CommandLine = $@"text/select/{category}/{containerName}/{fmgName}/{row.ID}";
            }
        }
        else
        {
            if (CommandLine != "")
            {
                if (ImGui.Selectable($@"Go to Text"))
                {
                    EditorCommandQueue.AddCommand(CommandLine);
                }
            }
        }
    }

    private void PopulateDecorator()
    {
        // FMG Name decoration on row 
        if (_entryCache.Count == 0 && TextBank.PrimaryBankLoaded)
        {
            List<FMG.Entry> fmgEntries = TextParamUtils.GetFmgEntriesByAssociatedParam(ParamName);
            foreach (FMG.Entry fmgEntry in fmgEntries)
            {
                _entryCache[fmgEntry.ID] = fmgEntry;
            }
        }
    }
}