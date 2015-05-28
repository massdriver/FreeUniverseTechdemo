using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace FreeUniverse
{
    public class OpenGL
    {
        [DllImport("opengl32.dll")]
        public static extern void glEnable(uint mode);

        [DllImport("opengl32.dll")]
        public static extern void glDisable(uint mode);

        [DllImport("opengl32.dll")]
        public static extern void glHint(uint target, uint mode);

        public static uint GL_LINE_SMOOTH = 0x0B20;
        public static uint GL_LINE_SMOOTH_HINT = 0x0C52;
        public static uint GL_NICEST = 0x1102;
    }
}
