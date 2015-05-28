using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;
using FreeUniverse.Core;

namespace FreeUniverse
{
    public class Approximate
    {
        public static float FastSqrt(float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp -= 1 << 23; /* Subtract 2^m. */
            u.tmp >>= 1; /* Divide by 2. */
            u.tmp += 1 << 29; /* Add ((b + 1) / 2) * 2^m. */
            return u.f;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatIntUnion
        {
            [FieldOffset(0)]
            public float f;

            [FieldOffset(0)]
            public int tmp;
        }
    }

    public class Helpers
    {
        private static System.Random randGen = new System.Random();

        public static uint RandUINT()
        {
            return (uint)randGen.Next();
        }

        public static bool IsValidAlphaNumeric(string str)
        {
            return true;
        }

        public static void SetLayerDeep(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform.root)
            {
                child.gameObject.layer = obj.layer;
            }
        }

        /*
         * for (var child : Transform in root)
    {
        if (child.collider)
        {
            Physics.IgnoreCollision(theCollider, child.collider);
            Debug.Log(child.name);
        }
        if (child.childCount > 0)
        {
            DisableAllColliders(child, theCollider);
        }
    }
         */

        public static Vector3 RandomVec3(Vector3 min, Vector3 max)
        {
            return new Vector3(
                UnityEngine.Random.Range(min.x, max.x),
                UnityEngine.Random.Range(min.y, max.y),
                UnityEngine.Random.Range(min.z, max.z));
        }

        public static uint Hash3D(Vector3 pos, uint size)
        {
            return ((uint)(pos.x / size) * 73856093) ^ ((uint)(pos.y / size) * 19349663) ^ ((uint)(pos.z / size) * 83492791);
        }

        public static float FastLength(Vector3 vec)
        {
            return Approximate.FastSqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        }

        public static Rect MakeRect( Vector2 center, float width, float height)
        {
            return Rect.MinMaxRect(center.x - width / 2.0f, center.y - height / 2.0f, center.x + width / 2.0f, center.y + height / 2.0f);
        }

        public static Rect MakeRectCentered(float width, float height)
        {
            return MakeRect(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), width, height);
        }

        public static GameObject LoadUnityObject(string path)
        {
            return (UnityEngine.GameObject)UnityEngine.Object.Instantiate((UnityEngine.GameObject)Resources.Load(path));
        }

        public static List<T> DeepCopyCloneableList<T>(List<T> src) where T : ICloneable
        {
            List<T> lst = new List<T>();

            foreach (T e in src)
                lst.Add((T)e.Clone());

            return lst;
        }

        public static Vector3 CopyVector3(Vector3 src)
        {
            return new Vector3(src.x, src.y, src.z);
        }

        public static Quaternion CopyQuaternion(Quaternion src)
        {
            return new Quaternion(src.x, src.y, src.z, src.w);
        }

        public static void SyncTransforms(Transform src, Transform dest)
        {
            src.transform.position = CopyVector3(dest.position);
            src.transform.rotation = CopyQuaternion(dest.rotation);
        }

        public static Quaternion GetRotationQuaternion(Transform tr)
        {
            Quaternion q = tr.rotation;
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static Vector3 CutToNearestIntegerValue(Vector3 vec, float factor)
        {
            return new Vector3(factor * (float)((int)(vec.x / factor)), factor * (float)((int)(vec.y / factor)), factor * (float)((int)(vec.z / factor)));
        }

        public static bool CompareAsIntegerVectors(Vector3 a, Vector3 b)
        {
            if (Vector3.Distance(a, b) < 1.0f)
                return true;

            return false;
        }

        public static Vector3 MakeAngularVelocity(Quaternion from, Quaternion to)
        {
            Quaternion conj = new Quaternion(-from.x, -from.y, -from.z, from.w);
            Quaternion dq = new Quaternion((to.x - from.x) * 2.0f, 2.0f * (to.y - from.y), 2.0f * (to.z - from.z), 2.0f * (to.w - from.w));
            Quaternion c = dq * conj;
            float dt = 1.0f;
            return new Vector3(c.x / dt, c.y / dt, c.z / dt);
        }

        public static void DrawTexture(Vector3 center, Texture tex)
        {
            GUI.DrawTexture( Helpers.MakeRect(center, tex.width, tex.height), tex);
        }

        public static Vector3 MakeLead(Vector3 launchPoint, Vector3 launchVelocity, Vector3 targetPos, Vector3 targetVelocity)
        {
            Vector3 V = targetVelocity;
            Vector3 D = targetPos - launchPoint;
            float A = V.sqrMagnitude - launchVelocity.sqrMagnitude;
            float B = 2 * Vector3.Dot(D, V);
            float C = D.sqrMagnitude;
            if (A >= 0)
            {
                return targetPos;
            }
            else
            {
                float rt = Mathf.Sqrt(B * B - 4 * A * C);
                float dt1 = (-B + rt) / (2 * A);
                float dt2 = (-B - rt) / (2 * A);
                float dt = (dt1 < 0 ? dt2 : dt1);
                return targetPos + V * dt;
            }
        }
    }
}
