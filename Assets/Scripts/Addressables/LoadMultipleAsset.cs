using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class LoadMultipleAsset : MonoBehaviour {
    private List<string> keys = new List<string>() { "odb", "ui_roster", "Leopard" };
    private Watch watch;
    private AsyncOperationHandle<IList<GameObject>> opHandle;

    private IEnumerator Start() {
      Debug.LogError($"Must enable Group > Include Labels in Catalog");
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      yield return Load();
    }

    private IEnumerator Load() {
      opHandle = Addressables.LoadAssetsAsync<GameObject>(keys, asset => { Debug.LogError($"Single asset.Name {asset.name}"); }, Addressables.MergeMode.Union, releaseDependenciesOnFailure: true);
      yield return opHandle;
      watch.StopAndLog($"LoadAssetsAsync opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        Debug.LogError($"Download multiple assets Succeeded");
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }

    private void OnDestroy() {
      if (opHandle.IsValid()) { // Checks to make sure that handle hasn't already been released.
        Addressables.Release(opHandle);
      }
    }
  }
}