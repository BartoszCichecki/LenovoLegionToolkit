using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace LenovoLegionToolkit.WPF.Controls.Custom;

public class Badge : Wpf.Ui.Controls.Badge
{
    protected override AutomationPeer OnCreateAutomationPeer() => new BadgeAutomationPeer(this);

    private class BadgeAutomationPeer : FrameworkElementAutomationPeer
    {
        private readonly Badge _owner;

        public BadgeAutomationPeer(Badge owner) : base(owner) => _owner = owner;

        protected override string GetClassNameCore() => nameof(Badge);

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

        public override object? GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ItemContainer)
                return this;

            return base.GetPattern(patternInterface);
        }

        protected override AutomationPeer? GetLabeledByCore()
        {
            if (_owner.Content is UIElement element)
                return CreatePeerForElement(element);

            return base.GetLabeledByCore();
        }

        protected override string GetNameCore()
        {
            var result = base.GetNameCore() ?? string.Empty;

            if (result == string.Empty)
                result = AutomationProperties.GetName(_owner);

            if (result == string.Empty && _owner.Content is DependencyObject d)
                result = AutomationProperties.GetName(d);

            if (result == string.Empty && _owner.Content is string s)
                result = s;

            return result;
        }
    }
}
