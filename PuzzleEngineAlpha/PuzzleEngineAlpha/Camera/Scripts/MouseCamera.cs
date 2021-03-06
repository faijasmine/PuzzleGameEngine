using System;
using Microsoft.Xna.Framework;

namespace PuzzleEngineAlpha.Camera.Scripts
{
    using Input;

    public class MouseCamera : ICameraScript
    {

        #region Declarations

        private Vector2 initialPos;
        private Vector2 worldLocation;
        private Vector2 velocity;
        private Vector2 deSquareerating;
        private bool scrolling;
        private Camera camera;

        #endregion

        #region Constructor

        public MouseCamera(Camera camera)
        {
            this.worldLocation = new Vector2(camera.ViewPortWidth / 2, camera.ViewPortHeight / 2);
            deSquareerating = initialPos = velocity = Vector2.Zero;
            scrolling = false;
            this.camera = camera;
        }

        #endregion

        #region Properties

        
        Rectangle GeneralArea
        {
            get
            {
                return camera.ScreenRectangle;
            }
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
        }

        public Vector2 TargetLocation
        {
            get;
            set;
        }


        public Vector2 WorldLocation
        {
            get
            {
                return worldLocation;
            }
            set
            {
                 worldLocation.X = MathHelper.Clamp(value.X, 0 ,
                                             Camera.WorldSize.X);
                 worldLocation.Y = MathHelper.Clamp(value.Y,0,
                                             Camera.WorldSize.Y);
            }
        }

        #endregion

        #region Scrolling

        private void ReduceVector(ref Vector2 vector,float maxAcceleration)
        {
            float reduceAmount = 15.0f;
            if (vector.X > 0)
                vector.X = MathHelper.Clamp(vector.X - reduceAmount, 0, maxAcceleration);
            else
                vector.X = MathHelper.Clamp(vector.X + reduceAmount, -maxAcceleration, 0);

            if (vector.Y > 0)
                vector.Y = MathHelper.Clamp(vector.Y - reduceAmount, 0, maxAcceleration);
            else
                vector.Y = MathHelper.Clamp(vector.Y + reduceAmount, -maxAcceleration, 0);
        }

        public void AdjustLocation()
        {
            camera.Position = new Vector2(MathHelper.Clamp(Camera.Position.X, -Resolution.ResolutionHandler.WindowWidth / 2, Camera.WorldSize.X - Resolution.ResolutionHandler.WindowWidth / 2),
                                            MathHelper.Clamp(Camera.Position.Y, -Resolution.ResolutionHandler.WindowHeight / 2, Camera.WorldSize.Y - Resolution.ResolutionHandler.WindowHeight / 2));
        }

        public void RepositionCamera()
        {
            Camera.Move(velocity);
            AdjustLocation();
           /* int screenLocX = (int)Camera.WorldToScreen(worldLocation).X;
            int screenLocY = (int)Camera.WorldToScreen(worldLocation).Y;
            if (screenLocY > Camera.ViewPortHeight / 2)
            {
                Camera.Move(new Vector2(0, screenLocY - Camera.ViewPortHeight / 2));
            }
            if (screenLocY < Camera.ViewPortHeight / 2)
            {
                Camera.Move(new Vector2(0, screenLocY - Camera.ViewPortHeight / 2));
            }

            if (screenLocX > Camera.ViewPortWidth / 2)
            {
                Camera.Move(new Vector2(screenLocX - Camera.ViewPortWidth / 2, 0));
            }

            if (screenLocX < Camera.ViewPortWidth / 2)
            {
                Camera.Move(new Vector2(screenLocX - Camera.ViewPortWidth / 2, 0));
            }*/
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (velocity != Vector2.Zero)
            {
                WorldLocation += velocity;
                RepositionCamera();
            }

            bool mouseInBounds = InputHandler.MouseRectangle.Intersects(GeneralArea);
            if (!mouseInBounds || !InputHandler.RightButtonIsClicked())
            {
                if (scrolling)
                    deSquareerating = velocity * 50;
                velocity = (float)gameTime.ElapsedGameTime.TotalSeconds * deSquareerating;
                if (!mouseInBounds)
                    ReduceVector(ref deSquareerating, 100f);
                else
                    ReduceVector(ref deSquareerating, 2000f);
                scrolling = false;
                return;
            }

            if (InputHandler.RightButtonIsClicked() && !scrolling)
            {
                Vector2 CurrentMousePosition = Vector2.Transform(InputHandler.MousePosition, Matrix.Invert(camera.GetTransformation()));
                scrolling = true;
                initialPos = CurrentMousePosition;
            }
            else if (InputHandler.RightButtonIsClicked() && scrolling)
            {
                Vector2 CurrentMousePosition = Vector2.Transform(InputHandler.MousePosition, Matrix.Invert(camera.GetTransformation()));
                velocity = initialPos - CurrentMousePosition;
                initialPos = CurrentMousePosition;
            }

        }

        #endregion
    }
}