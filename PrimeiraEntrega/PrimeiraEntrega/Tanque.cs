using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PrimeiraEntrega
{
    class Tanque
    {
        Model myModel;
        Matrix worldMatrix;
        Matrix view;
        Matrix projection;

        float scale;
        // float rotacao = 3.0f;

        public Vector3 direcao;
        public Vector3 position;

        // Transformações iniciais
        // (posicionar torre e canhão)
        Matrix cannonTransform;
        Matrix turretTransform;
        // Guarda todas as transformações
        Matrix[] boneTransforms;

        // Shortcut references to the bones that we are going to animate.
        // We could just look these up inside the Draw method, but it is more
        // efficient to do the lookups while loading and cache the results.
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;
        public float velocidade;
        public float rotacaoY;


        // Store the original transform matrix for each animating bone.
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;



        Matrix hatchTransform;

        // Current animation positions.
        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;


        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the steering rotation amount.
        /// </summary>
        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the turret rotation amount.
        /// </summary>
        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the cannon rotation amount.
        /// </summary>
        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }


        /// <summary>
        /// Gets or sets the entry hatch rotation amount.
        /// </summary>
        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }




        public Tanque(GraphicsDevice device, ContentManager content)
        {
            float aspectRatio = (float)device.Viewport.Width /
                           device.Viewport.Height;
            scale = 0.001f;
            worldMatrix = Matrix.Identity;
            view = Matrix.CreateLookAt(new Vector3(0.0f, 1.0f, 2.0f), Vector3.Zero, Vector3.Up);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 5.0f);

            // Lê os bones
            myModel = content.Load<Model>("tank");


            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = myModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = myModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = myModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = myModel.Bones["r_front_wheel_geo"];
            leftSteerBone = myModel.Bones["l_steer_geo"];
            rightSteerBone = myModel.Bones["r_steer_geo"];
            turretBone = myModel.Bones["turret_geo"];
            cannonBone = myModel.Bones["canon_geo"];
            hatchBone = myModel.Bones["hatch_geo"];

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



            // Lê as transformações iniciais dos bones
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            // cria o array que armazenará as transformações em cascata dos bones
            boneTransforms = new Matrix[myModel.Bones.Count];
        }


        public void Draw(BasicEffect efeito)
        {

            // Aplica as transformações em cascata por todos os bones
            myModel.Root.Transform = Matrix.CreateScale(scale);

            //turretBone.Transform = Matrix.CreateRotationY(MathHelper.ToRadians(30.0f)) * turretTransform;
            //cannonBone.Transform = Matrix.CreateRotationX(MathHelper.ToRadians(30.0f));

            // Calculate matrices based on the current animation position.
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;
           
            efeito.World = Matrix.Identity;
            efeito.View = Camera.View;
            efeito.Projection = Camera.Projection;

            efeito.CurrentTechnique.Passes[0].Apply();

            // Look up combined bone matrices for the entire model.
            myModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in myModel.Meshes) // Desenha o modelo
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view; effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        public void Update(float time, GameTime gameTime)
        {

            KeyboardState currentKeyboardState = Keyboard.GetState();

            //  Roda as rodas to tanque


            //  Move torre (só até 90 graus)
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                if (this.TurretRotation < 1.6f)
                    this.TurretRotation += 0.01f;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                if (this.TurretRotation > -1.6f)
                    this.TurretRotation -= 0.01f;
            }


            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                if (this.CannonRotation > -0.8f)
                    this.CannonRotation -= 0.01f;

            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                if (this.CannonRotation < 0.2f)
                    this.CannonRotation += 0.01f;
            }

            //  Abre e fecha porta
            if (currentKeyboardState.IsKeyDown(Keys.PageUp))
            {
                this.HatchRotation = -1;
            }
            if (currentKeyboardState.IsKeyDown(Keys.PageDown))
            {
                this.HatchRotation = 0;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A))
            {

                rotacaoY += 0.5f;
                steerRotationValue = 0.6f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.D))
            {

                rotacaoY -= 0.5f;
                steerRotationValue = -0.6f;

            }
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                this.WheelRotation += time * 5;
                position += direcao * velocidade;

            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                this.WheelRotation -= time * 5;
                position -= direcao * velocidade;

            }
            if (!currentKeyboardState.IsKeyDown(Keys.D) && !currentKeyboardState.IsKeyDown(Keys.A))
            {
                steerRotationValue = 0f;
            }

        }

    }
}

