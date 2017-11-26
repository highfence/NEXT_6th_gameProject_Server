using System;
using System.Collections.Generic;
using System.Text;

namespace LogicLibrary
{
    public class Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3(float xVal, float yVal, float zVal)
        {
            x = xVal;

            y = yVal;

            z = zVal;
        }

        public static Vector3 operator +(Vector3 c1, Vector3 c2)
        {
            return new Vector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }

    }
}
