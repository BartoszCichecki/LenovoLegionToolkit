using System.Collections.Generic;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.WPF.CLI.Features;

public interface IFeatureRegistration
{
    string Name { get; }
    Task<bool> IsSupportedAsync();
    Task<IEnumerable<string>> GetValuesAsync();
    Task<string> GetValueAsync();
    Task SetValueAsync(string value);
}
