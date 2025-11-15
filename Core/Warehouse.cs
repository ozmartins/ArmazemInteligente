using System.Collections.Generic;

namespace ArmazemInteligente.Core;

public class Warehouse 
{
    public List<Dock> Docks { get; } = [];
    public List<Pallet> Pallets { get; } = [];
    public List<Truck> Trucks { get; } = [];
}