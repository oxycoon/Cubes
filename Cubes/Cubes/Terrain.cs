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

        private static int CRANELENGTH = 100;
        private static float SIZEMULTIPLYER = 4.0f;

        private float[,] heightMap;

        private VertexPositionNormalTexture[] vertices;

        private int[] indices;

        private Vector3 ambientLight, ambientMaterial, diffuseLight, diffuseMaterial, lightDirection;

        private Texture2D terrTex;

        public Texture2D Texture
        {
            get { return terrTex; }
            set { terrTex = value; }
        }

        public Terrain(Game game)
            : this(game, 300, 300, 3)
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
            SetupLighting();
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
            vertices = new VertexPositionNormalTexture[width * length];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    if (Math.Abs(Math.Sqrt(Math.Pow(x - width / 2, 2) + Math.Pow(y - length / 2, 2))) <= (CRANELENGTH+20) / SIZEMULTIPLYER){
                        vertices[x + y * width].Position = new Vector3(x * SIZEMULTIPLYER, heightMap[x, y], y * SIZEMULTIPLYER);
                    } else {
                        vertices[x + y * width].Position = new Vector3(x * SIZEMULTIPLYER, heightMap[x, y] * SIZEMULTIPLYER, y * SIZEMULTIPLYER);
                    }
                    #region Failed attempts
                    //vertices[x + y * width].TextureCoordinate.X = (x) / (width);
                    //vertices[x + y * width].TextureCoordinate.Y = (y) / (length);
                    //vertices[x + y * width].TextureCoordinate.X = ((x % 2 == 0) ? 1 : 0);
                    //vertices[x + y * width].TextureCoordinate.Y = ((y % 2 == 0) ? 1 : 0);
                    //vertices[x + y * width].TextureCoordinate.X = (float)Math.Cos(x / (Math.PI * 4));
                    //vertices[x + y * width].TextureCoordinate.Y = (float)Math.Cos(y / (Math.PI * 4));

                    //if (x % 6 == 0)
                    //    vertices[x + y * width].TextureCoordinate.X = 0.0f;
                    //if ((x + 1) % 6 == 0)
                    //    vertices[x + y * width].TextureCoordinate.X = 0.25f;
                    //if ((x + 3) % 6 == 0)
                    //    vertices[x + y * width].TextureCoordinate.X = 0.50f;
                    //if ((x + 4) % 6 == 0)
                    //    vertices[x + y * width].TextureCoordinate.X = 0.75f;
                    //if ((x + 5) % 6 == 0)
                    //    vertices[x + y * width].TextureCoordinate.X = 1.0f;

                    //if (y % 5 == 0)
                    //    vertices[x + y * width].TextureCoordinate.Y = 0.0f;
                    //if ((y + 1) % 5 == 0)
                    //    vertices[x + y * width].TextureCoordinate.Y = 0.25f;
                    //if ((y + 3) % 5 == 0)
                    //    vertices[x + y * width].TextureCoordinate.Y = 0.50f;
                    //if ((y + 4) % 5 == 0)
                    //    vertices[x + y * width].TextureCoordinate.Y = 0.75f;
                    //if ((y + 5) % 5 == 0)
                    //    vertices[x + y * width].TextureCoordinate.Y = 1.0f;

                    #endregion
                    
                    float textureSpan = 150.0f;
                    float textureStart = 0.0f;
                    float textureSteps = 1.0f;
                    //-1 + (((x mod 4)/2) *2)
                    vertices[x + y * width].TextureCoordinate.X = textureStart + ((float)((float)((float)x % textureSpan) / textureSpan) * textureSteps);
                    vertices[x + y * width].TextureCoordinate.Y = textureStart + ((float)((float)((float)y % textureSpan) / textureSpan) * textureSteps);
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

        private void SetupLighting()
        {
            ambientLight = new Vector3(1.0f, 1.0f, 1.0f);
            ambientMaterial = new Vector3(0.7f, 0.7f, 0.7f);
            diffuseLight = new Vector3(1.0f, 1.0f, 1.0f);
            diffuseMaterial = new Vector3(0.4f, 0.7f, 0.6f);
            lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
        }

        #endregion

        public void Draw(GameTime gametime, Effect effect, GraphicsDevice device)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;

            Matrix world = Matrix.CreateTranslation(-(width * SIZEMULTIPLYER) / 2.0f, 0, -(length*SIZEMULTIPLYER) / 2.0f);
            effect.Parameters["xWorld"].SetValue(world);
            effect.Parameters["xEnableLightingTexture"].SetValue(true);
            effect.Parameters["xDiffuseLight"].SetValue(diffuseLight);
            effect.Parameters["xDiffuseMaterial"].SetValue(diffuseMaterial);
            effect.Parameters["xAmbientLight"].SetValue(ambientLight);
            effect.Parameters["xAmbientMaterial"].SetValue(ambientMaterial);
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xTexture"].SetValue(terrTex);

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>888
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
