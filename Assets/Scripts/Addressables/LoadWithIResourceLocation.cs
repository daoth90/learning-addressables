using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Addressables_Test {
  public class LoadWithIResourceLocation : MonoBehaviour {
    private List<string> keys = new List<string>() { "odb", "ui_roster", "Leopard"};
    private Watch watch;
    private Dictionary<string, AsyncOperationHandle> opDictToReleaseAssetWhenUnUsed = new Dictionary<string, AsyncOperationHandle>();

    private void Start() {
      Debug.LogError($"Must enable Group > Include Labels in Catalog");
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      StartCoroutine(Load());
    }

    private IEnumerator Load() {
      AsyncOperationHandle<IList<IResourceLocation>> opHandleLoadLocation = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Union, typeof(GameObject));
      yield return opHandleLoadLocation;
      watch.StopAndLog($"LoadResourceLocationsAsync opHandle.Status {opHandleLoadLocation.Status.ToString()}");
      watch.Start();
      if (opHandleLoadLocation.Status == AsyncOperationStatus.Succeeded) {
        List<AsyncOperationHandle> opHandleList = new List<AsyncOperationHandle>(opHandleLoadLocation.Result.Count);
        foreach (var location in opHandleLoadLocation.Result) {
          AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>(location);
          opHandle.Completed += tmpOpHandle => LoadAssetAsyncCallback(tmpOpHandle, location.PrimaryKey);
          opHandleList.Add(opHandle);
        }

        var genericGroupOp = Addressables.ResourceManager.CreateGenericGroupOperation(opHandleList, true);
        yield return genericGroupOp;
        watch.StopAndLog($"genericGroupOp Status {genericGroupOp.Status.ToString()}");
        if (genericGroupOp.Status != AsyncOperationStatus.Succeeded) {
          Addressables.Release(genericGroupOp);
        }
      }
      else {
        Debug.LogError($"opHandleLoadLocation.OperationException {opHandleLoadLocation.OperationException}");
        Addressables.Release(opHandleLoadLocation);
      }
    }

    private void LoadAssetAsyncCallback(AsyncOperationHandle<GameObject> opHandle, string primaryKey) {
      Debug.LogError($"LoadAssetAsyncCallback asset.Name {opHandle.Result.name}");
      opDictToReleaseAssetWhenUnUsed.Add(primaryKey, opHandle);
    }

    private void OnDestroy() {
      foreach (var kp in opDictToReleaseAssetWhenUnUsed) {
        if (kp.Value.IsValid()) {
          // Checks to make sure that handle hasn't already been released.
          Addressables.Release(kp.Value);
        }
      }
    }
  }
}