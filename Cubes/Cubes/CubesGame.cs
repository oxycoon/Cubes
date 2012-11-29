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
        private List<Cube> theCubeList = new List<Cube>();
        private InputHandler input;
        private Camera theCamera;
        private SkyDome theSky;
        private Building theCity;


        private Matrix world, view, projection;
        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        private KeyboardState oldState;
   

        //BasicEffect effect;
        Effect effect;

        private bool isFullScreen = false;
        //private bool activatedMagnet = false;

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
            theCubeList.Add(theCube);

            theCity = new Building(this);
            this.Components.Add(theCity);
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

            theCity.Effect = effect;
            theCity.Device = device;
            theCity.initElements();
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

            if (input.KeyboardState.IsKeyDown(Keys.T) && input.KeyboardState != oldState)
            {
                Random rnd = new Random();
                Cube tmp = new Cube(this, new Vector3(rnd.Next(-70, 70), rnd.Next(-70, 70), rnd.Next(-70, 70)));
                tmp.Model = Content.Load<Model>("Models\\testCube2");
                this.Components.Add(tmp);
                theCubeList.Add(tmp);
       
            }

            foreach (Cube c1 in theCubeList)
            {
                if (theHook.Active && !c1.Hooked && !theHook.HasBlock)
                {
                    c1.Hooked = theHook.HasBlock = checkHookCubeCollision(c1);
                }
                else if (!theHook.Active)
                {
                    c1.Hooked = false;
                }

                foreach (Cube c2 in theCubeList)
                {
                    if (!c1.Equals(c2))
                    {
                        if (checkCubeCollision(c1, c2))
                        {
                            c1.Move = false;
                            break;
                        }
                        else
                        {
                            c1.Move = true;
                        }
                    }
                }
            }

            oldState = input.KeyboardState;
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
            theCity.Draw(world);

            device.BlendState = BlendState.AlphaBlend;
            matrixStack.Push(theCrane.Draw(gameTime, theCamera, world));
            matrixStack.Push(theHook.Draw(gameTime, theCamera, matrixStack.Peek(), theCrane.Rotation));

            foreach (Cube c in theCubeList)
            {
                c.Draw(matrixStack.Peek(), theCamera, theCrane.Rotation);
            }


            matrixStack.Pop();
            matrixStack.Pop();

            #region SpriteBatch
            //spriteBatch.Begin();
            //spriteBatch.DrawString(spriteFont, theCamera.CamZoom.ToString(), new Vector2(20.0f, 20.0f), Color.Black);
            //spriteBatch.End();
            #endregion
            base.Draw(gameTime);
        }

        private bool checkCubeCollision(Cube cube1, Cube cube2)
        {
            BoundingBox cube1Sphere = TransformBoundingBox((BoundingSphere)cube1.Model.Tag, cube1.World);
            BoundingBox cube2Sphere = TransformBoundingBox((BoundingSphere)cube2.Model.Tag, cube2.World);

            if (cube1Sphere.Intersects(cube2Sphere))
                return true;
            else
                return false;
        }

        private bool checkHookCubeCollision(Cube cube)
        {
            BoundingSphere hookSphere = TransformBoundingSphere((BoundingSphere)theHook.Model.Tag, theHook.World);
            BoundingBox cubeSphere = TransformBoundingBox((BoundingSphere)cube.Model.Tag, cube.World);

            if (hookSphere.Intersects(cubeSphere))
                return true;
            else
                return false;
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

        private static BoundingBox TransformBoundingBox(BoundingSphere originalBoundingSphere, Matrix transformationMatrix)
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

            return BoundingBox.CreateFromSphere(transformedBoundingSphere);
        }
    }
}
