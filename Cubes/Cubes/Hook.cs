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
    public class Hook : Microsoft.Xna.Framework.GameComponent
    {
        private Model theHookModel;
        private Model theWireModel;
        private IInputHandler input;

        //private Vector3 position;

        private float xPos, yPos;

        public Model Model
        {
            get { return theHookModel; }
            set { theHookModel = value; }
        }

        public Model WireModel
        {
            get { return theWireModel; }
            set { theWireModel = value; }
        }

        //private double altitude;
        private bool isHooked;

        //private Matrix world, view, projection;

        public Hook(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (input.KeyboardState.IsKeyDown(Keys.W))
            {
                xPos -= 3.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.S))
            {
                xPos += 3.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.R))
            {
                yPos -= 3.0f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.F))
            {
                yPos += 3.0f;
            }

            if (yPos > 75.0f)
                yPos = 75.0f;
            if (yPos < 0.0f)
                yPos = 0.0f;
            if (xPos > 90.0f)
                xPos = 90.0f;
            if (xPos < 10.0f)
                xPos = 10.0f;

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the crane.
        /// </summary>
        /// <param name="gametime">Provides a snapshot of timing values</param>
        /// <param name="camera">Camera to draw object in correct areas</param>
        /// <param name="world">World matrix for translations, if neccesary</param>
        /// <returns name="world">The generated world matrix</returns>
        public Matrix Draw(GameTime gametime, Camera camera, Matrix _world, float craneRotation)
        {
            Matrix matTrans, matScale, matOrbit, world;

            matScale = Matrix.CreateScale(20.0f);
            matTrans = Matrix.CreateTranslation(0.0f, 80.0f - yPos, 100.0f - xPos);
            matOrbit = matTrans * Matrix.CreateRotationY(craneRotation);

            //TODO: Fix ISROT.
            //isrot, identify, scale, rotation, orbit, translation
            //world = matTrans * matScale * _world;

            world = _world * matScale * matOrbit;

            //matWireScale = Matrix.CreateScale(new Vector3(10.0f, yPos, 10.0f));
            //matWireTrans = Matrix.CreateTranslation(0.0f, 400.0f - (yPos/2), (470.0f * 2) - (xPos * 2));
            //wireWorld = matWireScale * matWireTrans *_world;

            theHookModel.Draw(world, camera.View, camera.Projection);
            //theWireModel.Draw(wireWorld, camera.View, camera.Projection);


            return world;
        }
    }
}
