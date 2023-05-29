using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Addressables_Test {
  public class LoadAddressablesScene : MonoBehaviour {
    private string key = "SceneAddressables";
    private Watch watch;

    private IEnumerator Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      yield return new WaitForSeconds(5f);
      watch = new Watch().Start();
      var opHandle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single, activateOnLoad: false);
      yield return opHandle;
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        // UNITY AUTO RELEASE ASSET IF YOU LOAD A NEW SCENE -> DON'T NEED TO CACHE IT
        yield return opHandle.Result.ActivateAsync();
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }
  }
}