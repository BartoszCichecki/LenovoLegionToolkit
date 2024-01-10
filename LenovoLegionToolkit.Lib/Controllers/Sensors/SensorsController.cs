using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Controllers.Sensors;

internal class SensorsController : ISensorsController
{
    private readonly SensorsControllerV1 _controllerV1;
    private readonly SensorsControllerV2 _controllerV2;
    private readonly SensorsControllerV3 _controllerV3;

    private ISensorsController? _controller;

    public SensorsController(SensorsControllerV1 controllerV1, SensorsControllerV2 controllerV2, SensorsControllerV3 controllerV3)
    {
        _controllerV1 = controllerV1 ?? throw new ArgumentNullException(nameof(controllerV1));
        _controllerV2 = controllerV2 ?? throw new ArgumentNullException(nameof(controllerV2));
        _controllerV3 = controllerV3 ?? throw new ArgumentNullException(nameof(controllerV3));
    }

    public async Task<bool> IsSupportedAsync() => await GetControllerAsync().ConfigureAwait(false) is not null;

    public async Task PrepareAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported controller found.");
        await controller.PrepareAsync().ConfigureAwait(false);
    }

    public async Task<SensorsData> GetDataAsync()
    {
        var controller = await GetControllerAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("No supported controller found.");
        return await controller.GetDataAsync().ConfigureAwait(false);
    }

    private async Task<ISensorsController?> GetControllerAsync()
    {
        if (_controller is not null)
            return _controller;

        if (await _controllerV3.IsSupportedAsync().ConfigureAwait(false))
            return _controller = _controllerV3;

        if (await _controllerV2.IsSupportedAsync().ConfigureAwait(false))
            return _controller = _controllerV2;

        if (await _controllerV1.IsSupportedAsync().ConfigureAwait(false))
            return _controller = _controllerV1;

        return null;
    }
}
