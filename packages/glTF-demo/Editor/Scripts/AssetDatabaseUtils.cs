using UnityEditor;
using UnityEngine;

namespace GltfDemo.Editor {

    static class AssetDatabaseUtils
    {
        [MenuItem("AssetDatabase/Force Reserialize Assets Example")]
        static void UpdateGroundMaterials() {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}