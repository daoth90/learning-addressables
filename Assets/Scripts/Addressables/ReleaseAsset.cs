using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables_Test {
  public class ReleaseAsset : MonoBehaviour {
    private LoadAsset loaderChimp;
    private LoadAsset loaderJaguar;
    private LoadAsset loaderGaur;
    private IEnumerator Start() {
      yield return Delay(2f);
      ResourceManager.ExceptionHandler = Utils.ExceptionHandler; // Exception Handler
      loaderChimp = new LoadAsset("Chimp", transform);
      loaderJaguar = new LoadAsset("Jaguar", transform);
      loaderGaur = new LoadAsset("Gaur", transform);
      yield return Case_1_BundleRefCounter_CannotUnloadBundlePartial();
      // yield return Case_2_DependencyIsBasedOnBundle_NotOnAsset();
      // yield return Case_3_ReleaseInstance();
      // yield return Case_4_ReleaseInstance();
    }

    private IEnumerator Delay(float second) {
      yield return new WaitForSeconds(second);
    }

    private IEnumerator Case_1_BundleRefCounter_CannotUnloadBundlePartial() {
      yield return loaderJaguar.Load(); // asset Jaguar counter += 1 -> = 1, bundle counter += 1 -> = 1
      yield return loaderGaur.Load(); // asset Gaur counter += 1 -> = 1, bundle counter += 1 -> = 2
      yield return Delay(1f);
      yield return loaderGaur.Load(); // asset Gaur counter += 1 -> = 2, bundle counter = 2
      yield return Delay(1f);
      loaderGaur.Release(destroyGameobjectInstance: true); // asset Gaur counter -= 1 -> = 1, bundle counter 2
      yield return Delay(1f);
      // asset Gaur counter 0, bundle counter 1 (because asset Jaguar counter > 0)
      // at this point, Gaur resource like texture, mesh, animation clip still in memory because of bundle ref counter still > 0
      // -> CAN'T UNLOAD BUNDLE PARTIAL
      loaderGaur.Release(destroyGameobjectInstance: true);
      yield return Delay(1f);
      // asset Jaguar counter 0, bundle counter 0
      // at this point, Gaur and Jaguar resource and bundle are unloaded from memory as bundle ref counter = 0
      loaderJaguar.Release(destroyGameobjectInstance: true);
    }

    private IEnumerator Case_2_DependencyIsBasedOnBundle_NotOnAsset() {
      // bundle A có asset Gaur, Jaguar. Bundle B có asset Chimp_Animation, Jaguar_Animation
      // Jaguar_Animation là dependency của asset Jaguar (trong bundle A)
      // -----------------
      // ref counter của bundle B += 1 -> = 1 vì bundle B được coi là dependency của bundle A (check Addressables Event Viewer )
      // nhưng asset trong bundle B chưa được load vào memory (check memory profiler)
      yield return loaderGaur.Load();
      yield return Delay(5f);
      // ref counter của bundle B += 1 -> = 2 (check Addressables Event Viewer )
      // asset Jaguar_Animation trong bundle B được load vào memory (check memory profiler)
      yield return loaderJaguar.Load();
    }

    private IEnumerator Case_3_ReleaseInstance() {
      // <param name="trackHandle">If true, Addressables will track this request to allow it to be released via the result object.</param>
      // Mỗi lần gọi InstantiateAsync đều tăng ref counter tới asset đó ("Jaguar") trong bundle lên 1, ref counter của bundle vẫn = 1
      // opHandle.Result là instance sau khi Instantiate
      // Không cần cache opHandle (param trackHandle PHẢI = true), chỉ cần gọi ReleaseInstance(gameobject) khi instance bị destroy
      // Nếu cache opHandle thì dùng Release hoặc ReleaseInstance như bình thường
      var opHandle = Addressables.InstantiateAsync("Jaguar", transform, false, trackHandle: true);
      yield return opHandle;
      Debug.LogError($"InstantiateAsync 1", opHandle.Result); // opHandle.Result là instance sau khi Instantiate
      yield return Delay(1f);
      yield return Addressables.InstantiateAsync("Jaguar", transform, false, trackHandle: true);
      Debug.LogError($"InstantiateAsync 2");
      yield return Delay(1f);
      Addressables.ReleaseInstance(transform.GetChild(0).gameObject);
      Debug.LogError($"ReleaseInstance 1");
      yield return Delay(1f);
      Addressables.ReleaseInstance(transform.GetChild(0).gameObject);
      Debug.LogError($"ReleaseInstance 2");
    }

    private IEnumerator Case_4_ReleaseInstance() {
      // can use ReleaseInstance but if you used LoadAssetAsync -> it only work with AsyncOperationHandle
      var opHandle = Addressables.LoadAssetAsync<GameObject>("Jaguar");
      yield return opHandle;
      Instantiate(opHandle.Result, transform);
      yield return Delay(1f);
      Addressables.ReleaseInstance(opHandle);
    }

    public class LoadAsset {
      private string addressName;
      private Transform parent;
      private AsyncOperationHandle<GameObject> opHandle;
      private Watch watch;
      public GameObject gameObject { get; private set; }

      public LoadAsset(string addressName, Transform parent) {
        this.addressName = addressName;
        this.parent = parent;
      }

      public IEnumerator Load() {
        watch = new Watch().Start();
        opHandle = Addressables.LoadAssetAsync<GameObject>(addressName);
        // yielding when already done still waits until the next frame
        // so don't yield if done.
        if (!opHandle.IsDone) {
          yield return opHandle;
        }

        watch.StopAndLog($"opHandle.Status {opHandle.Status.ToString()}");
        if (opHandle.Status == AsyncOperationStatus.Succeeded) {
          if (gameObject == null) {
            gameObject = Instantiate(opHandle.Result, parent); // ko làm tăng counter tới asset/bundle
          }
        }
        else {
          Debug.LogError($"opHandle.OperationException {opHandle.OperationException}");
          Addressables.Release(opHandle);
        }
      }

      public void Release(bool destroyGameobjectInstance = false) {
        if (destroyGameobjectInstance && gameObject != null) {
          Destroy(gameObject);
        }

        if (opHandle.IsValid()) {
          // Checks to make sure that handle hasn't already been released.
          Addressables.Release(opHandle);
        }
      }
    }
  }
}