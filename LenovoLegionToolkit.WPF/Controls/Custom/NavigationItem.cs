using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace LenovoLegionToolkit.WPF.Controls.Custom;

public class NavigationItem : Wpf.Ui.Controls.NavigationItem
{
    protected override AutomationPeer OnCreateAutomationPeer() => new NavigationItemAutomationPeer(this);

    private class NavigationItemAutomationPeer : FrameworkElementAutomationPeer
    {
        private readonly NavigationItem _owner;

        public NavigationItemAutomationPeer(NavigationItem owner) : base(owner) => _owner = owner;

        protected override string GetClassNameCore() => nameof(NavigationItem);

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Pane;

        public override object? GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ItemContainer)
                return this;

            return base.GetPattern(patternInterface);
        }

        protected override string GetNameCore()
        {
            var result = base.GetNameCore() ?? string.Empty;

            if (result == string.Empty)
                result = AutomationProperties.GetName(_owner);

            return result;
        }
    }
}
