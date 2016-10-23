using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PrimeiraEntrega
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect efeitoTerrain;
        Texture2D heightmap, terrainTexture;
        bool desenharTerreno;
        Random random;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

        
            graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            random = new Random();
            desenharTerreno = true;
            this.Window.Title = "1ºEntrega";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            heightmap = Content.Load<Texture2D>("terreno");
            terrainTexture = Content.Load<Texture2D>("textura");
            //terrainTexture = Content.Load<Texture2D>("tommy"); // If you want dat spicy texture for the map!! Até se nota os contornos melhor lol...
            efeitoTerrain = new BasicEffect(GraphicsDevice);
            efeitoTerrain.Texture = terrainTexture;
            //i am an idiot faltava isto.... //Acontece xD
            efeitoTerrain.TextureEnabled = true;
            Terreno.GenerateTerrain(GraphicsDevice, heightmap);

            VertexPositionNormalTexture[] vertices = Terreno.getVertexes();
            Camera.Initialize(GraphicsDevice, vertices, heightmap.Width);

            spriteBatch = new SpriteBatch(GraphicsDevice);
       
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Camera.Update(gameTime, GraphicsDevice);
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
           
            Terreno.Draw(GraphicsDevice, efeitoTerrain);

            base.Draw(gameTime);
        }
    }
}
