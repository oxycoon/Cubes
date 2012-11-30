using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Cubes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Hook : Microsoft.Xna.Framework.GameComponent
    {
        private Matrix world;
        private Matrix[] meshMatrix;

        private Boolean active = false;
        private Boolean hasBlock = false;

        private Model model;
        private Model theWireModel;

        private Texture2D magnetTexture;
        private Texture2D wireTexture;
        
        private Vector3 position;

        private KeyboardState oldState;
        private IInputHandler input;

        private Effect effect;

        #region Get/Set
        public Texture2D WireTexture
        {
            get { return wireTexture; }
            set { wireTexture = value; }
        }

        public Texture2D MagnetTexture
        {
            get { return magnetTexture; }
            set { magnetTexture = value; }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        public Matrix[] MeshMatrix
        {
            get { return meshMatrix; }
            set { meshMatrix = value; }
        }

        public Boolean Active
        {
            get { return active; }
            set { active = value; }
        }

        public Boolean HasBlock
        {
            get { return hasBlock; }
            set { hasBlock = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Get/Set for modellen til kroken
        /// 
        /// Set-metoden vil ogs� opprette BoundingSphere og legge den i modellens tag.
        /// </summary>
        public Model Model
        {
            get { return model; }
            set 
            {
                model = value;
                meshMatrix = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(meshMatrix);

                BoundingSphere compMeshSphere = new BoundingSphere();

                foreach (ModelMesh mesh in model.Meshes)
                {
                    BoundingSphere orgiMeshSphere = mesh.BoundingSphere;
                    orgiMeshSphere = TransformBoundingSphere(orgiMeshSphere, meshMatrix[mesh.ParentBone.Index]);
                    compMeshSphere = BoundingSphere.CreateMerged(compMeshSphere, orgiMeshSphere);
                }
                model.Tag = compMeshSphere;
            }
        }

        public Model WireModel
        {
            get { return theWireModel; }
            set { theWireModel = value; }
        }
        #endregion

        public Hook(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            #region Keyboard logic
            //Beveger kroken (og vaieren) bakover p� kranen.
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                position.X += 1.0f;
            }
            //Beveger kroken (og vaieren) fremover p� kranen.
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                position.X -= 1.0f;
            }
            //Beveger kroken (og vaieren) ned.
            if (input.KeyboardState.IsKeyDown(Keys.R))
            {
                position.Y -= 1.0f;
            }
            //Beveger kroken (og vaieren) opp.
            if (input.KeyboardState.IsKeyDown(Keys.F))
            {
                position.Y += 1.0f;
            }
            //Skrur av/p� magneten, hvis den har en blokk slipper den blokken
            if (checkKeyState(Keys.X))
            {
                active = !active;
                if (hasBlock)
                    hasBlock = !hasBlock;
            }
            #endregion
            #region Restriction logic
            //Sjekker om magneten er for langt nede
            if (position.Y > 67.0f)
                position.Y = 67.0f;
            //Sjekker om magneten er for langt oppe
            if (position.Y < 0.0f)
                position.Y = 0.0f;
            //Sjekker om magneten er for langt unna kranen
            if (position.X > 90.0f)
                position.X = 90.0f;
            //Sjekker om magneten er for n�rme kranen.
            if (position.X < 10.0f)
                position.X = 10.0f;
            #endregion

            oldState = input.KeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Tegner kroken og vaieren.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values</param>
        /// <param name="camera">Camera to draw object in correct areas</param>
        /// <param name="world">World matrix for translations, if neccesary</param>
        /// <returns name="world">The generated world matrix</returns>
        public Matrix Draw(GameTime gametime, Camera camera, Matrix _world, float craneRotation)
        {
            Matrix matHookTrans, matHookScale, matHookOrbit, hookWorld, matWireScale, matWireTrans, matWireOrb, wireWorld;

            //Transformerer Kroken
            matHookScale = Matrix.CreateScale(20.0f);
            matHookTrans = Matrix.CreateTranslation(0.0f, 80.0f - position.Y, position.X);
            matHookOrbit = matHookTrans * Matrix.CreateRotationY(craneRotation);
            hookWorld = _world * matHookScale * matHookOrbit;

            //Transformerer vaieren
            matWireScale = Matrix.CreateScale(new Vector3(5.0f, position.Y*2, 5.0f));
            matWireTrans = Matrix.CreateTranslation(0.0f, 86.0f - position.Y, position.X);
            matWireOrb = matWireTrans * Matrix.CreateRotationY(craneRotation);
            wireWorld = _world * matWireScale * matWireOrb;

            world = hookWorld;

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];

            Matrix[] modelTransformations = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransformations);

            #region Magnet foreach
            foreach (ModelMesh mm in model.Meshes)
            {
                foreach (Effect currentEffect in mm.Effects)
                {
                    // Setter alle parameterene til effekten.
                    currentEffect.CurrentTechnique = effect.CurrentTechnique;
                    currentEffect.Parameters["xWorld"].SetValue(modelTransformations[mm.ParentBone.Index] * hookWorld);
                    currentEffect.Parameters["xView"].SetValue(camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(camera.Projection);
                    currentEffect.Parameters["xEnableLightingTexture"].SetValue(true);
                    currentEffect.Parameters["xDiffuseLight"].SetValue(LightSettings.DiffuseLight);
                    currentEffect.Parameters["xDiffuseMaterial"].SetValue(LightSettings.DiffuseMaterial);
                    currentEffect.Parameters["xAmbientLight"].SetValue(LightSettings.AmbientLight);
                    currentEffect.Parameters["xAmbientMaterial"].SetValue(LightSettings.AmbientMaterial);
                    currentEffect.Parameters["xLightDirection"].SetValue(LightSettings.LightDirection);


                    currentEffect.Parameters["xTexture"].SetValue(magnetTexture);

                }
                mm.Draw();
            }
            #endregion

            theWireModel.Draw(wireWorld, camera.View, camera.Projection);

            return hookWorld;
        }

        /// <summary>
        /// Finner den transformerte BoundingSpheren
        /// </summary>
        /// <param name="originalBoundingSphere">Den opprinnelige BoundingSphera til objektet</param>
        /// <param name="transformationMatrix">Transformasjonsmatrisa til objektet</param>
        /// <returns></returns>
        public static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
        {
            Vector3 trans;
            Vector3 scaling;
            Quaternion rot;
            transformationMatrix.Decompose(out scaling, out rot, out trans);

            float maxScale = scaling.X;
            if (maxScale < scaling.Y)
                maxScale = scaling.Y;
            if (maxScale < scaling.Z)
                maxScale = scaling.Z;

            float transformedSphereRadius = originalBoundingSphere.Radius * maxScale;
            Vector3 transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

            BoundingSphere transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

            return transformedBoundingSphere;
        }

        /// <summary>
        /// S�rger for at spilleren ikke kan holde inne en knapp for �
        /// gj�re en handling gjentatte ganger hver gang spillet oppdateres.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool checkKeyState(Keys key)
        {
            return (input.KeyboardState.IsKeyUp(key) && oldState.IsKeyDown(key));
        }
    }
}
