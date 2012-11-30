using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Cubes
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SmokeEffect : Microsoft.Xna.Framework.GameComponent
    {
        private IInputHandler input;

        //Randomness 
        public Random rnd { get; protected set; }

        //Explosion stuff
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings =
            new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();

        public Effect explosionEffect { get; set; }
        public Texture2D explosionTexture { get; set; }
        public GraphicsDevice graphicsDevice { get; set; }

        public SmokeEffect(Game game)
            : base(game)
        {
            rnd = new Random();
            input = (IInputHandler)Game.Services.GetService(typeof(IInputHandler));
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update explosions 
            UpdateExplosions(gameTime);
            base.Update(gameTime);
        }

        protected void UpdateExplosions(GameTime gameTime)
        {
            if (input.KeyboardState.IsKeyDown(Keys.B)) //Bang!! 
                this.addExplotion(new Vector3(100.0f, 100.0f, 0.0f));
            // Loop through and update explosions 
            for (int i = 0; i < explosions.Count; ++i)
            {
                explosions[i].Update(gameTime);
                // If explosion is finished, remove it 
                if (explosions[i].IsDead)
                {
                    explosions.RemoveAt(i);
                    --i;
                }
            }
        }

        public void addExplotion(Vector3 position)
        {
            explosions.Add(new ParticleExplosion(graphicsDevice,
               position,
                      (rnd.Next(
                   particleExplosionSettings.minLife,
                   particleExplosionSettings.maxLife)),
               (rnd.Next(
                   particleExplosionSettings.minRoundTime,
                   particleExplosionSettings.maxRoundTime)),
               (rnd.Next(
                   particleExplosionSettings.minParticlesPerRound,
                   particleExplosionSettings.maxParticlesPerRound)),
               (rnd.Next(
                   particleExplosionSettings.minParticles,
                   particleExplosionSettings.maxParticles)),
               new Vector2(explosionTexture.Width,
                   explosionTexture.Height),
               particleSettings));
        }

        public void Draw(Camera camera, GraphicsDevice device)
        {
            explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
            explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);

            foreach (ParticleExplosion explosion in explosions)
            {
                explosion.Draw(explosionEffect, camera, device);
            }
        }
    }
}
