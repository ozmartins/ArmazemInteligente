using System;
using System.Collections.Generic;

namespace ArmazemInteligente.Core;

public class Blackboard {
    public Warehouse Warehouse { get; } = new Warehouse();
    public Dictionary<string, object> Data { get; } = [];
    public List<string> Logs { get; } = [];
    public void Log(string entry) => Logs.Add($"[{DateTime.Now:HH:mm:ss}] {entry}");
}