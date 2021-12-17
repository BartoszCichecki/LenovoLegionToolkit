namespace LenovoLegionToolkit.Lib.Listeners
{
    public class KeyListener: AbstractWMIListener<Key>
    {
        public KeyListener(): base("LENOVO_UTILITY_EVENT", "PressTypeDataVal", 0) { }

        protected override void OnChanged(Key value)
        {
        }
    }
}
