using Game.Data;
using Game.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        #region Fields

        public const string PREFAB_PATH = "Assets/Prefabs/";
        private static EditorWindow window;
        private LevelData levelData;
        private int levelIndex;
        private bool isLoadedLevel;

        private Transform levelHolder;
        private Transform startPlatform;
        private float sizePlatform;
        private int platformCount;

        private List<Transform> platforms;
        private Dictionary<int, List<Transform>> platformObjects;
        private Dictionary<int, List<Transform>> helicopterObjects;
        private List<PresetData> presets;

        private int helicopterObjectCount;
        private float helicopterSpawnRate;
        private float helicopterDuration;
        private ObjectSpawnType helicopterObjectSpawnType;
        private ObjectType helicopterObjectType;

        private int endScore;
        private int selectedPlatformIndex;
        private int selectedPresetIndex;
        private ObjectType selectedObjectType;
        private List<Vector3> curvePoints;

        private bool isOpenAddObjectGroup;
        private bool isOpenAddHelicopterGroup;
        private bool isOpenEditHelicopterGroup;

        private CustomObjectData selectedHelicopterData;

        #region Prefabs

        private Object platformPrefab;
        private Object collectableObjectGroupPrefab;
        private Object ballPrefab;
        private Object cubePrefab;
        private Object helicopterPrefab;

        #endregion

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            Initialize();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneView_duringSceneGui;
            EditorSceneManager.OpenScene(EditorSceneManager.GetActiveScene().path);
        }

        private void OnGUI()
        {
            levelData = EditorGUILayout.ObjectField("Level Data : ", levelData, typeof(LevelData), false) as LevelData;
            levelIndex = EditorGUILayout.IntField("Level Index : ", levelIndex);

            #region Platform

            if (GUILayout.Button("Add Platform"))
            {
                AddPlatform(null);
            }

            if (EditorGUILayout.BeginFadeGroup(selectedPlatformIndex == -1 ? 0 : 1))
            {
                if (GUILayout.Button("Remove Selected Platform"))
                {
                    RemovePlatform();
                }
            }
            EditorGUILayout.EndFadeGroup();

            #endregion

            #region Object

            if (GUILayout.Button("Remove Selected Object"))
            {
                RemoveObject();
            }

            GUILayout.Space(10);

            if (GUILayout.Button(isOpenAddObjectGroup ? "Close Add Object Panel" : "Open Add Object Panel"))
            {
                isOpenAddObjectGroup = !isOpenAddObjectGroup;
            }

            GUILayout.Space(10);

            if (EditorGUILayout.BeginFadeGroup(isOpenAddObjectGroup ? 1 : 0))
            {
                selectedObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type : ", selectedObjectType);
                selectedPresetIndex = EditorGUILayout.Popup("Preset : ", selectedPresetIndex, presets.Select(preset => preset.name).ToArray());

                if (GUILayout.Button("Confirm Object"))
                {
                    AddObject(null);
                }
            }

            EditorGUILayout.EndFadeGroup();

            #endregion

            GUILayout.Space(10);

            #region Helicopter

            if (GUILayout.Button(isOpenAddHelicopterGroup ? "Close Add Helicopter Panel" : "Open Add Helicopter Panel"))
            {
                isOpenAddHelicopterGroup = !isOpenAddHelicopterGroup;
            }

            GUILayout.Space(10);

            if (EditorGUILayout.BeginFadeGroup(isOpenAddHelicopterGroup ? 1 : 0))
            {
                helicopterObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type : ", helicopterObjectType);
                helicopterObjectCount = EditorGUILayout.IntField("Spawn Object Count : ", helicopterObjectCount);
                helicopterSpawnRate = EditorGUILayout.FloatField("Spawn Rate : ", helicopterSpawnRate);
                helicopterDuration = EditorGUILayout.FloatField("All Animation Duration : ", helicopterDuration);
                helicopterObjectSpawnType = (ObjectSpawnType)EditorGUILayout.EnumPopup("Object Spawn Type : ", helicopterObjectSpawnType);

                GUILayout.Space(5);
                SceneView.RepaintAll();
                curvePoints[0] = EditorGUILayout.Vector3Field("point 0 ", curvePoints[0]);
                curvePoints[1] = EditorGUILayout.Vector3Field("point 1 ", curvePoints[1]);
                curvePoints[2] = EditorGUILayout.Vector3Field("point 2 ", curvePoints[2]);
                curvePoints[3] = EditorGUILayout.Vector3Field("point 3 ", curvePoints[3]);

                if (GUILayout.Button("Confirm Helicopter"))
                {
                    AddHelicopter(null);
                    isOpenAddHelicopterGroup = false;
                }
            }

            EditorGUILayout.EndFadeGroup();

            if (GUILayout.Button(isOpenEditHelicopterGroup ? "Disable Edit Helicopter" : "Enable Edit Helicopter"))
            {
                isOpenEditHelicopterGroup = !isOpenEditHelicopterGroup;
            }

            if (EditorGUILayout.BeginFadeGroup(isOpenEditHelicopterGroup ? 1 : 0))
            {
                selectedHelicopterData.ObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type : ", selectedHelicopterData.ObjectType);
                selectedHelicopterData.SpawnObjectCount = EditorGUILayout.IntField("Spawn Object Count : ", selectedHelicopterData.SpawnObjectCount);
                selectedHelicopterData.ObjectSpawnTime = EditorGUILayout.FloatField("Spawn Rate : ", selectedHelicopterData.ObjectSpawnTime);
                selectedHelicopterData.Duration = EditorGUILayout.FloatField("All Animation Duration : ", selectedHelicopterData.Duration);
                selectedHelicopterData.ObjectSpawnType = (ObjectSpawnType)EditorGUILayout.EnumPopup("Object Spawn Type : ", selectedHelicopterData.ObjectSpawnType);

                GUILayout.Space(5);
                SceneView.RepaintAll();
                curvePoints[0] = EditorGUILayout.Vector3Field("point 0 ", curvePoints[0]);
                curvePoints[1] = EditorGUILayout.Vector3Field("point 1 ", curvePoints[1]);
                curvePoints[2] = EditorGUILayout.Vector3Field("point 2 ", curvePoints[2]);
                curvePoints[3] = EditorGUILayout.Vector3Field("point 3 ", curvePoints[3]);

                selectedHelicopterData.Position = curvePoints[0];
                selectedHelicopterData.CurvePoints = new List<Vector3>()
                {
                    curvePoints[3],
                    curvePoints[1],
                    curvePoints[2]
                };

                if (GUILayout.Button("Confirm Edit"))
                {
                    isOpenEditHelicopterGroup = false;
                }
            }

            EditorGUILayout.EndFadeGroup();

            #endregion

            GUILayout.Space(10);

            #region Level

            if (GUILayout.Button("Save Level"))
            {
                SaveData();
            }

            if (GUILayout.Button("Load Level"))
            {
                LoadData();
            }

            if (selectedPlatformIndex != -1)
            {
                GUILayout.Label("Selected platform index : " + selectedPlatformIndex);
                levelData.Platforms[selectedPlatformIndex].EndScore = EditorGUILayout.IntField("End Score : ", levelData.Platforms[selectedPlatformIndex].EndScore);
            }

            #endregion
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject == null)
            {
                selectedPlatformIndex = -1;
                return;
            }

            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject != null)
            {
                if (platforms.Contains(selectedObject.transform))
                {
                    selectedPlatformIndex = platforms.IndexOf(selectedObject.transform);
                    endScore = levelData.Platforms[selectedPlatformIndex].EndScore;
                }
                else if (selectedObject.GetComponent<Helicopter>())
                {
                    int platformIndex = platforms.IndexOf(Selection.activeGameObject.transform.parent);
                    int helicopterIndex = helicopterObjects[platformIndex].IndexOf(Selection.activeGameObject.transform);
                    selectedHelicopterData = levelData.Platforms[platformIndex].HelicopterData[helicopterIndex];

                    curvePoints = new List<Vector3>()
                    {
                        selectedHelicopterData.Position,
                        selectedHelicopterData.CurvePoints[1],
                        selectedHelicopterData.CurvePoints[2],
                        selectedHelicopterData.CurvePoints[0]
                    };
                }
                Repaint();
            }
        }

        #endregion

        #region Private Methods

        [MenuItem("LevelEditor/Show")]
        private static void ShowWindow()
        {
            window = GetWindow(typeof(LevelEditorWindow));
            window.minSize = new Vector2(350, 600);
            window.titleContent.text = "Level Editor";

            if (SceneManager.GetActiveScene().name != "LevelEditor")
            {
                Debug.LogError("Please open LevelEditor scene!");
                window.Close();
            }
        }

        private void Initialize()
        {
            LoadPrefabs();

            levelData = ScriptableObject.CreateInstance<LevelData>();
            levelHolder = GameObject.Find("LevelHolder").transform;
            startPlatform = GameObject.Find("StartPlatform").transform;
            presets = Resources.LoadAll<PresetData>("Preset Data").ToList();
            platformCount = 0;
            isLoadedLevel = false;
            platforms = new List<Transform>();
            platformObjects = new Dictionary<int, List<Transform>>();
            helicopterObjects = new Dictionary<int, List<Transform>>();
            sizePlatform = Mathf.Abs(startPlatform.transform.position.z) + 20;
            selectedPlatformIndex = -1;
            curvePoints = new List<Vector3>()
                    {
                        new Vector3(0,1,0),
                        new Vector3(1,1,0),
                        new Vector3(1,1,1),
                        new Vector3(-1,1,1),
                    };
        }

        private void LoadPrefabs()
        {
            platformPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Platform.prefab");
            ballPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Ball.prefab");
            cubePrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Cube.prefab");
            helicopterPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "Helicopter.prefab");
            collectableObjectGroupPrefab = AssetDatabase.LoadMainAssetAtPath(PREFAB_PATH + "CollectableObjectGroup.prefab");
        }

        private void CreateScriptableObject()
        {
            string path = $"Assets/Resources/Level Data/Level_{levelIndex}.asset";
            AssetDatabase.CreateAsset(levelData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelData;
        }

        private void SaveData()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (levelData.Platforms[i].ObjectsData != null)
                {
                    for (int j = 0; j < levelData.Platforms[i].ObjectsData.Count; j++)
                    {
                        levelData.Platforms[i].ObjectsData[j].Position = platformObjects[i][j].localPosition;
                        levelData.Platforms[i].ObjectsData[j].Rotation = platformObjects[i][j].eulerAngles;
                    }
                }
            }

            if (isLoadedLevel)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                CreateScriptableObject();
            }

            levelData.Index = levelIndex;
            Debug.Log("Level Saved!");
            window.Close();
        }

        private void LoadData()
        {
            string levelPath = $"Level Data/Level_{levelIndex}";
            levelData = Resources.Load<LevelData>(levelPath);
            EditorUtility.SetDirty(levelData);

            if (levelData != null)
            {
                isLoadedLevel = true;
                for (int i = 0; i < levelData.Platforms.Count; i++)
                {
                    endScore = levelData.Platforms[i].EndScore;
                    selectedPlatformIndex = i;
                    AddPlatform(levelData.Platforms[i]);

                    if (levelData.Platforms[i].ObjectsData != null)
                    {
                        for (int j = 0; j < levelData.Platforms[i].ObjectsData.Count; j++)
                        {
                            AddObject(levelData.Platforms[i].ObjectsData[j]);
                        }
                    }

                    if (levelData.Platforms[i].HelicopterData != null)
                    {
                        for (int j = 0; j < levelData.Platforms[i].HelicopterData.Count; j++)
                        {
                            AddHelicopter(levelData.Platforms[i].HelicopterData[j]);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Level could not be found! path : " + levelPath);
            }
        }

        private void AddPlatform(PlatformData platformData)
        {
            if (levelData.Platforms == null)
                levelData.Platforms = new List<PlatformData>();

            GameObject instantiatedPlatform = PrefabUtility.InstantiatePrefab(platformPrefab, levelHolder) as GameObject;

            instantiatedPlatform.gameObject.SetActive(true);
            instantiatedPlatform.transform.position = new Vector3(0, 0, sizePlatform * platformCount);
            platforms.Add(instantiatedPlatform.transform);

            if (platformData == null)
            {
                platformData = new PlatformData()
                {
                    Index = platformCount,
                    EndScore = endScore,
                    Position = instantiatedPlatform.transform.position
                };

                levelData.Platforms.Add(platformData);
            }

            platformObjects.Add(platformCount, new List<Transform>());
            helicopterObjects.Add(platformCount, new List<Transform>());

            platformCount++;
            Selection.activeObject = instantiatedPlatform;
        }

        private void AddObject(PlatformObjectData platformObjectData)
        {
            if (levelData.Platforms[selectedPlatformIndex].ObjectsData == null)
                levelData.Platforms[selectedPlatformIndex].ObjectsData = new List<PlatformObjectData>();

            GameObject instantiatedObjectGroup = PrefabUtility.InstantiatePrefab(collectableObjectGroupPrefab) as GameObject;
            if (platformObjectData == null)
            {
                platformObjectData = new PlatformObjectData()
                {
                    Position = new Vector3(0.0f, 0.551f, 0.0f),
                    Rotation = Vector3.zero,
                    ObjectType = selectedObjectType,
                    PresetData = presets[selectedPresetIndex]
                };

                levelData.Platforms[selectedPlatformIndex].ObjectsData.Add(platformObjectData);
            }

            selectedObjectType = platformObjectData.ObjectType;
            selectedPresetIndex = presets.IndexOf(platformObjectData.PresetData);
            ApplyPreset(instantiatedObjectGroup.transform);

            instantiatedObjectGroup.transform.SetParent(platforms[selectedPlatformIndex].transform);
            instantiatedObjectGroup.transform.localPosition = platformObjectData.Position;
            instantiatedObjectGroup.transform.localEulerAngles = platformObjectData.Rotation;
            instantiatedObjectGroup.gameObject.SetActive(true);

            platformObjects[selectedPlatformIndex].Add(instantiatedObjectGroup.transform);

            Selection.activeObject = instantiatedObjectGroup;
        }

        private void AddHelicopter(CustomObjectData helicopterData)
        {
            if (levelData.Platforms[selectedPlatformIndex].HelicopterData == null)
                levelData.Platforms[selectedPlatformIndex].HelicopterData = new List<CustomObjectData>();

            GameObject instantiatedHelicopter = PrefabUtility.InstantiatePrefab(helicopterPrefab) as GameObject;

            instantiatedHelicopter.transform.SetParent(platforms[selectedPlatformIndex].transform);
            instantiatedHelicopter.transform.localPosition = new Vector3(0, 1f, 0);
            instantiatedHelicopter.gameObject.SetActive(true);
            if (helicopterData == null)
            {
                helicopterData = new CustomObjectData()
                {
                    ObjectType = helicopterObjectType,
                    Position = curvePoints[0],
                    ObjectSpawnType = helicopterObjectSpawnType,
                    ObjectSpawnTime = helicopterSpawnRate,
                    SpawnObjectCount = helicopterObjectCount,
                    Duration = helicopterDuration,
                    CurvePoints = new List<Vector3>()
                    {
                        curvePoints[3],
                        curvePoints[1],
                        curvePoints[2]
                    },
                };

                levelData.Platforms[selectedPlatformIndex].HelicopterData.Add(helicopterData);
            }

            instantiatedHelicopter.transform.position = helicopterData.Position;
            helicopterObjects[selectedPlatformIndex].Add(instantiatedHelicopter.transform);
            Selection.activeObject = instantiatedHelicopter;
        }

        private void ApplyPreset(Transform holder)
        {
            Object selectedPrefab = null;
            switch (selectedObjectType)
            {
                case ObjectType.Ball:
                    selectedPrefab = ballPrefab;
                    break;
                case ObjectType.Cube:
                    selectedPrefab = cubePrefab;
                    break;
            }

            for (int i = 0; i < presets[selectedPresetIndex].Positions.Count; i++)
            {
                GameObject presetObject = PrefabUtility.InstantiatePrefab(selectedPrefab, holder) as GameObject;
                presetObject.transform.localPosition = presets[selectedPresetIndex].Positions[i];
                presetObject.SetActive(true);
            }
        }

        private void RemoveObject()
        {
            if (Selection.activeGameObject != null)
            {
                int platformIndex = platforms.IndexOf(Selection.activeGameObject.transform.parent);

                if (Selection.activeGameObject.GetComponent<CollectableObjectGroup>())
                {
                    int selectedObjectIndex = platformObjects[platformIndex].IndexOf(Selection.activeGameObject.transform);
                    DestroyImmediate(platformObjects[platformIndex][selectedObjectIndex].gameObject);
                    levelData.Platforms[platformIndex].ObjectsData.RemoveAt(selectedObjectIndex);
                    platformObjects[platformIndex].RemoveAt(selectedObjectIndex);
                }
                else if (Selection.activeGameObject.GetComponent<Helicopter>())
                {
                    int selectedHelicopterIndex = helicopterObjects[platformIndex].IndexOf(Selection.activeGameObject.transform);
                    DestroyImmediate(helicopterObjects[platformIndex][selectedHelicopterIndex].gameObject);
                    levelData.Platforms[platformIndex].HelicopterData.RemoveAt(selectedHelicopterIndex);
                    helicopterObjects[platformIndex].RemoveAt(selectedHelicopterIndex);
                }
            }
        }

        private void RemovePlatform()
        {
            if (selectedPlatformIndex == -1)
            {
                return;
            }

            if (selectedPlatformIndex + 1 < platforms.Count)
            {
                for (int i = selectedPlatformIndex + 1; i < levelData.Platforms.Count; i++)
                {
                    levelData.Platforms[i].Position.z -= sizePlatform;
                    platforms[i].transform.position = new Vector3(0, 0, platforms[i].transform.position.z - sizePlatform);
                }
            }

            platformCount--;
            DestroyImmediate(platforms[selectedPlatformIndex].gameObject);
            levelData.Platforms.RemoveAt(selectedPlatformIndex);
            platforms.RemoveAt(selectedPlatformIndex);
        }

        private void SceneView_duringSceneGui(SceneView obj)
        {
            if (isOpenAddHelicopterGroup || isOpenEditHelicopterGroup)
            {
                Handles.DrawBezier(curvePoints[0], curvePoints[3], curvePoints[1], curvePoints[2], Color.red, null, 25);
                curvePoints[0] = Handles.FreeMoveHandle(curvePoints[0], Quaternion.identity, .1f, Vector3.zero, Handles.DotHandleCap);
                curvePoints[1] = Handles.FreeMoveHandle(curvePoints[1], Quaternion.identity, .1f, Vector3.zero, Handles.DotHandleCap);
                curvePoints[2] = Handles.FreeMoveHandle(curvePoints[2], Quaternion.identity, .1f, Vector3.zero, Handles.DotHandleCap);
                curvePoints[3] = Handles.FreeMoveHandle(curvePoints[3], Quaternion.identity, .1f, Vector3.zero, Handles.DotHandleCap);
            }
        }

        #endregion
    }
}