﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DynamicCamera.Scene
{
    interface IScene
    {
        bool IsActive
        {
            get;
        }

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
