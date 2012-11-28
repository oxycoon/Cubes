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
    public class Crane : Microsoft.Xna.Framework.GameComponent
    {
        private Model theCraneModel;
        private Texture2D theCraneTexture;
        private Effect effect;
        private IInputHandler input;
        private Camera camera;

        private float rotation = 0.0f;
        private Vector3 ambientLight, ambientMaterial, diffuseLight, diffuseMaterial, lightDirection;

        #region Get/Set methods
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Model Model
        {
            get { return theCraneModel; }
            set { theCraneModel = value; }
        }

        public Texture2D Texture
        {
            get { return theCraneTexture; }
            set { theCraneTexture = value; }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        #endregion

        public Crane(Game game, Camera camera)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
            this.camera = camera;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            SetupLighting();
            base.Initialize();
        }

        private void SetupLighting()
        {
            ambientLight = new Vector3(1.0f, 1.0f, 1.0f);
            ambientMaterial = new Vector3(0.7f, 0.7f, 0.7f);
            diffuseLight = new Vector3(1.0f, 1.0f, 1.0f);
            diffuseMaterial = new Vector3(0.4f, 0.7f, 0.6f);
            lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            if (input.KeyboardState.IsKeyDown(Keys.Right) || input.KeyboardState.IsKeyDown(Keys.D))
            {
                rotation -= 0.03f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.Left) || input.KeyboardState.IsKeyDown(Keys.A))
            {
                rotation += 0.03f;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the crane.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values</param>
        /// <param name="camera">Camera to draw object in correct areas</param>
        /// <param name="world">World matrix for translations, if neccesary</param>
        /// <returns name="world">The generated world matrix</returns>
        public Matrix Draw(GameTime gametime, Camera camera, Matrix world)
        {
            Matrix matY, matTrans, matScale;

            matY = Matrix.CreateRotationY(rotation);
            matTrans = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            matScale = Matrix.CreateScale(0.01f);

            //isrot, identify, scale, rotation, orbit, translation
            world = matScale * matY * matTrans;

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];

            Matrix[] modelTransformations = new Matrix[theCraneModel.Bones.Count];
            theCraneModel.CopyAbsoluteBoneTransformsTo(modelTransformations);

            foreach (ModelMesh mm in theCraneModel.Meshes)
            {
                foreach (Effect currentEffect in mm.Effects)
                {
                    currentEffect.CurrentTechnique = effect.CurrentTechnique;
                    currentEffect.Parameters["xWorld"].SetValue(modelTransformations[mm.ParentBone.Index] * world);
                    currentEffect.Parameters["xView"].SetValue(camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(camera.Projection);
                    currentEffect.Parameters["xEnableLightingTexture"].SetValue(true);
                    currentEffect.Parameters["xDiffuseLight"].SetValue(diffuseLight);
                    currentEffect.Parameters["xDiffuseMaterial"].SetValue(diffuseMaterial);
                    currentEffect.Parameters["xAmbientLight"].SetValue(ambientLight);
                    currentEffect.Parameters["xAmbientMaterial"].SetValue(ambientMaterial);
                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xTexture"].SetValue(theCraneTexture);
                }
                
                mm.Draw();
            }


            //theCraneModel.Draw(world, camera.View, camera.Projection);

            return world;
        }
    }
}
