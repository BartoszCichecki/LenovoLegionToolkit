namespace LenovoLegionToolkit.Lib.Listeners
{
    public class PowerPlanListener : AbstractEventLogListener
    {
        public PowerPlanListener() : base("System", "*[System[Provider[@Name='Microsoft-Windows-UserModePowerService'] and EventID=12]]")
        {
        }

        protected override void OnChanged()
        {
        }
    }
}
