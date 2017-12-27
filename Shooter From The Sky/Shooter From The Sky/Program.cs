using System;

namespace Shooter_From_The_Sky
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ShooterFromTheSky game = new ShooterFromTheSky())
            {
                game.Run();
            }
        }
    }
#endif
}

