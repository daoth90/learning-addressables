using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnityWebRequest_GetFileSize : MonoBehaviour {
  /*
   * https://forum.unity.com/threads/check-the-size-of-a-www-file.521070/#post-3418645
   * https://stackoverflow.com/a/51672775
   * 1. Send HEAD request instead of GET (response sẽ chỉ có HEADER)
   * 2. Get HTTP header from the server (if the server supports it)
   * 3. Content-Length header chính là file size
   */
  
  // why this url doesn't work https://speed.hetzner.de/100MB.bin
  
  // https://testfiledownload.com/
  private string fileUrl = "http://speedtest.ftp.otenet.gr/files/test1Mb.db"; // 1MB
  [SerializeField] private Text logTxt;
  
  private IEnumerator Start() {
    yield return new WaitForSeconds(1f);
    yield return GetFileSize(fileUrl);
  }

  private IEnumerator GetFileSize(string url) {
    using (UnityWebRequest uwr = UnityWebRequest.Head(url)) {
      yield return uwr.SendWebRequest();
      string contentLength = uwr.GetResponseHeader("Content-Length");
      if (uwr.result == UnityWebRequest.Result.Success) {
        string log = $"File size {Utils.ConvertToKbyte(contentLength)}KB or {Utils.ConvertToMbyte(contentLength)}MB" +
                     $"\nurl {url}";
        logTxt.text = log;
        Debug.LogError(log);
      }
      else {
        Debug.Log($"Error While Get File size {uwr.error} __ url {url}");
      }
    }
  }
}