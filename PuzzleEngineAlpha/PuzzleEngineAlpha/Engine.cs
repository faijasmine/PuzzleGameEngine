﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace PuzzleEngineAlpha
{
    using Scene;
    using Input;
    using Resolution;
    using Diagnostics;
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SceneDirector sceneDirector;
        ResolutionHandler resolutionHandler;
        FpsMonitor fpsMonitor;

        public Engine()
            : base()
        {
           this.graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,
                PreferredBackBufferWidth = 600,
                PreferredBackBufferHeight = 600
            };

            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(OnWindowClientSizeChanged);

            this.graphics.PreferMultiSampling = true;
            //this.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            this.graphics.ApplyChanges();

            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            resolutionHandler = new ResolutionHandler(ref this.graphics, false);
            InputHandler.Initialize();
            fpsMonitor = new FpsMonitor();
            WindowText.Initialize(Content.Load<SpriteFont>("Fonts/font"));
            WindowText.AddText(new Vector2(10, 100), " ");
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sceneDirector = new SceneDirector(GraphicsDevice, Content);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputHandler.Update(gameTime);
            resolutionHandler.Update(gameTime);
            sceneDirector.Update(gameTime);

            fpsMonitor.Update(gameTime);

            WindowText.SetText(new Vector2(10, 100), "FPS: " + fpsMonitor.FPS);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sceneDirector.Draw(spriteBatch);

            base.Draw(gameTime);

            fpsMonitor.AddFrame();
        }

        private void OnWindowClientSizeChanged(object sender, System.EventArgs e)
        {
            this.resolutionHandler.SetResolution(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);
            sceneDirector.UpdateRenderTargets();
        }
    }
}
