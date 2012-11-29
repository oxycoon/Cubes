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
    public class Building : Microsoft.Xna.Framework.GameComponent
    {
        private Effect effect;
        private GraphicsDevice device;

        private int[,] indices;

        private int[] heights;
        private Color[] colors;
        private VertexBuffer buildingBuffer;

        

        #region Get/sets
        #endregion

        public Building(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public Building(Game game, Color[] colors, int[] heights, Effect effect, GraphicsDevice device)
            : base(game)
        {
            // TODO: Construct any child components here
            this.heights = heights;
            this.colors = colors;
            this.device = device;
            this.effect = effect;
        }

        public Building(Game game, Effect effect, GraphicsDevice device) : base(game)
        {
            this.device = device;
            this.effect = effect;

            heights = new int[] { 50, 60, 75, 40, 80, 120, 105 };
            colors = new Color[] { Color.Blue, Color.Yellow, Color.Red, Color.Red, Color.Blue, Color.Brown, Color.White };
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

        private void initVertices()
        {
            int differentBuildings = heights.Length - 1;
            float imagesInTexture = 1 + differentBuildings * 2;

            int cityWidth = indices.GetLength(0);
            int cityLength = indices.GetLength(1);


            List<VertexPositionColorNormal> verticesList = new List<VertexPositionColorNormal>();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentbuilding = indices[x, z];

                    //floor or ceiling
                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 1, 0), colors[currentbuilding]));
                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), colors[currentbuilding]));
                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 1, 0), colors[currentbuilding]));

                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), colors[currentbuilding]));
                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), colors[currentbuilding]));
                    verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 1, 0), colors[currentbuilding]));

                    if (currentbuilding != 0)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));

                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), colors[currentbuilding]));

                        //back wall
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, 0, -z), new Vector3(0, 0, 1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 0, 1), colors[currentbuilding]));

                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 0, 1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 0, 1), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), colors[currentbuilding]));

                        //left wall
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), colors[currentbuilding]));

                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, heights[currentbuilding], -z), new Vector3(-1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), colors[currentbuilding]));

                        //right wall
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), colors[currentbuilding]));

                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), colors[currentbuilding]));
                        verticesList.Add(new VertexPositionColorNormal(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(1, 0, 0), colors[currentbuilding]));
                    }
                }
            }
            buildingBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly); buildingBuffer.SetData<VertexPositionColorNormal>(verticesList.ToArray());
        }

        private void initIndices()
        {
            indices = new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,1,1,0,0,0,1,1,0,0,1,0,1},
                {1,0,0,1,1,0,0,0,1,0,0,0,1,0,1},
                {1,0,0,0,1,1,0,1,1,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,1,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,1,1,0,0,0,0,1,1,0,0,0,1,1},
                {1,0,0,0,0,0,0,0,1,1,0,0,0,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
             };
        }

        private void calculateNormals()
        { 
            
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

        public void Draw()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;

            //effect.Parameters["xWorld"].SetValue(world);
            effect.Parameters["xEnableLightingTexture"].SetValue(true);
            effect.Parameters["xDiffuseLight"].SetValue(LightSettings.DiffuseLight);
            effect.Parameters["xDiffuseMaterial"].SetValue(LightSettings.DiffuseMaterial);
            effect.Parameters["xAmbientLight"].SetValue(LightSettings.AmbientLight);
            effect.Parameters["xAmbientMaterial"].SetValue(LightSettings.AmbientMaterial);
            effect.Parameters["xLightDirection"].SetValue(LightSettings.LightDirection);

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];
        }
    }
}
