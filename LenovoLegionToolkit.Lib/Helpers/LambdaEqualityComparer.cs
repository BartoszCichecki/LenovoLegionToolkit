using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LenovoLegionToolkit.Lib.Helpers;

internal class LambdaEqualityComparer<T> : EqualityComparer<T>
{
    public override bool Equals(T? x, T? y)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode([DisallowNull] T obj)
    {
        throw new NotImplementedException();
    }
}