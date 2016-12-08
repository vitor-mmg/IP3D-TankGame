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
        Model bulletModel;
        public Matrix world,view,projection;
        public Vector3 position,direcao;
        float velocidade;
        float time;
        Vector3 vetorBase;
        Tanque playerTank;
        public BoundingSphere boundingSphere;
        public bool balaDestruida;
        public Balas(Tanque tank,ContentManager content)
        {
            playerTank = tank;
            vetorBase = new Vector3(0, 0, 1);
            balaDestruida = false;
           
            world = Matrix.CreateScale(0.3f) * Matrix.CreateTranslation(position);
            LoadContent(content);
            //poicao e direcao da bala. cria se um offset para a bala começar na ponta do canhao, aplica-se todas as rotaçoes existentes no canhaoe calcula a direcao
            //transformando o vetor direcao ( cross da normal do tanque com o vetor rigth da turret) com a rotacao.
            Vector3 offset = new Vector3(0, 2, 3);
            Matrix rotacao = Matrix.CreateRotationX(tank.CannonRotation) * Matrix.CreateRotationY(tank.TurretRotation) * Matrix.CreateFromQuaternion(tank.rotacaoFinal.Rotation);
            offset = Vector3.Transform(offset, rotacao);
            direcao = Vector3.Transform(Vector3.Cross(tank.newRigth, tank.newNormal), rotacao);
            position = tank.position + offset;
            boundingSphere = new BoundingSphere();
            boundingSphere.Radius = 0.3f;
        }

        public void LoadContent(ContentManager content)
        {
            velocidade = 0.05f;
            bulletModel = content.Load<Model>("Sphere");


        }

   

        public void Update(GameTime gameTime,Tanque tank)
        {
            boundingSphere.Center = this.position;
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 4096f;

            position += (Vector3.Normalize(direcao) * velocidade);
            position.Y -= 0.098f * (time * time);
            world = Matrix.CreateScale(0.3f) * Matrix.CreateTranslation(position);

            
        }

        public void Draw(Matrix cameraView, Matrix cameraProjection)
        {
            bulletModel.Root.Transform = world;
          
            view = cameraView;
            projection = cameraProjection;
            
            // Draw the model.
            foreach (ModelMesh mesh in bulletModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = bulletModel.Root.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                    
                    effect.EnableDefaultLighting();
                    
                }
                mesh.Draw();
            }
        }
    }
}
