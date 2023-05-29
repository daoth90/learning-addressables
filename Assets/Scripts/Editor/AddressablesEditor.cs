using UnityEditor;
using UnityEngine;

namespace Addressables_Test {
  #if UNITY_EDITOR

  public static class AddressablesEditor {
    [MenuItem("Tool/Addressables/Clear Caching")]
    public static void ClearCaching() {
      if (Caching.ClearCache()) {
        Debug.LogError("Caching.ClearCache success");
      }
      else {
        Debug.LogError("Caching.ClearCache failed");
      }
    }

    [MenuItem("Tool/Open Persistent Data Folder")]
    public static void OpenPersistentDataFolder() {
      EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
  }

  #endif
}