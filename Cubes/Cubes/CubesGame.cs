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
    /// This is the main type for your game
    /// </summary>
    public class CubesGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        private Crane theCrane;
        private Hook theHook;
        private Cube theCube;
        private Terrain theTerrain;
        private List<Cube> theCubeList;
        private InputHandler input;
        private Camera theCamera;
        private SkyDome theSky;

        private Matrix world, view, projection;
        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        private int debug = 0;

        //BasicEffect effect;
        Effect effect;

        private bool isFullScreen = false;


        public CubesGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            this.Components.Add(input);

            theCamera = new Camera(this);
            this.Components.Add(theCamera);

            theCrane = new Crane(this, theCamera);
            this.Components.Add(theCrane);

            theHook = new Hook(this);
            this.Components.Add(theHook);

            theTerrain = new Terrain(this);
            this.Components.Add(theTerrain);

            theSky = new SkyDome(this);
            this.Components.Add(theSky);

            theCube = new Cube(this);
            this.Components.Add(theCube);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            initDevice();

            this.IsMouseVisible = true;
           

            base.Initialize();
        }

        #region Initialize methods
        private void initDevice()
        {
            device = graphics.GraphicsDevice;

            //Setter størrelse på framebuffer:
            graphics.PreferredBackBufferWidth = 1400;
            graphics.PreferredBackBufferHeight = 800;
            graphics.IsFullScreen = isFullScreen;
            graphics.ApplyChanges();

            Window.Title = "Crane Simulator";

            //Initialiserer Effect-objektet:
        }
        #endregion


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            effect = Content.Load<Effect>("MyEffect");
            spriteFont = Content.Load<SpriteFont>("font");

            theCrane.Model = LoadModel("Models\\Crane");
            theCrane.CraneTexture = Content.Load<Texture2D>("Textures\\Texture_Crane");
            theCrane.WeightTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.BaseTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.WireTexture = Content.Load<Texture2D>("Textures\\SupportWire");

            theHook.Model = Content.Load<Model>("Models\\Hook");
            theHook.WireModel = Content.Load<Model>("Models\\Wire");

            theSky.Model = Content.Load<Model>("Models\\dome");
            theSky.Texture = Content.Load<Texture2D>("Textures\\clouds2");

            theCube.Model = Content.Load<Model>("Models\\testCube2");

            theTerrain.Texture = Content.Load<Texture2D>("Textures\\MC_Dirt");

            theCrane.Effect = effect;
            theSky.Device = device;
            theSky.Effect = effect;
        }

        private Model LoadModel(String name)
        {
            Model newModel = Content.Load<Model>(name);

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect;
            return newModel;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            view = theCamera.View;
            projection = theCamera.Projection;
            world = Matrix.Identity;

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xProjection"].SetValue(projection);
            effect.Parameters["xView"].SetValue(view);

            theTerrain.Draw(gameTime, effect, device);
            theSky.Draw(view, projection);
            

            

            BoundingSphere s1 = (BoundingSphere)theHook.Model.Tag;
            BoundingSphere s2 = (BoundingSphere)theCube.Model.Tag;
            
            BoundingSphere ss1 = TransformBoundingSphere(s1, theHook.World);
            BoundingSphere ss2 = TransformBoundingSphere(s2, theCube.World);

            if (Hook.Active)
            {
                if (ss1.Intersects(ss2))
                {

                    theCube.Hooked = true;
                }
                else
                {
                    theCube.Hooked = false;
                }
            }

            device.BlendState = BlendState.AlphaBlend;
            matrixStack.Push(theCrane.Draw(gameTime, theCamera, world));
            matrixStack.Push(theHook.Draw(gameTime, theCamera, matrixStack.Peek(), theCrane.Rotation));
            theCube.Draw(matrixStack.Peek(), theCamera, theCrane.Rotation);

            matrixStack.Pop();
            matrixStack.Pop();

            #region SpriteBatch
            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, theCamera.CamZoom.ToString(), new Vector2(10.0f, 10.0f), Color.Black);
            //spriteBatch.End();
            #endregion
            base.Draw(gameTime);
        }

        private static BoundingSphere TransformBoundingSphere(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
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
