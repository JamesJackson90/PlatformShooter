using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;



namespace PlatformShooter
{
    enum Dir
    {
        Left,
        Right,
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        private Dictionary<Vector2, int> fg;

        private Dictionary<Vector2, int> collisions;

        private Texture2D textureAtlas;

        private Texture2D collision;


        Texture2D playerSprite;
        Texture2D walkRight;
        Texture2D walkLeft;

        Player player = new Player();
        private List<Rectangle> intersections;

        private int TILESIZE = 32;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            fg = LoadMap("../../../Data/level1_Tile Layer 1.csv");
            collisions = LoadMap("../../../Data/MapOutput_Collisions.csv");
            intersections = new();
        }

        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > -1) {
                            result[new Vector2(x, y)] = value;}
                    }
                }
                y++;
            }
            return result;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1480;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            textureAtlas = Content.Load<Texture2D>("Tileset");
            collision = Content.Load<Texture2D>("collision");

            walkRight = Content.Load<Texture2D>("Player/Biker_run");
            walkLeft = Content.Load<Texture2D>("Player/Biker_runLeft");

            player.animations[0] = new SpriteAnimation(walkLeft, 6, 9);
            player.animations[1] = new SpriteAnimation(walkRight, 6, 9);
                                     //(direction, how many frames in the sprite, fps)
            player.anim = new SpriteAnimation(walkRight, 6, 1);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            player.Update(gameTime);
            if (!player.dead)

            intersections = getIntersectingTilesHorizontal(player.rect);
            intersections = getIntersectingTilesVertical(player.rect);

            foreach (var rect in intersections)
            {
                if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {
                    Rectangle collision = new Rectangle(
                        rect.X * TILESIZE,
                        rect.Y * TILESIZE,
                        TILESIZE,
                        TILESIZE);

                    if (player.velocity > 0.0f)
                    {
                        player.rect.X = collision.Left - player.rect.Width;
                    }
                    else if (player.velocity > 0.0f)
                    {
                        player.rect.X = collision.Right;
                    }
                }

                // TODO: Add your update logic here

                base.Update(gameTime);
            }
        }

        public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target){
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightInTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthInTiles; x++) {
                for (int y = 0; y <= heightInTiles; y++){
                    intersections.Add(new Rectangle(
                        (target.X + x*TILESIZE) / TILESIZE,
                        target.Y + y*(TILESIZE-1) / TILESIZE, TILESIZE, TILESIZE));
                }
            }
            return intersections;
        }


        public List<Rectangle> getIntersectingTilesVertical(Rectangle target)
        {
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % TILESIZE)) / TILESIZE;
            int heightInTiles = (target.Height - (target.Height % TILESIZE)) / TILESIZE;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x*(TILESIZE -1)) / TILESIZE,
                        target.Y + y*(TILESIZE) / TILESIZE, TILESIZE, TILESIZE));
                }
            }
            return intersections;
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            _spriteBatch.Begin();

            int display_tilesize = 32;
            //relates to pixelsize of sprites
            int num_tiles_per_row = 12;
            int pixel_tilesize = 32;

            foreach (var item in fg)
            {
                Rectangle drect = new(
                    (int)item.Key.X * display_tilesize,
                    (int)item.Key.Y * display_tilesize,
                    display_tilesize,
                    display_tilesize
                    );
                int x = item.Value % num_tiles_per_row;
                int y = item.Value / num_tiles_per_row;

                Rectangle src = new(x * pixel_tilesize, y * pixel_tilesize, pixel_tilesize, pixel_tilesize);

                _spriteBatch.Draw(textureAtlas, drect, src, Color.White);
            }

            foreach (var item in collisions)
            {
                Rectangle drect = new(
                    (int)item.Key.X * display_tilesize,
                    (int)item.Key.Y * display_tilesize,
                    display_tilesize,
                    display_tilesize
                    );
                int x = item.Value % num_tiles_per_row;
                int y = item.Value / num_tiles_per_row;

                Rectangle src = new(x * pixel_tilesize, y * pixel_tilesize, pixel_tilesize, pixel_tilesize);

                _spriteBatch.Draw(collision, drect, src, Color.White);
            }

            foreach (var rect in intersections)
            {
                DrawRectHollow(_spriteBatch,
                    new Rectangle(rect.X * TILESIZE, rect.Y * TILESIZE, TILESIZE, TILESIZE), 4);
            }

            if (!player.dead)
                player.anim.Draw(_spriteBatch);

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rectangle, int v)
        {
            //throw new NotImplementedException();
        }

    }
}
