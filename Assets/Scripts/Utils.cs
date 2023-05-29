using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class Utils {
  private static bool cachingCleared;
  
  public static long ConvertToKbyte(string contentLength) {
    return ConvertToKbyte(Convert.ToInt64(contentLength));
  }

  public static long ConvertToMbyte(string contentLength) {
    return ConvertToMbyte(Convert.ToInt64(contentLength));
  }

  public static long ConvertToKbyte(long contentLength) {
    return contentLength / 1000;
  }

  public static long ConvertToMbyte(long contentLength) {
    return contentLength / 1000000;
  }
  
  public static void LogPath() {
    Debug.LogError($"Addressables.PlayerBuildDataPath {Addressables.PlayerBuildDataPath}");
    Debug.LogError($"Addressables.BuildPath {Addressables.BuildPath}");
    Debug.LogError($"Addressables.BuildReportPath {Addressables.BuildReportPath}");
    Debug.LogError($"Addressables.kAddressablesRuntimeBuildLogPath {Addressables.kAddressablesRuntimeBuildLogPath}");
    Debug.LogError($"Addressables.kAddressablesRuntimeDataPath {Addressables.kAddressablesRuntimeDataPath}");
    Debug.LogError($"Addressables.LibraryPath {Addressables.LibraryPath}");
    Debug.LogError($"Addressables.RuntimePath {Addressables.RuntimePath}");
    Debug.LogError($"\n");
  }

  public static void LogIResourceLocation(IResourceLocation location, IResourceLocation refBy = null, bool logDependencies = false) {
    string refByLog = refBy != null ? $"dependencies of {refBy.PrimaryKey}" : "";
    Debug.LogError($"{refByLog} IResourceLocation.PrimaryKey {location.PrimaryKey}");
    Debug.LogError($"{refByLog} IResourceLocation.InternalId {location.InternalId}");
    Debug.LogError($"{refByLog} IResourceLocation.HasDependencies {location.HasDependencies}");
    Debug.LogError($"{refByLog} IResourceLocation.ResourceType {location.ResourceType}");
    Debug.LogError($"\n");
    if (logDependencies && location.HasDependencies) {
      foreach (var dependencyLocation in location.Dependencies) {
        LogIResourceLocation(dependencyLocation, location);
      }
    }
  }

  public static void LogDownloadBytesStatus(AsyncOperationHandle opHandle) {
    Debug.LogError($"AsyncOperationHandle.PercentComplete {opHandle.PercentComplete}");
    var downloadStatus = opHandle.GetDownloadStatus();
    Debug.LogError($"DownloadStatus.IsDone {downloadStatus.IsDone}");
    Debug.LogError($"DownloadStatus.TotalBytes {downloadStatus.TotalBytes}");
    Debug.LogError($"DownloadStatus.DownloadedBytes {downloadStatus.DownloadedBytes}");
    Debug.LogError($"DownloadStatus.Percent {downloadStatus.Percent}");
    Debug.LogError($"\n");
  }
  
  // Gets called for every error scenario encountered during an operation.
  // A common use case for this is having InvalidKeyExceptions fail silently when 
  // a location is missing for a given key.
  public static void ExceptionHandler(AsyncOperationHandle handle, Exception exception) {
    if (exception.GetType() != typeof(InvalidKeyException))
      Addressables.LogException(handle, exception);
  }

  public static void ClearAssetBundleCache() {
    if (cachingCleared) {
      return;
    }
    
    if (Caching.ClearCache()) {
      cachingCleared = true;
      Debug.LogError("ClearAssetBundleCache Success");
    }
    else {
      Debug.LogError("ClearAssetBundleCache Failed");
    }
  }
}