using UnityEditor;

namespace MyUtils.Separator
{
    public static class SeparatorPresets
    {
        [MenuItem("GameObject/Create Separator/SYSTEM", false, 0)]
        public static void CreateSystem() => CreateSeparatorObject.Execute("SYSTEM");

        [MenuItem("GameObject/Create Separator/STAGE", false, 1)]
        public static void CreateStage() => CreateSeparatorObject.Execute("STAGE");

        [MenuItem("GameObject/Create Separator/UI", false, 2)]
        public static void CreateUI() => CreateSeparatorObject.Execute("UI");
    }
}