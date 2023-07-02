using LenovoLegionToolkit.Lib.ADL;

Console.WriteLine("Initializing API...");

using var adl = new ADLAPI();

var adapter = adl.GetDiscreteAdapter();

if (adapter is null)
{
    Console.WriteLine("Adapter not found.");
    Console.ReadKey();
    return;
}

Console.WriteLine($"Adapter found: {adapter.AdapterIndex},{adapter.IsActive}");

var odStatus = adl.GetOverdriveInfo(adapter);
Console.WriteLine($"Overdrive status: {odStatus.supported},{odStatus.enabled},{odStatus.version}");


Console.ReadKey();

