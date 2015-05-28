using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Core
{
    public class ResourceTypeManager<T> where T : UnityEngine.Object
    {
        private class ResourceEntry
        {
            public int refCount { get; set; }
            public T obj { get; set; }

            public ResourceEntry(T obj)
            {
                refCount = 1;
                this.obj = obj;
            }

            public void Unload()
            {
                UnityEngine.Object.Destroy(obj);
                obj = null;
            }
        }

        private Dictionary<uint, ResourceEntry> resources { get; set; }

        public ResourceTypeManager()
        {
            resources = new Dictionary<uint, ResourceTypeManager<T>.ResourceEntry>();
        }

        public T LoadByAssetPath(string assetPath)
        {
            uint key = HashUtils.StringToUINT32(assetPath);

            ResourceEntry e = null;
            resources.TryGetValue(key, out e);

            if (e != null)
            {
                e.refCount++;
                return e.obj;
            }

            T obj = Resources.Load(assetPath) as T;

            if (obj == null) return null;

            resources.Add(key,new ResourceEntry(obj));

            return obj;
        }

        public void ReleaseByAssetPath(string assetPath)
        {
            uint key = HashUtils.StringToUINT32(assetPath);

            ResourceEntry e = null;
            resources.TryGetValue(key, out e);

            if (e == null) return;

            e.refCount--;
        }

        private const float UNLOAD_INTERVAL = 10.0f;
        private float unloadTimer { get; set; }

        public void Update(float time)
        {
            unloadTimer += time;

            if (unloadTimer < UNLOAD_INTERVAL) return;

            RemoveUnused();

            unloadTimer = 0.0f;
        }

        void RemoveUnused()
        {
            List<uint> removeKeys = new List<uint>();

            foreach (KeyValuePair<uint, ResourceEntry> e in resources)
            {
                if (e.Value.refCount <= 0) removeKeys.Add(e.Key);
            }

            foreach (uint k in removeKeys)
            {
                resources[k].Unload();
                resources.Remove(k);
            }
        }
    }

    public class ResourceManager
    {
        private static ResourceTypeManager<AudioClip> audioClips { get; set; }

        public static void Init()
        {
            audioClips = new ResourceTypeManager<AudioClip>();
        }

        public static void Update(float time)
        {
            audioClips.Update(time);
        }

        public static AudioClip LoadAudioClip(string assetPath)
        {
            return audioClips.LoadByAssetPath(assetPath);
        }

        public static void ReleaseAudioClip(string assetPath)
        {
            // MH: Unity is not pleased with automation of resources since looks like it handles this itself
            //audioClips.ReleaseByAssetPath(assetPath);
        }
    }
}
