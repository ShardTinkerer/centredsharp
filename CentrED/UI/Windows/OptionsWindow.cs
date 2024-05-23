﻿using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using static CentrED.Application;
using Vector4 = System.Numerics.Vector4;

namespace CentrED.UI.Windows;

public class OptionsWindow : Window
{
    public override string Name => "Options";
    public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize;

    private int _lightLevel = 30;
    private Vector4 _virtualLayerFillColor = new(0.2f, 0.2f, 0.2f, 0.1f);
    private Vector4 _virtualLayerBorderColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    private Vector4 _terrainGridFlatColor = new(0.5f, 0.5f, 0.0f, 0.5f);
    private Vector4 _terrainGridAngledColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    protected override void InternalDraw()
    {
        if (ImGui.BeginTabBar("Options"))
        {
            if (ImGui.BeginTabItem("General"))
            {
                if (ImGui.SliderInt("LightLevel", ref _lightLevel, 0, 30))
                {
                    CEDGame.MapManager.MapEffect.LightLevel = (_lightLevel + 2) / 32f;
                }
                ImGui.Checkbox("Show NoDraw tiles", ref CEDGame.MapManager.ShowNoDraw);
                if (ImGui.Checkbox("Prefer Texture Map for land tiles", ref Config.Instance.PreferTexMaps))
                {
                    CEDGame.MapManager.Reset();
                }
                ImGui.Checkbox("Legacy mouse scroll behavior", ref Config.Instance.LegacyMouseScroll);
                UIManager.Tooltip("Mouse scroll up/down: elevate tile\nCtrl + Mouse scroll up/down: Zoom in/out");
                ImGui.EndTabItem();
            }
            DrawKeymapOptions();
            if (ImGui.BeginTabItem("Virtual Layer"))
            {
                if (ImGui.ColorPicker4("Virtual Layer Fill Color", ref _virtualLayerFillColor))
                {
                    CEDGame.MapManager.MapEffect.VirtualLayerFillColor = new Microsoft.Xna.Framework.Vector4
                    (
                        _virtualLayerFillColor.X,
                        _virtualLayerFillColor.Y,
                        _virtualLayerFillColor.Z,
                        _virtualLayerFillColor.W
                    );
                }
                if (ImGui.ColorPicker4("Virtual Layer Border Color", ref _virtualLayerBorderColor))
                {
                    CEDGame.MapManager.MapEffect.VirtualLayerBorderColor = new Microsoft.Xna.Framework.Vector4
                    (
                        _virtualLayerBorderColor.X,
                        _virtualLayerBorderColor.Y,
                        _virtualLayerBorderColor.Z,
                        _virtualLayerBorderColor.W
                    );
                }
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Terrain Grid"))
            {
                if (ImGui.ColorPicker4("Flat tile color", ref _terrainGridFlatColor))
                {
                    CEDGame.MapManager.MapEffect.TerrainGridFlatColor = new Microsoft.Xna.Framework.Vector4
                    (
                        _terrainGridFlatColor.X,
                        _terrainGridFlatColor.Y,
                        _terrainGridFlatColor.Z,
                        _terrainGridFlatColor.W
                    );
                }
                if (ImGui.ColorPicker4("Angled tile color", ref _terrainGridAngledColor))
                {
                    CEDGame.MapManager.MapEffect.TerrainGridAngledColor = new Microsoft.Xna.Framework.Vector4
                    (
                        _terrainGridAngledColor.X,
                        _terrainGridAngledColor.Y,
                        _terrainGridAngledColor.Z,
                        _terrainGridAngledColor.W
                    );
                }
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private string assigningActionName = "";
    private byte assignedKeyNumber = 0;

    private void DrawKeymapOptions()
    {
        if (ImGui.BeginTabItem("Keymap"))
        {
            DrawSingleKey(Keymap.MoveUp);
            DrawSingleKey(Keymap.MoveDown);
            DrawSingleKey(Keymap.MoveLeft);
            DrawSingleKey(Keymap.MoveRight);
            ImGui.Separator();
            DrawSingleKey(Keymap.ToggleAnimatedStatics);
        }
    }


    private bool _showNewKeyPopup;

    private void DrawSingleKey(string action)
    {
        var keys = Keymap.GetKeys(action);
        ImGui.Text(Keymap.PrettyName(action));
        ImGui.SameLine();
        if (assigningActionName != "")
        {
            ImGui.BeginDisabled();
        }
        var label1 = (assigningActionName == action && assignedKeyNumber == 1) ?
            "Assign new key" :
            string.Join(" + ", keys.Item1.Select(x => x.ToString()));
        if (ImGui.Button($"{label1}##{action}1"))
        {
            assigningActionName = action;
            assignedKeyNumber = 1;
            ImGui.OpenPopup("NewKey");
            _showNewKeyPopup = true;
        }
        ImGui.SameLine();
        var label2 = (assigningActionName == action && assignedKeyNumber == 2) ?
            "Assign new key" :
            string.Join(" + ", keys.Item2.Select(x => x.ToString()));
        if (ImGui.Button($"{label2}##{action}2"))
        {
            assigningActionName = action;
            assignedKeyNumber = 2;
            ImGui.OpenPopup("NewKey");
            _showNewKeyPopup = true;
        }
        if (assigningActionName != "")
        {
            ImGui.EndDisabled();
        }
        if (assigningActionName == action && ImGui.BeginPopupModal
                ("NewKey", ref _showNewKeyPopup, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar))
        {
            var pressedKeys = Keymap.GetKeysPressed();
            ImGui.Text($"Enter new key for {assigningActionName}");
            ImGui.Text(string.Join("+", pressedKeys));
            ImGui.Text("Press ESCAPE to cancel");

            
            foreach (var pressedKey in pressedKeys)
            {
                if (pressedKey == Keys.Escape)
                {
                    assigningActionName = "";
                    assignedKeyNumber = 0;
                    break;
                }
                if (pressedKey is >= Keys.A and <= Keys.Z)
                {
                    var sortedKeys = pressedKeys.Order(new Keymap.LetterLastComparer()).ToArray();
                    var oldKeys = Config.Instance.Keymap[action];
                    var newKeys = assignedKeyNumber == 1 ? (sortedKeys, oldKeys.Item2) : (oldKeys.Item1, sortedKeys);
                    Config.Instance.Keymap[action] = newKeys;
                    assigningActionName = "";
                    assignedKeyNumber = 0;
                }
            }
            if (assigningActionName == "")
            {
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }
}