using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Core
{
    public class Trace
    {
        private static int traceStep = 0;

        public const bool RESET = true;
        public const bool NORESET = false;

        public static void Step(bool reset)
        {
            if (reset)
                traceStep = 0;

            Debug.Log("trace step=" + traceStep);

            traceStep++;
        }
    }
}
