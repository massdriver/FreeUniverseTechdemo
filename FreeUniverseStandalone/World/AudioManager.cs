using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.World
{
    public class AudioManager
    {
        private class LayerSoundContainer
        {
            public int layer { get; set; }

            public LayerSoundContainer(int layer)
            {
                this.layer = layer;
            }

            public void Activate()
            {

            }

            public void Deactivate()
            {

            }

            public AudioSource CreateAudioSource(int layer, string assetPath)
            {
                return null;
            }
        }


        private static int activeLayer { get; set; }
        private const int MAX_LAYERS = 32;
        private static LayerSoundContainer[] layerContainers { get; set; }

        public static void Init()
        {
            activeLayer = -1;
            layerContainers = new LayerSoundContainer[MAX_LAYERS];

            for( int i = 0; i < layerContainers.Length;i++)
                layerContainers[i] = new LayerSoundContainer(i);
        }

        public static void SwitchToLayer(int layer)
        {
            if (activeLayer == layer)
                return;

            layerContainers[activeLayer].Deactivate();
            layerContainers[layer].Activate();
            activeLayer = layer;
        }

        public static AudioSource CreateAudioSource(int layer, string assetPath)
        {
            return layerContainers[layer].CreateAudioSource(layer,assetPath);
        }
    }
}
