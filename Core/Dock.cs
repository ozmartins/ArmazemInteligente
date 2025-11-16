using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmazemInteligente.Core;

public class Dock(int id, Vector2 pos, Texture2D texture)
{
    public int Id { get; } = id;
    public Vector2 Position { get; } = pos;
    public Texture2D Texture = texture;
    public bool HasTruckArrived { get; set; }
    public string TruckId { get; set; }
    public bool Released { get; private set; }

    public void Release() { Released = true; HasTruckArrived = false; }
}
