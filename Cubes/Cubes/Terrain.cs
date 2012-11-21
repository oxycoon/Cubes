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
    public class Terrain : Microsoft.Xna.Framework.GameComponent
    {
        private int width, length, height;

        private int[,] heightMap;

        private VertexPositionColor[] vertices;
        private int[] indices;

        public Terrain(Game game)
            : this(game, 200, 200, 4)
        {
        }


        public Terrain(Game game, int width, int length, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            this.width = width;
            this.length = length;
            this.height = height;

            generateTerrain();
        }

        public void generateTerrain()
        {
            generateHeightMap();
            SetUpVertices();
            SetUpIndices();
        }

        private void generateHeightMap()
        {
            Random random = new Random((int) ((long) DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            heightMap = new int[width,length];
            for (int x = 0; x < width; x++)
                for (int y = 0; x < length; x++)
                    heightMap[x,y] = random.Next(height);
        }

        private void SetUpVertices()
        {
            vertices = new VertexPositionColor[width * length];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    vertices[x + y * width].Position = new Vector3(x, heightMap[x,y], -y);
                }
            }
        }

        private void SetUpIndices()
        {
            indices = new int[(width - 1) * (length - 1) * 6];
            int counter = 0;
            for (int y = 0; y < length - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
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

        public void Draw(GameTime gametime, BasicEffect effect, GraphicsDevice device)
        {
            Matrix world = Matrix.CreateTranslation(-width / 2.0f, 0, length / 2.0f);

            effect.World = world;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
            }
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

        public void Draw(GameTime time, Matrix world, Matrix view, Matrix projection)
        { 
            
        }
    }
}
