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

        //Randomness 
        public Random rnd { get; protected set; }

        //Explosion stuff
        List<ParticleExplosion> explosions = new List<ParticleExplosion>();
        ParticleExplosionSettings particleExplosionSettings =
            new ParticleExplosionSettings();
        ParticleSettings particleSettings = new ParticleSettings();

        //public Effect explosionEffect { get; set; }
        public Texture2D explosionTexture { get; set; }
        public GraphicsDevice graphicsDevice { get; set; }

        public SmokeEffect(Game game)
            : base(game)
        {
            rnd = new Random();
        }


        //protected override void LoadContent()
        //{
        //    // Load explosion stuff
        //    explosionEffect.CurrentTechnique = explosionEffect.Techniques["Technique1"];
        //    explosionEffect.Parameters["theTexture"].SetValue(explosionTexture);
        //}

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
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.B)) //Bang!! 
                this.addExplotion();
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

        private void addExplotion()
        {
            explosions.Add(new ParticleExplosion(graphicsDevice,
               new Vector3(0.0f, 0.0f, 0.0f),
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

        public void Draw(Effect effect, Camera camera, GraphicsDevice device)
        {
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xEnableLightingTexture"].SetValue(true);
            effect.Parameters["xDiffuseLight"].SetValue(LightSettings.DiffuseLight);
            effect.Parameters["xDiffuseMaterial"].SetValue(LightSettings.DiffuseMaterial);
            effect.Parameters["xAmbientLight"].SetValue(LightSettings.AmbientLight);
            effect.Parameters["xAmbientMaterial"].SetValue(LightSettings.AmbientMaterial);
            effect.Parameters["xLightDirection"].SetValue(LightSettings.LightDirection);
            effect.Parameters["xTexture"].SetValue(explosionTexture);

            effect.CurrentTechnique = effect.Techniques["PhongTexturedShader"];
            foreach (ParticleExplosion explosion in explosions)
            {
                explosion.Draw(effect, camera, device);
            }
        }
    }
}
