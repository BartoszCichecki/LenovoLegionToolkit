using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Automation.Steps
{
    public interface IAutomationStep
    {
        Task RunAsync();

        IAutomationStep DeepCopy();
    }
}
