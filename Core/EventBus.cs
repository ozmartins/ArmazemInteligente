using System.Collections.Generic;

namespace ArmazemInteligente.Core;

public class EventBus {
    private readonly Dictionary<string, Queue<Message>> _inboxes = new();
    
    public void Register(string agentId) 
    {
        if (!_inboxes.ContainsKey(agentId)) 
            _inboxes[agentId] = new Queue<Message>();
    }
    
    public void Send(Message msg) 
    {
        if (_inboxes.TryGetValue(msg.Receiver, out var q)) 
            q.Enqueue(msg);
    }
    
    public IEnumerable<Message> Drain(string agentId) 
    {
        if (!_inboxes.TryGetValue(agentId, out var q)) yield break;
        while (q.Count > 0) 
            yield return q.Dequeue();
    }
}