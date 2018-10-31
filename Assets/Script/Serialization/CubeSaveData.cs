using System;
using UnityEngine;

namespace Serialization
{
    [Serializable]
    public class CubeSaveData
    {
        public int primitiveType = (int)PrimitiveType.Cube;
        public bool isSelected = false;
        public bool isHovered = false;
        public string ID = Guid.Empty.ToString();
        public bool showConnectors = false;
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }
}
