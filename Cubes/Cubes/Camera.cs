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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private IInputHandler input;

        private Matrix view, projection;

        //private Vector3 camPos = new Vector3(2.0f, 20.0f, 4.0f);
        //private Vector3 camTar = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 camPos = new Vector3(400.0f, 50.0f, 0.0f);
        private Vector3 camTar = new Vector3(0.0f, 50.0f, 0.0f);
        private Vector3 camUp = Vector3.Up;
        private Vector3 camRef = new Vector3(0.0f, 0.0f, -1.0f);
        

        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float camZoom = 1.0f;

        private float spinrate = 3.0f;

        private int mouseX, mouseY, mouseLockedX, mouseLockedY;
        private bool lockedCamera = true;
        private Game game;

        private KeyboardState oldState;

        #region Get/Set methods
        public Vector3 CamPos
        {
            get { return camPos; }
            set
            {
                camPos = value;
                camRef = ((-1.0f) * camPos);
                camRef.Normalize();
            }
        }

        public Vector3 CamTar
        {
            get { return camTar; }
            set { camTar = value; }
        }

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        #endregion

        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            device = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));

            this.game = game;
        }

        private void Rotate(float degrees)
        {
            yaw += degrees;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            this.InitializeCam();
            camRef = ((-1.0f) * camPos);

            mouseX = mouseLockedX = Game.Window.ClientBounds.Width / 2;
            mouseY = mouseLockedY = Game.Window.ClientBounds.Height / 2;

            Mouse.SetPosition(mouseX, mouseY);


            base.Initialize();
        }

        #region Initialize methods
        private void InitializeCam()
        {
            float aspecRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspecRatio, 1.0f, 5000.0f, out projection);
            Matrix.CreateLookAt(ref camPos, ref camTar, ref camUp, out view);
        }
        #endregion

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouse = Mouse.GetState();

            #region Mouse logic
            camZoom = 1.0f - (0.001f * mouse.ScrollWheelValue);
            if (ButtonState.Pressed.Equals(mouse.LeftButton) && Game.IsActive)
            {
                Mouse.SetPosition(mouseLockedX, mouseLockedY);
                game.IsMouseVisible = false;

                if (mouse.X > mouseX + spinrate)
                {
                    yaw += spinrate;
                }

                if (mouse.X < mouseX - spinrate)
                {
                    yaw -= spinrate;
                }

                if (mouse.Y > mouseY - spinrate)
                {
                    pitch -= spinrate;
                }
                if (mouse.Y < mouseY + spinrate)
                {
                    pitch += spinrate;
                }
            }
            else
            {
                game.IsMouseVisible = true;
            }
            #endregion
            #region Keyboard input logic
            if (input.KeyboardState.IsKeyDown(Keys.Right) || input.KeyboardState.IsKeyDown(Keys.D) && lockedCamera)
            {
                Rotate(MathHelper.ToDegrees(-0.03f));
            }
            if (input.KeyboardState.IsKeyDown(Keys.Left) || input.KeyboardState.IsKeyDown(Keys.A) && lockedCamera)
            {
                Rotate(MathHelper.ToDegrees(0.03f));
            }
            if (checkKeyState(Keys.C)) 
            {
                lockedCamera = !lockedCamera;
            }

            #endregion
            #region Rotation limits
            if (yaw > 360)
            {
                yaw -= 359;
            }
            else if (yaw < 0)
            {
                yaw += 359;
            }

            if (pitch > 170)
            {
                pitch = 170;
            }
            else if (pitch < 100)
            {
                pitch = 100;
            }
            #endregion

            Matrix rotMat = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), 1.0f);
            Vector3 tempCamRef = Vector3.Multiply(camRef, camZoom);

            Vector3 transRef;
            Vector3.Transform(ref tempCamRef, ref rotMat, out transRef);

            camPos = transRef;

            Matrix.CreateLookAt(ref camPos, ref camTar, ref camUp, out view);

            oldState = input.KeyboardState;
            base.Update(gameTime);
        }

        private bool checkKeyState(Keys key)
        {
            if (input.KeyboardState.IsKeyUp(key) && oldState.IsKeyDown(key))
                return true;
            return false;
        }
    }
}
