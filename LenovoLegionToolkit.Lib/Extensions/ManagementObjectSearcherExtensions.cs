using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Extensions;

public static class ManagementObjectSearcherExtensions
{
    public static Task<IEnumerable<ManagementBaseObject>> GetAsync(this ManagementObjectSearcher mos)
    {
        var list = new ConcurrentBag<ManagementBaseObject>();
        var tcs = new TaskCompletionSource<IEnumerable<ManagementBaseObject>>();
        var watcher = new ManagementOperationObserver();
        watcher.ObjectReady += (_, args) => list.Add(args.NewObject);
        watcher.Completed += (_, args) =>
        {
            if (args.Status == ManagementStatus.NoError)
                tcs.SetResult(list);
            else
                tcs.SetException(new ManagementException($"GetAsync failed with code {args.Status}."));
        };
        mos.Get(watcher);
        return tcs.Task;
    }
}