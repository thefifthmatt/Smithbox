﻿using SoulsFormats;
using StudioCore.Editors.MapEditor;
using StudioCore.Editors.ModelEditor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static SoulsFormats.FLVER2;

namespace StudioCore.Editors.ModelEditor.Actions.Texture;

public class AddTexture : ViewportAction
{
    private ModelEditorScreen Screen;
    private ModelSelectionManager Selection;
    private ModelViewportManager ViewportManager;

    private FLVER2 CurrentFLVER;
    private FLVER2.Material CurrentMaterial;
    private FLVER2.Texture NewObject;

    public AddTexture(ModelEditorScreen screen, FLVER2 flver)
    {
        Screen = screen;
        Selection = screen.Selection;
        ViewportManager = screen.ViewportManager;

        CurrentFLVER = flver;
        CurrentMaterial = flver.Materials[Selection._selectedMaterial];

        NewObject = new FLVER2.Texture();
    }

    public override ActionEvent Execute(bool isRedo = false)
    {
        CurrentMaterial.Textures.Add(NewObject);

        return ActionEvent.NoEvent;
    }

    public override ActionEvent Undo()
    {
        if (CurrentMaterial.Textures.Count > 1)
            Selection._subSelectedTextureRow = 0;
        else
            Selection._subSelectedTextureRow = -1;

        CurrentMaterial.Textures.Remove(NewObject);

        return ActionEvent.NoEvent;
    }
}