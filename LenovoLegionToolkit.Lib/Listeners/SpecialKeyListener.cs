namespace LenovoLegionToolkit.Lib.Listeners
{
    public class SpecialKeyListener : AbstractWMIListener<SpecialKey>
    {
        public SpecialKeyListener() : base("LENOVO_UTILITY_EVENT", "PressTypeDataVal", 0) { }

        protected override void OnChanged(SpecialKey value)
        {
        }
    }
}
