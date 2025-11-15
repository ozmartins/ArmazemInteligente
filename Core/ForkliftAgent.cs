using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ArmazemInteligente.Core;

public class ForkliftAgent(string id, EventBus bus, Blackboard bb, Vector2 startPos) : Agent(id, bus, bb) {
    private Vector2 _pos = startPos;
    private Queue<Pallet> _tasks = new();
    private Dock _targetDock;

    protected override void Tick(GameTime gameTime) 
    {
        if (_tasks.Count > 0) 
        {
            var pallet = _tasks.Peek();
            MoveTowards(pallet.Position, gameTime);
            if (Vector2.Distance(_pos, pallet.Position) < 4f) 
            {
                pallet.AttachToForklift(Id);
                MoveTowards(_targetDock.Position, gameTime);
                if (Vector2.Distance(_pos, _targetDock.Position) < 4f) 
                {
                    pallet.DropAtDock(_targetDock.Id);
                    _tasks.Dequeue();
                    Send("Dock1", EnumPerformative.Inform, "LogisticaRacoes", new { delivered = true, palletId = pallet.Id });
                    if (_tasks.Count == 0) Send("Vision", EnumPerformative.Request, "LogisticaRacoes", new { action = "conference", dockId = _targetDock.Id });
                    Bb.Log($"{Id}: Delivered pallet {pallet.Id} to dock {_targetDock.Id}.");
                }
            }
        }
    }

    protected override void OnMessage(Message msg) 
    {
        if (msg.Performative == EnumPerformative.Request) {
            var payload = System.Text.Json.JsonDocument.Parse(msg.Content).RootElement;
            if (payload.GetProperty("action").GetString() == "transport") 
            {
                var dockId = payload.GetProperty("dockId").GetString();
                _targetDock = Bb.Warehouse.Docks.First(d => d.Id == dockId);
                foreach (var item in payload.GetProperty("items").EnumerateArray()) 
                {
                    var code = item.GetString();
                    var pallet = Bb.Warehouse.Pallets.FirstOrDefault(p => p.ProductCode == code && !p.Reserved);
                    if (pallet != null) 
                    {
                        pallet.Reserved = true;
                        _tasks.Enqueue(pallet);
                    }
                }
                Status = "transporting";
                Bb.Log($"{Id}: Accepted { _tasks.Count } transport tasks to dock {dockId}.");
            }
        }
    }

    private void MoveTowards(Vector2 target, GameTime gt) 
    {
        var dir = target - _pos;
        if (dir.LengthSquared() > 0.0001f) 
        {
            dir.Normalize();
            _pos += dir * (float)(80 * gt.ElapsedGameTime.TotalSeconds);
        }
    }
}