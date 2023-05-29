using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class LoadFromReference : MonoBehaviour {
    // https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/runtime/LoadingAssetReferences.html
    // MUST ENABLE OPTION 'Include GUIDs in Catalog' (group setting) for loading asset by AssetReference
    public AssetReferenceGameObject assetRef;

    private AsyncOperationHandle<GameObject> opHandle;
    private Watch watch;
    
    private IEnumerator Start() {
      Debug.LogError($"Must enable Group > Include GUIDs in Catalog");
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      // yield return Load();
      yield return LoadWithDownloadStatus();
    }

    private IEnumerator Load() {
      AsyncOperationHandle<GameObject> opHandle = assetRef.LoadAssetAsync<GameObject>();
      yield return opHandle;
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        Instantiate(assetRef.Asset, transform);
      }
      else {
        Debug.LogError($"Load assetbundle failed AssetGUID {assetRef.AssetGUID} __ {opHandle.OperationException}");
        assetRef.ReleaseAsset();
      }
    }

    private IEnumerator LoadWithDownloadStatus() {
      Utils.ClearAssetBundleCache();
      AsyncOperationHandle<GameObject> opHandle = assetRef.LoadAssetAsync<GameObject>();
      while (!opHandle.IsDone) {
        Utils.LogDownloadBytesStatus(opHandle);
        yield return null;
      }

      Utils.LogDownloadBytesStatus(opHandle);
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        Instantiate(assetRef.Asset, transform);
      }
      else {
        Debug.LogError($"Load assetbundle failed AssetGUID {assetRef.AssetGUID} __ {opHandle.OperationException}");
        assetRef.ReleaseAsset();
      }
    }

    private void OnDestroy() {
      if (assetRef.Asset != null) { // Releases the asset when its object is destroyed
        assetRef.ReleaseAsset();
      }
    }
  }
}