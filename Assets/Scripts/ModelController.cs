using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnemiesSpawnManager;

public static class ModelController
{
    public static bool IsUsingNightVision = false;
    public static bool IsUsingFlashlight = false;
    private const string EnemySavePrefix = "Enemy_";

    private static List<EnemyType> _seenTypes = null;

    public static void Init()
    {
        _seenTypes = new List<EnemyType>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    public static void OnSceneChanged(Scene first, Scene second)
    {
        _seenTypes.Clear();
    }

    public static bool CheckEnemySeen(EnemyType type)
    {
        if (_seenTypes == null) Init(); 
        return _seenTypes.Contains(type);
    }

    public static void SetEnemySeen(EnemyType type)
    {
        if (_seenTypes == null) Init();
        _seenTypes.Add(type);
    }
}
