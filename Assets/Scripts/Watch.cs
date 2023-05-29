using System;
using UnityEngine;

public class Watch {
  private DateTimeOffset cache;

  public Watch Start() {
    cache = DateTimeOffset.Now;
    return this;
  }

  public double Stop() {
    TimeSpan timeSpan = DateTimeOffset.Now - cache;
    return timeSpan.TotalMilliseconds;
  }

  public void StopAndLog(string prefix) {
    double milliseconds = Stop();
    Debug.LogError($"{prefix} __ {milliseconds} (Milliseconds)");
  }
}