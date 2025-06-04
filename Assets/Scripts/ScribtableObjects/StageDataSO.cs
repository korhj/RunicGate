using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "SO/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    public List<GameStage> stages;

    [Serializable]
    public class GameStage
    {
        public string sceneName;
        public Vector3Int playerStartPos;
        public string name;
    }
}
