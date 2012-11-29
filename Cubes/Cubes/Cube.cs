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
        private Matrix world, view, projection;
        private Boolean hooked = false;
        private IInputHandler input;
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
        private Matrix[] meshMatrix;

        public Matrix[] MeshMatrix
        {
            get { return meshMatrix; }
            set { meshMatrix = value; }
        }

        private Vector3 position = new Vector3(50.0f, 0.0f, 0.0f);
        private Vector3 scale;

        private Model model;
        private Texture texture;

        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
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

        private Vector3 cubePosition;
        

        public Cube(Game game)
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
            base.Update(gameTime);
        }

        public void Draw(Matrix world, Camera camera, float craneRotation)
        {
            // ISROT
            Matrix matWorld, matCubeTrans, matCubeOrbit, matScale;
            
            if (hooked)
            {
                matScale = Matrix.CreateScale(10.0f);
                matCubeTrans = Matrix.CreateTranslation(new Vector3 (0.0f, 5.0f, 0.0f));
                //matCubeOrbit = matCubeTrans * Matrix.CreateRotationY(craneRotation);
                matWorld = world * matScale * matCubeTrans;
            }
            else
            {
                matWorld = Matrix.Identity * Matrix.CreateTranslation(position);
            }

            this.world = matWorld;

            model.Draw(matWorld, camera.View, camera.Projection);
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
