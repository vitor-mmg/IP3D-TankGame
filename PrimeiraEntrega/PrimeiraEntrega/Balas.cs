using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    public class Balas
    {
        private Model bala;
        public Vector3 posicao;
        private Matrix inclinationMatrix;
        private Vector3 vetorBase;
        private float speed;
        private Vector3 direcao;
        private Matrix rotationMatrix;
        private float totalTimePassed;
        public bool alive;
        public Tanque tanqueQueDisparou;
        private Matrix[] transformacoes;
        private BoundingSphere BoundingS;
        
        
        public Balas(ContentManager content)
        {
            speed = 0.3f;
            alive = false;

            vetorBase = new Vector3(0, 0, 1);

            LoadContent(content);
        }
        public BoundingSphere BoundingSphere
        {
            get { return BoundingS; }
            set { BoundingS = value; }
        }
        

        private void LoadContent(ContentManager content)
        {
            bala = content.Load<Model>("Sphere");

        }

        public void Disparo(Tanque tanqueQueDisparou, float desvioAleatorio)
        {
            this.alive = true;
            this.totalTimePassed = 0;
            this.tanqueQueDisparou = tanqueQueDisparou;
            this.inclinationMatrix = tanqueQueDisparou.inclinationMatrix;
            rotationMatrix = Matrix.CreateRotationX(tanqueQueDisparou.CannonRotation)
                   * Matrix.CreateRotationY(tanqueQueDisparou.TurretRotation + desvioAleatorio)
                   * Matrix.CreateFromQuaternion(tanqueQueDisparou.inclinationMatrix.Rotation)
                   ;

            direcao = Vector3.Transform(vetorBase, rotationMatrix);

            Vector3 offset = Vector3.Transform(new Vector3(0, 0.4f, 0), rotationMatrix);
            offset = direcao + offset;

            posicao = tanqueQueDisparou.posicao + offset;
            
            BoundingS.Center = posicao;
            BoundingS.Radius = 0.1f;

        }

        public void KillBala()
        {
            this.alive = false;
        }
       
        public void Update(GameTime gameTime)
        {
            totalTimePassed += (float)gameTime.ElapsedGameTime.Milliseconds / 4096.0f;
            posicao += direcao * speed;
            posicao.Y -= totalTimePassed * totalTimePassed * speed * 2; //Gravidade
            BoundingS.Center = posicao;

        }
        public void Draw()
        {
            // Copy any parent transforms.
            transformacoes = new Matrix[bala.Bones.Count];
            bala.CopyAbsoluteBoneTransformsTo(transformacoes);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in bala.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateScale(0.05f)
                        * Matrix.CreateTranslation(this.posicao);
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;



                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

        }
    }
}
