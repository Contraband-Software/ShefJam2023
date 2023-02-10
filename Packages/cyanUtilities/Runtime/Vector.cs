using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyan
{
    public static class Vector
    {
        public static Vector2 Mult_CWise(Vector2 vec1, Vector2 vec2)
        {
            return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y);
        }
        public static Vector3 Mult_CWise(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
        }
    }
}