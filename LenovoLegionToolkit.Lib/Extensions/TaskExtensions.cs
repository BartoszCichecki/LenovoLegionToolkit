using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Extensions
{
    public static class TaskExtensions
    {
        public static ValueTask AsValueTask(this Task task) => new(task);
    }
}
