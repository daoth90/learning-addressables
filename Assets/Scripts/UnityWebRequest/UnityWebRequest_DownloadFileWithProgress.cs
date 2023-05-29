using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnityWebRequest_DownloadFileWithProgress : MonoBehaviour {
  /*
   * https://stackoverflow.com/a/58353717
   * https://stackoverflow.com/a/56273055
   * 1. Create a request with UnityWebRequest
   * 2. Send the request WITHOUT a yield
   *     2.1 uwr.SendWebRequest()
   * 3. yield return null cho tới khi uwr.isDone == true
   * 4. khi uwr.isDone == true
   * 5. yield cho tới khi www.downloadHandler.isDone == true # thường sẽ true cùng lúc với uwr
   * 6. DONE
   * NOTE: nếu ko có progress thì kiểm tra xem server có trả về Content-Length header hay ko
   */
  
  [SerializeField] private Text logTxt;

  private string fileUrl = "http://speedtest.ftp.otenet.gr/files/test1Mb.db"; // 1MB
  // https://testfiledownload.com/

  private IEnumerator Start() {
    yield return new WaitForSeconds(1f);
    using (UnityWebRequest uwr = UnityWebRequest.Get(fileUrl)) {
      uwr.SendWebRequest();
      while (!uwr.isDone) {
        LogProgress(uwr);
        yield return null;
      }

      LogProgress(uwr);
      while (!uwr.downloadHandler.isDone) {
        Debug.LogError($"wait downloadHandler");
        yield return null;
      }

      Debug.LogError("Download done");
    }
  }

  private void LogProgress(UnityWebRequest uwr) {
    float downloadProgress = uwr.downloadProgress * 100;
    long downloadedBytes = Utils.ConvertToKbyte((long)uwr.downloadedBytes); // KB
    string log = $"downloadProgress {downloadProgress:F1}%" +
                 $"\ndownloadedBytes {downloadedBytes}KB";
    logTxt.text = log;
    Debug.LogError(log);
  }
}