using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

public class SaveProfilerData : MonoBehaviour {
  private const int saveFramePerFile = 299; // save profiled frame per file
  private Queue<int> queueRun = new Queue<int>();
  public bool isRunning { get; private set; }
  private string GetFilePath(string folderPath, int count) => Path.Combine(folderPath, $"{count}");

  private void Start() {
    DontDestroyOnLoad(gameObject);
    Run();
  }

  private void Update() {
    if (!isRunning && queueRun.Count > 0) {
      Run(queueRun.Dequeue());
    }
  }

  public bool Run(int saveFilePerSession = 5) {
    if (isRunning) {
      queueRun.Enqueue(saveFilePerSession);
      return false;
    }

    StartCoroutine(Save(saveFilePerSession));
    return true;
  }

  private void EnableProfilerArea(params ProfilerArea[] areas) {
    foreach (ProfilerArea area in Enum.GetValues(typeof(ProfilerArea))) {
      // Debug.LogError($"{area.ToString()} enabled {Profiler.GetAreaEnabled(area)}");
      Profiler.SetAreaEnabled(area, false);
    }

    for (int i = 0; i < areas.Length; i++) {
      Profiler.SetAreaEnabled(areas[i], true);
    }
  }

  private IEnumerator Save(int saveFilePerSession) {
    Debug.LogError($"SaveProfilerData isRunning = true");
    isRunning = true;
    int count = 0;
    string folderPath = Path.Combine(Application.persistentDataPath, "Profiler Data", DateTimeOffset.Now.ToString("yyyy-mm-dd HH:mm:ss:mm.ffff")); // HH = 24h format, .ffff = millisecond
    Directory.CreateDirectory(folderPath);
    EnableProfilerArea(ProfilerArea.CPU, ProfilerArea.Rendering, ProfilerArea.GPU, ProfilerArea.Memory);
    while (count < saveFilePerSession) {
      // set the log file and enable the profiler
      string filePath = GetFilePath(folderPath, count);
      Profiler.logFile = filePath;
      Debug.LogError($"SaveProfilerData.Save() {count}/{saveFilePerSession} __ filePath {filePath}");
      Profiler.enableBinaryLog = true;
      Profiler.enabled = true;
      Profiler.maxUsedMemory = 256 * 1024 * 1024;

      for (int i = 0; i < saveFramePerFile; i++) {
        yield return new WaitForEndOfFrame();
        // Unity force Profiler.enabled to false in next frame for performance reason
        // so this is workaround to keep the Profiler working
        if (!Profiler.enabled) {
          Profiler.enabled = true;
        }
      }

      // continue with new file
      ++count;
    }

    Debug.LogError($"SaveProfilerData isRunning = false");
    isRunning = false;
  }
}