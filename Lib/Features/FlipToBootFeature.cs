using System.Runtime.InteropServices;

namespace LenovoLegionToolkit.Lib.Features
{
    public enum FlipToBootState
    {
        Off,
        On
    }

    public class FlipToBootFeature : AbstractUEFIFeature<FlipToBootState>
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FlipToBootStruct
        {
            [MarshalAs(UnmanagedType.U1)]
            public byte FlipToBootEn;
            [MarshalAs(UnmanagedType.U1)]
            public byte Reserved1;
            [MarshalAs(UnmanagedType.U1)]
            public byte Reserved2;
            [MarshalAs(UnmanagedType.U1)]
            public byte Reserved3;
        }

        protected override string Guid => "{D743491E-F484-4952-A87D-8D5DD189B70C}";
        protected override string ScopeName => "FBSWIF";
        protected override int ScopeAttribute => 7;

        public override FlipToBootState GetState()
        {
            var result = ReadFromUefi(new FlipToBootStruct
            {
                FlipToBootEn = 0,
                Reserved1 = 0,
                Reserved2 = 0,
                Reserved3 = 0
            });

            return result.FlipToBootEn == 0 ? FlipToBootState.Off : FlipToBootState.On;
        }

        public override void SetState(FlipToBootState state)
        {
            var structure = new FlipToBootStruct
            {
                FlipToBootEn = state == FlipToBootState.On ? (byte)1 : (byte)0,
                Reserved1 = 0,
                Reserved2 = 0,
                Reserved3 = 0
            };
            WriteToUefi(structure);
        }
    }
}
