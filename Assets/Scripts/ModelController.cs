using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemiesSpawnManager;

public class ModelController : Singleton<ModelController>
{
    public static bool IsUsingNightVision = false;
    public static bool IsUsingFlashlight = false;

    private List<EnemyType> _seenTypes = new List<EnemyType>();

    public static void OnSceneChanged(Scene first, Scene second)
    {
         Instance._seenTypes.Clear();
    }

    public static bool CheckEnemySeen(EnemyType type)
    {
        return Instance._seenTypes.Contains(type);
    }

    public static void SetEnemySeen(EnemyType type)
    {
        Instance._seenTypes.Add(type);
    }
}
