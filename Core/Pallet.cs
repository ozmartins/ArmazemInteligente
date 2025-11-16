using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

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