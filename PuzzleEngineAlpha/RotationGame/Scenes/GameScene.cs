﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PuzzleEngineAlpha;
using PuzzleEngineAlpha.Input;
using PuzzleEngineAlpha.Level;
using PuzzleEngineAlpha.Actors;
using PuzzleEngineAlpha.Camera.Scripts;
using PuzzleEngineAlpha.Camera.Handlers;
using PuzzleEngineAlpha.Camera.Managers;
using PuzzleEngineAlpha.Resolution;
using PuzzleEngineAlpha.Camera;

namespace RotationGame.Scene
{

    public class GameScene : PuzzleEngineAlpha.Scene.IScene
    {
        #region Declarations

        RenderTarget2D renderTarget;
        CameraManager cameraManager;
        bool isActive;
        Actors.Player player;
        TileMap tileMap;
        GraphicsDevice graphicsDevice;
        PuzzleEngineAlpha.Camera.Camera camera;
        Vector2 sceneryOffSet;

        #endregion

        #region Constructor

        public GameScene(GraphicsDevice graphicsDevice, ContentManager content,TileMap tileMap, Vector2 sceneryOffSet)
        {
            this.graphicsDevice = graphicsDevice;
            this.sceneryOffSet = sceneryOffSet;
            cameraManager = new CameraManager();
            camera = new Camera( Vector2.Zero,new Vector2(this.Width, this.Height), new Vector2(50000, 50000));
            this.tileMap = tileMap;
            this.tileMap.Camera = camera;

            player = new Actors.Player(tileMap, camera, new Vector2(-100,-100), content.Load<Texture2D>(@"Textures/player"), 350.0f, 16, 16, 16, 16);
            cameraManager.SetCameraScript(new ChasingCamera(player.location, camera,6.0f));
            cameraManager.AddCameraHandler(new Rotater(0.0f, MathHelper.PiOver2, 8));
            cameraManager.AddCameraHandler(new Zoomer(1.0f, 1.0f, 0.5f, 0.01f));

            UpdateRenderTarget();
            this.sceneryOffSet = sceneryOffSet;
            PuzzleEngineAlpha.Resolution.ResolutionHandler.Changed += ResetSizes;
            this.tileMap.NewMap += NewMapHandling;

        }

        #endregion

        #region Handle New Map

        void NewMapHandling(object sender, EventArgs e)
        {
            List<Vector2> playerLocations = tileMap.GetLocationOfCodeValue("player");

            if (playerLocations.Count == 0)
                this.player.location = new Vector2(16, 16);
            else
                this.player.location = playerLocations[0];

            this.player.InitialLocation = this.player.location;
        }

        #endregion

        #region Handle Resolution Change

        void ResetSizes(object sender, EventArgs e)
        {
            UpdateRenderTarget();
            camera.ViewPortWidth = (int)this.Width;
            camera.ViewPortHeight = (int)this.Height;
        }

        #endregion    

        public void GoInactive()
        {
            this.IsActive = false;
        }

        #region Properties

        Vector2 SceneLocation
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        #region Boundaries

        int Width
        {
            get
            {
                return ResolutionHandler.WindowWidth - (int)sceneryOffSet.X;
            }
        }

        int Height
        {
            get
            {
                return ResolutionHandler.WindowHeight - (int)sceneryOffSet.Y;
            }
        }

        #endregion
        
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        #endregion

        #region Camera Related

        #region Properties

        public Vector2 CameraLocation
        {
            get
            {
                return cameraManager.Camera.Position;
            }
        }

        #endregion

        #region Helper Methods

        public void UpdateRenderTarget()
        {
            renderTarget = new RenderTarget2D(graphicsDevice, this.Width, this.Height);
            cameraManager.Camera.ViewPortWidth = this.Width;
            cameraManager.Camera.ViewPortHeight = this.Height;
        }

        #endregion

        #endregion

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);

            cameraManager.TargetLocation = player.Center;
            cameraManager.Update(gameTime);
        }

        //TODO: fix rendertarget order
        public void Draw(SpriteBatch spriteBatch)
        {

            graphicsDevice.Clear(new Color(20, 20, 20)) ;
      //      graphicsDevice.SetRenderTarget(renderTarget);

            spriteBatch.Begin(SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointWrap,
                        null,
                        null,
                        null,
                        cameraManager.Camera.GetTransformation());

            tileMap.Draw(spriteBatch);
            player.Draw(spriteBatch);

            spriteBatch.End();

           /* graphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(renderTarget, SceneLocation, Color.White);

            spriteBatch.End();*/
        }
    }
}
