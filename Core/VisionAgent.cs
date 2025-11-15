using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

public class VisionAgent(EventBus bus, Blackboard bb) : Agent("Vision", bus, bb) 
{
    protected override void Tick(GameTime gameTime) { }

    protected override void OnMessage(Message msg) 
    {
        if (msg.Performative == EnumPerformative.Request) 
        {
            var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
            var dockId = payload.GetProperty("dockId").GetString();
            var expected = payload.TryGetProperty("expected", out var exp) ? exp.EnumerateArray().Select(x => x.GetString()).ToList() : [];
            var atDock = Bb.Warehouse.Pallets.Where(p => p.CurrentDockId == dockId).Select(p => p.ProductCode).ToList();
            var ok = expected.Count > 0 ? expected.OrderBy(x => x).SequenceEqual(atDock.OrderBy(x => x)) : atDock.Count > 0;
            Send("Dock1", EnumPerformative.Inform, "LogisticaRacoes", new { result = ok ? "ok" : "divergencia" });
            Bb.Log($"Vision: Conference at dock {dockId} -> {(ok ? "OK" : "Divergência")}.");
        }
    }
}