using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class LoadIResourceLocation : MonoBehaviour {
    private string key = "Jaguar";
    private Watch watch;

    private void Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      StartCoroutine(Load());
    }

    private IEnumerator Load() {
      var opHandle = Addressables.LoadResourceLocationsAsync(key);
      // yielding when already done still waits until the next frame
      // so don't yield if done.
      if (!opHandle.IsDone) {
        yield return opHandle;
      }

      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        foreach (var location in opHandle.Result) {
          Utils.LogIResourceLocation(location);
        }
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }
  }
}