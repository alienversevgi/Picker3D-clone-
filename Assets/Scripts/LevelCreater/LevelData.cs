using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data", order = 51)]
    public class LevelData : ScriptableObject
    {
        public List<PlatformData> Platforms;
    }
}