using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmazemInteligente.Core;

public class DockAgent(string id, EventBus bus, Blackboard bb, Dock dock) : Agent(id, bus, bb) {
    private Dock _dock = dock;
    private bool _awaitingErp;
    private List<string> _requestedItems;

    protected override void Tick(GameTime gameTime) 
    {
        if (_dock.HasTruckArrived && Status == "idle") 
        {
            Status = "requesting_notes";
            Send("ERP", EnumPerformative.Request, "LogisticaRacoes", new { docaId = _dock.Id, truckId = _dock.TruckId });
            _awaitingErp = true;
            Bb.Log($"Dock {Id}: Truck { _dock.TruckId } arrived, requesting notes.");
        }
        if (_requestedItems != null && Status == "assigning_transport") 
        {
            var content = new 
            {
                action = "transport",
                dockId = _dock.Id,
                items = _requestedItems
            };
            Send("Forklift1", EnumPerformative.Request, "LogisticaRacoes", content);
            Status = "waiting_transport";
            Bb.Log($"Dock {Id}: Assigned transport to Forklift1.");
        }
    }

    protected override void OnMessage(Message msg) 
    {
        switch (msg.Performative) {
            case EnumPerformative.Inform:
                if (msg.Sender == "ERP") 
                {
                    var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
                    _requestedItems = payload.GetProperty("items").EnumerateArray().Select(x => x.GetString()).ToList();
                    Status = "assigning_transport";
                    Bb.Log($"Dock {Id}: Received ERP items: {_requestedItems.Count}.");
                } 
                else if (msg.Sender.StartsWith("Forklift")) 
                {
                    var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
                    var delivered = payload.GetProperty("delivered").GetBoolean();
                    if (delivered) 
                    {
                        var content = new 
                        {
                            action = "conference",
                            dockId = _dock.Id,
                            expected = _requestedItems
                        };
                        Status = "requesting_conference";
                        Send("Vision", EnumPerformative.Request, "LogisticaRacoes", content);
                        Bb.Log($"Dock {Id}: Requested conference.");
                    }
                } 
                else if (msg.Sender == "Vision") 
                {
                    var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
                    var result = payload.GetProperty("result").GetString();
                    if (result == "ok") 
                    {
                        _dock.Release();
                        Status = "released";
                        Bb.Log($"Dock {Id}: Conference OK. Dock released.");
                    } 
                    else 
                    {
                        Status = "divergence";
                        Bb.Log($"Dock {Id}: Divergence detected. Hold loading.");
                    }
                }
                break;
            case EnumPerformative.Failure:
                Bb.Log($"Dock {Id}: FAILURE from {msg.Sender} -> {msg.Content}");
                break;
        }
    }

    protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(_dock.Texture, _dock.Position, null, Color.White, MathHelper.ToRadians(0), new Vector2(_dock.Texture.Width / 2f, _dock.Texture.Height / 2f), 0.25f, SpriteEffects.None, 0f);
    }
}