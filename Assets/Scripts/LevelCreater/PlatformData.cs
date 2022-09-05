using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformData 
{
    public int Index;
    public int EndScore;
    public Vector3 Position;

    public List<PlatformObjectData> ObjectsData;
    public List<CustomObjectData> HelicopterData;
}