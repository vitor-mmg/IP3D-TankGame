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
        BasicEffect efeitoTerreno;
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
           
            
            efeitoTerreno = new BasicEffect(GraphicsDevice);
            efeitoTerreno.Texture = terrainTexture;
            efeitoTerreno.LightingEnabled = true;
            efeitoTerreno.DirectionalLight0.Enabled = true;
            efeitoTerreno.DirectionalLight0.Direction = new Vector3(1, -1, 1);
            efeitoTerreno.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
            efeitoTerreno.SpecularPower = 1000f;
            efeitoTerreno.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
           
            efeitoTerreno.SpecularColor = new Vector3(1, 1, 1);
            efeitoTerreno.DirectionalLight1.Enabled = false;
            efeitoTerreno.DirectionalLight2.Enabled = true;

            efeitoTerreno.FogEnabled = true;
            efeitoTerreno.FogColor = Color.Gray.ToVector3(); // For best results, ake this color whatever your background is.
            efeitoTerreno.FogStart = 15;
            efeitoTerreno.FogEnd = 50.0f;     
       
            //i am an idiot faltava isto.... //Acontece xD
            efeitoTerreno.TextureEnabled = true;
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
           
            Terreno.Draw(GraphicsDevice, efeitoTerreno);

            base.Draw(gameTime);
        }
    }
}
