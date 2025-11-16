using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmazemInteligente.Core;

public abstract class Agent {
    public string Id { get; }
    protected EventBus Bus { get; }
    protected Blackboard Bb { get; }
    public string Status { get; protected set; } = "idle";
    
    protected Agent(string id, EventBus bus, Blackboard bb) 
    { 
        Id = id; 
        Bus = bus; 
        Bb = bb; 
        Bus.Register(id); 
    }
    
    public virtual void Update(GameTime gameTime) 
    {
        foreach (var msg in Bus.Drain(Id)) 
            OnMessage(msg);
        Tick(gameTime);
    }

    protected abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    
    protected abstract void Tick(GameTime gameTime);
    
    protected abstract void OnMessage(Message msg);
    
    protected void Send(string receiver, EnumPerformative perf, string ontology, object payload) 
    {
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        Bus.Send(new Message(Id, receiver, perf, ontology, "JSON", json));
    }
}