using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Helpers
{
    public static class Primitives
    {
        public static  Mesh cubeMesh = GetMeshForPrimitive(PrimitiveType.Cube);
        public static  Mesh cylinderMesh = GetMeshForPrimitive(PrimitiveType.Cylinder);
        public static  Mesh capsuleMesh = GetMeshForPrimitive(PrimitiveType.Capsule);
        public static  Mesh sphereMesh = GetMeshForPrimitive(PrimitiveType.Sphere);

        public static Mesh GetMeshForPrimitive(PrimitiveType type)
        {
            GameObject primitiveObject = GameObject.CreatePrimitive(type);
            Mesh mesh = primitiveObject.GetComponent<MeshFilter>().sharedMesh;
            GameObject.Destroy(primitiveObject);
            return UnityEngine.Object.Instantiate(mesh);
        }
    }
}
