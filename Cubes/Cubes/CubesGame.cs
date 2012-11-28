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

        private Crane theCrane;
        private Hook theHook;
        private Terrain theTerrain;
        private List<Cube> theCubeList;
        private InputHandler input;
        private Camera theCamera;
        private SkyDome theSky;

        private Matrix world, view, projection;
        private Stack<Matrix> matrixStack = new Stack<Matrix>();


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

            theCrane.Model = LoadModel("Models\\Crane");
            theCrane.CraneTexture = Content.Load<Texture2D>("Textures\\Texture_Crane");
            theCrane.WeightTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.BaseTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.WireTexture = Content.Load<Texture2D>("Textures\\SupportWire");

            theHook.Model = Content.Load<Model>("Models\\Hook");
            theHook.WireModel = Content.Load<Model>("Models\\Wire");

            theSky.Model = Content.Load<Model>("Models\\dome");
            theSky.Texture = Content.Load<Texture2D>("Textures\\clouds2");

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

            device.BlendState = BlendState.AlphaBlend;
            matrixStack.Push(theCrane.Draw(gameTime, theCamera, world));
            matrixStack.Push(theHook.Draw(gameTime, theCamera, matrixStack.Peek(), theCrane.Rotation));

            matrixStack.Pop();
            matrixStack.Pop();
            
            
            base.Draw(gameTime);
        }
    }
}
