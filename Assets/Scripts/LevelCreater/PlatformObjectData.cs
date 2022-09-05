using System;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Data
{
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
        public PresetData PresetData;
        public ObjectType ObjectType;
        public Vector3 Position;
        public Vector3 Rotation;
    }
}