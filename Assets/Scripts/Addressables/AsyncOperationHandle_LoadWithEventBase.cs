using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class AsyncOperationHandle_LoadWithEventBase : MonoBehaviour {
    private string key = "Jaguar";
    private AsyncOperationHandle<GameObject> opHandle;
    private Watch watch;

    private void Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      Load();
    }

    private void Load() {
      // Create operation
      opHandle = Addressables.LoadAssetAsync<GameObject>(key);
      // Add event handler
      opHandle.Completed += LoadCallback; // callback will be called immediately (same frame) if opHandle already done
    }

    private void LoadCallback(AsyncOperationHandle<GameObject> _opHandle) {
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (_opHandle.Status == AsyncOperationStatus.Succeeded) {
        Instantiate(_opHandle.Result, transform);
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
        Addressables.Release(_opHandle); // _opHandle is same reference with opHandle, you can use either _opHandle or opHandle
      }
    }

    private void OnDestroy() {
      // Checks to make sure that handle hasn't already been released.
      if (opHandle.IsValid()) {
        Addressables.Release(opHandle);
      }
    }
  }
}