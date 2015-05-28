using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LibNoise;

namespace FreeUniverse.Core
{
    public class Noise
    {
        const int NOISE_GENERATOR_SEED = 5677;
        static IModule _noiseModule = new LibNoise.FastNoise(NOISE_GENERATOR_SEED);

        public static float Noise3D(Vector3 position)
        {
            return (float)_noiseModule.GetValue(position.x, position.y, position.z);
        }
    }
}
