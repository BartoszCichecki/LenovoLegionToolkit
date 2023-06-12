using System;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class VersionExtensions
{
    public static bool IsBeta(this Version version) => version switch
    {
        { Major: 0, Minor: 0, Build: 1, Revision: 0 } => true,
        { Build: 99 } => true,
        _ => false
    };
}
