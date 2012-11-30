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
    /// Kamera komponenten til spillet. Tar for seg alt som har med kameraet å gjøre
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private GraphicsDeviceManager graphics;
        private GraphicsDevice device;
        private IInputHandler input;

        private Matrix view, projection;

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

        public bool LockedCamera
        {
            get { return lockedCamera; }
            set { lockedCamera = value; }
        }
        private Game game;

        private KeyboardState oldState;
        private int oldMouseScroll;

        #region Get/Set methods
        public float CamZoom
        {
            get { return camZoom; }
        }

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
            graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            device = (GraphicsDevice)Game.Services.GetService(typeof(GraphicsDevice));
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));

            this.game = game;
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
            oldMouseScroll = Mouse.GetState().ScrollWheelValue;

            base.Initialize();
        }

        #region Initialize methods
        /// <summary>
        /// Initialiserer kameraet.
        /// </summary>
        private void InitializeCam()
        {
            float aspecRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspecRatio, 1.0f, 5000.0f, out projection);
            Matrix.CreateLookAt(ref camPos, ref camTar, ref camUp, out view);
        }
        #endregion

        /// <summary>
        /// Oppdaterer kameraet gitt forskjellig input fra keyboard og mus.
        /// 
        /// Rotering av kameraet i x,y,z, samt zoom.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouse = Mouse.GetState();

            //Alt som har med lesing av mus å gjøre
            #region Mouse logic
            if (oldMouseScroll < mouse.ScrollWheelValue)
                camZoom -= 0.05f;
            if (oldMouseScroll > mouse.ScrollWheelValue)
                camZoom += 0.05f;

            oldMouseScroll = mouse.ScrollWheelValue; // Setter oldMouseScroll, gjøres for å 
                                                     //kunne zoome kameraet uavhengig av ScrollWheelValue.

            //Sjekker om spilleren holder inne venstre musetast og om spillet er det aktive vinduet.
            if (ButtonState.Pressed.Equals(mouse.LeftButton) && Game.IsActive)
            {
                Mouse.SetPosition(mouseLockedX, mouseLockedY); // Låser musens posisjon til midten av spillet.
                game.IsMouseVisible = false; // Skjuler musen

                //Snur kameraet mot høyre
                if (mouse.X > mouseX + spinrate)
                {
                    yaw += spinrate;
                }

                //Snur kameraet mot venstre
                if (mouse.X < mouseX - spinrate)
                {
                    yaw -= spinrate;
                }

                //Snur kameraet ned
                if (mouse.Y > mouseY - spinrate)
                {
                    pitch -= spinrate;
                }

                //Snur kameraet opp
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
            //Alt som har med lesing av keyboard å gjøre
            #region Keyboard input logic
            //Roterer kameraet med krana dersom kameraet er låst
            if (input.KeyboardState.IsKeyDown(Keys.Right) || input.KeyboardState.IsKeyDown(Keys.D) && lockedCamera)
            {
                Rotate(MathHelper.ToDegrees(-0.03f));
            }

            //Roterer kameraet med krana dersom kameraet er låst
            if (input.KeyboardState.IsKeyDown(Keys.Left) || input.KeyboardState.IsKeyDown(Keys.A) && lockedCamera)
            {
                Rotate(MathHelper.ToDegrees(0.03f));
            }

            //Skrur av/på kameralås.
            if (checkKeyState(Keys.C)) 
            {
                lockedCamera = !lockedCamera;
            }

            #endregion
            //Sjekker alle begrensninger knyttet til kameraposisjon.
            #region Rotation limits
            //Sjekker om kameraet har overskredet 360 grader for å holde yawen mellom 0 og 360
            if (yaw > 360)
            {
                yaw -= 359;
            }
            else if (yaw < 0)
            {
                yaw += 359;
            }

            //Sjekker om kameraet er for høyt/lavt, forhindrer kameraet å bevege seg mer oppover/nedover
            if (pitch > 170)
            {
                pitch = 170;
            }
            else if (pitch < 100)
            {
                pitch = 100;
            }

            // Sjekker om zoomen på kameraet er for stor/liten
            if (camZoom > 1.50f)
                camZoom = 1.50f;
            else if (camZoom < 0.50f)
                camZoom = 0.50f;
            #endregion

            Matrix rotMat = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), 1.0f);
            Vector3 tempCamRef = Vector3.Multiply(camRef, camZoom);

            Vector3 transRef;
            Vector3.Transform(ref tempCamRef, ref rotMat, out transRef);

            camPos = transRef;

            Matrix.CreateLookAt(ref camPos, ref camTar, ref camUp, out view);

            oldState = input.KeyboardState; //Oppdaterer oldState.
            base.Update(gameTime);
        }

        //Roterer kameratet rundt Y-aksen gitt grader.
        private void Rotate(float degrees)
        {
            yaw += degrees;
        }

        /// <summary>
        /// Sørger for at spilleren ikke kan holde inne en knapp for å
        /// gjøre en handling gjentatte ganger hver gang spillet oppdateres.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool checkKeyState(Keys key)
        {
            if (input.KeyboardState.IsKeyUp(key) && oldState.IsKeyDown(key))
                return true;
            return false;
        }
    }
}
