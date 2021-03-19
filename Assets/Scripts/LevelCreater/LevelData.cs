using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level Data", order = 51)]
public class LevelData : ScriptableObject
{
    public int Index;
    public List<PlatformData> Platforms;
    public List<PlatformObjectData> ObjectsData;
}