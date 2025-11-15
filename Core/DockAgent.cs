using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

public class DockAgent : Agent {
    private Dock _dock;
    private bool _awaitingErp;
    private List<string> _requestedItems;
    public DockAgent(string id, EventBus bus, Blackboard bb, Dock dock) : base(id, bus, bb) { _dock = dock; }

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
                } else if (msg.Sender == "Vision") {
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
}