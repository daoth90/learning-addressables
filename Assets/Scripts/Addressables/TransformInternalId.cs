using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Addressables_Test {
  public class TransformInternalId : MonoBehaviour {
    // https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/runtime/TransformInternalId.html
    private string key = "Jaguar";
    private AsyncOperationHandle<GameObject> opHandle;
    private Watch watch;

    //Implement a method to transform the internal ids of locations
    static string MyCustomTransform(IResourceLocation location) {
      // if (location.ResourceType == typeof(IAssetBundleResource)
      //  && location.InternalId.StartsWith("http", System.StringComparison.Ordinal))
      //   return location.InternalId + "?customQueryTag=customQueryValue";
      // Utils.LogIResourceLocation(location);
      return location.InternalId;
    }

    //Override the Addressables transform method with your custom method.
    //This can be set to null to revert to default behavior.
    [RuntimeInitializeOnLoadMethod]
    static void SetInternalIdTransform() {
      Addressables.InternalIdTransformFunc = MyCustomTransform;
    }

    //Override the url of the WebRequest, the request passed to the method is what would be used as standard by Addressables.
    private void EditWebRequestURL(UnityWebRequest request) {
      Debug.LogError($"MyCustomTransform {request.url}");
      // if (request.url.EndsWith(".bundle", StringComparison.OrdinalIgnoreCase))
      //   request.url = request.url + "?customQueryTag=customQueryValue";
      // else if (request.url.EndsWith(".json", StringComparison.OrdinalIgnoreCase) || request.url.EndsWith(".hash", StringComparison.OrdinalIgnoreCase))
      //   request.url = request.url + "?customQueryTag=customQueryValue";
    }

    private void Start() {
      Addressables.WebRequestOverride = EditWebRequestURL;
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      StartCoroutine(Load());
    }

    private IEnumerator Load() {
      opHandle = Addressables.LoadAssetAsync<GameObject>(key);
      // yielding when already done still waits until the next frame
      // so don't yield if done.
      if (!opHandle.IsDone) {
        yield return opHandle;
      }

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