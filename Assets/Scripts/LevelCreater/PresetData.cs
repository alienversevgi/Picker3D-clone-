﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New PresetData", menuName = "Preset Data", order = 51)]
    public class PresetData : ScriptableObject
    {
        public List<Vector3> Positions;
    }
}