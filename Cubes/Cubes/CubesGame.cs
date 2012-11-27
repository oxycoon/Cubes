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
            theCrane.Model = Content.Load<Model>("Crane");
            theHook.Model = Content.Load<Model>("Hook");
            theHook.WireModel = Content.Load<Model>("Wire");
            theTerrain.TerrTex = Content.Load<Texture2D>("MC_Dirt");

            effect = Content.Load<Effect>("MyEffect");
            
            theSky.Model = Content.Load<Model>("dome");
            theSky.Texture = Content.Load<Texture2D>("clouds2");
            theSky.Device = device;
            theSky.Effect = effect;

            //theTerrain.TerrTex = Content.Load<Texture2D>("Dirt");



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

            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xProjection"].SetValue(projection);
            effect.Parameters["xView"].SetValue(view);

            theTerrain.Draw(gameTime, effect, device);
            theSky.Draw(view, projection);

            matrixStack.Push(theCrane.Draw(gameTime, theCamera, world));
            matrixStack.Push(theHook.Draw(gameTime, theCamera, matrixStack.Peek(), theCrane.Rotation));

            matrixStack.Pop();
            matrixStack.Pop();
            
            
            base.Draw(gameTime);
        }
    }
}
