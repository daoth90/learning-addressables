using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace Addressables_Test {
  public class LoadSubAsset_SpriteInSpriteAtlas : MonoBehaviour {
    private string keyAtlas = "hero-idle-atlas";
    private string keySprite = "hero-idle-atlas[hero-idle_1]";
    private AsyncOperationHandle<Sprite> opHandleSprite;
    private AsyncOperationHandle<SpriteAtlas> opHandleAtlas;
    private Watch watch;

    private IEnumerator Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      yield return LoadSprite();
      yield return LoadAtlas();
    }

    private IEnumerator LoadAtlas() {
      opHandleAtlas = Addressables.LoadAssetAsync<SpriteAtlas>(keyAtlas);
      // yielding when already done still waits until the next frame
      // so don't yield if done.
      if (!opHandleAtlas.IsDone) {
        yield return opHandleAtlas;
      }

      watch.StopAndLog($"opHandle.Status {opHandleAtlas.Status.ToString()}");
      if (opHandleAtlas.Status == AsyncOperationStatus.Succeeded) {
        // get Sprite from SpriteAtlas here
      }
      else {
        Debug.LogError($"opHandle.OperationException {opHandleAtlas.OperationException}");
        Addressables.Release(opHandleAtlas);
      }
    }

    private IEnumerator LoadSprite() {
      opHandleSprite = Addressables.LoadAssetAsync<Sprite>(keySprite);
      // yielding when already done still waits until the next frame
      // so don't yield if done.
      if (!opHandleSprite.IsDone) {
        yield return opHandleSprite;
      }

      watch.StopAndLog($"opHandle.Status {opHandleSprite.Status.ToString()}");
      if (opHandleSprite.Status == AsyncOperationStatus.Succeeded) { }
      else {
        Debug.LogError($"opHandle.OperationException {opHandleSprite.OperationException}");
        Addressables.Release(opHandleSprite);
      }
    }

    private void OnDestroy() {
      if (opHandleSprite.IsValid()) {
        // Checks to make sure that handle hasn't already been released.
        Addressables.Release(opHandleSprite);
      }
      
      if (opHandleAtlas.IsValid()) {
        // Checks to make sure that handle hasn't already been released.
        Addressables.Release(opHandleSprite);
      }
    }
  }
}