using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomObjectData
{
    public int PlatformIndex;
    public ObjectType ObjectType;
    public Vector3 Position;
    [SerializeField] public PresetData PresetData;
    public List<PlatformObjectData> ObjectData;
    public ObjectSpawnType objectSpawnType;
    public float ObjectSpawnTime;
    public bool IsMoveableObject;
    public float Duration;

    public List<Vector3> CurvePoints;
}