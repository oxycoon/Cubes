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
    /// 
    /// Denne klassen er basert på Reimers:
    /// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Creating_the_3D_city.php
    /// </summary>
    public class Building : Microsoft.Xna.Framework.GameComponent
    {
        #region Variables
        private Effect effect;
        private GraphicsDevice device;

        private int[,] indices;

        private int[] heights;
        private Color[] colors;
        private VertexBuffer buildingBuffer;

        private Texture2D texture;
        #endregion

        #region Get/sets
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }
        #endregion

        public Building(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            heights = new int[] { 0, 60, 75, 40, 80, 120, 105, 90, 110};
            colors = new Color[] { Color.Blue, Color.Yellow, Color.Red, Color.Red, Color.Blue, Color.Brown, Color.Purple, Color.Red, Color.Green};
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

            heights = new int[] { 0, 60, 75, 40, 80, 120, 105, 30, 110};
            colors = new Color[] { Color.Blue, Color.Yellow, Color.Red, Color.Red, Color.Blue, Color.Brown, Color.White, Color.Red};
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
        public void initElements()
        {
            initIndices();
            initVertices();
        }
        private void initVertices()
        {
            int differentBuildings = heights.Length - 1;
            float imagesInTexture = 1 + differentBuildings * 2;

            int cityWidth = indices.GetLength(0);
            int cityLength = indices.GetLength(1);


            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentbuilding = indices[x, z];

                    //floor or ceiling
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2(currentbuilding * 2 / imagesInTexture, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 1, 0), new Vector2((currentbuilding * 2 + 1) / imagesInTexture, 1)));

                    if (currentbuilding != 0)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, heights[currentbuilding], -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, heights[currentbuilding], -z), new Vector3(1, 0, 0), new Vector2((currentbuilding * 2) / imagesInTexture, 0)));
                    }
                }
            }

            buildingBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            buildingBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
        }
        private void initIndices()
        {
            indices = new int[,]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1,1,0,0,1,1},
                {1,0,0,1,1,0,0,0,1,1,0,0,1,0,1,1,0,0,1,1},
                {1,0,0,1,1,0,1,0,1,0,0,0,1,0,0,1,0,1,0,1},
                {1,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
                {1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
                {1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
                {1,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1},
                {1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,1,0,0,1,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,1,1,1,1},
                {1,0,1,1,0,0,0,0,1,1,0,0,0,1,0,0,1,1,1,1},
                {1,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
             };

            Random random = new Random();
            int differentBuildings = heights.Length - 1;
            for (int x = 0; x < indices.GetLength(0); x++)
                for (int y = 0; y < indices.GetLength(1); y++)
                    if (indices[x, y] == 1)
                        indices[x, y] = random.Next(differentBuildings) + 1;
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

            Matrix world = Matrix.CreateScale(new Vector3 (55.0f, 2.0f, 55.0f)) * Matrix.CreateTranslation(new Vector3(-525.0f, -10.0f, 525.0f));

            effect.Parameters["xWorld"].SetValue(world);
            effect.Parameters["xEnableLightingColor"].SetValue(true);
            effect.Parameters["xDiffuseLight"].SetValue(LightSettings.DiffuseLight);
            effect.Parameters["xDiffuseMaterial"].SetValue(LightSettings.DiffuseMaterial);
            effect.Parameters["xAmbientLight"].SetValue(LightSettings.AmbientLight);
            effect.Parameters["xAmbientMaterial"].SetValue(LightSettings.AmbientMaterial);
            effect.Parameters["xLightDirection"].SetValue(LightSettings.LightDirection);
            effect.Parameters["xTexture"].SetValue(texture);


            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(buildingBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, buildingBuffer.VertexCount / 3);
            }
        }
    }
}
