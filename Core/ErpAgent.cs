using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

public class ErpAgent(EventBus bus, Blackboard bb) : Agent("ERP", bus, bb) {
    protected override void Tick(GameTime gameTime) { }
    protected override void OnMessage(Message msg) 
    {
        if (msg.Performative == EnumPerformative.Request) 
        {
            var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
            var truckId = payload.GetProperty("truckId").GetString();
            var items = Bb.Data.TryGetValue($"notes:{truckId}", out var obj) ? (List<string>)obj : new List<string>();
            Send(msg.Sender, EnumPerformative.Inform, "LogisticaRacoes", new { items });
            Bb.Log($"ERP: Provided {items.Count} items for truck {truckId}.");
        }
    }
}