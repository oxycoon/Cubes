using System;

namespace Cubes
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CubesGame game = new CubesGame())
            {
                game.Run();
            }
        }
    }
#endif
}

