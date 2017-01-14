﻿//  
// Copyright (c) Jesse Freeman. All rights reserved.  
// 
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
// 

using System;
using MoonSharp.Interpreter;
using PixelVisionSDK;
using PixelVisionSDK.Utils;
using UnityEngine;

public class LuaDemoRunner : UnityRunner
{

    public enum Demos
    {

        DrawSprite,
        Font,
        Controller,
        Mouse,
        SpriteStressTest,
        Tilemap

    }

    private readonly Demos defaultDemoID = Demos.DrawSprite;

    // Lua Layer
    public LuaBridge bridge;
    public string luaScript;
    protected MouseInput mouseInput;

    // MoonSharp script
    public Script script;

    public override void LoadGame()
    {
        // Demos
        LuaGameChip gameChip = null;


        // Source code for these demos are part of the DLLs/PixelVision8Demos.dll file.
        switch (defaultDemoID)
        {
            case Demos.DrawSprite:
                luaScript = Resources.Load<TextAsset>("DrawSpriteDemo").text;
                break;
            case Demos.Font:
                luaScript = Resources.Load<TextAsset>("FontDemo").text;
                break;
            case Demos.Controller:
                luaScript = Resources.Load<TextAsset>("ControllerDemo").text;
                break;
            case Demos.Mouse:
                luaScript = Resources.Load<TextAsset>("MouseDemo").text;
                break;
            case Demos.SpriteStressTest:
                luaScript = Resources.Load<TextAsset>("SpriteStressDemo").text;
                break;
            case Demos.Tilemap:
                luaScript = Resources.Load<TextAsset>("TilemapDemo").text;
                break;
        }

        gameChip = new LuaGameChip();

        // Make sure we load the correct resources for the default demo. You'll need to set all of the Resources to Read/Write 
        // in Unity for this to work.
        if (defaultDemoID == Demos.Tilemap)
        {
            // This demo uses a custom resolution and different files
            ImportUtil.ImportColorsFromTexture(Resources.Load<Texture2D>("TileMapDemo/colors"), engine);
            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("TileMapDemo/message-font"), engine, "message-font");

            // The engine will automatically add sprites from tile maps into the SpriteChip so we only load this image.
            ImportUtil.ImportTileMapFromTexture(Resources.Load<Texture2D>("TileMapDemo/tilemap"), engine);

            // Change the resolution to match 144p.
            ResetResolution(160, 144);
        }
        else
        {
            // For all other demos, use the default file
            ImportUtil.ImportColorsFromTexture(Resources.Load<Texture2D>("colors"), engine);
            ImportUtil.ImportSpritesFromTexture(Resources.Load<Texture2D>("sprites"), engine);
            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("large-font"), engine, "large-font");
            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("small-font"), engine, "small-font");

            // Change the resolution to match 240p.
            ResetResolution(256, 240);
        }

        // Configure the input
        ConfigureInput();

        // With everything configured, it's time to load the game into memory. The LoadGame() method sets the GameChip instance 
        // as the active game and also registers it with the ChipManager.
        engine.LoadGame(gameChip);

        RunGame();

    }

    public virtual void RunGame()
    {
        bridge = new LuaBridge();
        LuaBridge.runner = this;

        UserData.RegisterType<LuaBridge>();
        script = new Script();
        script.Globals["LuaBridge"] = new Func<LuaBridge>(() => bridge);

        script.DoString(luaScript);
        OnLuaReady();

    }

    public virtual void OnLuaReady()
    {

        // After loading the game, we are ready to run it.
        engine.RunGame();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        CacheColors();

    }

    private void ConfigureInput()
    {
        var controllerChip = engine.controllerChip;

        // This allows the engine to access Unity keyboard input and the inputString
        controllerChip.RegisterKeyInput(new KeyInput());

        // Map Controller and Keyboard
        var keys1 = new[]
        {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.X, KeyCode.C,
            KeyCode.A, KeyCode.S
        };

        var keys2 = new[]
        {
            KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Quote, KeyCode.Return,
            KeyCode.Y, KeyCode.U
        };

        var total = keys1.Length;
        for (var i = 0; i < total; i++)
        {
            controllerChip.UpdateControllerKey(0, new KeyboardButtonInput((Buttons) i, (int) keys1[i]));
            controllerChip.UpdateControllerKey(1, new KeyboardButtonInput((Buttons) i, (int) keys2[i]));
        }

        // Register mouse input
        controllerChip.RegisterMouseInput(new MouseInput(displayTarget.rectTransform));
    }

}