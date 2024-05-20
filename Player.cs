using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace PlatformShooter
{

    public class Player
    {
        private Vector2 position = new Vector2(500, 300);
        public int velocity = 200;
        private Dir direction = Dir.Right;
        private bool isMoving = false;
        private KeyboardState kStateOld = Keyboard.GetState();
        public bool dead = false;
        public Rectangle rect;
        public Rectangle srect;
        

        public SpriteAnimation anim;

        public SpriteAnimation[] animations = new SpriteAnimation[6];

        

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }
        public void setX(float newX)
        {
            position.X = newX;
        }
        public void setY(float newY)
        {
            position.Y = newY;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            isMoving = false;

            if (kState.IsKeyDown(Keys.Right))
            {
                direction = Dir.Right;
                isMoving = true;
               
            }
            if (kState.IsKeyDown(Keys.Left))
            {
                direction = Dir.Left;
                isMoving = true;
            }
           

            //if (kState.IsKeyDown(Keys.Space))
                //isMoving = false;

            if (dead)
                isMoving = false;

            if (isMoving)
            {
                switch (direction)
                {
                    case Dir.Right:
                        //if (position.X < 1275)
                            position.X += velocity * dt;
                        break;
                    case Dir.Left:
                        //if (position.X > 225)
                            position.X -= velocity * dt;
                        break;
                    
                }
            }

            anim = animations[(int)direction];

            anim.Position = new Vector2(position.X - 48, position.Y - 48);

            if (kState.IsKeyDown(Keys.Space))
                anim.setFrame(0);

            else if (isMoving)
            {
                anim.Update(gameTime);
            }
            else
            {
                anim.setFrame(1);
            }

            if (kState.IsKeyDown(Keys.Space) && kStateOld.IsKeyUp(Keys.Space))
            {
                //Projectile.projectiles.Add(new Projectile(Position, direction));
                //MySounds.projectileSound.Play(1f, 0.5f, 0f);
            }

            kStateOld = kState;
        }
    }
}
