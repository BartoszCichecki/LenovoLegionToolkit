using LenovoLegionToolkit.Lib.ADL;

Console.WriteLine("Initializing API...");

var adl = new ADLAPI();
var adapter = adl.GetDiscreteAdapter();

if (adapter.HasValue)
    Console.WriteLine($"Adapter found: {adapter.Value.AdapterIndex},{adapter.Value.IsActive}");
else
    Console.WriteLine("Adapter not found.");

Console.ReadKey();
