using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Cubes
{
    class LightSettings
    {
        public static Vector3 AmbientLight = new Vector3(1.0f, 1.0f, 1.0f);
        public static Vector3 AmbientMaterial = new Vector3(0.7f, 0.7f, 0.7f);

        public static Vector3 DiffuseLight = new Vector3(1.0f, 1.0f, 1.0f);
        public static Vector3 DiffuseMaterial = new Vector3(0.4f, 0.7f, 0.6f);
        public static Vector3 LightDirection = new Vector3(1.0f, -1.0f, -1.0f);
    }
}
