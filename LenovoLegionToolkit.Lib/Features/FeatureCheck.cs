using System;

namespace LenovoLegionToolkit.Lib.Features
{
    public class FeatureCheck
    {
        public Action Check { get; }
        public Action Disable { get; }

        public FeatureCheck(Action check, Action disable)
        {
            Check = check;
            Disable = disable;
        }
    }
}
