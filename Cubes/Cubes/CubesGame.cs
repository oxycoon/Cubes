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
        #region Variables
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        private Crane theCrane;
        private Hook theHook;
        private Terrain theTerrain;
        private List<Cube> theCubeList = new List<Cube>();
        public static SmokeEffect theSmokeEffect;
        private InputHandler input;
        private Camera theCamera;
        private SkyDome theSky;
        private Building theCity;

        private Texture2D boxIcon;
        private Texture2D magnetIcon;
        private Texture2D camLock;


        private Matrix world, view, projection;
        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        private KeyboardState oldState;

        private SpriteBatch hudDrawer;
        private SpriteFont hudFont;
        private DepthStencilState dsState;
   

        //BasicEffect effect;
        Effect effect;

        private bool isFullScreen = false;
        //private bool activatedMagnet = false;
        #endregion

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

            theCity = new Building(this);
            this.Components.Add(theCity);

            theSmokeEffect = new SmokeEffect(this);
            this.Components.Add(theSmokeEffect);

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
            #region Kran
            theCrane.Model = LoadModel("Models\\Crane");
            theCrane.CraneTexture = Content.Load<Texture2D>("Textures\\Texture_Crane");
            theCrane.WeightTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.BaseTexture = Content.Load<Texture2D>("Textures\\Concrete");
            theCrane.WireTexture = Content.Load<Texture2D>("Textures\\SupportWire");
            theCrane.Effect = effect;
            #endregion

            #region Magnet
            theHook.Model = LoadModel("Models\\Hook");
            theHook.WireModel = Content.Load<Model>("Models\\Wire");
            theHook.Effect = effect;
            theHook.MagnetTexture = Content.Load<Texture2D>("Textures\\pic2");
            theHook.WireTexture = Content.Load<Texture2D>("Textures\\Cable");
            #endregion

            #region Skydome
            theSky.Model = Content.Load<Model>("Models\\dome");
            theSky.Texture = Content.Load<Texture2D>("Textures\\clouds2");
            theSky.Device = device;
            theSky.Effect = effect;
            #endregion

            #region Terreng
            theTerrain.Texture = Content.Load<Texture2D>("Textures\\MC_Dirt");
            #endregion

            #region By
            theCity.Texture = Content.Load<Texture2D>("Textures\\texturemap");
            theCity.Effect = effect;
            theCity.Device = device;
            theCity.initElements();
            #endregion

            #region HUD-elementer
            hudFont = Content.Load<SpriteFont>("font");
            hudDrawer = new SpriteBatch( graphics.GraphicsDevice );
            #endregion

            #region Load sprites
            boxIcon = Content.Load<Texture2D>("Sprites\\boxCarry");
            magnetIcon = Content.Load<Texture2D>("Sprites\\magnetPower");
            camLock = Content.Load<Texture2D>("Sprites\\camLock");
            #endregion

            #region Partikkeleffect
            theSmokeEffect.graphicsDevice = graphics.GraphicsDevice;
            theSmokeEffect.explosionEffect = Content.Load<Effect>("Particle"); ;
            theSmokeEffect.explosionTexture = Content.Load<Texture2D>("Textures\\Dirt");
            #endregion
        }

        /// <summary>
        /// Laster inn modellen mens den setter alle meshenes effekt til spillets
        /// effect.
        /// </summary>
        /// <param name="name">String til resursen</param>
        /// <returns>Ferdige modellen</returns>
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
            //Oppretter kuber i et tilfeldig sted
            if (input.KeyboardState.IsKeyDown(Keys.T) && input.KeyboardState != oldState)
            {
                Random rnd = new Random();
                Cube tmp = new Cube(this, new Vector3(rnd.Next(-70, 70), rnd.Next(-70, 70), rnd.Next(-70, 70)), effect);
                //Cube tmp = new Cube(this, new Vector3(50, 300, 0), effect);
                tmp.Texture = Content.Load<Texture2D>("Textures\\box_d");
                tmp.Model = LoadModel("Models\\Cube2");
                this.Components.Add(tmp);
                theCubeList.Add(tmp);

            }

            #region Kubenes kollisjonsdeteksjonalgoritme
            foreach (Cube c1 in theCubeList)
            {
                List<Cube> collideList = new List<Cube>();
                float lowY = c1.World.Translation.Y;
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
                            Vector3 c1Pos = c1.World.Translation;
                            Vector3 c2Pos = c2.World.Translation;
                            if (c1Pos.Y < c2Pos.Y && !c1.Hooked && !c2.Hooked)
                            {
                                collideList.Add(c2);
                                foreach (Cube cLow in collideList)
                                {
                                    if (cLow.World.Translation.Y < lowY)
                                    {
                                        lowY = cLow.World.Translation.Y;
                                    }
                                }
                                if (c1.World.Translation.Y > lowY)
                                {
                                    c1.Move = true;
                                    break;
                                }
                            }
                            else
                            {
                                c1.Move = false;
                                c1.FallSpeed = 0;
                                break;
                            }
                        }
                        else
                        {
                            c1.Move = true;
                        }
                    }
                }
            }
            #endregion

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
            theCity.Draw();
            theSky.Draw(view, projection);
            theSmokeEffect.Draw(theCamera, device);

            device.BlendState = BlendState.AlphaBlend;
            matrixStack.Push(theCrane.Draw(gameTime, theCamera, world));
            matrixStack.Push(theHook.Draw(gameTime, theCamera, matrixStack.Peek(), theCrane.Rotation));

            foreach (Cube c in theCubeList)
            {
                c.Draw(matrixStack.Peek(), theCamera);
            }


            matrixStack.Pop();
            matrixStack.Pop();

            #region SpriteBatch
            dsState = new DepthStencilState();
            dsState.DepthBufferWriteEnable = false;
            device.DepthStencilState = dsState;

            hudDrawer.Begin();
            String output = "Blocks on map: " + theCubeList.Count;
            hudDrawer.DrawString(hudFont, output, new Vector2(20, 20), Color.White);
            if (theHook.HasBlock)
            {
                hudDrawer.Draw(boxIcon, new Rectangle((device.Viewport.Width / 2) - 50, device.Viewport.Height - 50, 32, 32), new Rectangle(0, 32, 32, 32), Color.White);
            }
            else
            {
                hudDrawer.Draw(boxIcon, new Rectangle((device.Viewport.Width / 2) - 50, device.Viewport.Height - 50, 32, 32), new Rectangle(0, 0, 32, 32), Color.White);
            }
            if (theHook.Active)
            {
                hudDrawer.Draw(magnetIcon, new Rectangle((device.Viewport.Width / 2) + 50, device.Viewport.Height - 50, 32, 32), new Rectangle(0, 0, 32, 32), Color.White);
            }
            else
            {
                hudDrawer.Draw(magnetIcon, new Rectangle((device.Viewport.Width / 2) + 50, device.Viewport.Height - 50, 32, 32), new Rectangle(0, 32, 32, 32), Color.White);
            }

            if(theCamera.LockedCamera)
            {
                hudDrawer.Draw(camLock, new Rectangle((device.Viewport.Width / 2), device.Viewport.Height - 50, 32, 32), new Rectangle(0, 0, 32, 32), Color.White);
            }
            else
            {
                hudDrawer.Draw(camLock, new Rectangle((device.Viewport.Width / 2), device.Viewport.Height - 50, 32, 32), new Rectangle(0, 32, 32, 32), Color.White);
            }
            hudDrawer.End();

            dsState = new DepthStencilState();
            dsState.DepthBufferWriteEnable = true;
            device.DepthStencilState = dsState;
            #endregion
            base.Draw(gameTime);
        }

        /// <summary>
        /// checkCubeCollision check wheter the cubes collide.
        /// True: the cubes collide
        /// False: the cubes do not collide.
        /// </summary>
        /// <param name="cube1"></param>
        /// <param name="cube2"></param>
        /// <returns></returns>
        private bool checkCubeCollision(Cube cube1, Cube cube2)
        {
            BoundingBox cube1Sphere = TransformBoundingBox((BoundingSphere)cube1.Model.Tag, cube1.World);
            BoundingBox cube2Sphere = TransformBoundingBox((BoundingSphere)cube2.Model.Tag, cube2.World);

            if (cube1Sphere.Intersects(cube2Sphere))
                return true;
            else
                return false;
        }

        /// <summary>
        /// checkHookCubeCollision checks wheter the magnet collides with a cube.
        /// True: the magnet collides with a cube
        /// False: the manget does not collide with a cube
        /// </summary>
        /// <param name="cube"></param>
        /// <returns></returns>
        private bool checkHookCubeCollision(Cube cube)
        {
            BoundingSphere hookSphere = TransformBoundingSphere((BoundingSphere)theHook.Model.Tag, theHook.World);
            BoundingBox cubeSphere = TransformBoundingBox((BoundingSphere)cube.Model.Tag, cube.World);

            if (hookSphere.Intersects(cubeSphere))
                return true;
            else
                return false;
        }


        /// <summary>
        ///     Denne metoden er hentet fra undervisnings dokument på it's learning.
        /// </summary>
        /// <param name="originalBoundingSphere"></param>
        /// <param name="transformationMatrix"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Denne metoden er hentet fra undervisnings dokument på it's learning.
        /// </summary>
        /// <param name="originalBoundingSphere"></param>
        /// <param name="transformationMatrix"></param>
        /// <returns></returns>
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
