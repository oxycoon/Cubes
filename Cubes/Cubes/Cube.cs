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
    public class Cube : Microsoft.Xna.Framework.GameComponent
    {
        #region Variabler
        private Matrix world;
        private Boolean hooked = false;
        private Boolean move = true;

        private Vector3 position;
        private Vector3 scale;

        private Model model;
        private Texture textureSide, textureTop;
        private IInputHandler input;
        private Effect effect;



        private Matrix[] meshMatrix;
        #endregion

        #region Get/Sets
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public Boolean Move
        {
            get { return move; }
            set { move = value; }
        }

        private float fallSpeed = 0;

        public float FallSpeed
        {
            get { return fallSpeed; }
            set { fallSpeed = value; }
        }

        public Boolean Hooked
        {
            get { return hooked; }
            set { hooked = value; }
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

        public Texture TextureSide
        {
            get { return textureSide; }
            set { textureSide = value; }
        }

        public Texture TextureTop
        {
            get { return textureTop; }
            set { textureTop = value; }
        }

        /// <summary>
        /// Get/Set for modellen til kuben
        /// 
        /// Set-metoden vil også opprette BoundingSphere og legge den i modellens tag.
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

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        /// <summary>
        /// Standardkonstruktør for kuben
        /// </summary>
        /// <param name="game">Spillet</param>
        public Cube(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
        }

        /// <summary>
        /// Konstruktør for kube med gitt posisjon.
        /// </summary>
        /// <param name="game">Spillet</param>
        /// <param name="position">Kubens posisjon</param>
        public Cube(Game game, Vector3 position, Effect effect)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
            this.position = position;
            this.effect = effect;
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
            if (move)
            {
                if (!hooked && position.Y > 0)
                {
                    position.Y -= (fallSpeed += 0.1f);
                }
                if (!hooked && position.Y < 0)
                {
                    position.Y = 0;
                    fallSpeed = 0;
                    CubesGame.theSmokeEffect.addExplotion(new Vector3(world.Translation.X, world.Translation.Y, world.Translation.Z));
                    //CubesGame.theSmokeEffect.addExplotion(new Vector3(world.Translation.X, world.Translation.Y, world.Translation.Z));
                    //CubesGame.theSmokeEffect.addExplotion(new Vector3(world.Translation.X, world.Translation.Y, world.Translation.Z));
                    move = false;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Tegner kuben
        /// </summary>
        /// <param name="world">Verdensmatrisen kuben skal tegnes i dersom den er plukket opp</param>
        /// <param name="camera">Spillets kamera</param>
        public void Draw(Matrix world, Camera camera)
        {
            Matrix matWorld, matCubeTrans, matScale;
            
            matWorld = Matrix.Identity;
            if (hooked)
            {
                matScale = Matrix.CreateScale(5.0f);
                matCubeTrans = Matrix.CreateTranslation(new Vector3(0.0f, -65.0f, 0.0f));            
                matWorld = matScale * matCubeTrans * world;
                
            }
            else
            {
                    matWorld =  Matrix.CreateTranslation(position);
            }
            if (!position.Equals(matWorld.Translation))
                position = matWorld.Translation;

            this.world = matWorld;

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];

            Matrix[] modelTransformations = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransformations);

            foreach (ModelMesh mm in model.Meshes)
            {
                foreach (Effect currentEffect in mm.Effects)
                {
                    // Setter alle parameterene til effekten.
                    currentEffect.CurrentTechnique = effect.CurrentTechnique;
                    currentEffect.Parameters["xWorld"].SetValue(modelTransformations[mm.ParentBone.Index] * matWorld);
                    currentEffect.Parameters["xView"].SetValue(camera.View);
                    currentEffect.Parameters["xProjection"].SetValue(camera.Projection);
                    currentEffect.Parameters["xEnableLightingTexture"].SetValue(true);
                    currentEffect.Parameters["xDiffuseLight"].SetValue(LightSettings.DiffuseLight);
                    currentEffect.Parameters["xDiffuseMaterial"].SetValue(LightSettings.DiffuseMaterial);
                    currentEffect.Parameters["xAmbientLight"].SetValue(LightSettings.AmbientLight);
                    currentEffect.Parameters["xAmbientMaterial"].SetValue(LightSettings.AmbientMaterial);
                    currentEffect.Parameters["xLightDirection"].SetValue(LightSettings.LightDirection);

                    if (mm.Name.Equals("Box"))
                        currentEffect.Parameters["xTexture"].SetValue(textureSide);
                    else
                        currentEffect.Parameters["xTexture"].SetValue(textureTop);
                }
                mm.Draw();
            }



            //model.Draw(matWorld, camera.View, camera.Projection);
        }

        /// <summary>
        /// Finner den transformerte BoundingSpheren
        /// 
        /// Denne metoden er hentet fra undervisnings dokument på it's learning.
        /// Er kopiert og tilpasset i andre klasser.
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

            float transformedSphereRadius = (originalBoundingSphere.Radius * maxScale) * 0.015f;
            Vector3 transformedSphereCenter = Vector3.Transform(originalBoundingSphere.Center, transformationMatrix);

            BoundingSphere transformedBoundingSphere = new BoundingSphere(transformedSphereCenter, transformedSphereRadius);

            return transformedBoundingSphere; // Returning the transformed BoundingSphere
        }

    }
}
