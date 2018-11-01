using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Serialization
{
    [Serializable]
    public class CameraSaveData
    {
        public Vector3 position = new Vector3();
        public Quaternion rotation = new Quaternion();
    }
}
