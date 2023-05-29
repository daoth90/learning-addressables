using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class LoadSubAsset_AnimationClip : MonoBehaviour {
    private string key = "Chimp_Animations[Fear]";
    private AsyncOperationHandle<AnimationClip> opHandle;
    private Watch watch;

    private IEnumerator Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      yield return Load();
    }

    private IEnumerator Load() {
      opHandle = Addressables.LoadAssetAsync<AnimationClip>(key);
      // yielding when already done still waits until the next frame
      // so don't yield if done.
      if (!opHandle.IsDone) {
        yield return opHandle;
      }

      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) { }
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