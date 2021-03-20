using System;
using UnityEngine;
using System.Collections.Generic;

public enum ObjectType
{
    Ball,
    Cube,
}

public enum PresetShapeType
{
    None,
    Rectangle,
    SlashLine,
    Line,
    Cross
}

[Serializable]
public class PlatformObjectData
{
    public int PlatformIndex;
    [SerializeField] public PresetData PresetData;
    public ObjectType ObjectType;
    public Vector3 Position;
}