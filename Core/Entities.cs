using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

public class Dock(string id, Vector2 pos)
{
    public string Id { get; } = id; public Vector2 Position { get; } = pos; public bool HasTruckArrived { get; set; }
    public string TruckId { get; set; }
    public bool Released { get; private set; }

    public void Release() { Released = true; HasTruckArrived = false; }
}

public class Truck(string id, string dockId)
{
    public string Id { get; } = id; public string DockId { get; } = dockId;
}

public class Pallet(string id, string productCode, Vector2 pos)
{
    public string Id { get; } = id; 
    public string ProductCode { get; } = productCode; 
    public Vector2 Position { get; private set; } = pos; 
    public bool Reserved { get; set; }
    public string CurrentDockId { get; private set; }

    public void AttachToForklift(string forkliftId) 
    { 
        /* mark attached */ 
    
    }
    public void DropAtDock(string dockId) 
    { 
        CurrentDockId = dockId; 
    }
}