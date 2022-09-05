using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class PlatformData
    {
        public int Index;
        public int EndScore;
        public Vector3 Position;

        public List<PlatformObjectData> ObjectsData;
        public List<CustomObjectData> HelicopterData;
    }
}