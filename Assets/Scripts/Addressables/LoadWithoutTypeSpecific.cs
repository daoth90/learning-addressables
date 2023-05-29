using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class LoadWithoutTypeSpecific : MonoBehaviour {
    // MUST BE A LIST OR IT WILL BE FAILED OR EXCEPTION
    
    // private string key = "Chimp_Animations"; // uncommend this to see issues
    private List<string> key = new List<string>() { "Chimp_Animations" }; // key must be a List
    
    private Watch watch;

    private IEnumerator Start() {
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();

      Debug.LogError("\n");
      Debug.LogError("\n");
      Debug.LogError("LoadResourceLocationsAsync_ObjectType");
      yield return LoadResourceLocationsAsync_ObjectType();

      Debug.LogError("\n");
      Debug.LogError("\n");
      Debug.LogError("LoadResourceLocationsAsync_NoType");
      yield return LoadResourceLocationsAsync_NoType();

      Debug.LogError("\n");
      Debug.LogError("\n");
      Debug.LogError("LoadAssetsAsync");
      yield return LoadAssetsAsync();
    }

    private IEnumerator LoadAssetsAsync() {
      // nếu đổi Object sang AnimationClip thì vẫn có result
      // nhưng chỉ lấy được 1 AnimationClip (FAILED nếu .fbx asset chứa nhiều hơn 1 AnimationClip)
      var opHandle = Addressables.LoadAssetsAsync<Object>(key, obj => { Debug.LogError($"obj.name {obj.name} __ obj.GetType {obj.GetType()}"); }, Addressables.MergeMode.Union, releaseDependenciesOnFailure: true);
      yield return opHandle;
      watch.StopAndLog($"LoadAssetsAsync.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) { }
      else {
        Debug.LogError($"LoadAssetsAsync.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }

    private IEnumerator LoadResourceLocationsAsync_ObjectType() {
      // chỉ lấy được 1 AnimationClip (FAILED nếu .fbx asset chứa nhiều hơn 1 AnimationClip)
      var opHandle = Addressables.LoadResourceLocationsAsync(key, Addressables.MergeMode.Union, typeof(Object));
      yield return opHandle;
      watch.StopAndLog($"LoadResourceLocationsAsync_ObjectType.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        foreach (var location in opHandle.Result) {
          Debug.LogError($"location.ResourceType {location.ResourceType}");
        }
      }
      else {
        Debug.LogError($"LoadResourceLocationsAsync_ObjectType.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }

    private IEnumerator LoadResourceLocationsAsync_NoType() {
      // chỉ lấy được 1 AnimationClip (FAILED nếu .fbx asset chứa nhiều hơn 1 AnimationClip)
      var opHandle = Addressables.LoadResourceLocationsAsync(key, Addressables.MergeMode.Union);
      yield return opHandle;
      watch.StopAndLog($"LoadResourceLocationsAsync_NoType.Status {opHandle.Status.ToString()}");
      if (opHandle.Status == AsyncOperationStatus.Succeeded) {
        foreach (var location in opHandle.Result) {
          Debug.LogError($"location.ResourceType {location.ResourceType}");
        }
      }
      else {
        Debug.LogError($"LoadResourceLocationsAsync_NoType.OperationException {opHandle.OperationException}");
        Addressables.Release(opHandle);
      }
    }
  }
}