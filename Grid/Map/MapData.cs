using UnityEngine;

namespace MyUtils.Grid.Map
{
    [CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObject/MapData")]
    public class MapData : ScriptableObject
    {
        public TextAsset GridData;
        public SceneReference Scene;
    }
}