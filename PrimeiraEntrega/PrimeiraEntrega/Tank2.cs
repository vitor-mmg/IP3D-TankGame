using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    public class Tank2
    {

        Model tankModel;
        public Matrix world;
        private GraphicsDevice device;
        public bool alive;

        Vector3 vetorBase;
        public Vector3 direcao, direcaoAnterior, target, posicao, positionAnterior;
        public Matrix rotacao;
        public float rotacaoY;
        public float scale;
        float velocidade;

        private KeyboardState kbAnterior;
        private KeyboardState kbAnterior2;
        public Matrix inclinationMatrix; //Matriz que descreve a inclinação do tanque, causada pelos declives do terreno



        private bool tanqueLigado;



        public void ativarTanque2(List<Tank2> listaTanques2)
        {

            this.alive = true;
            this.CannonRotation = 0f;
            this.TurretRotation = 0f;
        }





        public bool moving;



        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;


        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;


        Matrix[] boneTransforms;


        float wheelBackLeftRotationValue, wheelBackRightRotationValue, wheelFrontLeftRotationValue, wheelFrontRightRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        public float cannonRotationValue;
        float hatchRotationValue;






        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }


        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }


        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }



        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }



        public Tank2(Random random, GraphicsDevice graphicsDevice, Vector3 position)
        {
            moving = false;
            alive = true;
            vetorBase = new Vector3(0, 0, 1);
            direcao = vetorBase;
            direcaoAnterior = vetorBase;
            positionAnterior = position;
            this.posicao = position;
            target = position + direcao;
            rotacaoY = 0;
            rotacao = Matrix.CreateRotationY(rotacaoY);
            velocidade = 0.05f;
            scale = 0.00125f;


            device = graphicsDevice;
            world = rotacao
                * Matrix.CreateScale(scale)
                * Matrix.CreateTranslation(position);



        }


        public void LoadContent(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            tankModel = content.Load<Model>("tank");

            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;

            // Allocate the transform matrix array.
            boneTransforms = new Matrix[tankModel.Bones.Count];

        }


        public void Update(GameTime gameTime, List<Tank2> listaTanques2, ContentManager content, Random random)
        {
            moving = false;

            UpdateInput(gameTime, content, random);


            positionAnterior = posicao;
        }
       



        public bool Ativado()
        {
            return this.tanqueLigado;
        }

         private void UpdateInput(GameTime gameTime, ContentManager content, Random random)
          {
               KeyboardState currentKeyboardState2 = Keyboard.GetState();

              steerRotationValue = 0;

              //  Move torre (só até 45 graus)
              if (currentKeyboardState2.IsKeyDown(Keys.Left))
              {
                  if (this.TurretRotation < 1.6f)
                      this.TurretRotation += 0.01f;
              }

              if (currentKeyboardState2.IsKeyDown(Keys.Right))
              {
                  if (this.TurretRotation >- 1.6f)
                      this.TurretRotation -= 0.01f;
              }

              //  Move canhão 
              if (currentKeyboardState2.IsKeyDown(Keys.Up))
              {
                  if (this.CannonRotation > -0.8f)
                      this.CannonRotation -= 0.01f;

              }
              if (currentKeyboardState2.IsKeyDown(Keys.Down))
              {
                  if (this.CannonRotation < 0.2f)
                      this.CannonRotation += 0.01f;
              }

              //  Abre e fecha a Comporta
              if (currentKeyboardState2.IsKeyDown(Keys.PageUp))
              {
                  this.HatchRotation = -1;
              }
              if (currentKeyboardState2.IsKeyDown(Keys.PageDown))
              {
                  this.HatchRotation = 0;
              }

              if (currentKeyboardState2.IsKeyDown(Keys.I))
              {
                  //Mover para a frente
                  this.wheelBackLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 5;
                  this.wheelBackRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 5;
                  this.wheelFrontLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 5;
                  this.wheelFrontRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 5;

                  posicao += Vector3.Normalize(direcao) * velocidade;
                  moving = true;
              }

              if (currentKeyboardState2.IsKeyDown(Keys.K))
              {
                  //Mover para trás
                  this.wheelBackLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                  this.wheelBackRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                  this.wheelFrontLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                  this.wheelFrontRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;

                  posicao -= Vector3.Normalize(direcao) * velocidade;
                  moving = true;
              }

              if (currentKeyboardState2.IsKeyDown(Keys.L))
              {
                  //Virar para a direita
                  if (!currentKeyboardState2.IsKeyDown(Keys.I) && !currentKeyboardState2.IsKeyDown(Keys.K))
                  {
                      this.wheelBackLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelBackRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelFrontLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelFrontRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                  }
                  else
                  {
                      if (currentKeyboardState2.IsKeyDown(Keys.I))
                      {
                          this.wheelBackLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelBackRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                      }
                      if (currentKeyboardState2.IsKeyDown(Keys.K))
                      {
                          this.wheelBackLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelBackRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                      }

                  }
                  rotacaoY -= 0.8f;
                  steerRotationValue = -0.5f;

              }

              if (currentKeyboardState2.IsKeyDown(Keys.J))
              {
                  //Virar para a esquerda
                  if (!currentKeyboardState2.IsKeyDown(Keys.I) && !currentKeyboardState2.IsKeyDown(Keys.K))
                  {
                      this.wheelBackLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelBackRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelFrontLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                      this.wheelFrontRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                  }
                  else
                  {
                      if (currentKeyboardState2.IsKeyDown(Keys.I))
                      {
                          this.wheelBackLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelBackRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontLeftRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontRightRotationValue = (float)gameTime.TotalGameTime.TotalSeconds * 10;
                      }
                      if (currentKeyboardState2.IsKeyDown(Keys.K))
                      {
                          this.wheelBackLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelBackRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontLeftRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                          this.wheelFrontRightRotationValue = -(float)gameTime.TotalGameTime.TotalSeconds * 10;
                      }
                  }
                  rotacaoY += 0.8f;
                  steerRotationValue = 0.5f;
              }

        
     

              posicao.Y = Terreno.AlturaHeighmap(posicao);

              rotacao = Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(rotacaoY));
              direcao = Vector3.Transform(vetorBase, rotacao);

              Vector3 Up = Terreno.NormalHeighmap(posicao);
              Vector3 Right = Vector3.Cross(Up, direcao);
              Vector3 Frente = Vector3.Cross(Up, Right);

              inclinationMatrix = Matrix.CreateWorld(posicao, Frente, Up);

              this.world = inclinationMatrix
                  * Matrix.CreateScale(scale)
                  * Matrix.CreateTranslation(posicao);

              kbAnterior2 = currentKeyboardState2;
        

          }

      

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(GraphicsDevice graphics, BasicEffect efeito)
        {
            // Set the world matrix as the root transform of the model.
            tankModel.Root.Transform = world;

            // Calculate matrices based on the current animation position.
            Matrix wheelRotationBackLeft = Matrix.CreateRotationX(wheelBackLeftRotationValue);
            Matrix wheelRotationBackRight = Matrix.CreateRotationX(wheelBackRightRotationValue);
            Matrix wheelRotationFrontLeft = Matrix.CreateRotationX(wheelFrontLeftRotationValue);
            Matrix wheelRotationFrontRight = Matrix.CreateRotationX(wheelFrontRightRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotationBackLeft * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotationBackRight * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotationFrontLeft * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotationFrontRight * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;

            // Look up combined bone matrices for the entire model.
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {

                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;

                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = efeito.DirectionalLight0.Direction;


                    effect.DirectionalLight0.Enabled = true;


                }
                mesh.Draw();
            }

        }
    }
}