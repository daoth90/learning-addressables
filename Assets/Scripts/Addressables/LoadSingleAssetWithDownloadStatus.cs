using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ResourceManager = UnityEngine.ResourceManagement.ResourceManager;

namespace Addressables_Test {
  public class LoadSingleAssetWithDownloadStatus : MonoBehaviour {
    private string address = "Jaguar";
    private AsyncOperationHandle<GameObject> opHandle;
    private Watch watch;

    private void Start() {
      Debug.LogError($"This only work with Remote Editor Hosted or real Server (must clear Caching to see progress Tool > Addressables > Clear Caching)");
      Utils.ClearAssetBundleCache();
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      StartCoroutine(LoadWithIEnumerator());
    }

    private IEnumerator LoadWithIEnumerator() {
      opHandle = Addressables.LoadAssetAsync<GameObject>(address);
      while (!opHandle.IsDone) {
        Utils.LogDownloadBytesStatus(opHandle);
        yield return null;
      }

      // done
      Utils.LogDownloadBytesStatus(opHandle);
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        Instantiate(opHandle.Result, transform);
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }

    private void OnDestroy() {
      if (opHandle.IsValid()) {
        // Checks to make sure that handle hasn't already been released.
        Addressables.Release(opHandle);
      }
    }
  }
}