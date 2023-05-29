using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Addressables_Test {
  public class LoadNormalScene : MonoBehaviour {
    // This class is used to test with ReleaseAsset class
    // to check unload asset when a new scene loaded
    public float delayTime;
    public string sceneName;

    private IEnumerator Start() {
      yield return new WaitForSeconds(delayTime);
      yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
  }
}