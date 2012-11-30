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
        #region Variables
        private Matrix world;
        private Matrix[] meshMatrix;
        private Boolean active = false;
        private Boolean hasBlock = false;
        private Model model;
        private Model theWireModel;
        private IInputHandler input;
        //private double altitude;
        private bool isHooked;
        private KeyboardState oldState;
        //private Matrix world, view, projection;
        #endregion

        #region Get/Set
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

        public Boolean HasBlock
        {
            get { return hasBlock; }
            set { hasBlock = value; }
        }

        public Boolean Active
        {
            get { return active; }
            set { active = value; }
        }
       
        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

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

        public Hook(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
        }
#endregion

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
           
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                position.X -= 1.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                position.X += 1.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.R))
            {
                position.Y -= 1.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.F))
            {
                position.Y += 1.0f;
            }

            if (input.KeyboardState.IsKeyDown(Keys.X) && input.KeyboardState != oldState)
            {
                active = !active;
                if (hasBlock)
                    hasBlock = !hasBlock;
            }

            if (position.Y > 75.0f)
                position.Y = 75.0f;
            if (position.Y < 0.0f)
                position.Y = 0.0f;
            if (position.X > 90.0f)
                position.X = 90.0f;
            if (position.X < 10.0f)
                position.X = 10.0f;

            oldState = input.KeyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the crane.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values</param>
        /// <param name="camera">Camera to draw object in correct areas</param>
        /// <param name="world">World matrix for translations, if neccesary</param>
        /// <returns name="world">The generated world matrix</returns>
        public Matrix Draw(GameTime gametime, Camera camera, Matrix _world, float craneRotation)
        {
            Matrix matHookTrans, matHookScale, matHookOrbit, hookWorld, matWireScale, matWireTrans, matWireOrb, wireWorld;

            //ISROT, identity, scale, rotation, orbit, translation
            matHookScale = Matrix.CreateScale(20.0f);
            matHookTrans = Matrix.CreateTranslation(0.0f, 80.0f - position.Y, 100.0f - position.X);
            matHookOrbit = matHookTrans * Matrix.CreateRotationY(craneRotation);
            hookWorld = _world * matHookScale * matHookOrbit;

            matWireScale = Matrix.CreateScale(new Vector3(5.0f, position.Y*2, 5.0f));
            matWireTrans = Matrix.CreateTranslation(0.0f, 86.0f - position.Y, 100 - position.X);
            matWireOrb = matWireTrans * Matrix.CreateRotationY(craneRotation);
            wireWorld = _world * matWireScale * matWireOrb;
            world = hookWorld;

            model.Draw(hookWorld, camera.View, camera.Projection);
            theWireModel.Draw(wireWorld, camera.View, camera.Projection);

            return hookWorld;
        }

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
    }
}
