﻿using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Threading;
using FurrySharp.Audio;
using FurrySharp.DeveloperTools;
using FurrySharp.Drawing;
using FurrySharp.Input;
using FurrySharp.Registry;
using FurrySharp.Resources;
using FurrySharp.States;
using FurrySharp.UI;
using FurrySharp.UI.Font;
using FurrySharp.Utilities;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp;

public class FurryGame : Game, IStateSetter
{
    private readonly GraphicsDeviceManager graphics;

    public State CurrentState { get; private set; }

    public UILabel FpsLabel { get; private set; }
    public Terminal Terminal { get; private set; }

    public static ImGuiRenderer DeerImGooeyRenderer;


#if DEBUG
    public GameLoopTestBundle Test;
#endif

    public FurryGame()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        CurrentState = null;

        IsMouseVisible = false;

        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        if (!Directory.Exists(SavePath + "Saves/"))
        {
            Directory.CreateDirectory(SavePath + "Saves/");
        }

        if (!Directory.Exists(SavePath + "OttoSaves/"))
        {
            Directory.CreateDirectory(SavePath + "OttoSaves/");
        }

        ResourceInstanceAutoSaver.AutoSaveDir = SavePath + "OttoSaves/";
    }

    protected override void Initialize()
    {
        InitGraphics();
        SpriteDrawer.Initialize(graphics.GraphicsDevice);
        DeerImGooeyRenderer = new ImGuiRenderer(this);
        ImGui.GetIO().MouseDrawCursor = true;

        //GlobalState.ResetValues();

        //EntityManager.Initialize();

        base.Initialize();

        FpsLabel = new UILabel(new Point(0, 0), 8, 1, FontManager.InitFont(true));
        Terminal = new Terminal();

        Window.Title = "Furry Game";
        CreateAndSetState<SandboxState>();
    }

    protected override void LoadContent()
    {
        ResourceManager.LoadContentManagerResources(Content);
        ResourceManager.LoadOtherResources();
        DeerImGooeyRenderer.RebuildFontAtlas();
    }

    protected override void UnloadContent()
    {
        ResourceManager.UnloadResources();
        AudioManager.StopSong();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        GameTimes.UpdateTimes(gameTime);
        GameInput.UpdateInputs();

        if (GameInput.JustPressedKey(Keys.F12))
        {
            GlobalState.ShowFPS = !GlobalState.ShowFPS;
        }

        if (GameInput.JustPressedKey(Keys.F3))
        {
            GlobalState.ShowDevTools = !GlobalState.ShowDevTools;
        }

        if (GameInput.JustPressedKey(Keys.OemTilde))
        {
            GlobalState.ShowTerminal = !GlobalState.ShowTerminal;
            if (GlobalState.ShowTerminal)
            {
                Window.TextInput += Terminal.TextInputHandler;
            }
            else
            {
                Window.TextInput -= Terminal.TextInputHandler;
            }
        }

        if (GameInput.JustPressedKey(Keys.Enter) && GameInput.IsKeyPressed(Keys.LeftAlt))
        {
            if (GlobalState.GetResolution() == Resolution.Windowed)
            {
                GlobalState.SetResolution(Resolution.Fullscreen);
            }
            else
            {
                GlobalState.SetResolution(Resolution.Windowed);
            }
        }

        CurrentState.UpdateState();

        if (GlobalState.ResolutionDirty)
        {
            InitGraphics();
            GlobalState.ResolutionDirty = false;
        }

        GameTimes.UpdateFPS(gameTime);
        base.Update(gameTime);

#if DEBUG
        if (Test != null)
        {
            Test.DoTest();
            if (Test.Done)
            {
                Exit();
            }
        }
#endif
    }

    protected override void Draw(GameTime gameTime)
    {
        SpriteDrawer.BeginDraw();
        CurrentState.DrawState();
        SpriteDrawer.EndDraw();

        SpriteDrawer.BeginGUIDraw();
        CurrentState.DrawUI();
        if (GlobalState.ShowFPS)
        {
            FpsLabel.Text = $"FPS:{GameTimes.FPS}";
            FpsLabel.Draw();
        }

        if (GlobalState.ShowTerminal)
        {
            Terminal.Draw();
        }

        SpriteDrawer.EndGUIDraw();
        SpriteDrawer.Render();

        if (GlobalState.ShowDevTools)
        {
            DeerImGooeyRenderer.BeginLayout(gameTime);
            DevTools.DoIMGUI();
            if (CurrentState.ShowIMGUI)
            {
                CurrentState.DoIMGUI();
            }

            DeerImGooeyRenderer.EndLayout();
        }
    }

    public void CreateAndSetState<T>() where T : State, new()
    {
        CurrentState = new T();
        CurrentState.Create();
    }

    public void CreateAndSetState(Type type)
    {
        CurrentState = (State)Activator.CreateInstance(type);
        CurrentState!.Create();
    }

    private void InitGraphics()
    {
        graphics.GraphicsProfile = GraphicsProfile.HiDef;
        graphics.IsFullScreen = false;

        int displayWidth;
        int displayHeight;
        int scale;
        switch (GlobalState.Settings.Resolution)
        {
            case Resolution.Windowed:
                graphics.IsFullScreen = false;
                displayWidth = GAME_WIDTH_IN_PIXELS * GlobalState.Settings.PreferredWindowScale;
                displayHeight = GAME_HEIGHT_IN_PIXELS * GlobalState.Settings.PreferredWindowScale;
                scale = GlobalState.Settings.PreferredWindowScale;
                break;
            case Resolution.Fullscreen:
                graphics.IsFullScreen = true;
                displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                scale = MaximizeScale(displayWidth, displayHeight);
                break;
            default:
                throw new Exception("This is just here to make the compiler happy. It should never™ be thrown.");
        }

        graphics.PreferredBackBufferWidth = displayWidth;
        graphics.PreferredBackBufferHeight = displayHeight;
        SpriteDrawer.UpdateRenderDestination(displayWidth, displayHeight, scale);
        graphics.SynchronizeWithVerticalRetrace = true;
        // switch (GlobalState.settings.fps)
        // {
        //     case FPS.Fixed:
        //         IsFixedTimeStep = true;
        //         graphics.SynchronizeWithVerticalRetrace = true;
        //         break;
        //     case FPS.VSync:
        //         IsFixedTimeStep = false;
        //         graphics.SynchronizeWithVerticalRetrace = true;
        //         break;
        //     case FPS.Unlocked:
        //         IsFixedTimeStep = false;
        //         graphics.SynchronizeWithVerticalRetrace = false;
        //         break;
        // }

        graphics.ApplyChanges();

        int MaximizeScale(int width, int height)
        {
            var scaleX = width / GAME_WIDTH_IN_PIXELS;
            var scaleY = height / GAME_HEIGHT_IN_PIXELS;

            return scale = Math.Min(scaleX, scaleY);
        }
    }
}