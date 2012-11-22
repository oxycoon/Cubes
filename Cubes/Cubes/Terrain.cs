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

    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Terrain : Microsoft.Xna.Framework.GameComponent
    {
        private int width, length, height;

        private float[,] heightMap;

        private VertexPositionColorNormal[] vertices;

        private int[] indices;

        private Texture2D terrTex;

        public Texture2D TerrTex
        {
            get { return terrTex; }
            set { terrTex = value; }
        }

        public Terrain(Game game)
            : this(game, 200, 200, 2)
        {
        }


        public Terrain(Game game, int width, int length, int height)
            : base(game)
        {
            // TODO: Construct any child components here
            this.width = width;
            this.length = length;
            this.height = height;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            GenerateTerrain();
            base.Initialize();
        }

        #region Initialize methods
        public void GenerateTerrain()
        {
            GenerateHeightMap();
            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
        }

        private void GenerateHeightMap()
        {
            Random random = new Random((int)((long)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
            heightMap = new float[width, length];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < length - 1; y++)
                    heightMap[x, y] = (float)random.Next(height) / 4;
        }

        private void SetUpVertices()
        {
            vertices = new VertexPositionColorNormal[width * length];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    vertices[x + y * width].Position = new Vector3(x, heightMap[x, y], -y);
                    vertices[x + y * width].Color = Color.Brown;
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

        private void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }
        #endregion

        public void Draw(GameTime gametime, BasicEffect effect, GraphicsDevice device)
        {
            Matrix world = Matrix.CreateTranslation(-width / 2.0f, 0, length / 2.0f);
            effect.EnableDefaultLighting();
            effect.World = world;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
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
