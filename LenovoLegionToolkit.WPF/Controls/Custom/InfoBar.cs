using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;

namespace LenovoLegionToolkit.WPF.Controls.Custom;

public class InfoBar : Wpf.Ui.Controls.InfoBar
{
    protected override AutomationPeer OnCreateAutomationPeer() => new InfoBarAutomationPeer(this);

    private class InfoBarAutomationPeer : FrameworkElementAutomationPeer
    {
        private readonly InfoBar _owner;

        public InfoBarAutomationPeer(InfoBar owner) : base(owner) => _owner = owner;

        protected override string GetClassNameCore() => nameof(InfoBar);

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Pane;

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

            if (result == string.Empty)
                result = $"{_owner.Title}, {_owner.Message}";

            return result;
        }
    }
}
