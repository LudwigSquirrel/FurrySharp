using System;
using System.Globalization;
using System.IO;
using System.Threading;
using FurrySharp.Drawing;
using FurrySharp.Input;
using FurrySharp.Registry;
using FurrySharp.Resources;
using FurrySharp.States;
using FurrySharp.UI;
using FurrySharp.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static FurrySharp.Registry.GameConstants;
using TextWriter = FurrySharp.UI.Font.Writing.TextWriter;

namespace FurrySharp;

public class FurryGame : Game, IStateSetter
{
    private readonly GraphicsDeviceManager graphics;

    public State CurrentState { get; private set; }

    public UILabel FpsLabel { get; private set; }
    public Terminal Terminal { get; private set; }

    public FurryGame()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        CurrentState = null;

        IsMouseVisible = true;

        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        if (!Directory.Exists(SavePath + "Saves/"))
        {
            Directory.CreateDirectory(SavePath + "Saves/");
        }
    }

    protected override void Initialize()
    {
        InitGraphics();
        SpriteDrawer.Initialize(graphics.GraphicsDevice);

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
        ResourceManager.LoadResources(Content);
    }

    protected override void UnloadContent()
    {
        ResourceManager.UnloadResources();
    }

    protected override void Update(GameTime gameTime)
    {
        GameTimes.TimeScale = 1;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        GameTimes.UpdateTimes(gameTime);
        KeyInput.UpdateInputs();

        if (KeyInput.JustPressedKey(Keys.F12))
        {
            GlobalState.ShowFPS = !GlobalState.ShowFPS;
        }

        if (KeyInput.JustPressedKey(Keys.OemTilde))
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

        if (KeyInput.JustPressedKey(Keys.Enter) && KeyInput.IsKeyPressed(Keys.LeftAlt))
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

        // TODO: Add your update logic here

        if (GlobalState.ResolutionDirty)
        {
            InitGraphics();
            GlobalState.ResolutionDirty = false;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        SpriteDrawer.BeginDraw();
        CurrentState.DrawState();
        if (GlobalState.ShowFPS)
        {
            FpsLabel.Text = $"FPS:{GameTimes.FPS}";
            FpsLabel.Draw();
        }

        if (GlobalState.ShowTerminal)
        {
            Terminal.Draw();
        }

        SpriteDrawer.EndDraw();
    }

    public void CreateAndSetState<T>() where T : State, new()
    {
        // foreach (var effect in GlobalState.AllEffects)
        // {
        //     effect.Deactivate();
        // }

        CurrentState = new T();

        CurrentState.Create();
    }

    private void InitGraphics()
    {
        graphics.GraphicsProfile = GraphicsProfile.HiDef;
        graphics.IsFullScreen = false;

        graphics.PreferredBackBufferWidth = GAME_WIDTH_IN_PIXELS;
        graphics.PreferredBackBufferHeight = GAME_HEIGHT_IN_PIXELS;
        switch (GlobalState.Settings.Resolution)
        {
            case Resolution.Windowed:
                graphics.IsFullScreen = false;
                break;
            case Resolution.Fullscreen:
                // todo: fullscreen is blurry. How do we fix this?
                graphics.IsFullScreen = true;
                break;
        }

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
    }
}