using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;

namespace Addressables_Test {
  public class GetDownloadSize : MonoBehaviour {
    private List<string> keys = new List<string>() { "odb", "ui_roster", "Leopard" };
    private Watch watch;

    private IEnumerator Start() {
      Debug.LogError($"Must enable Group > Include Labels in Catalog");
      Debug.LogError($"This only work with Remote Editor Hosted or real Server (must clear Caching to see progress Tool > Addressables > Clear Caching)");
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      watch = new Watch().Start();
      yield return GetSize();
    }

    private IEnumerator GetSize() {
      var opHandle = Addressables.GetDownloadSizeAsync(keys);
      yield return opHandle;
      watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
      Debug.LogError($"GetDownloadSizeAsync {Utils.ConvertToKbyte(opHandle.Result)}Kb __ {Utils.ConvertToMbyte(opHandle.Result)}Mb");
    }
  }
}