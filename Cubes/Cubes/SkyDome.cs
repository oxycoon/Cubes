using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cubes
{
    class SkyDome : Microsoft.Xna.Framework.GameComponent
    {
        private Effect effect;      
        private GraphicsDevice device;      
        private Texture2D texture;
        // SkyDome modelen er hentet fra reimers.
        private Model model;        
        private DepthStencilState dsState;
        private Vector3 scale = new Vector3(2000.0f, 3000.0f, 2000.0f);
        private Vector3 translation = new Vector3(0, -0.3f, 0);

        public Vector3 Translation
        {
            get { return translation; }
            set { translation = value; }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }


        public Effect Effect
        {
            get { return effect; }
            set 
            { 
                effect = value;
                model.Meshes[0].MeshParts[0].Effect = effect;
            }
        }

        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public SkyDome(Game game) : base(game)
        {
         
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public void Draw(Matrix currentViewMatrix, Matrix currentProjection)
        {
            dsState = new DepthStencilState();
            dsState.DepthBufferWriteEnable = false;
            device.DepthStencilState = dsState;

            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            Matrix wMatrix = Matrix.CreateTranslation(translation) * Matrix.CreateScale(scale); //* Matrix.CreateTranslation(theCamera.CamPos)
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(currentViewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(currentProjection);
                    currentEffect.Parameters["xTexture1"].SetValue(texture);
                    currentEffect.Parameters["xEnableLighting"].SetValue(false);
                }
                mesh.Draw();
            }
            dsState = new DepthStencilState();
            dsState.DepthBufferWriteEnable = true;
            device.DepthStencilState = dsState;
        }

    }
}
