using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FurrySharp
{
    public static class GameTimes
    {
        private const int MaxSamples = 100;
        private static readonly Queue<float> FpsQueue;

        public static float FPS { get; private set; }
        public static float TimeScale { get; set; }
        public static float TrueDeltaTime { get; private set; }

        public static float DeltaTime
        {
            get { return TrueDeltaTime * TimeScale; }
        }

        static GameTimes()
        {
            FpsQueue = new Queue<float>(MaxSamples);

            TimeScale = 1;
        }

        public static void UpdateTimes(GameTime gameTime)
        {
            TrueDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static void UpdateFPS(GameTime gameTime)
        {
            if (FpsQueue.Count > MaxSamples)
            {
                FpsQueue.Dequeue();
                FPS = FpsQueue.Average(i => i);
            }

            FpsQueue.Enqueue(1f / (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}