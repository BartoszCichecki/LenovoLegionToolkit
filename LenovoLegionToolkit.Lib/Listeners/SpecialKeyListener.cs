using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
    {
        public SpecialKeyListener() : base("LENOVO_UTILITY_EVENT", "PressTypeDataVal", 0) { }

        protected override Task OnChangedAsync(SpecialKey value)
        {
            return Task.CompletedTask;
        }
    }
}
