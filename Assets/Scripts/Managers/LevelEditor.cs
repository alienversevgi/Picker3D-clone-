using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LevelEditor : EditorWindow
{
    public const string PREFAB_PATH = "Assets/Prefabs/";
    public static LevelData LevelData;

    public Transform LevelHolder;
    public Transform StartPlatform;
    public float SizePlatform;
    public int PlatformCount;

    public static List<Transform> platforms;
    public static Dictionary<int, List<Transform>> PlatformObjects;
    public static List<PresetData> Presets;

    public static int SelectedPlatformIndex;
    public static int EndScore;
    public static int SelectedPresetIndex;
    public static ObjectType SelectedObjectType;

    #region Prefabs

    public static Object PlatformPrefab;
    public static Object BallPrefab;
    public static Object CubePrefab;
    public static Object HelicopterPrefab;

    #endregion

    [MenuItem("LevelEditor/New Level")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelEditor));
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        LoadPrefabs();
        LevelData = new LevelData();
        LevelHolder = GameObject.Find("LevelHolder").transform;
        StartPlatform = GameObject.Find("StartPlatform").transform;
        Presets = Resources.LoadAll<PresetData>("Preset Data").ToList();
        PlatformCount = 0;
        platforms = new List<Transform>();
        PlatformObjects = new Dictionary<int, List<Transform>>();
        SizePlatform = Mathf.Abs(StartPlatform.transform.position.z) + 20;
        SelectedPlatformIndex = -1;
    }

    private static void LoadPrefabs()
    {
        PlatformPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Platform.prefab");
        BallPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Ball.prefab");
        CubePrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Cube.prefab");
        HelicopterPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Helicopter.prefab");
    }

    private void OnGUI()
    {
        LevelData = EditorGUILayout.ObjectField("Level Data : ", LevelData, typeof(LevelData), false) as LevelData;

        if (GUILayout.Button("Add Platform"))
        {
            AddPlatform();
        }

        if (GUILayout.Button("Remove Selected Platform"))
        {
            RemovePlatform();
        }

        if (GUILayout.Button("Add Object"))
        {
            AddObject();
        }

        SelectedObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type : ", SelectedObjectType);
        SelectedPresetIndex = EditorGUILayout.Popup("Preset : ", SelectedPresetIndex, Presets.Select(preset => preset.name).ToArray());

        GUILayout.Space(50);

        if (GUILayout.Button("Save Level"))
        {
            SaveData();
            CreateScriptableObject();
        }

        if (SelectedPlatformIndex != -1)
        {
            GUILayout.Label("Selected platform index : " + SelectedPlatformIndex);
            LevelData.Platforms[SelectedPlatformIndex].EndScore = EditorGUILayout.IntField("End Score : ", LevelData.Platforms[SelectedPlatformIndex].EndScore);
        }
    }

    private void CreateScriptableObject()
    {
        string path = "Assets/Resources/NewLevel.asset";
        AssetDatabase.CreateAsset(LevelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = LevelData;
    }

    private void RemovePlatform()
    {
        if (SelectedPlatformIndex == -1)
        {
            return;
        }

        if (SelectedPlatformIndex + 1 < platforms.Count)
        {
            for (int i = SelectedPlatformIndex + 1; i < LevelData.Platforms.Count; i++)
            {
                LevelData.Platforms[i].Position.z -= SizePlatform;
                platforms[i].transform.position = new Vector3(0, 0, platforms[i].transform.position.z - SizePlatform);
            }
        }

        PlatformCount--;
        DestroyImmediate(platforms[SelectedPlatformIndex].gameObject);
        LevelData.Platforms.RemoveAt(SelectedPlatformIndex);
        LevelData.ObjectsData.RemoveAll(it => it.PlatformIndex == SelectedPlatformIndex);
        platforms.RemoveAt(SelectedPlatformIndex);
    }

    private void SaveData()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            List<PlatformObjectData> objects = LevelData.ObjectsData.Where(it => it.PlatformIndex == i).ToList();
            for (int j = 0; j < objects.Count; j++)
            {
                objects[j].Position = PlatformObjects[i][j].position;
            }
        }
    }

    private void AddPlatform()
    {
        if (LevelData.Platforms == null)
        {
            LevelData.Platforms = new List<PlatformData>();
        }

        GameObject instantiatedPlatform = PrefabUtility.InstantiatePrefab(PlatformPrefab, LevelHolder) as GameObject;
        instantiatedPlatform.gameObject.SetActive(true);

        instantiatedPlatform.transform.position = new Vector3(0, 0, SizePlatform * PlatformCount);
        platforms.Add(instantiatedPlatform.transform);
        PlatformData platformData = new PlatformData()
        {
            Index = PlatformCount,
            EndScore = 1,
            Position = instantiatedPlatform.transform.position
        };

        PlatformObjects.Add(PlatformCount, new List<Transform>());
        LevelData.Platforms.Add(platformData);
        PlatformCount++;
        Selection.activeObject = instantiatedPlatform;
    }

    private void AddObject()
    {
        if (LevelData.ObjectsData == null)
        {
            LevelData.ObjectsData = new List<PlatformObjectData>();
        }

        Object collectableObjectGroupPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/Prefabs/CollectableObjectGroup.prefab");
        GameObject instantiatedObjectGroup = PrefabUtility.InstantiatePrefab(collectableObjectGroupPrefab) as GameObject;

        PlatformObjectData platformObjectData = new PlatformObjectData()
        {
            PlatformIndex = SelectedPlatformIndex,
            Position = Vector3.zero,
            ObjectType = SelectedObjectType,
            PresetData = Presets[SelectedPresetIndex]
        };

        ApplyPreset(instantiatedObjectGroup.transform);
        instantiatedObjectGroup.transform.SetParent(platforms[SelectedPlatformIndex].transform);
        instantiatedObjectGroup.transform.localPosition = new Vector3(0.0f, 0.551f, 0.0f);
        instantiatedObjectGroup.gameObject.SetActive(true);
        PlatformObjects[SelectedPlatformIndex].Add(instantiatedObjectGroup.transform);

        Selection.activeObject = instantiatedObjectGroup;
        LevelData.ObjectsData.Add(platformObjectData);
    }

    private void ApplyPreset(Transform holder)
    {
        Object selectedPrefab = null;
        switch (SelectedObjectType)
        {
            case ObjectType.Ball:
                selectedPrefab = BallPrefab;
                break;
            case ObjectType.Cube:
                selectedPrefab = CubePrefab;
                break;
        }

        for (int i = 0; i < Presets[SelectedPresetIndex].Positions.Count; i++)
        {
            GameObject presetObject = PrefabUtility.InstantiatePrefab(selectedPrefab, holder) as GameObject;
            presetObject.transform.localPosition = Presets[SelectedPresetIndex].Positions[i];
            presetObject.SetActive(true);
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject == null)
        {
            SelectedPlatformIndex = -1;
            return;
        }

        GameObject selectedPlatform = Selection.activeGameObject;
        if (selectedPlatform != null && platforms.Contains(selectedPlatform.transform))
        {
            SelectedPlatformIndex = platforms.IndexOf(selectedPlatform.transform);
            EndScore = LevelData.Platforms[SelectedPlatformIndex].EndScore;
            Repaint();
        }
    }
}