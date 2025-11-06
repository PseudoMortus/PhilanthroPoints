using PhilanthroPoints.Models;


namespace PhilanthroPoints.Services;


public class CartState
{
    private readonly Dictionary<int,int> _lines = new(); // itemId -> qty
    public event Action? Changed;


    public void Add(Item item)
    {
        _lines[item.Id] = _lines.TryGetValue(item.Id, out var q) ? q + 1 : 1;
        Changed?.Invoke();
    }


    public void Remove(Item item)
    {
        if (_lines.TryGetValue(item.Id, out var q))
        {
            if (q <= 1) _lines.Remove(item.Id); else _lines[item.Id] = q - 1;
            Changed?.Invoke();
        }
    }


    public int Quantity(int itemId) => _lines.TryGetValue(itemId, out var q) ? q : 0;


    public IReadOnlyDictionary<int,int> Lines => _lines;


    public int TotalPoints(Func<int,int> priceById)
        => _lines.Sum(kv => priceById(kv.Key) * kv.Value);


    public void Clear(){ _lines.Clear(); Changed?.Invoke(); }
}