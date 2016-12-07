using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PrimeiraEntrega
{
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect efeitoTerreno,efeitoBasico;
        Texture2D heightmap, terrainTexture;
        bool desenharTerreno;
        bool desenhatank;
        Random random;
        Tanque tankPlayer1;
        Tank2 tankplayer2;
        List<Tanque> listaTanque;
        List<Tank2> listaTanque2;
        Tanque tank;
        Tank2 tank2;
        SistemaP particulasChuva;
        int tamanhoPlano;

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
            //tank
            listaTanque = new List<Tanque>();
            listaTanque2 = new List<Tank2>();
            desenhatank = true;
            this.Window.Title = "1ºEntrega";
            
            
            //Define o tamanho do plano, sendo que o raio da nuvem depende deste tamanho
            tamanhoPlano = 500;

            //Inicializar o sistema de partículas da chuva
            particulasChuva = new SistemaP(random, tamanhoPlano , 15000, 20);

            //GeneradorBalas.Initialize(Content);

            base.Initialize();

        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            heightmap = Content.Load<Texture2D>("terreno");
            terrainTexture = Content.Load<Texture2D>("textura");
            
            //tank
            tankPlayer1 = new Tanque( GraphicsDevice, new Vector3(random.Next(10, 50), 5, random.Next(Terreno.altura + 10 , Terreno.altura+10)));
            tankPlayer1.LoadContent(Content);
            tankPlayer1.ativarTanque(listaTanque);
            listaTanque.Add(tankPlayer1);

            tankplayer2 = new Tank2( GraphicsDevice, new Vector3(random.Next(10, 50), 5, random.Next(Terreno.altura + 5, Terreno.altura + 10)));
            tankplayer2.LoadContent(Content);
            tankplayer2.ativarTanque2(listaTanque2);
            listaTanque2.Add(tankplayer2);

            efeitoTerreno = new BasicEffect(GraphicsDevice);
            efeitoTerreno.Texture = terrainTexture;
            //luzes

            efeitoTerreno.LightingEnabled = true;
            efeitoTerreno.DirectionalLight0.Enabled = true;
            efeitoTerreno.DirectionalLight0.Direction = new Vector3(1, -1, 1);
            efeitoTerreno.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
            efeitoTerreno.SpecularPower = 1000f;
            efeitoTerreno.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f);
     

            efeitoTerreno.FogEnabled = true;
            efeitoTerreno.FogColor = Color.CornflowerBlue.ToVector3(); // For best results, ake this color whatever your background is.
            efeitoTerreno.FogStart = 15;
            efeitoTerreno.FogEnd = 50.0f;     
       
            efeitoTerreno.TextureEnabled = true;
            Terreno.GenerateTerrain(GraphicsDevice, heightmap);

            VertexPositionNormalTexture[] vertices = Terreno.getVertices();
            Camera.Initialize(GraphicsDevice, vertices, heightmap.Width);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            efeitoBasico = new BasicEffect(GraphicsDevice);
            efeitoBasico.VertexColorEnabled = true;

       
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //Destruir a textura do plano
        }
    
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Camera.Update(gameTime, GraphicsDevice);
            //update tank
            foreach (Tanque tank in listaTanque)
            {
                tank.Update(gameTime, listaTanque, Content);
                //tank.Update2(gameTime, listaTanque, Content, random);
            }
            foreach(Tank2 tank2 in listaTanque2)
            {
                tank2.Update(gameTime, listaTanque2, Content);
            }


            //Atualizar sistemas de partículas
            particulasChuva.Update(random);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (desenhatank)
            {
                //Desenhar os tanques visiveis do ponto de vista da camara
                foreach (Tanque tank in listaTanque)
                {

                    tank.Draw(GraphicsDevice, efeitoTerreno);
                    

                }
            }
            if (desenhatank)
            {
                //Desenhar os tanques visiveis do ponto de vista da camara
                foreach (Tank2 tank2 in listaTanque2)
                {

                    tank2.Draw(GraphicsDevice, efeitoTerreno);


                }
            }
            //Desenhar os sistemas de partículas
            particulasChuva.Draw(GraphicsDevice, efeitoBasico);

            Terreno.Draw(GraphicsDevice, efeitoTerreno);
            /*
            foreach (Bala bala in GeneradoBalas.getListaBalas())
            {
                if (Camera.frustum.Contains(bala.BoundingSphere) != ContainmentType.Disjoint)
                {
                    bala.Draw();
                }

            }*/
            base.Draw(gameTime);
        }
    }
}
