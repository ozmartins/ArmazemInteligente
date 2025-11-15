using System;

namespace ArmazemInteligente.Core;

public class Message(
    string sender,
    string receiver,
    EnumPerformative perf,
    string ontology,
    string language,
    string content)
{ 
    public string Sender { get; } = sender;
    public string Receiver { get; } = receiver;
    public EnumPerformative Performative { get; } = perf;
    public string Ontology { get; } = ontology; // e.g., "LogisticaRacoes"
    public string Language { get; } = language; // e.g., "JSON"
    public string Content { get; } = content; // JSON payload
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}