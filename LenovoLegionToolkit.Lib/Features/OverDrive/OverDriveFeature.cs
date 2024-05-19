namespace LenovoLegionToolkit.Lib.Features.OverDrive;

public class OverDriveFeature(OverDriveCapabilityFeature feature1, OverDriveGameZoneFeature feature2)
    : AbstractCompositeFeature<OverDriveState>(feature1, feature2);
