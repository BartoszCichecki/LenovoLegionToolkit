//
// Copyright (c) 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDISPLAYSETTING_H
#define ADLX_IDISPLAYSETTING_H

#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"

#pragma region IADLXDisplayFreeSync interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayFreeSync : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayFreeSync")
        /**
        *@page DOX_IADLXDisplayFreeSync_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the AMD FreeSync™ is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD FreeSync is returned. The variable is __true__ if AMD FreeSync is supported. The variable is __false__ if AMD FreeSync is not supported.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD FreeSync is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of AMD FreeSync is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  AMD FreeSync technology synchronizes the refresh rate of a display with the framerate of AMD FreeSync compatible graphics card to deliver dynamic refresh rate.<br>
        *
        * AMD FreeSync technology reduces or eliminates visual artifacts, input latency, screen tearing and stuttering during gaming and video playback. AMD FreeSync technology can be delivered through DisplayPort and HDMI® connections. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayFreeSync_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if the AMD FreeSync™ is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD FreeSync is returned. The variable is __true__ if AMD FreeSync is enabled. The variable is __false__ if AMD FreeSync is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD FreeSync is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of AMD FreeSync is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  AMD FreeSync technology synchronizes the refresh rate of a display with the framerate of AMD FreeSync compatible graphics card to deliver dynamic refresh rate.<br>
        *
        * AMD FreeSync technology reduces or eliminates visual artifacts, input latency, screen tearing and stuttering during gaming and video playback. AMD FreeSync technology can be delivered through DisplayPort and HDMI® connections. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayFreeSync_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the AMD FreeSync™ to enabled or disabled state on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD FreeSync state. Set __true__ to enable AMD FreeSync. Set __false__ to disable AMD FreeSync. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the AMD FreeSync enabled status is successfully returned, __ADLX_OK__ is returned. <br>
        * If the AMD FreeSync enabled status is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  AMD FreeSync technology synchronizes the refresh rate of a display with the framerate of AMD FreeSync compatible graphics card to deliver dynamic refresh rate.<br>
        *
        * AMD FreeSync technology reduces or eliminates visual artifacts, input latency, screen tearing and stuttering during gaming and video playback. AMD FreeSync technology can be delivered through DisplayPort and HDMI® connections. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
    };
    typedef IADLXInterfacePtr_T<IADLXDisplayFreeSync> IADLXDisplayFreeSyncPtr;
}

#else
ADLX_DECLARE_IID (IADLXDisplayFreeSync, L"IADLXDisplayFreeSync")
typedef struct IADLXDisplayFreeSync IADLXDisplayFreeSync;

typedef struct IADLXFreeSyncVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayFreeSync* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayFreeSync* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayFreeSync* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDisplayFreeSync
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayFreeSync* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLXDisplayFreeSync* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLXDisplayFreeSync* pThis, adlx_bool enabled);
} IADLXFreeSyncVtbl;

struct IADLXDisplayFreeSync
{
    const IADLXFreeSyncVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayFreeSync interface

#pragma region IADLXDisplayVSR interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayVSR : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayVSR")
        /**
        *@page DOX_IADLXDisplayVSR_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Virtual Super Resolution is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Virtual Super Resolution is returned. The variable is __true__ if Virtual Super Resolution is supported. The variable is __false__ if Virtual Super Resolution is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Virtual Super Resolution is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the state of AMD Virtual Super Resolution is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Virtual Super Resolution allows applications to render at resolutions higher than the display's native pixel grid and then scales images down to fit the display, producing higher quality visuals at the expense of performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayVSR_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Virtual Super Resolution is enabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Virtual Super Resolution is returned. The variable is __true__ if Virtual Super Resolution is enabled. The variable is __false__ if Virtual Super Resolution is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Virtual Super Resolution is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the state of AMD Virtual Super Resolution is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Virtual Super Resolution allows applications to render at resolutions higher than the display's native pixel grid and then scales images down to fit the display, producing higher quality visuals at the expense of performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayVSR_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the AMD Virtual Super Resolution to enabled or disabled on this display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Virtual Super Resolution state. Set __true__ to enable Virtual Super Resolution. Set __false__ to disable Virtual Super Resolution. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Virtual Super Resolution is successfully set, __ADLX_OK__ is returned.<br/>
		* If the state of AMD Virtual Super Resolution is not successfully set, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Virtual Super Resolution allows applications to render at resolutions higher than the display's native pixel grid and then scales images down to fit the display, producing higher quality visuals at the expense of performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
    };
    typedef IADLXInterfacePtr_T<IADLXDisplayVSR> IADLXDisplayVSRPtr;
}

#else
ADLX_DECLARE_IID (IADLXDisplayVSR, L"IADLXDisplayVSR")
typedef struct IADLXDisplayVSR IADLXDisplayVSR;

typedef struct IADLXVSRVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayVSR* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayVSR* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayVSR* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDisplayVSR
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayVSR* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLXDisplayVSR* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLXDisplayVSR* pThis, adlx_bool enabled);
} IADLXVSRVtbl;

struct IADLXDisplayVSR
{
    const IADLXVSRVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayVSR interface

#pragma region IADLXDisplayGPUScaling interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGPUScaling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayGPUScaling")
        /**
        *@page DOX_IADLXDisplayGPUScaling_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the GPU scaling is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU scaling is returned. The variable is __true__ if GPU scaling is supported. The variable is __false__ if GPU scaling is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU scaling is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of GPU scaling is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling uses the GPU to scale up lower resolutions to fit the display. <br>
        * GPU scaling requires a digital connection (DVI, HDMI or DisplayPort™) from the display to the GPU. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayGPUScaling_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if the GPU scaling is enabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU scaling is returned. The variable is __true__ if GPU scaling is enabled. The variable is __false__ if GPU scaling is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU scaling is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of GPU scaling is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling uses the GPU to scale up lower resolutions to fit the display. <br>
        * GPU scaling requires a digital connection (DVI, HDMI or DisplayPort™) from the display to the GPU. <br>
        * __Note__: @ref DOX_IADLXDisplayIntegerScaling "Integer Display Scaling" is not supported when GPU scaling is disabled.<br>
        * GPU scaling is required for @ref DOX_IADLX3DRadeonSuperResolution "Radeon™ Super Resolution". GPU scaling is automatically enabled when Radeon Super Resolution is enabled. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayGPUScaling_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the GPU scaling to enabled or disabled state on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new GPU scaling state. Set __true__ to enable GPU scaling. Set __false__ to disable GPU scaling. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU scaling is successfully set, __ADLX_OK__ is returned. <br>
        * If the state of GPU scaling is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling uses the GPU to scale up lower resolutions to fit the display. <br>
        * GPU scaling requires a digital connection (DVI, HDMI or DisplayPort™) from the display to the GPU.<br>
        * __Note:__ @ref DOX_IADLXDisplayIntegerScaling "Integer Display Scaling" is not supported when GPU scaling is disabled. <br>
        * GPU scaling is required for @ref DOX_IADLX3DRadeonSuperResolution "Radeon™ Super Resolution". By disabling GPU scaling when Radeon Super Resolution is enabled, Radeon Super Resolution is automatically disabled.@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayGPUScaling> IADLXDisplayGPUScalingPtr;
}

#else
ADLX_DECLARE_IID (IADLXDisplayGPUScaling, L"IADLXDisplayGPUScaling")
typedef struct IADLXDisplayGPUScaling IADLXDisplayGPUScaling;

typedef struct IADLXGPUScalingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayGPUScaling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayGPUScaling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayGPUScaling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // GPU Scaling
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayGPUScaling* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLXDisplayGPUScaling* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLXDisplayGPUScaling* pThis, adlx_bool enabled);
} IADLXGPUScalingVtbl;

struct IADLXDisplayGPUScaling
{
    const IADLXGPUScalingVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayGPUScaling interface

#pragma region IADLXDisplayScalingMode interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayScalingMode : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayScalingMode")
        /**
        *@page DOX_IADLXDisplayScalingMode_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the scaling mode is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of scaling mode is returned. The variable is __true__ if scaling mode is supported. The variable is __false__ if scaling mode is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of scaling mode is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of scaling mode is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling determines the method used to stretch and position images to fit the display. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayScalingMode_GetMode GetMode
        *@ENG_START_DOX @brief Gets the current scaling mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMode(@ref ADLX_SCALE_MODE* currentMode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMode,@ref ADLX_SCALE_MODE*,@ENG_START_DOX  The pointer to a variable where the current scaling mode of a display is returned.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current scaling mode is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current scaling mode is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling determines the method used to stretch and position images to fit the display.<br>
        * __Note:__ On some AMD GPUs, center scaling and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Super Resolution is enabled when the scaling mode is __CENTERED__, the scaling mode is automatically set to __FULL_PANEL__.@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetMode (ADLX_SCALE_MODE* currentMode) = 0;
        /**
        *@page DOX_IADLXDisplayScalingMode_SetMode SetMode
        *@ENG_START_DOX @brief Sets the scaling mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMode(@ref ADLX_SCALE_MODE mode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],mode,@ref ADLX_SCALE_MODE,@ENG_START_DOX The new scaling mode.0@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the scaling mode is successfully set, __ADLX_OK__ is returned. <br>
        * If the scaling mode is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  GPU scaling determines the method used to stretch and position images to fit the display.<br>
        * __Note:__ On some AMD GPUs, center scaling and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If the scaling mode is set to __CENTERED__, Radeon Super Resolution is automatically disabled.@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetMode (ADLX_SCALE_MODE mode) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayScalingMode> IADLXDisplayScalingModePtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayScalingMode, L"IADLXDisplayScalingMode")

typedef struct IADLXDisplayScalingMode IADLXDisplayScalingMode;
typedef struct IADLXDisplayScalingModeVtbl
{
    // IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayScalingMode* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayScalingMode* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayScalingMode* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Scaling mode
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayScalingMode* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetMode)(IADLXDisplayScalingMode* pThis, ADLX_SCALE_MODE* currentMode);
    ADLX_RESULT (ADLX_STD_CALL *SetMode)(IADLXDisplayScalingMode* pThis, ADLX_SCALE_MODE mode);
} IADLXDisplayScalingModeVtbl;

struct IADLXDisplayScalingMode
{
    const IADLXDisplayScalingModeVtbl *pVtbl;
};

#endif
#pragma endregion IADLXDisplayScalingMode interface

#pragma region IADLXDisplayIntegerScaling interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayIntegerScaling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayIntegerScaling")
        /**
        *@page DOX_IADLXDisplayIntegerScaling_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the Integer Display Scaling is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Integer Display Scaling is returned. The variable is __true__ if Integer Display Scaling is supported. The variable is __false__ if Integer Display Scaling is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Integer Display Scaling is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of Integer Display Scaling is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX  Integer Display Scaling gives a sharp, pixelated look to images scaled up to fit the display. Images that can't be scaled to match the display's exact size and shape will be centered on screen. Integer Display Scaling enhances visuals in old games to revive vintage gaming experiences on a modern display.<br>
        * __Note__: Integer Display Scaling is not supported when @ref DOX_IADLXDisplayGPUScaling "GPU scaling" is disabled. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayIntegerScaling_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if the Integer Display Scaling is enabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Integer Display Scaling is returned. The variable is __true__ if Integer Display Scaling is enabled. The variable is __false__ if Integer Display Scaling is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Integer Display Scaling is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of Integer Display Scaling is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  Integer Display Scaling gives a sharp, pixelated look to images scaled up to fit the display. Images that can't be scaled to match the display's exact size and shape will be centered on screen. Integer Display Scaling enhances visuals in old games to revive vintage gaming experiences on a modern display.<br>
        * __Note:__ On some AMD GPUs, Integer Display Scaling and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Super Resolution is enabled, Integer Display Scaling is automatically disabled.@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayIntegerScaling_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the AMD integer scaling to enabled or disabled state on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new Integer Display Scaling state. Set __true__ to enable Integer Display Scaling. Set __false__ to disable Integer Display Scaling. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Integer Display Scaling is successfully set, __ADLX_OK__ is returned. <br>
        * If the state of Integer Display Scaling is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  Integer Display Scaling gives a sharp, pixelated look to images scaled up to fit the display. Images that can't be scaled to match the display's exact size and shape will be centered on screen. Integer Display Scaling enhances visuals in old games to revive vintage gaming experiences on a modern display.<br>
        * __Note:__ On some AMD GPUs, Integer Display Scaling and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Integer Display Scaling is enabled, Radeon Super Resolution is automatically disabled.@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
    };
    typedef IADLXInterfacePtr_T<IADLXDisplayIntegerScaling> IADLXDisplayIntegerScalingPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayIntegerScaling, L"IADLXDisplayIntegerScaling")

typedef struct IADLXDisplayIntegerScaling IADLXDisplayIntegerScaling;
typedef struct IADLXIntegerScalingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayIntegerScaling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayIntegerScaling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayIntegerScaling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Integer scaling
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayIntegerScaling* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLXDisplayIntegerScaling* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLXDisplayIntegerScaling* pThis, adlx_bool enabled);
} IADLXIntegerScalingVtbl;

struct IADLXDisplayIntegerScaling
{
    const IADLXIntegerScalingVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayIntegerScaling interface

#pragma region IADLXDisplayColorDepth interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayColorDepth : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayColorDepth")
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the color format can be configured on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color format configuration is returned.   The variable is __true__ if color format configuration is supported. The variable is __false__ if color format configuration is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the color format configuration is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of the color format configuration is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  Color format configuration is supported on some AMD GPUs if the display is connected to the GPU using Dual-Link DVI or DisplayPort cable. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_GetValue GetValue
        *@ENG_START_DOX @brief Gets the current color format of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetValue(@ref ADLX_COLOR_DEPTH* currentColorDepth)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentColorDepth,@ref ADLX_COLOR_DEPTH*,@ENG_START_DOX The pointer to a variable where the color format is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current color format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current color format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  Color format configuration is supported on some AMD GPUs if the display is connected to the GPU using Dual-Link DVI or DisplayPort cable. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetValue (ADLX_COLOR_DEPTH* currentColorDepth) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_SetValue SetValue
        *@ENG_START_DOX @brief Sets the color format on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetValue(@ref ADLX_COLOR_DEPTH colorDepth)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],colorDepth,@ref ADLX_COLOR_DEPTH,@ENG_START_DOX The new color format. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the color format is successfully set, __ADLX_OK__ is returned. <br>
        * If the color format is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  Color format configuration is supported on some AMD GPUs if the display is connected to the GPU using Dual-Link DVI or DisplayPort cable. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetValue (ADLX_COLOR_DEPTH colorDepth) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedColorDepth IsSupportedColorDepth
        *@ENG_START_DOX @brief Checks if a color format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedColorDepth (@ref ADLX_COLOR_DEPTH colorDepth, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],colorDepth,@ref ADLX_COLOR_DEPTH,@ENG_START_DOX The color format. @ENG_END_DOX}
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the color format is returned. The variable is __true__ if the color format is supported. The variable is __false__ if color format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the color format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the color format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedColorDepth (ADLX_COLOR_DEPTH colorDepth, adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_6 IsSupportedBPC_6
        *@ENG_START_DOX @brief Checks if color component/pixel with 6 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_6 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 6 bits is returned. The variable is __true__ if color component/pixel with 6 bits is supported. The variable is __false__ if color component/pixel with 6 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 6 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 6 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_6 (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_8 IsSupportedBPC_8
        *@ENG_START_DOX @brief Checks if color component/pixel with 8 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_8 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 8 bits is returned. The variable is __true__ if color component/pixel with 8 bits is supported. The variable is __false__ if color component/pixel with 8 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 8 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 8 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_8 (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_10 IsSupportedBPC_10
        *@ENG_START_DOX @brief Checks if color component/pixel with 10 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_10 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 10 bits is returned. The variable is __true__ if color component/pixel with 10 bits is supported. The variable is __false__ if color component/pixel with 10 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 10 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 10 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_10 (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_12 IsSupportedBPC_12
        *@ENG_START_DOX @brief Checks if color component/pixel with 12 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_12 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 12 bits is returned. The variable is __true__ if color component/pixel with 12 bits is supported. The variable is __false__ if color component/pixel with 12 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 12 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 12 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_12 (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_14 IsSupportedBPC_14
        *@ENG_START_DOX @brief Checks if color component/pixel with 14 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_14 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 14 bits is returned. The variable is __true__ if color component/pixel with 14 bits is supported. The variable is __false__ if color component/pixel with 14 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 14 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 14 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_14 (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayColorDepth_IsSupportedBPC_16 IsSupportedBPC_16
        *@ENG_START_DOX @brief Checks if color component/pixel with 16 bits is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBPC_16 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color component/pixel with 16 bits is returned. The variable is __true__ if color component/pixel with 16 bits is supported. The variable is __false__ if color component/pixel with 16 bits is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color component/pixel with 16 bits is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color component/pixel with 16 bits is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedBPC_16 (adlx_bool* supported) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayColorDepth> IADLXDisplayColorDepthPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayColorDepth, L"IADLXDisplayColorDepth")
typedef struct IADLXDisplayColorDepth IADLXDisplayColorDepth;

typedef struct IADLX_COLOR_DEPTHVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)(IADLXDisplayColorDepth* pThis);
    adlx_long (ADLX_STD_CALL* Release)(IADLXDisplayColorDepth* pThis);
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)(IADLXDisplayColorDepth* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Color depth
    ADLX_RESULT (ADLX_STD_CALL* IsSupported)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* GetValue)(IADLXDisplayColorDepth* pThis, ADLX_COLOR_DEPTH* currentColorDepth);
    ADLX_RESULT (ADLX_STD_CALL* SetValue)(IADLXDisplayColorDepth* pThis, ADLX_COLOR_DEPTH colorDepth);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedColorDepth)(IADLXDisplayColorDepth* pThis, ADLX_COLOR_DEPTH colorDepth, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_6)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_8)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_10)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_12)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_14)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedBPC_16)(IADLXDisplayColorDepth* pThis, adlx_bool* supported);
} IADLX_COLOR_DEPTHVtbl;

struct IADLXDisplayColorDepth
{
    const IADLX_COLOR_DEPTHVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayColorDepth interface

#pragma region IADLXDisplayPixelFormat interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayPixelFormat : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayPixelFormat")
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the pixel format can be configured on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of pixel format configuration is returned. The variable is __true__ if pixel format configuration is supported. The variable is __false__ if pixel format configuration is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of pixel format configuration is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of pixel format configuration is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  The pixel format is configurable if the display is connected to the GPU using direct HDMI™-HDMI. <br>
        * Pixel format configuration is not supported for DVI-HDMI and DisplayPort-HDMI connections. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_GetValue GetValue
        *@ENG_START_DOX @brief Gets the current pixel format of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetValue(@ref ADLX_PIXEL_FORMAT* pixelFormat)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],pixelFormat,@ref ADLX_PIXEL_FORMAT*,@ENG_START_DOX The pointer to a variable where the pixel format is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  The pixel format is configurable if the display is connected to the GPU using direct HDMI™-HDMI. <br>
        * Pixel format configuration is not supported for DVI-HDMI and DisplayPort-HDMI connections. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetValue (ADLX_PIXEL_FORMAT* pixelFormat) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_SetValue SetValue
        *@ENG_START_DOX @brief Sets the pixel format on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetValue(@ref ADLX_PIXEL_FORMAT pixelFormat)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],pixelFormat,@ref ADLX_PIXEL_FORMAT,@ENG_START_DOX The new pixel format. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the pixel format is successfully set, __ADLX_OK__ is returned. <br>
        * If the pixel format  is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  The pixel format is configurable if the display is connected to the GPU using direct HDMI™-HDMI. <br>
        * Pixel format configuration is not supported for DVI-HDMI and DisplayPort-HDMI connections. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetValue (ADLX_PIXEL_FORMAT pixelFormat) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedPixelFormat IsSupportedPixelFormat
        *@ENG_START_DOX @brief Checks if the given pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedPixelFormat (@ref ADLX_PIXEL_FORMAT pixelFormat, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],pixelFormat,@ref ADLX_PIXEL_FORMAT,@ENG_START_DOX The pixel format. @ENG_END_DOX}
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the pixel format is returned. The variable is __true__ if the pixel format is supported. The variable is __false__ if pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedPixelFormat (ADLX_PIXEL_FORMAT pixelFormat, adlx_bool* supportd) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedRGB444Full IsSupportedRGB444Full
        *@ENG_START_DOX @brief Checks if the RGB 4:4:4 PC Standard (Full RGB) pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedRGB444Full (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the RGB 4:4:4 PC Standard (Full RGB) pixel format is returned. The variable is __true__ if the RGB 4:4:4 PC Standard (Full RGB) pixel format is supported. The variable is __false__ if the RGB 4:4:4 PC Standard (Full RGB) pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the RGB 4:4:4 PC Standard (Full RGB) pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the RGB 4:4:4 PC Standard (Full RGB) pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedRGB444Full (adlx_bool* supportd) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedYCbCr444 IsSupportedYCbCr444
        *@ENG_START_DOX @brief Checks if the YCbCr 4:4:4 pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedYCbCr444 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the YCbCr 4:4:4 pixel format is returned. The variable is __true__ if the YCbCr 4:4:4 pixel format is supported. The variable is __false__ if the YCbCr 4:4:4 pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the YCbCr 4:4:4 pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the YCbCr 4:4:4 pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedYCbCr444 (adlx_bool* supportd) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedYCbCr422 IsSupportedYCbCr422
        *@ENG_START_DOX @brief Checks if the YCbCr 4:2:2 pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedYCbCr422 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the YCbCr 4:2:2 pixel format is returned. The variable is __true__ if the YCbCr 4:2:2 pixel format is supported. The variable is __false__ if the YCbCr 4:2:2 pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the YCbCr 4:2:2 pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the YCbCr 4:2:2 pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedYCbCr422 (adlx_bool* supportd) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedRGB444Limited IsSupportedRGB444Limited
        *@ENG_START_DOX @brief Checks if the RGB 4:4:4 Studio (Limited RGB) pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedRGB444Limited (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the RGB 4:4:4 Studio (Limited RGB) pixel format is returned. The variable is __true__ if the RGB 4:4:4 Studio (Limited RGB) pixel format is supported. The variable is __false__ if the RGB 4:4:4 Studio (Limited RGB) pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the RGB 4:4:4 Studio (Limited RGB) pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the RGB 4:4:4 Studio (Limited RGB) pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedRGB444Limited (adlx_bool* supportd) = 0;
        /**
        *@page DOX_IADLXDisplayPixelFormat_IsSupportedYCbCr420 IsSupportedYCbCr420
        *@ENG_START_DOX @brief Checks if the YCbCr 4:2:0 pixel format is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedYCbCr420 (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the support state of the YCbCr 4:2:0 pixel format is returned. The variable is __true__ if the YCbCr 4:2:0 pixel format is supported. The variable is __false__ if the YCbCr 4:2:0 pixel format is not supported. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the support state of the YCbCr 4:2:0 pixel format is successfully returned, __ADLX_OK__ is returned. <br>
        * If the support state of the YCbCr 4:2:0 pixel format is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedYCbCr420 (adlx_bool* supportd) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayPixelFormat> IADLXDisplayPixelFormatPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayPixelFormat, L"IADLXDisplayPixelFormat")
typedef struct IADLXDisplayPixelFormat IADLXDisplayPixelFormat;

typedef struct IADLX_PIXEL_FORMATVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayPixelFormat* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayPixelFormat* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayPixelFormat* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Pixel format
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayPixelFormat* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetValue)(IADLXDisplayPixelFormat* pThis, ADLX_PIXEL_FORMAT* pixelFormat);
    ADLX_RESULT (ADLX_STD_CALL *SetValue)(IADLXDisplayPixelFormat* pThis, ADLX_PIXEL_FORMAT pixelFormat);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedPixelFormat)(IADLXDisplayPixelFormat* pThis, ADLX_PIXEL_FORMAT pixelFormat, adlx_bool* supportd);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedRGB444Full)(IADLXDisplayPixelFormat* pThis, adlx_bool* supportd);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedYCbCr444)(IADLXDisplayPixelFormat* pThis, adlx_bool* supportd);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedYCbCr422)(IADLXDisplayPixelFormat* pThis, adlx_bool* supportd);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedRGB444Limited)(IADLXDisplayPixelFormat* pThis, adlx_bool* supportd);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedYCbCr420)(IADLXDisplayPixelFormat* pThis, adlx_bool* supportd);
} IADLX_PIXEL_FORMATVtbl;

struct IADLXDisplayPixelFormat
{
    const IADLX_PIXEL_FORMATVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayPixelFormat interface

#pragma region IADLXDisplayCustomColor interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayCustomColor : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayCustomColor")
        /**
        *@page DOX_IADLXDisplayCustomColor_IsHueSupported IsHueSupported
        *@ENG_START_DOX @brief Checks if customization of hue is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsHueSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of hue is returned. The variable is __true__ if customization of hue is supported. The variable is __false__ if customization of hue is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of hue is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of hue is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsHueSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetHueRange GetHueRange
        *@ENG_START_DOX @brief Gets the maximum hue, minimum hue, and step hue of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetHueRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the hue range of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the hue range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the hue range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetHueRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetHue GetHue
        *@ENG_START_DOX @brief Gets the current hue of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetHue(adlx_int* currentHue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentHue,adlx_int*,@ENG_START_DOX The pointer to a variable where the current hue of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current hue is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current hue is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetHue (adlx_int* currentHue) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_SetHue SetHue
        *@ENG_START_DOX @brief Sets the hue on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetHue(adlx_int value)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],hue,adlx_int,@ENG_START_DOX The new hue value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the hue is successfully set, __ADLX_OK__ is returned. <br>
        * If the hue is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetHue (adlx_int hue) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_IsSaturationSupported IsSaturationSupported
        *@ENG_START_DOX @brief Checks if customization of saturation is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSaturationSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of saturation is returned. The variable is __true__ if customization of saturation is supported. The variable is __false__ if customization of saturation is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of saturation is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of saturation is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSaturationSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetSaturationRange GetSaturationRange
        *@ENG_START_DOX @brief Gets the maximum saturation, minimum saturation, and step saturation of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSaturationRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the saturation range of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the saturation range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the saturation range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSaturationRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetSaturation GetSaturation
        *@ENG_START_DOX @brief Gets the current saturation of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSaturation(adlx_int* currentSaturation)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentSaturation,adlx_int*,@ENG_START_DOX The pointer to a variable where the current saturation of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current saturation is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current saturation is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSaturation (adlx_int* currentSaturation) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_SetSaturation SetSaturation
        *@ENG_START_DOX @brief Sets the saturation on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSaturation(adlx_int saturation)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],saturation,adlx_int,@ENG_START_DOX The new saturation value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the saturation is successfully set, __ADLX_OK__ is returned. <br>
        * If the saturation is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetSaturation (adlx_int saturation) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_IsBrightnessSupported IsBrightnessSupported
        *@ENG_START_DOX @brief Checks if customization of brightness is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsBrightnessSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of brightness is returned. The variable is __true__ if customization of brightness is supported. The variable is __false__ if customization of brightness is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of brightness is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of brightness is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsBrightnessSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetBrightnessRange GetBrightnessRange
        *@ENG_START_DOX @brief Gets the maximum brightness, minimum brightness, and step brightness of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetBrightnessRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the brightness range of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the brightness range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the brightness range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetBrightnessRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetBrightness GetBrightness
        *@ENG_START_DOX @brief Gets the current brightness of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetBrightness(adlx_int* currentBrightness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentBrightness,adlx_int*,@ENG_START_DOX The pointer to a variable where the current brightness of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current brightness is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current brightness is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetBrightness (adlx_int* currentBrightness) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_SetBrightness SetBrightness
        *@ENG_START_DOX @brief Sets the brightness on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetBrightness(adlx_int brightness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],brightness,adlx_int,@ENG_START_DOX The new brightness value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the brightness is successfully set, __ADLX_OK__ is returned. <br>
        * If the brightness is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetBrightness (adlx_int brightness) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_IsContrastSupported IsContrastSupported
        *@ENG_START_DOX @brief Checks if customization of color contrast is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsContrastSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color contrast is returned. The variable is __true__ if customization of color contrast is supported. The variable is __false__ if customization of color contrast is not supported @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color contrast is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color contrast is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsContrastSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetContrastRange GetContrastRange
        *@ENG_START_DOX @brief Gets the maximum color contrast, minimum contrast contrast, and step color contrast of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetContrastRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the color contrast range of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the color contrast range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the color contrast range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetContrastRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetContrast GetContrast
        *@ENG_START_DOX @brief Gets the current color contrast of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetContrast(adlx_int* currentContrast)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentContrast,adlx_int*,@ENG_START_DOX The pointer to a variable where the current color contrast of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current color contrast is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current color contrast is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetContrast (adlx_int* currentContrast) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_SetContrast SetContrast
        *@ENG_START_DOX @brief Sets the color contrast on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetContrast(adlx_int contrast)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],contrast,adlx_int,@ENG_START_DOX The new color contrast value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the color contrast value is successfully set, __ADLX_OK__ is returned. <br>
        * If the color contrast value is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetContrast (adlx_int contrast) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_IsTemperatureSupported IsTemperatureSupported
        *@ENG_START_DOX @brief Checks if customization of color temperature is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsTemperatureSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color temperature is returned. The variable is __true__ if customization of color temperature is supported. The variable is __false__ if customization of color temperature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color temperature is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color temperature is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsTemperatureSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetTemperatureRange GetTemperatureRange
        *@ENG_START_DOX @brief Gets the maximum color temperature, minimum color temperature, and step color temperature of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTemperatureRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the color temperature range is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the color temperature range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the color temperature range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetTemperatureRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_GetTemperature GetTemperature
        *@ENG_START_DOX @brief Gets the current color temperature of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTemperature(adlx_int* currentTemperature)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentTemperature,adlx_int*,@ENG_START_DOX The pointer to a variable where the current color temperature of the display is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current color temperature is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current color temperature is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetTemperature (adlx_int* currentTemperature) = 0;
        /**
        *@page DOX_IADLXDisplayCustomColor_SetTemperature SetTemperature
        *@ENG_START_DOX @brief Sets the color temperature on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetTemperature(adlx_int value)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],temperature,adlx_int,@ENG_START_DOX The new color temperature value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the color temperature is successfully set, __ADLX_OK__ is returned. <br>
        * If the color temperature is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetTemperature (adlx_int temperature) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayCustomColor> IADLXDisplayCustomColorPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayCustomColor, L"IADLXDisplayCustomColor")
typedef struct IADLXDisplayCustomColor IADLXDisplayCustomColor;

typedef struct IADLXCustomColorVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayCustomColor* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayCustomColor* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayCustomColor* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Custom color
    ADLX_RESULT (ADLX_STD_CALL *IsHueSupported)(IADLXDisplayCustomColor* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetHueRange)(IADLXDisplayCustomColor* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetHue)(IADLXDisplayCustomColor* pThis, adlx_int* currentHue);
    ADLX_RESULT (ADLX_STD_CALL *SetHue)(IADLXDisplayCustomColor* pThis, adlx_int hue);

    ADLX_RESULT (ADLX_STD_CALL *IsSaturationSupported)(IADLXDisplayCustomColor* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetSaturationRange)(IADLXDisplayCustomColor* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetSaturation)(IADLXDisplayCustomColor* pThis, adlx_int* currentSaturation);
    ADLX_RESULT (ADLX_STD_CALL *SetSaturation)(IADLXDisplayCustomColor* pThis, adlx_int saturation);

    ADLX_RESULT (ADLX_STD_CALL *IsBrightnessSupported)(IADLXDisplayCustomColor* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetBrightnessRange)(IADLXDisplayCustomColor* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetBrightness)(IADLXDisplayCustomColor* pThis, adlx_int* currentBrightness);
    ADLX_RESULT (ADLX_STD_CALL *SetBrightness)(IADLXDisplayCustomColor* pThis, adlx_int brightness);

    ADLX_RESULT (ADLX_STD_CALL *IsContrastSupported)(IADLXDisplayCustomColor* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetContrastRange)(IADLXDisplayCustomColor* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetContrast)(IADLXDisplayCustomColor* pThis, adlx_int* currentContrast);
    ADLX_RESULT (ADLX_STD_CALL *SetContrast)(IADLXDisplayCustomColor* pThis, adlx_int contrast);

    ADLX_RESULT (ADLX_STD_CALL *IsTemperatureSupported)(IADLXDisplayCustomColor* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetTemperatureRange)(IADLXDisplayCustomColor* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetTemperature)(IADLXDisplayCustomColor* pThis, adlx_int* currentTemperature);
    ADLX_RESULT (ADLX_STD_CALL *SetTemperature)(IADLXDisplayCustomColor* pThis, adlx_int temperature);


} IADLXCustomColorVtbl;

struct IADLXDisplayCustomColor
{
    const IADLXCustomColorVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayCustomColor interface

#pragma region IADLXDisplayHDCP interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayHDCP : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayHDCP")
        /**
        *@page DOX_IADLXDisplayHDCP_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the HDCP can be configured on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the HDCP configuration is returned. The variable is __true__ if HDCP configuration is supported. The variable is __false__ if HDCP configuration is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of HDCP configuration is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the state of HDCP configuration is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * If the HDCP is disabled on this display, digitally protected content may be unplayable, or played at a lower resolution.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayHDCP_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if the HDCP is enabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of HDCP is returned. The variable is __true__ if HDCP is enabled. The variable is __false__ if HDCP is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of HDCP configuration is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the state of HDCP configuration is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * If the HDCP is disabled on this display, digitally protected content may be unplayable, or played at a lower resolution.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayHDCP_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the HDCP to enabled or disabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new HDCP configuration state. Set __true__ to enable HDCP. Set __false__ to disable HDCP. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of HDCP configuration is successfully set, __ADLX_OK__ is returned.<br/>
		* If the state of HDCP configuration is not successfully set, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * If the HDCP is disabled on this display, digitally protected content may be unplayable, or played at a lower resolution.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayHDCP> IADLXDisplayHDCPPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayHDCP, L"IADLXDisplayHDCP")
typedef struct IADLXDisplayHDCP IADLXDisplayHDCP;

typedef struct IADLXHDCPVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayHDCP* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayHDCP* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayHDCP* pThis, const wchar_t* interfaceId, void** ppInterface);

    // HDCP
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayHDCP* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLXDisplayHDCP* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLXDisplayHDCP* pThis, adlx_bool enabled);
} IADLXHDCPVtbl;

struct IADLXDisplayHDCP
{
    const IADLXHDCPVtbl *pVtbl;
};

#endif

#pragma endregion IADLXDisplayHDCP interface

#pragma region IADLXDisplayResolution interface
#if defined (__cplusplus)

namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayResolution : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayResolution")
        /**
        *@page DOX_IADLXDisplayResolution_GetValue GetValue
        *@ENG_START_DOX @brief Gets the properties of a custom display resolution. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetValue(@ref ADLX_CustomResolution* customResolution)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],customResolution,@ref ADLX_CustomResolution*,@ENG_START_DOX The pointer to a variable where the properties of a custom display resolution are returned.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the properties of a custom display resolution are successfully returned, __ADLX_OK__ is returned.<br/>
		* If the properties of a custom display resolution are not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetValue (ADLX_CustomResolution* customResolution) = 0;
        /**
        *@page DOX_IADLXDisplayResolution_SetValue SetValue
        *@ENG_START_DOX @brief Sets the properties of a custom display resolution. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetValue(@ref ADLX_CustomResolution customResolution)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],customResolution,@ref ADLX_CustomResolution,@ENG_START_DOX The properties of a custom display resolution.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the properties of a custom display resolution are successfully set, __ADLX_OK__ is returned.<br/>
		* If the properties of a custom display resolution are not successfully set, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetValue (ADLX_CustomResolution customResolution) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayResolution> IADLXDisplayResolutionPtr;
}

#else

ADLX_DECLARE_IID (IADLXDisplayResolution, L"IADLXDisplayResolution")

typedef struct IADLXDisplayResolution IADLXDisplayResolution;

typedef struct IADLXDisplayResolutionVtbl
{
    // IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayResolution* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayResolution* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayResolution* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLXDisplayResolution
    ADLX_RESULT (ADLX_STD_CALL *GetValue)(IADLXDisplayResolution* pThis, ADLX_CustomResolution* cr);
    ADLX_RESULT (ADLX_STD_CALL *SetValue)(IADLXDisplayResolution* pThis, ADLX_CustomResolution cr);
} IADLXDisplayResolutionVtbl;

struct IADLXDisplayResolution { const IADLXDisplayResolutionVtbl *pVtbl; };

#endif
#pragma endregion IADLXDisplayResolution interface

#pragma region IADLXDisplayResolutionList interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayResolutionList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayResolutionList")
        ADLX_DECLARE_ITEM_IID (IADLXDisplayResolution::IID ())
        /**
        * @page DOX_IADLXDisplayResolutionList_At At
        * @ENG_START_DOX
        * @brief Returns the item at the asked location.
        * @ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXDisplayResolution** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX Location index to retrieve the resolution item from the list.  @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXDisplayResolution** ,@ENG_START_DOX The address of a pointer variable that receives a pointer to the @ref DOX_IADLXDisplayResolution interface.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * Returns __ADLX_OK__ for success and ADLX error code @ref ADLX_RESULT for failure.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include"IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXDisplayResolution** ppItem) = 0;
        /**
         * @page DOX_IADLXDisplayResolutionList_Add_Back Add_Back
         * @ENG_START_DOX
         * @brief Adds an element to the back.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXDisplayResolution* pItem)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pItem,@ref DOX_IADLXDisplayResolution* ,@ENG_START_DOX The address of a pointer variable that receives the @ref DOX_IADLXDisplayResolution interface.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * Returns __ADLX_OK__ for success and ADLX error code @ref ADLX_RESULT for failure.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details Adds an element to the back.
         *
         * You should call this method whenever you need to add the resolution item to the end of the list.
         * @ENG_END_DOX
         *
         *
         * @requirements
         * @DetailsTable{#include"IDisplaySettings.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXDisplayResolution* pItem) = 0;
    };  //IADLXDisplayResolutionList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayResolutionList> IADLXDisplayResolutionListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayResolutionList, L"IADLXDisplayResolutionList")
ADLX_DECLARE_ITEM_IID (IADLXDisplayResolution, IID_IADLXDisplayResolution ())

typedef struct IADLXDisplayResolutionList IADLXDisplayResolutionList;

typedef struct IADLXDisplayResolutionListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayResolutionList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayResolutionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayResolutionList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXDisplayResolutionList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXDisplayResolutionList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXDisplayResolutionList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXDisplayResolutionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXDisplayResolutionList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXDisplayResolutionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXDisplayResolutionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXDisplayResolutionList* pThis, IADLXInterface* pItem);

    //IADLXDisplayResolutionList
    ADLX_RESULT (ADLX_STD_CALL *At_DisplayResolutionList)(IADLXDisplayResolutionList* pThis, const adlx_uint location, IADLXDisplayResolution** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_DisplayResolutionList)(IADLXDisplayResolutionList* pThis, IADLXDisplayResolution* pItem);

} IADLXDisplayResolutionListVtbl;

struct IADLXDisplayResolutionList { const IADLXDisplayResolutionListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayResolutionList interface

#pragma region IADLXDisplayCustomResolution interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayCustomResolution : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayCustomResolution")
        /**
        *@page DOX_IADLXDisplayCustomResolution_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if custom resolution is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the custom resolution support status is returned. The variable is __true__ if custom resolution is supported. The variable is __false__ if custom resolution is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom resolution status is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the custom resolution status is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* Applying custom resolution in certain games and applications may provide a better user experience. Consult the display user manual for specifications and compatibility information before use.<br/>
		* __Note__: Displays running in duplicate or Eyefinity mode do not support custom resolutions.
        * @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayCustomResolution_GetResolutionList GetResolutionList
        *@ENG_START_DOX @brief Gets the reference counted list of available resolutions of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetResolutionList(@ref DOX_IADLXDisplayResolutionList** ppResolutionList)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],cr,@ref DOX_IADLXDisplayResolutionList*,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __ppResolutionList__ to __nullptr__.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the resolution list is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the resolution list is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
		*
        * @addinfo
        * @ENG_START_DOX
		*In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.<br/>
		*
		* Applying custom resolution in certain games and applications may provide a better user experience. Consult the display user manual for specifications and compatibility information before use.<br/>
		* __Note__: Displays running in duplicate or Eyefinity mode do not support custom resolutions.
        * @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetResolutionList (IADLXDisplayResolutionList** ppResolutionList) = 0;
        /**
        *@page DOX_IADLXDisplayCustomResolution_GetCurrentAppliedResolution GetCurrentAppliedResolution
        *@ENG_START_DOX @brief Gets the reference counted current resolution interface of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentAppliedResolution(@ref DOX_IADLXDisplayResolution** ppResolution)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],ppResolution,@ref DOX_IADLXDisplayResolution**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __ppResolution__ to __nullptr__.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current resolution is successfully returned, __ADLX_OK__ is returned.<br/>
		* If the current resolution is not successfully returned, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.<br/>
		*
		* Applying custom resolution in certain games and applications may provide a better user experience. Consult the display user manual for specifications and compatibility information before use.<br/>
		* __Note__: Displays running in duplicate or Eyefinity mode do not support custom resolutions.
        * @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentAppliedResolution (IADLXDisplayResolution** ppResolution) = 0;
        /**
        *@page DOX_IADLXDisplayCustomResolution_CreateNewResolution CreateNewResolution
        *@ENG_START_DOX @brief Creates a custom resolution on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    CreateNewResolution(@ref DOX_IADLXDisplayResolution* pResolution)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],pResolution,@ref DOX_IADLXDisplayResolution*,@ENG_START_DOX The pointer to the custom resolution interface.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom resolution is successfully created, __ADLX_OK__ is returned.<br/>
		* If the custom resolution is not successfully created, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* Applying custom resolution in certain games and applications may provide a better user experience. Consult the display user manual for specifications and compatibility information before use.<br/>
		* __Note__: Displays running in duplicate or Eyefinity mode do not support custom resolutions.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL CreateNewResolution (IADLXDisplayResolution* pResolution) = 0;
        /**
        *@page DOX_IADLXDisplayCustomResolution_DeleteResolution DeleteResolution
        *@ENG_START_DOX @brief Deletes a custom resolution from a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    DeleteResolution(@ref DOX_IADLXDisplayResolution* pResolution)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],pResolution,@ref DOX_IADLXDisplayResolution*,@ENG_START_DOX The pointer to a variable where the display resolution @ref DOX_IADLXDisplayResolution is returned.@ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom resolution is successfully deleted, __ADLX_OK__ is returned.<br/>
		* If the custom resolution is not successfully deleted, an error code is returned.<br/>
		* Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* Applying custom resolution in certain games and applications may provide a better user experience. Consult the display user manual for specifications and compatibility information before use.<br/>
		* __Note__: Displays running in duplicate or Eyefinity mode do not support custom resolutions.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL DeleteResolution (IADLXDisplayResolution* pResolution) = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayCustomResolution> IADLXDisplayCustomResolutionPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayCustomResolution, L"IADLXDisplayCustomResolution")
typedef struct IADLXDisplayCustomResolution IADLXDisplayCustomResolution;

typedef struct IADLXDisplayCustomResolutionVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayCustomResolution* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayCustomResolution* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayCustomResolution* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Custom resolution
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLXDisplayCustomResolution* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetResolutionList)(IADLXDisplayCustomResolution* pThis, IADLXDisplayResolutionList** ppResolutionList);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentAppliedResolution)(IADLXDisplayCustomResolution* pThis, IADLXDisplayResolution** ppResolution);
    ADLX_RESULT (ADLX_STD_CALL *CreateNewResolution)(IADLXDisplayCustomResolution* pThis, IADLXDisplayResolution* pResolution);
    ADLX_RESULT (ADLX_STD_CALL *DeleteResolution)(IADLXDisplayCustomResolution* pThis, IADLXDisplayResolution* pResolution);
} IADLXDisplayCustomResolutionVtbl;

struct IADLXDisplayCustomResolution
{
    const IADLXDisplayCustomResolutionVtbl *pVtbl;
};
#endif

#pragma endregion IADLXDisplayCustomResolution interface

#pragma region IADLXDisplayVariBright interface

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayVariBright : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayVariBright")
        /**
        *@page DOX_IADLXDisplayVariBright_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if Vari-Bright can be configured on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Vari-Bright is returned. The variable is __true__ if Vari-Bright is supported. The variable is __false__ if Vari-Bright is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Vari-Bright is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of Vari-Bright is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if Vari-Bright is enabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Vari-Bright is returned. The variable is __true__ if Vari-Bright is enabled. The variable is __false__ if Vari-Bright is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Vari-Bright is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of Vari-Bright is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the Vari-Bright to enabled or disabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new Vari-Bright state. Set __true__ to enable Vari-Bright. Set __false__ to disable Vari-Bright. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Vari-Bright is successfully set, __ADLX_OK__ is returned. <br>
        * If the state of Vari-Bright is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enabled) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsCurrentMaximizeBrightness IsCurrentMaximizeBrightness
        *@ENG_START_DOX @brief Checks if the maximized brightness Vari-Bright preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentMaximizeBrightness (adlx_bool* maximizeBrightness)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],maximizeBrightness,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the maximize brightness preset is returned. The variable is __true__ if the maximize brightness preset is used. The variable is __false__ if the maximize brightness preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of maximize brightness preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of maximize brightness preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentMaximizeBrightness (adlx_bool* maximizeBrightness) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsCurrentOptimizeBrightness IsCurrentOptimizeBrightness
        *@ENG_START_DOX @brief Checks if the optimized brightness Vari-Bright preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentOptimizeBrightness (adlx_bool* optimizeBrightness)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],optimizeBrightness,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the optimized brightness preset is returned. The variable is __true__ if the optimized brightness preset is used. The variable is __false__ if the optimized brightness preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of optimized brightness preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of optimized brightness preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentOptimizeBrightness (adlx_bool* optimizeBrightness) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsCurrentBalanced IsCurrentBalanced
        *@ENG_START_DOX @brief Checks if the balanced Vari-Bright preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentBalanced (adlx_bool* balanced)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],balanced,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the Vari-Bright preset is returned. The variable is __true__ if the balanced preset is used. The variable is __false__ if the balanced preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of balanced preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of balanced preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentBalanced (adlx_bool* balanced) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsCurrentOptimizeBattery IsCurrentOptimizeBattery
        *@ENG_START_DOX @brief Checks if the optimized battery Vari-Bright preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentOptimizeBattery (adlx_bool* optimizeBattery)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],optimizeBattery,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the optimized battery preset is returned. The variable is __true__ if the optimized battery preset is used. The variable is __false__ if the optimized battery preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of optimized battery preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of optimized battery preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentOptimizeBattery (adlx_bool* optimizeBattery) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_IsCurrentMaximizeBattery IsCurrentMaximizeBattery
        *@ENG_START_DOX @brief Checks if the maximize battery Vari-Bright preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentMaximizeBattery (adlx_bool* maximizeBattery)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],maximizeBattery,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the maximize battery preset is returned. The variable is __true__ if the maximize battery preset is used. The variable is __false__ if the maximize battery preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of maximize battery preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of maximize battery preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentMaximizeBattery (adlx_bool* maximizeBattery) = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetMaximizeBrightness SetMaximizeBrightness
        *@ENG_START_DOX @brief Sets the maximize brightness Vari-Bright preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMaximizeBrightness ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the maximized brightness preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the maximized brightness preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMaximizeBrightness () = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetOptimizeBrightness SetOptimizeBrightness
        *@ENG_START_DOX @brief Sets the optimize brightness Vari-Bright preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetOptimizeBrightness ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the optimize brightness preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the optimize brightness preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetOptimizeBrightness () = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetBalanced SetBalanced
        *@ENG_START_DOX @brief Sets the balanced Vari-Bright preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetBalanced ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the balanced preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the balanced preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetBalanced () = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetOptimizeBattery SetOptimizeBattery
        *@ENG_START_DOX @brief Sets the optimize battery Vari-Bright preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetOptimizeBattery ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the optimize battery preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the optimize battery preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetOptimizeBattery () = 0;
        /**
        *@page DOX_IADLXDisplayVariBright_SetMaximizeBattery SetMaximizeBattery
        *@ENG_START_DOX @brief Sets the maximize battery Vari-Bright preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMaximizeBattery ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the maximize battery preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the maximize battery preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *
        *@ENG_START_DOX @details The maximized battery preset maximizes battery time. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMaximizeBattery () = 0;
    };

    typedef IADLXInterfacePtr_T<IADLXDisplayVariBright> IADLXDisplayVariBrightPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayVariBright, L"IADLXDisplayVariBright")
typedef struct IADLXDisplayVariBright IADLXDisplayVariBright;

typedef struct IADLXDisplayVariBrightVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)(IADLXDisplayVariBright* pThis);
    adlx_long (ADLX_STD_CALL* Release)(IADLXDisplayVariBright* pThis);
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)(IADLXDisplayVariBright* pThis, const wchar_t* interfaceId, void** ppInterface);

    // Vari-Bright interface
    ADLX_RESULT (ADLX_STD_CALL* IsSupported)(IADLXDisplayVariBright* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsEnabled)(IADLXDisplayVariBright* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL* SetEnabled)(IADLXDisplayVariBright* pThis, adlx_bool enabled);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentMaximizeBrightness)(IADLXDisplayVariBright* pThis, adlx_bool* maximizeBrightness);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentOptimizeBrightness)(IADLXDisplayVariBright* pThis, adlx_bool* optimizeBrightness);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentBalanced)(IADLXDisplayVariBright* pThis, adlx_bool* balanced);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentOptimizeBattery)(IADLXDisplayVariBright* pThis, adlx_bool* optimizeBattery);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentMaximizeBattery)(IADLXDisplayVariBright* pThis, adlx_bool* maximizeBattery);
    ADLX_RESULT (ADLX_STD_CALL* SetMaximizeBrightness)(IADLXDisplayVariBright* pThis);
    ADLX_RESULT (ADLX_STD_CALL* SetOptimizeBrightness)(IADLXDisplayVariBright* pThis);
    ADLX_RESULT (ADLX_STD_CALL* SetBalanced)(IADLXDisplayVariBright* pThis);
    ADLX_RESULT (ADLX_STD_CALL* SetOptimizeBattery)(IADLXDisplayVariBright* pThis);
    ADLX_RESULT (ADLX_STD_CALL* SetMaximizeBattery)(IADLXDisplayVariBright* pThis);
} IADLXDisplayVariBrightVtbl;

struct IADLXDisplayVariBright
{
    const IADLXDisplayVariBrightVtbl* pVtbl;
};
#endif

#pragma endregion IADLXDisplayVariBright interface
#endif // ADLX_IDISPLAYSETTING_H