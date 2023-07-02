//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_I3DSETTINGS_H
#define ADLX_I3DSETTINGS_H
#pragma once

#include "ADLXStructures.h"
#include "IChangedEvent.h"

//-------------------------------------------------------------------------------------------------
//I3DSetting.h - Interfaces for ADLX GPU 3DSetting functionality

//3DAntiLag setting interface
#pragma region IADLX3DAntiLag
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DAntiLag : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DAntiLag")

        /**
        *@page DOX_IADLX3DAntiLag_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Anti-Lag is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Radeon Anti-Lag is returned. The variable is __true__ if Radeon Anti-Lag is supported. The variable is __false__ if Radeon Anti-Lag is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Anti-Lag is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Anti-Lag is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Anti-Lag helps reduce input lag in GPU-limited cases by controlling the pace of the CPU work to ensure it doesn't get too far ahead of the GPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DAntiLag_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Anti-Lag is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Radeon Anti-Lag is returned. The variable is __true__ if Radeon Anti-Lag is enabled. The variable is __false__ if Radeon Anti-Lag is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Anti-Lag is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Anti-Lag is returned is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Anti-Lag helps reduce input lag in GPU-limited cases by controlling the pace of the CPU work to ensure it doesn't get too far ahead of the GPU. <br>
        * __Note:__ @ref DOX_IADLX3DChill "AMD Radeon Chill", @ref DOX_IADLX3DBoost "AMD Radeon Boost", and AMD Radeon Anti-Lag features cannot be enabled simultaneously. If AMD Radeon Boost or AMD Radeon Chill is enabled, AMD Radeon Anti-Lag is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;

        /**
        *@page DOX_IADLX3DAntiLag_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets AMD Radeon™ Anti-Lag to enabled or disabled state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new Radeon Anti-Lag state. Set __true__ to enable Radeon Anti-Lag. Set __false__ to disable Radeon Anti-Lag. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Anti-Lag is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Anti-Lag is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Anti-Lag helps reduce input lag in GPU-limited cases by controlling the pace of the CPU work to ensure it doesn't get too far ahead of the GPU. <br>
        * __Note:__ @ref DOX_IADLX3DChill "AMD Radeon Chill", @ref DOX_IADLX3DBoost "AMD Radeon Boost", and AMD Radeon Anti-Lag features cannot be enabled simultaneously. If AMD Radeon Anti-Lag is enabled, AMD Radeon Boost and AMD Radeon Chill are automatically disabled. However, the configuration of the disabled feature is preserved.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

    };  //IADLX3DAntiLag
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DAntiLag> IADLX3DAntiLagPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DAntiLag, L"IADLX3DAntiLag")

typedef struct IADLX3DAntiLag IADLX3DAntiLag;

typedef struct IADLX3DAntiLagVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DAntiLag* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DAntiLag* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DAntiLag* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DAntiLag
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DAntiLag* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DAntiLag* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DAntiLag* pThis, adlx_bool enable);
}IADLX3DAntiLagVtbl;

struct IADLX3DAntiLag { const IADLX3DAntiLagVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DAntiLag

//3DChill setting interface
#pragma region IADLX3DChill
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DChill : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DChill")

        /**
        *@page DOX_IADLX3DChill_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Chill is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Chill is returned. The variable is __true__ if AMD Radeon Chill is supported. The variable is __false__ if AMD Radeon Chill is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Chill is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Chill is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DChill_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Chill is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Chill is returned. The variable is __true__ if AMD Radeon Chill is enabled. The variable is __false__ if AMD Radeon Chill is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Chill is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Chill is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.<br>
        * __Note:__ AMD Radeon Chill, @ref DOX_IADLX3DBoost "AMD Radeon Boost", and @ref DOX_IADLX3DAntiLag "AMD Radeon Anti-Lag" features cannot be enabled simultaneously. If AMD Radeon Boost or AMD Radeon Anti-Lag is enabled, AMD Radeon Chill is automatically disabled.<br>
        * On some AMD GPUs, AMD Radeon Chill and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Super Resolution is enabled, AMD Radeon Chill is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DChill_GetFPSRange GetFPSRange
        *@ENG_START_DOX @brief Gets the AMD Radeon™ Chill maximum FPS, minimum FPS, and step FPS on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFPSRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the FPS range of AMD Radeon Chill is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the FPS range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the FPS range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum FPS, minimum FPS, and step FPS values are read only. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
		*
		* The maximum FPS determines the target frame rate during periods of intense action.
		*
		* The minimum FPS determines the target frame rate when in-game interaction is minimal.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFPSRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLX3DChill_GetMinFPS GetMinFPS
        *@ENG_START_DOX @brief Gets the AMD Radeon™ Chill current minimum FPS on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMinFPS (adlx_int* currentMinFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMinFPS,adlx_int*,@ENG_START_DOX The pointer to a variable where the current minimum FPS value of AMD Radeon Chill is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current minimum FPS is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current minimum FPS is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
		*
		* The minimum FPS determines the target frame rate when in-game interaction is minimal.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMinFPS (adlx_int* currentMinFPS) = 0;

        /**
        *@page DOX_IADLX3DChill_GetMaxFPS GetMaxFPS
        *@ENG_START_DOX @brief Gets the AMD Radeon™ Chill current maximum FPS on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMaxFPS (adlx_int* currentMaxFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMaxFPS,adlx_int*,@ENG_START_DOX The pointer to a variable where the current maximum FPS value of AMD Radeon Chill is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current maximum FPS is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current maximum FPS is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
		*
		*The maximum FPS determines the target frame rate during periods of intense action.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMaxFPS (adlx_int* currentMaxFPS) = 0;

        /**
        *@page DOX_IADLX3DChill_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the activation status of AMD Radeon™ Chill on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Radeon Chill state. Set __true__ to enable AMD Radeon Chill. Set __false__ to disable AMD Radeon Chill. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Chill is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Chill is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement. <br>
        * __Note:__ AMD Radeon Chill, @ref DOX_IADLX3DBoost "AMD Radeon Boost", and @ref DOX_IADLX3DAntiLag "AMD Radeon Anti-Lag" features cannot be enabled simultaneously. If AMD Radeon Chill is enabled, AMD Radeon Anti-Lag and AMD Radeon Boost are automatically disabled. However, the configurations of the disabled feature is preserved.<br>
        * On some AMD GPUs, AMD Radeon Chill and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If AMD Radeon Chill is enabled, Radeon Super Resolution is automatically disabled.

        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DChill_SetMinFPS SetMinFPS
        *@ENG_START_DOX @brief Sets the AMD Radeon™ Chill minimum FPS on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMinFPS (adlx_int minFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],minFPS,adlx_int,@ENG_START_DOX The new minimum FPS value of AMD Radeon Chill. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum FPS is successfully set, __ADLX_OK__ is returned.<br>
        * If the minimum FPS is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
		*
		* The minimum FPS determines the target frame rate when in-game interaction is minimal.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMinFPS (adlx_int minFPS) = 0;

        /**
        *@page DOX_IADLX3DChill_SetMaxFPS SetMaxFPS
        *@ENG_START_DOX @brief Sets the AMD Radeon™ Chill maximum FPS on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMaxFPS (adlx_int maxFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],maxFPS,adlx_int,@ENG_START_DOX The new maximum FPS value of AMD Radeon Chill. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the maximum FPS is successfully set, __ADLX_OK__ is returned.<br>
        * If the maximum FPS is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Chill conserves GPU power and reduces heat by adjusting the FPS based on the intensity of in-game movement.
		*
		* The maximum FPS determines the target frame rate during periods of intense action.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMaxFPS (adlx_int maxFPS) = 0;

    };  //IADLX3DChill
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DChill> IADLX3DChillPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DChill, L"IADLX3DChill")

typedef struct IADLX3DChill IADLX3DChill;

typedef struct IADLX3DChillVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DChill* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DChill* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DChill* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DChill
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DChill* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DChill* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetFPSRange)(IADLX3DChill* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetMinFPS)(IADLX3DChill* pThis, adlx_int* currentMinFPS);
    ADLX_RESULT (ADLX_STD_CALL *GetMaxFPS)(IADLX3DChill* pThis, adlx_int* currentMaxFPS);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DChill* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *SetMinFPS)(IADLX3DChill* pThis, adlx_int minFPS);
    ADLX_RESULT (ADLX_STD_CALL *SetMaxFPS)(IADLX3DChill* pThis, adlx_int maxFPS);
}IADLX3DChillVtbl;

struct IADLX3DChill { const IADLX3DChillVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DChill

//3DBoost setting interface
#pragma region IADLX3DBoost
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DBoost : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DBoost")

        /**
        *@page DOX_IADLX3DBoost_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Boost is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Boost is returned. The variable is __true__ if AMD Radeon Boost is supported. The variable is __false__ if AMD Radeon Boost is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Boost is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Boost is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality.<br>
        * Only works in supported games.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DBoost_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Boost is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Boost is returned. The variable is __true__ if AMD Radeon Boost is enabled. The variable is __false__ if AMD Radeon Boost is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Boost is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Boost is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality. Only works in supported games.<br>
        * __Note:__ @ref DOX_IADLX3DChill "AMD Radeon Chill", AMD Radeon Boost, and @ref DOX_IADLX3DAntiLag "AMD Radeon Anti-Lag" features cannot be enabled simultaneously. If AMD Radeon Chill or AMD Radeon Anti-Lag is enabled, AMD Radeon Boost is automatically disabled.<br>
        * On some AMD GPUs, AMD Radeon Boost and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Super Resolution is enabled, AMD Radeon Boost is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DBoost_GetResolutionRange GetResolutionRange
        *@ENG_START_DOX @brief Gets the maximum resolution, minimum resolution, and step resolution of AMD Radeon™ Boost on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetResolutionRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the resolution range of AMD Radeon Boost is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the resolution range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the resolution range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum resolution, minimum resolution, and step resolution of AMD Radeon Boost are read only. @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality.<br>
        * Only works in supported games.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetResolutionRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLX3DBoost_GetResolution GetResolution
        *@ENG_START_DOX @brief Gets the current minimum resolution of AMD Radeon™ Boost on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetResolution (adlx_int* currentMinRes)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMinRes,adlx_int*,@ENG_START_DOX The pointer to a variable where the current minimum resolution (in %) of AMD Radeon Boost is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current minimum resolution is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current minimum resolution is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality.<br>
        * Only works in supported games.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetResolution (adlx_int* currentMinRes) = 0;

        /**
        *@page DOX_IADLX3DBoost_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets AMD Radeon™ Boost to enabled or disabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Radeon Boost state. Set __true__ to enable AMD Radeon Boost. Set __false__ to disable AMD Radeon Boost. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Boost is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Boost is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality. Only works in supported games. <br>
        * __Note:__ @ref DOX_IADLX3DChill "AMD Radeon Chill", AMD Radeon Boost, and @ref DOX_IADLX3DAntiLag "AMD Radeon Anti-Lag" features cannot be enabled simultaneously. If AMD Radeon Boost is enabled, AMD Radeon Anti-Lag and AMD Radeon Chill are automatically disabled. However, the configuration of the disabled feature is preserved.<br>
        * On some AMD GPUs, AMD Radeon Boost and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If AMD Radeon Boost is enabled, Radeon Super Resolution is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DBoost_SetResolution SetResolution
        *@ENG_START_DOX @brief Sets the minimum resolution of AMD Radeon™ Boost on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetResolution (adlx_int minRes)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],minRes,adlx_int,@ENG_START_DOX The new minimum resolution (in %) of AMD Radeon Boost. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum resolution is successfully set, __ADLX_OK__ is returned.<br>
        * If the minimum resolution is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Boost dynamically reduces resolution during motion to improve performance with little perceptible impact on image quality. Only works in supported games. <br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetResolution (adlx_int minRes) = 0;

    };  //IADLX3DBoost
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DBoost> IADLX3DBoostPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DBoost, L"IADLX3DBoost")

typedef struct IADLX3DBoost IADLX3DBoost;

typedef struct IADLX3DBoostVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DBoost* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DBoost* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DBoost* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DBoost
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DBoost* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DBoost* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetResolutionRange)(IADLX3DBoost* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetResolution)(IADLX3DBoost* pThis, adlx_int* currentMinRes);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DBoost* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *SetResolution)(IADLX3DBoost* pThis, adlx_int minRes);
}IADLX3DBoostVtbl;

struct IADLX3DBoost { const IADLX3DBoostVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DBoost

//3DImageSharpening setting interface
#pragma region IADLX3DImageSharpening
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DImageSharpening : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DImageSharpening")

        /**
        *@page DOX_IADLX3DImageSharpening_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Image Sharpening is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Image Sharpening is returned. The variable is __true__ if AMD Radeon Image Sharpening is supported. The variable is __false__ if AMD Radeon Image Sharpening is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Image Sharpening is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Image Sharpening is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DImageSharpening_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Image Sharpening is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Image Sharpening is returned. The variable is __true__ if AMD Radeon Image Sharpening is enabled. The variable is __false__ if AMD Radeon Image Sharpening is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Image Sharpening is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Image Sharpening is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.<br>
        * __Note:__ On some AMD GPUs, AMD Radeon Image Sharpening and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Super Resolution is enabled, AMD Radeon Image Sharpening is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DImageSharpening_GetSharpnessRange GetSharpnessRange
        *@ENG_START_DOX @brief Gets the AMD Radeon™ Image Sharpening maximum sharpness, minimum sharpness, and step sharpness on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSharpnessRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the sharpness range is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sharpness range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the sharpness range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum sharpness, minimum sharpness, and step sharpness of AMD Radeon Image Sharpening are read only. @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSharpnessRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLX3DImageSharpening_GetSharpness GetSharpness
        *@ENG_START_DOX @brief Gets the current sharpness of AMD Radeon™ Image Sharpening on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSharpness (adlx_int* currentSharpness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentSharpness,adlx_int*,@ENG_START_DOX The pointer to a variable where the current sharpness (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current sharpness is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current sharpness is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSharpness (adlx_int* currentSharpness) = 0;

        /**
        *@page DOX_IADLX3DImageSharpening_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets AMD Radeon™ Image Sharpening to enabled or disabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Radeon Image Sharpening state. Set __true__ to enable AMD Radeon Image Sharpening. Set __false__ to disable AMD Radeon Image Sharpening. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Image Sharpening is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Image Sharpening is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.<br>
        * __Note:__ On some AMD GPUs, AMD Radeon Image Sharpening and @ref DOX_IADLX3DRadeonSuperResolution "Radeon Super Resolution" cannot be enabled simultaneously. If Radeon Image Sharpening is enabled, AMD Radeon Super Resolution is automatically disabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DImageSharpening_SetSharpness SetSharpness
        *@ENG_START_DOX @brief Sets the sharpness of AMD Radeon™ Image Sharpening on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSharpness (adlx_int sharpness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],sharpness,adlx_int,@ENG_START_DOX The new sharpness (in %). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sharpness is successfully set, __ADLX_OK__ is returned.<br>
        * If the sharpness is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Radeon Image Sharpening restores clarity softened by other effects to in-game visuals, and select productivity and media applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetSharpness (adlx_int sharpness) = 0;

    };  //IADLX3DImageSharpening
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DImageSharpening> IADLX3DImageSharpeningPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DImageSharpening, L"IADLX3DImageSharpening")

typedef struct IADLX3DImageSharpening IADLX3DImageSharpening;

typedef struct IADLX3DImageSharpeningVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DImageSharpening* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DImageSharpening* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DImageSharpening* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DImageSharpening
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DImageSharpening* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DImageSharpening* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetSharpnessRange)(IADLX3DImageSharpening* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetSharpness)(IADLX3DImageSharpening* pThis, adlx_int* currentSharpness);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DImageSharpening* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *SetSharpness)(IADLX3DImageSharpening* pThis, adlx_int sharpness);
}IADLX3DImageSharpeningVtbl;

struct IADLX3DImageSharpening { const IADLX3DImageSharpeningVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DImageSharpening

//3DEnhancedSync setting interface
#pragma region IADLX3DEnhancedSync
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DEnhancedSync : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DEnhancedSync")

        /**
        *@page DOX_IADLX3DEnhancedSync_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Enhanced Sync is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Enhanced Sync is returned. The variable is __true__ if AMD Radeon Enhanced Sync is supported. The variable is __false__ if AMD Radeon Enhanced Sync is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Enhanced Sync is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Enhanced Sync is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Enhanced Sync synchronizes the transition to a new frame of animation with the display refresh rate at a low latency, so no tearing is visible between frames.<br/>
        * Does not limit the frame rate at the display’s refresh rate.<br/>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DEnhancedSync_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Radeon™ Enhanced Sync is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Radeon Enhanced Sync is returned. The variable is __true__ if AMD Radeon Enhanced Sync is enabled. The variable is __false__ if AMD Radeon Enhanced Sync is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Enhanced Sync is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Enhanced Sync is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Enhanced Sync synchronizes the transition to a new frame of animation with the display refresh rate at a low latency, so no tearing is visible between frames.<br/>
        * Does not limit the frame rate at the display’s refresh rate.<br/>
        * __Note__: AMD Radeon Enhanced Sync configuration is dependent on the state of VSync.<br/>
        * If VSync is enabled, AMD Radeon Enhanced Sync is automatically disabled.<br/>
        * If Vsync is disabled, AMD Radeon Enhanced Sync is automatically enabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DEnhancedSync_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets AMD Radeon™ Enhanced Sync to enabled or disabled state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Radeon Enhanced Sync state. Set __true__ to enable AMD Radeon Enhanced Sync. Set __false__ to disable AMD Radeon Enhanced Sync. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Radeon Enhanced Sync is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of AMD Radeon Enhanced Sync is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * AMD Radeon Enhanced Sync synchronizes the transition to a new frame of animation with the display refresh rate at a low latency, so no tearing is visible between frames.<br/>
        * Does not limit the frame rate at the display’s refresh rate.<br/>
        * __Note__: AMD Radeon Enhanced Sync configuration affects the state of VSync.<br/>
        * If AMD Radeon Enhanced Sync is enabled, VSync is automatically disabled.<br/>
        * If AMD Radeon Enhanced Sync is disabled, VSync is automatically enabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

    };  //IADLX3DEnhancedSync
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DEnhancedSync> IADLX3DEnhancedSyncPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DEnhancedSync, L"IADLX3DEnhancedSync")

typedef struct IADLX3DEnhancedSync IADLX3DEnhancedSync;

typedef struct IADLX3DEnhancedSyncVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DEnhancedSync* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DEnhancedSync* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DEnhancedSync* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DEnhancedSync
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DEnhancedSync* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DEnhancedSync* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DEnhancedSync* pThis, adlx_bool enable);
}IADLX3DEnhancedSyncVtbl;

struct IADLX3DEnhancedSync { const IADLX3DEnhancedSyncVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DEnhancedSync

//3DWaitForVerticalRefresh setting interface
#pragma region IADLX3DWaitForVerticalRefresh
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DWaitForVerticalRefresh : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DWaitForVerticalRefresh")

        /**
        *@page DOX_IADLX3DWaitForVerticalRefresh_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if VSync is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of VSync is returned. The variable is __true__ if VSync is supported. The variable is __false__ if VSync is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of VSync is successfully returned, __ADLX_OK__ is returned.<br/>
        * If the state of VSync is not successfully returned, an error code is returned.<br/>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br/> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * VSync synchronizes the transition to a new frame of animation with the display update so no tearing is visible between frames.<br>
        * Limits the frame rate at the display’s refresh rate.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DWaitForVerticalRefresh_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if VSync is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of VSync is returned. The variable is __true__ if VSync is enabled. The variable is __false__ if VSync is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of VSync is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of VSync is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br/> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * VSync synchronizes the transition to a new frame of animation with the display update so no tearing is visible between frames.<br>
        * Limits the frame rate at the display’s refresh rate.<br>
        * __Note__: VSync configuration is dependent on the state of AMD Radeon™ Enhanced Sync.<br>
        * If AMD Radeon Enhanced Sync is enabled, VSync is automatically disabled.<br>
        * If AMD Radeon Enhanced Sync is disabled, VSync is automatically enabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DWaitForVerticalRefresh_GetMode GetMode
        *@ENG_START_DOX @brief Gets the current VSync mode on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMode (@ref ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE* currentMode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMode,@ref ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE*,@ENG_START_DOX The pointer to a variable where the current VSync mode is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current VSync mode is successfully returned, __ADLX_OK__ is returned.<br/>
        * If the current VSync mode is not successfully returned, an error code is returned.<br/>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br/> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * VSync synchronizes the transition to a new frame of animation with the display update so no tearing is visible between frames.<br>
        * Limits the frame rate at the display’s refresh rate.<br>
        * __Note__: VSync configuration is dependent on the state of AMD Radeon™ Enhanced Sync.<br>
        * If AMD Radeon Enhanced Sync is enabled, VSync is automatically disabled.<br>
        * If AMD Radeon Enhanced Sync is disabled, VSync is automatically enabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMode (ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE* currentMode) = 0;

        /**
        *@page DOX_IADLX3DWaitForVerticalRefresh_SetMode SetMode
        *@ENG_START_DOX @brief Sets the VSync mode on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMode (@ref ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE mode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],mode,@ref ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE,@ENG_START_DOX The new VSync mode. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the VSync mode is successfully set, __ADLX_OK__ is returned.<br>
        * If the VSync mode is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * VSync synchronizes the transition to a new frame of animation with the display update so no tearing is visible between frames.<br>
        * Limits the frame rate at the display’s refresh rate.<br>
        * __Note__: VSync configuration affects the state of AMD Radeon™ Enhanced Sync.<br>
        * If VSync is enabled, AMD Radeon Enhanced Sync is automatically disabled.<br>
        * If VSync is disabled, AMD Radeon Enhanced Sync is automatically enabled.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMode (ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE mode) = 0;

    };  //IADLX3DWaitForVerticalRefresh
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DWaitForVerticalRefresh> IADLX3DWaitForVerticalRefreshPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DWaitForVerticalRefresh, L"IADLX3DWaitForVerticalRefresh")

typedef struct IADLX3DWaitForVerticalRefresh IADLX3DWaitForVerticalRefresh;

typedef struct IADLX3DWaitForVerticalRefreshVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DWaitForVerticalRefresh* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DWaitForVerticalRefresh* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DWaitForVerticalRefresh* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DWaitForVerticalRefresh
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DWaitForVerticalRefresh* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DWaitForVerticalRefresh* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetMode)(IADLX3DWaitForVerticalRefresh* pThis, ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE* currentMode);
    ADLX_RESULT (ADLX_STD_CALL *SetMode)(IADLX3DWaitForVerticalRefresh* pThis, ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE mode);
}IADLX3DWaitForVerticalRefreshVtbl;

struct IADLX3DWaitForVerticalRefresh { const IADLX3DWaitForVerticalRefreshVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DWaitForVerticalRefresh

//3DFrameRateTargetControl setting interface
#pragma region IADLX3DFrameRateTargetControl
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DFrameRateTargetControl : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DFrameRateTargetControl")

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if AMD Frame Rate Target Control is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Frame Rate Target Control is returned. The variable is __true__ if AMD Frame Rate Target Control is supported. The variable is __false__ if AMD Frame Rate Target Control is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Frame Rate Target Control is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Frame Rate Target Control is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if AMD Frame Rate Target Control is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of AMD Frame Rate Target Control is returned. The variable is __true__ if AMD Frame Rate Target Control is enabled. The variable is __false__ if AMD Frame Rate Target Control is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Frame Rate Target Control is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of AMD Frame Rate Target Control is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_GetFPSRange GetFPSRange
        *@ENG_START_DOX @brief Gets the maximum FPS, minimum FPS, and step FPS of AMD Frame Rate Target Control on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFPSRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the FPS range of AMD Frame Rate Target Control is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the FPS range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the FPS range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum FPS, minimum FPS, and step FPS of AMD Frame Rate Target Control are read only. @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFPSRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_GetFPS GetFPS
        *@ENG_START_DOX @brief Gets the current FPS of AMD Frame Rate Target Control on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFPS (adlx_int* currentFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentFPS,adlx_int*,@ENG_START_DOX The pointer to a variable where the current FPS value of AMD Frame Rate Target Control is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current FPS is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current FPS is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFPS (adlx_int* currentFPS) = 0;

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets AMD Frame Rate Target Control to enabled or disabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new AMD Frame Rate Target Control state. Set __true__ to enable AMD Frame Rate Target Control. Set __false__ to disable AMD Frame Rate Target Control. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of AMD Frame Rate Target Control is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of AMD Frame Rate Target Control is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DFrameRateTargetControl_SetFPS SetFPS
        *@ENG_START_DOX @brief Sets the maximum FPS of AMD Frame Rate Target Control on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetFPS (adlx_int maxFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],maxFPS,adlx_int,@ENG_START_DOX The new maximum FPS value of AMD Frame Rate Target Control. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the maximum FPS is successfully set, __ADLX_OK__ is returned.<br>
        * If the maximum FPS is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * AMD Frame Rate Target Control sets a user-defined target maximum frame rate in full-screen applications to reduce GPU power consumption.<br>
        * Gaming quality is maintained while GPU power consumption, noise, and heat levels are reduced when running games at higher FPS than the display refresh rate.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetFPS (adlx_int maxFPS) = 0;

    };  //IADLX3DFrameRateTargetControl
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DFrameRateTargetControl> IADLX3DFrameRateTargetControlPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DFrameRateTargetControl, L"IADLX3DFrameRateTargetControl")

typedef struct IADLX3DFrameRateTargetControl IADLX3DFrameRateTargetControl;

typedef struct IADLX3DFrameRateTargetControlVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DFrameRateTargetControl* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DFrameRateTargetControl* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DFrameRateTargetControl* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DFrameRateTargetControl
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DFrameRateTargetControl* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DFrameRateTargetControl* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetFPSRange)(IADLX3DFrameRateTargetControl* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetFPS)(IADLX3DFrameRateTargetControl* pThis, adlx_int* currentFPS);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DFrameRateTargetControl* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *SetFPS)(IADLX3DFrameRateTargetControl* pThis, adlx_int maxFPS);
}IADLX3DFrameRateTargetControlVtbl;

struct IADLX3DFrameRateTargetControl { const IADLX3DFrameRateTargetControlVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DFrameRateTargetControl

//3DAntiAliasing setting interface
#pragma region IADLX3DAntiAliasing
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DAntiAliasing : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DAntiAliasing")

        /**
        *@page DOX_IADLX3DAntiAliasing_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if anti-aliasing is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of anti-aliasing is returned. The variable is __true__ if anti-aliasing is supported. The variable is __false__ if anti-aliasing is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of anti-aliasing is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of anti-aliasing is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_GetMode GetMode
        *@ENG_START_DOX @brief Gets the current anti-aliasing mode of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMode (@ref ADLX_ANTI_ALIASING_MODE* currentMode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMode,@ref ADLX_ANTI_ALIASING_MODE*,@ENG_START_DOX The pointer to a variable where the current anti-aliasing mode is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current anti-aliasing mode is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current anti-aliasing mode is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMode (ADLX_ANTI_ALIASING_MODE* currentMode) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_GetLevel GetLevel
        *@ENG_START_DOX @brief Gets the current anti-aliasing level of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetLevel (@ref ADLX_ANTI_ALIASING_LEVEL* currentLevel)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentLevel,@ref ADLX_ANTI_ALIASING_LEVEL*,@ENG_START_DOX The pointer to a variable where the current anti-aliasing level is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current anti-aliasing level is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current anti-aliasing level is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetLevel (ADLX_ANTI_ALIASING_LEVEL* currentLevel) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_GetMethod GetMethod
        *@ENG_START_DOX @brief Gets the current anti-aliasing method of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMethod (@ref ADLX_ANTI_ALIASING_METHOD* currentMethod)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMethod,@ref ADLX_ANTI_ALIASING_METHOD*,@ENG_START_DOX The pointer to a variable where the current anti-aliasing method is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current anti-aliasing method is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current anti-aliasing method is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMethod (ADLX_ANTI_ALIASING_METHOD* currentMethod) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_SetMode SetMode
        *@ENG_START_DOX @brief Sets the anti-aliasing mode on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMode (@ref ADLX_ANTI_ALIASING_MODE mode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],mode,@ref ADLX_ANTI_ALIASING_MODE,@ENG_START_DOX The new anti-aliasing mode. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the anti-aliasing mode is successfully set, __ADLX_OK__ is returned.<br>
        * If the anti-aliasing mode is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        *
        * __Note__: Set the mode to __AA_MODE_ENHANCE_APP_SETTINGS__ or __AA_MODE_OVERRIDE_APP_SETTINGS__ to concurrently enable other anti-aliasing methods, such as morphological anti-aliasing.<br/>
        * For more information, refer to @ref DOX_IADLX3DMorphologicalAntiAliasing "IADLX3DMorphologicalAntiAliasing".
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMode (ADLX_ANTI_ALIASING_MODE mode) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_SetLevel SetLevel
        *@ENG_START_DOX @brief Sets the anti-aliasing level on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetLevel (@ref ADLX_ANTI_ALIASING_LEVEL level)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],level,@ref ADLX_ANTI_ALIASING_LEVEL,@ENG_START_DOX The new anti-aliasing level. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the anti-aliasing level is successfully set, __ADLX_OK__ is returned.<br>
        * If the anti-aliasing level is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetLevel (ADLX_ANTI_ALIASING_LEVEL level) = 0;

        /**
        *@page DOX_IADLX3DAntiAliasing_SetMethod SetMethod
        *@ENG_START_DOX @brief Sets the anti-aliasing method on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMethod (@ref ADLX_ANTI_ALIASING_METHOD method)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],method,@ref ADLX_ANTI_ALIASING_METHOD,@ENG_START_DOX The new anti-aliasing method. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the anti-aliasing method is successfully set, __ADLX_OK__ is returned.<br>
        * If the anti-aliasing method is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anti-aliasing improves image quality by smoothing jagged edges at the cost of some performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMethod (ADLX_ANTI_ALIASING_METHOD method) = 0;

    };  //IADLX3DAntiAliasing
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DAntiAliasing> IADLX3DAntiAliasingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DAntiAliasing, L"IADLX3DAntiAliasing")

typedef struct IADLX3DAntiAliasing IADLX3DAntiAliasing;

typedef struct IADLX3DAntiAliasingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DAntiAliasing* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DAntiAliasing* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DAntiAliasing* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DAntiAliasing
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DAntiAliasing* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetMode)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_MODE* currentMode);
    ADLX_RESULT (ADLX_STD_CALL *GetLevel)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_LEVEL* currentLevel);
    ADLX_RESULT (ADLX_STD_CALL *GetMethod)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_METHOD* currentMethod);
    ADLX_RESULT (ADLX_STD_CALL *SetMode)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_MODE mode);
    ADLX_RESULT (ADLX_STD_CALL *SetLevel)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_LEVEL level);
    ADLX_RESULT (ADLX_STD_CALL *SetMethod)(IADLX3DAntiAliasing* pThis, ADLX_ANTI_ALIASING_METHOD method);
}IADLX3DAntiAliasingVtbl;

struct IADLX3DAntiAliasing { const IADLX3DAntiAliasingVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DAntiAliasing

//3DMorphologicalAntiAliasing setting interface
#pragma region IADLX3DMorphologicalAntiAliasing
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DMorphologicalAntiAliasing : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DMorphologicalAntiAliasing")

        /**
        *@page DOX_IADLX3DMorphologicalAntiAliasing_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if morphological anti-aliasing is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of morphological anti-aliasing is returned. The variable is __true__ if morphological anti-aliasing is supported. The variable is __false__ if morphological anti-aliasing is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of morphological anti-aliasing is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of morphological anti-aliasing is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Morphological anti-aliasing is an edge-smoothing technique with minimal performance overhead.<br/>
        * The applications must run exclusively in full-screen mode.<br/>
        * Not applicable to DirectX® 12 and Vulkan® applications.
        *
        * __Note__: When morphological anti-aliasing is enabled, the anti-aliasing mode must either be __AA_MODE_ENHANCE_APP_SETTINGS__ or __AA_MODE_OVERRIDE_APP_SETTINGS__, as returned by @ref DOX_IADLX3DAntiAliasing_GetMode.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DMorphologicalAntiAliasing_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if morphological anti-aliasing is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of morphological anti-aliasing is returned. The variable is __true__ if morphological anti-aliasing is enabled. The variable is __false__ if morphological anti-aliasing is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of morphological anti-aliasing is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of morphological anti-aliasing is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Morphological anti-aliasing is an edge-smoothing technique with minimal performance overhead.<br/>
        * The applications must run exclusively in full-screen mode.<br/>
        * Not applicable to DirectX® 12 and Vulkan® applications.
        *
        * __Note__: When morphological anti-aliasing is enabled, the anti-aliasing mode must either be __AA_MODE_ENHANCE_APP_SETTINGS__ or __AA_MODE_OVERRIDE_APP_SETTINGS__, as returned by @ref DOX_IADLX3DAntiAliasing_GetMode.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DMorphologicalAntiAliasing_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets morphological anti-aliasing to enabled or disabled state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new morphological anti-aliasing state. Set __true__ to enable morphological anti-aliasing. Set __false__ to disable morphological anti-aliasing. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of morphological anti-aliasing is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of morphological anti-aliasing is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Morphological anti-aliasing is an edge-smoothing technique with minimal performance overhead.<br/>
        * The applications must run exclusively in full-screen mode.<br/>
        * Not applicable to DirectX® 12 and Vulkan® applications.
        *
        * __Note__: When morphological anti-aliasing is enabled, the anti-aliasing mode must either be __AA_MODE_ENHANCE_APP_SETTINGS__ or __AA_MODE_OVERRIDE_APP_SETTINGS__, as returned by @ref DOX_IADLX3DAntiAliasing_GetMode.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;
    };  //IADLX3DMorphologicalAntiAliasing
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DMorphologicalAntiAliasing> IADLX3DMorphologicalAntiAliasingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DMorphologicalAntiAliasing, L"IADLX3DMorphologicalAntiAliasing")

typedef struct IADLX3DMorphologicalAntiAliasing IADLX3DMorphologicalAntiAliasing;

typedef struct IADLX3DMorphologicalAntiAliasingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DMorphologicalAntiAliasing* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DMorphologicalAntiAliasing* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DMorphologicalAntiAliasing* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DMorphologicalAntiAliasing
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DMorphologicalAntiAliasing* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DMorphologicalAntiAliasing* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DMorphologicalAntiAliasing* pThis, adlx_bool enable);
}IADLX3DMorphologicalAntiAliasingVtbl;

struct IADLX3DMorphologicalAntiAliasing { const IADLX3DMorphologicalAntiAliasingVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DMorphologicalAntiAliasing

//3DAnisotropicFiltering setting interface
#pragma region IADLX3DAnisotropicFiltering
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DAnisotropicFiltering : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DAnisotropicFiltering")

        /**
        *@page DOX_IADLX3DAnisotropicFiltering_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if anisotropic filtering is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of anisotropic filtering is returned. The variable is __true__ if anisotropic filtering is supported. The variable is __false__ if anisotropic filtering is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of anisotropic filtering is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of anisotropic filtering is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anisotropic filtering improves texture quality in most 3D applications on surfaces that appear far away or at odd angles – such as roads or trees – at the cost of some frame rate performance.<br/>
        * Only affects DirectX® 9 applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DAnisotropicFiltering_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if anisotropic filtering is enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of anisotropic filtering is returned. The variable is __true__ if anisotropic filtering is enabled. The variable is __false__ if anisotropic filtering is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of anisotropic filtering is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of anisotropic filtering is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anisotropic filtering improves texture quality in most 3D applications on surfaces that appear far away or at odd angles – such as roads or trees – at the cost of some frame rate performance.<br/>
        * Only affects DirectX® 9 applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsEnabled (adlx_bool* isEnabled) = 0;

        /**
        *@page DOX_IADLX3DAnisotropicFiltering_GetLevel GetLevel
        *@ENG_START_DOX @brief Gets the current anisotropic filtering level on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetLevel (@ref ADLX_ANISOTROPIC_FILTERING_LEVEL* currentLevel)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentLevel,@ref ADLX_ANISOTROPIC_FILTERING_LEVEL*,@ENG_START_DOX The pointer to a variable where the current anisotropic filtering level is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current anisotropic filtering level is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current anisotropic filtering level is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anisotropic filtering improves texture quality in most 3D applications on surfaces that appear far away or at odd angles – such as roads or trees – at the cost of some frame rate performance.<br/>
        * Only affects DirectX® 9 applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetLevel (ADLX_ANISOTROPIC_FILTERING_LEVEL* currentLevel) = 0;

        /**
        *@page DOX_IADLX3DAnisotropicFiltering_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets anisotropic filtering to an enabled or disabled state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new anisotropic filtering state. Set __true__ to enable anisotropic filtering. Set __false__ to disable anisotropic filtering. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of anisotropic filtering is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of anisotropic filtering is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anisotropic filtering improves texture quality in most 3D applications on surfaces that appear far away or at odd angles – such as roads or trees – at the cost of some frame rate performance.<br/>
        * Only affects DirectX® 9 applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DAnisotropicFiltering_SetLevel SetLevel
        *@ENG_START_DOX @brief Sets the anisotropic filtering level of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetLevel (@ref ADLX_ANISOTROPIC_FILTERING_LEVEL level)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],level,@ref ADLX_ANISOTROPIC_FILTERING_LEVEL,@ENG_START_DOX The new anisotropic filtering level. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the anisotropic filtering level is successfully set, __ADLX_OK__ is returned.<br>
        * If the anisotropic filtering level is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Anisotropic filtering improves texture quality in most 3D applications on surfaces that appear far away or at odd angles – such as roads or trees – at the cost of some frame rate performance.<br/>
        * Only affects DirectX® 9 applications.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetLevel (ADLX_ANISOTROPIC_FILTERING_LEVEL level) = 0;

    };  //IADLX3DAnisotropicFiltering
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DAnisotropicFiltering> IADLX3DAnisotropicFilteringPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DAnisotropicFiltering, L"IADLX3DAnisotropicFiltering")

typedef struct IADLX3DAnisotropicFiltering IADLX3DAnisotropicFiltering;

typedef struct IADLX3DAnisotropicFilteringVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DAnisotropicFiltering* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DAnisotropicFiltering* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DAnisotropicFiltering* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DAnisotropicFiltering
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DAnisotropicFiltering* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled)(IADLX3DAnisotropicFiltering* pThis, adlx_bool* isEnabled);
    ADLX_RESULT (ADLX_STD_CALL *GetLevel)(IADLX3DAnisotropicFiltering* pThis, ADLX_ANISOTROPIC_FILTERING_LEVEL* currentLevel);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled)(IADLX3DAnisotropicFiltering* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *SetLevel)(IADLX3DAnisotropicFiltering* pThis, ADLX_ANISOTROPIC_FILTERING_LEVEL level);
}IADLX3DAnisotropicFilteringVtbl;

struct IADLX3DAnisotropicFiltering { const IADLX3DAnisotropicFilteringVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DAnisotropicFiltering

//3DTessellation setting interface
#pragma region IADLX3DTessellation
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DTessellation : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DTessellation")

        /**
        *@page DOX_IADLX3DTessellation_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if tessellation is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of tessellation is returned. The variable is __true__ if tessellation is supported. The variable is __false__ if tessellation is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of tessellation is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of tessellation is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Tessellation adjusts the number of polygons used to render objects with enhanced detail, at the cost of some frame rate performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DTessellation_GetMode GetMode
        *@ENG_START_DOX @brief Gets the current tessellation mode on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMode (@ref ADLX_TESSELLATION_MODE* currentMode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentMode,@ref ADLX_TESSELLATION_MODE*,@ENG_START_DOX The pointer to a variable where the current tessellation mode is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current tessellation mode is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current tessellation mode is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Tessellation adjusts the number of polygons used to render objects with enhanced detail, at the cost of some frame rate performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMode (ADLX_TESSELLATION_MODE* currentMode) = 0;

        /**
        *@page DOX_IADLX3DTessellation_GetLevel GetLevel
        *@ENG_START_DOX @brief Gets the current tessellation level on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetLevel (@ref ADLX_TESSELLATION_LEVEL* currentLevel)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentLevel,@ref ADLX_TESSELLATION_LEVEL*,@ENG_START_DOX The pointer to a variable where the current tessellation level is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current tessellation level is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current tessellation level is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Tessellation adjusts the number of polygons used to render objects with enhanced detail, at the cost of some frame rate performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetLevel (ADLX_TESSELLATION_LEVEL* currentLevel) = 0;

        /**
        *@page DOX_IADLX3DTessellation_SetMode SetMode
        *@ENG_START_DOX @brief Sets the tessellation mode on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMode (@ref ADLX_TESSELLATION_MODE mode)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],mode,@ref ADLX_TESSELLATION_MODE,@ENG_START_DOX The new tessellation mode. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the tesssellation mode is successfully set, __ADLX_OK__ is returned.<br>
        * If the tessellation mode is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Tessellation adjusts the number of polygons used to render objects with enhanced detail, at the cost of some frame rate performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMode (ADLX_TESSELLATION_MODE mode) = 0;

        /**
        *@page DOX_IADLX3DTessellation_SetLevel SetLevel
        *@ENG_START_DOX @brief Sets the tessellation level on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetLevel (@ref ADLX_TESSELLATION_LEVEL level)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],level,@ref ADLX_TESSELLATION_LEVEL,@ENG_START_DOX The new tessellation level. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the tessellation level is successfully set, __ADLX_OK__ is returned.<br>
        * If the tessellation level is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * Tessellation adjusts the number of polygons used to render objects with enhanced detail, at the cost of some frame rate performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetLevel (ADLX_TESSELLATION_LEVEL level) = 0;

    };  //IADLX3DTessellation
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DTessellation> IADLX3DTessellationPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DTessellation, L"IADLX3DTessellation")

typedef struct IADLX3DTessellation IADLX3DTessellation;

typedef struct IADLX3DTessellationVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DTessellation* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DTessellation* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DTessellation* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DTessellation
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DTessellation* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetMode)(IADLX3DTessellation* pThis, ADLX_TESSELLATION_MODE* currentMode);
    ADLX_RESULT (ADLX_STD_CALL *GetLevel)(IADLX3DTessellation* pThis, ADLX_TESSELLATION_LEVEL* currentLevel);
    ADLX_RESULT (ADLX_STD_CALL *SetMode)(IADLX3DTessellation* pThis, ADLX_TESSELLATION_MODE mode);
    ADLX_RESULT (ADLX_STD_CALL *SetLevel)(IADLX3DTessellation* pThis, ADLX_TESSELLATION_LEVEL level);
}IADLX3DTessellationVtbl;

struct IADLX3DTessellation { const IADLX3DTessellationVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DTessellation

//3DRadeonSuperResolution interface
#pragma region IADLX3DRadeonSuperResolution
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DRadeonSuperResolution : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DRadeonSuperResolution")

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if Radeon™ Super Resolution is supported. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Radeon Super Resolution is returned. The variable is __true__ if Radeon Super Resolution is supported. The variable is __false__ if Radeon Super Resolution is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Super Resolution is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Super Resolution is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_IsEnabled IsEnabled
        *@ENG_START_DOX @brief Checks if Radeon™ Super Resolution is enabled. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsEnabled (adlx_bool* enabled)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],enabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of Radeon Super Resolution is returned. The variable is __true__ if Radeon Super Resolution is enabled. The variable is __false__ if Radeon Super Resolution is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Super Resolution is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Super Resolution is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates. <br>
        *
        * __Note:__ @ref DOX_IADLXDisplayGPUScaling "GPU scaling" is a requirement for Radeon Super Resolution. When Radeon Super Resolution is enabled, GPU scaling is automatically enabled. If GPU scaling is disabled while Radeon Super Resolution is enabled, Radeon Super Resolution is automatically disabled.
        *
        * On some AMD GPUs, Radeon Super Resolution is mutually exclusive with @ref DOX_IADLX3DChill "AMD Radeon Chill", @ref DOX_IADLX3DBoost "AMD Radeon Boost", @ref DOX_IADLX3DImageSharpening "AMD Radeon Image Sharpening", @ref DOX_IADLXDisplayIntegerScaling "Integer Display Scaling", and @ref DOX_IADLXDisplayScalingMode_GetMode "Center Scaling". When Radeon Super Resolution is enabled, the mutually exclusive features are automatically disabled.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL IsEnabled (adlx_bool* enabled) = 0;

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_SetEnabled SetEnabled
        *@ENG_START_DOX @brief Sets the activation status of Radeon™ Super Resolution. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetEnabled (adlx_bool enable)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],enable,adlx_bool,@ENG_START_DOX The new Radeon Super Resolution state. Set __true__ to enable Radeon Super Resolution. Set __false__ to disable Radeon Super Resolution. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of Radeon Super Resolution is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of Radeon Super Resolution is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates. <br>
        *
        * __Note:__ @ref DOX_IADLXDisplayGPUScaling "GPU scaling" is a requirement for Radeon Super Resolution. When Radeon Super Resolution is enabled, GPU scaling is automatically enabled. If GPU scaling is disabled while Radeon Super Resolution is enabled, Radeon Super Resolution is automatically disabled.
        *
        * On some AMD GPUs, Radeon Super Resolution is mutually exclusive with @ref DOX_IADLX3DChill "AMD Radeon Chill", @ref DOX_IADLX3DBoost "AMD Radeon Boost", @ref DOX_IADLX3DImageSharpening "AMD Radeon Image Sharpening", @ref DOX_IADLXDisplayIntegerScaling "Integer Display Scaling", and @ref DOX_IADLXDisplayScalingMode_GetMode "Center Scaling". When Radeon Super Resolution is enabled, the mutually exclusive features are automatically disabled.<br>

        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL SetEnabled (adlx_bool enable) = 0;

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_GetSharpnessRange GetSharpnessRange
        *@ENG_START_DOX @brief Gets the Radeon™ Super Resolution maximum sharpness, minimum sharpness, and step sharpness. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSharpnessRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the sharpness range of Radeon Super Resolution is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sharpness range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the sharpness range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum sharpness, minimum sharpness, and step sharpness values are read only. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL GetSharpnessRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_GetSharpness GetSharpness
        *@ENG_START_DOX @brief Gets the Radeon™ Super Resolution current sharpness. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSharpness (adlx_int* currentSharpness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],currentSharpness,adlx_int*,@ENG_START_DOX The pointer to a variable where the current sharpness of Radeon Super Resolution is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the current sharpness is successfully returned, __ADLX_OK__ is returned.<br>
        * If the current sharpness is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL GetSharpness (adlx_int* currentSharpness) = 0;

        /**
        *@page DOX_IADLX3DRadeonSuperResolution_SetSharpness SetSharpness
        *@ENG_START_DOX @brief Sets the Radeon™ Super Resolution sharpness. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSharpness (adlx_int sharpness)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],sharpness,adlx_int,@ENG_START_DOX The new sharpness of Radeon Super Resolution. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sharpness is successfully set, __ADLX_OK__ is returned.<br>
        * If the sharpness is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Radeon Super Resolution is an upscaling feature for faster game frame rates.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT     ADLX_STD_CALL SetSharpness (adlx_int sharpness) = 0;
    };  //IADLX3DRadeonSuperResolution
      //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DRadeonSuperResolution> IADLX3DRadeonSuperResolutionPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DRadeonSuperResolution, L"IADLX3DRadeonSuperResolution")

typedef struct IADLX3DRadeonSuperResolution IADLX3DRadeonSuperResolution;

typedef struct IADLX3DRadeonSuperResolutionVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DRadeonSuperResolution* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DRadeonSuperResolution* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DRadeonSuperResolution* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DRadeonSuperResolution
    ADLX_RESULT (ADLX_STD_CALL *IsSupported) (IADLX3DRadeonSuperResolution* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsEnabled) (IADLX3DRadeonSuperResolution* pThis, adlx_bool* enabled);
    ADLX_RESULT (ADLX_STD_CALL *SetEnabled) (IADLX3DRadeonSuperResolution* pThis, adlx_bool enable);
    ADLX_RESULT (ADLX_STD_CALL *GetSharpnessRange) (IADLX3DRadeonSuperResolution* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *GetSharpness) (IADLX3DRadeonSuperResolution* pThis, adlx_int* currentSharpness);
    ADLX_RESULT (ADLX_STD_CALL *SetSharpness) (IADLX3DRadeonSuperResolution* pThis, adlx_int sharpness);
}IADLX3DRadeonSuperResolutionVtbl;

struct IADLX3DRadeonSuperResolution { const IADLX3DRadeonSuperResolutionVtbl* pVtbl; };

#endif //__cplusplus
#pragma endregion IADLX3DRadeonSuperResolution

//3DResetShaderCache setting interface
#pragma region IADLX3DResetShaderCache
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DResetShaderCache : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DResetShaderCache")

        /**
        *@page DOX_IADLX3DResetShaderCache_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if resetting the shader cache of a GPU is supported. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of shader cache reset is returned. The variable is __true__ if shader cache reset is supported. The variable is __false__ if shader cache reset is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of shader cache reset is successfully returned, ADLX_OK is returned.<br>
        * If the state of shader cache reset is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* The shader cache stores frequently used in-game shaders to reduce loading time and CPU usage. Resetting the shader cache clears contents of the disk-based shader cache.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLX3DResetShaderCache_ResetShaderCache ResetShaderCache
        *@ENG_START_DOX @brief Resets the content in a disk-based shader cache on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    ResetShaderCache ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the shader cache is successfully reset, ADLX_OK is returned.<br>
        * If the shader cache is not successfully reset, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
		* The shader cache stores frequently used in-game shaders to reduce loading time and CPU usage. Resetting the shader cache clears the contents of the disk-based shader cache.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL ResetShaderCache () = 0;

    };  //IADLX3DResetShaderCache
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DResetShaderCache> IADLX3DResetShaderCachePtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DResetShaderCache, L"IADLX3DResetShaderCache")

typedef struct IADLX3DResetShaderCache IADLX3DResetShaderCache;

typedef struct IADLX3DResetShaderCacheVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DResetShaderCache* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DResetShaderCache* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DResetShaderCache* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DResetShaderCache
    ADLX_RESULT (ADLX_STD_CALL *IsSupported)(IADLX3DResetShaderCache* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *ResetShaderCache)(IADLX3DResetShaderCache* pThis);
}IADLX3DResetShaderCacheVtbl;

struct IADLX3DResetShaderCache { const IADLX3DResetShaderCacheVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DResetShaderCache

//Interface with information on 3D setting changes on a display. ADLX passes this to application that registered for 3D setting changed event in the IADLX3DSettingsChangedListener::On3DSettingsChanged()
#pragma region IADLX3DSettingsChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU;
    class ADLX_NO_VTABLE IADLX3DSettingsChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DSettingsChangedEvent")

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_GetGPU GetGPU
        *@ENG_START_DOX @brief Gets the reference-counted GPU interface on which 3D Graphics settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPU (@ref DOX_IADLXGPU **ppGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppGPU,@ref DOX_IADLXGPU** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPU__ to __nullptr__. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLX3DSettingsChangedEvent_GetGPU returns the reference counted GPU used by all the methods in this interface to check if there are any changes in 3D Graphics settings.
        @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "3DSetting.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPU (IADLXGPU** ppGPU) = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsAntiLagChanged IsAntiLagChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Radeon™ Anti-Lag settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsAntiLagChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Radeon Anti-Lag settings, __true__ is returned.<br>
        * If there are on changes to the Radeon Anti-Lag settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsAntiLagChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsChillChanged IsChillChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Radeon™ Chill settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsChillChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the AMD Radeon Chill settings, __true__ is returned.<br>
        * If there are on changes to the AMD Radeon Chill settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsChillChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsBoostChanged IsBoostChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Radeon™ Boost settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsBoostChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the AMD Radeon Boost settings, __true__ is returned.<br>
        * If there are on changes to the AMD Radeon Boost settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsBoostChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsImageSharpeningChanged IsImageSharpeningChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Radeon Image Sharpening settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsImageSharpeningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Radeon Image Sharpening settings, __true__ is returned.<br>
        * If there are on changes to the Radeon Image Sharpening settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsImageSharpeningChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsEnhancedSyncChanged IsEnhancedSyncChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Radeon™ Enhanced Sync settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsEnhancedSyncChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the AMD Radeon Enhanced Sync settings, __true__ is returned.<br>
        * If there are on changes to the AMD Radeon Enhanced Sync settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsEnhancedSyncChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsWaitForVerticalRefreshChanged IsWaitForVerticalRefreshChanged
        *@ENG_START_DOX @brief Checks for changes to the Wait for Vertical Refresh settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsWaitForVerticalRefreshChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Wait for Vertical Refresh settings, __true__ is returned.<br>
        * If there are on changes to the Wait for Vertical Refresh settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsWaitForVerticalRefreshChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsFrameRateTargetControlChanged IsFrameRateTargetControlChanged
        *@ENG_START_DOX @brief Checks for changes to the AMD Frame Rate Target Control settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsFrameRateTargetControlChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the AMD Frame Rate Target Control settings, __true__ is returned.<br>
        * If there are on changes to the AMD Frame Rate Target Control settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsFrameRateTargetControlChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsAntiAliasingChanged IsAntiAliasingChanged
        *@ENG_START_DOX @brief Checks for changes to the Anti-Aliasing settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsAntiAliasingChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Anti-Aliasing settings, __true__ is returned.<br>
        * If there are on changes to the Anti-Aliasing settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsAntiAliasingChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsMorphologicalAntiAliasingChanged IsMorphologicalAntiAliasingChanged
        *@ENG_START_DOX @brief Checks for changes to the Morphological Anti-Aliasing settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsMorphologicalAntiAliasingChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Morphological Anti-Aliasing settings, __true__ is returned.<br>
        * If there are on changes to the Morphological Anti-Aliasing settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsMorphologicalAntiAliasingChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsAnisotropicFilteringChanged IsAnisotropicFilteringChanged
        *@ENG_START_DOX @brief Checks for changes to the Anisotropic Filtering settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsAnisotropicFilteringChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Anisotropic Filtering settings, __true__ is returned.<br>
        * If there are on changes to the Anisotropic Filtering settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsAnisotropicFilteringChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsTessellationModeChanged IsTessellationModeChanged
        *@ENG_START_DOX @brief Checks for changes to the Tessellation settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsTessellationModeChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Tessellation settings, __true__ is returned.<br>
        * If there are no changes to the Tessellation settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsTessellationModeChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsRadeonSuperResolutionChanged IsRadeonSuperResolutionChanged
        *@ENG_START_DOX @brief Checks for changes to the Radeon™ Super Resolution settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsRadeonSuperResolutionChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If there are any changes to the Radeon Super Resolution settings, __true__ is returned.<br>
        * If there are no changes to the Radeon Super Resolution settings, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ Radeon Super Resolution settings are global for all the supported GPUs. For this event notification, @ref DOX_IADLX3DSettingsChangedEvent_GetGPU returns __nullpr__.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL IsRadeonSuperResolutionChanged () = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedEvent_IsResetShaderCache IsResetShaderCache
        *@ENG_START_DOX @brief Checks if shader cache is reset. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsResetShaderCache ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If shader cache settings is reset, __true__ is returned.<br>
        * If shader cache settings is not reset, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLX3DSettingsChangedEvent_GetGPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsResetShaderCache () = 0;

    }; //IADLX3DSettingsChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DSettingsChangedEvent> IADLX3DSettingsChangedEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DSettingsChangedEvent, L"IADLX3DSettingsChangedEvent")
typedef struct IADLX3DSettingsChangedEvent IADLX3DSettingsChangedEvent;

typedef struct IADLX3DSettingsChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DSettingsChangedEvent* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DSettingsChangedEvent* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DSettingsChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLX3DSettingsChangedEvent* pThis);

    // IADLX3DSettingsChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL *GetGPU)(IADLX3DSettingsChangedEvent* pThis, IADLXGPU** ppGPU);
    adlx_bool (ADLX_STD_CALL *IsAntiLagChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsChillChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsBoostChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsImageSharpeningChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsEnhancedSyncChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsWaitForVerticalRefreshChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsFrameRateTargetControlChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsAntiAliasingChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsMorphologicalAntiAliasingChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsAnisotropicFilteringChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsTessellationModeChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsRadeonSuperResolutionChanged)(IADLX3DSettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsResetShaderCache)(IADLX3DSettingsChangedEvent* pThis);

} IADLX3DSettingsChangedEventVtbl;

struct IADLX3DSettingsChangedEvent { const IADLX3DSettingsChangedEventVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DSettingsChangedEvent

//GPU 3D setting changed listener interface. To be implemented in application and passed in IADLX3DSettingsChangedHandling::Add3DSettingEventListener()
#pragma region IADLX3DSettingsChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DSettingsChangedListener
    {
    public:
        /**
        *@page DOX_IADLX3DSettingsChangedListener_On3DSettingsChanged On3DSettingsChanged
        *@ENG_START_DOX @brief __On3DSettingsChanged__ is called by ADLX when 3D Graphics settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    On3DSettingsChanged (@ref DOX_IADLX3DSettingsChangedEvent* p3DSettingsChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,p3DSettingsChangedEvent,@ref DOX_IADLX3DSettingsChangedEvent* ,@ENG_START_DOX The pointer to a 3D Graphics settings change event. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  Once the application registers to the notifications with @ref DOX_IADLX3DSettingsChangedHandling_Add3DSettingsEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLX3DSettingsChangedHandling_Remove3DSettingsEventListener.
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the 3D Graphics settings change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the 3D Graphics settings change event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL On3DSettingsChanged (IADLX3DSettingsChangedEvent* p3DSettingsChangedEvent) = 0;
    }; //IADLX3DSettingsChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLX3DSettingsChangedListener IADLX3DSettingsChangedListener;

typedef struct IADLX3DSettingsChangedListenerVtbl
{
    // IADLX3DSettingsChangedListener interface
    adlx_bool (ADLX_STD_CALL *On3DSettingsChanged)(IADLX3DSettingsChangedListener* pThis, IADLX3DSettingsChangedEvent* p3DSettingsChangedEvent);

} IADLX3DSettingsChangedListenerVtbl;

struct IADLX3DSettingsChangedListener { const IADLX3DSettingsChangedListenerVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DSettingsChangedListener

//Interface that allows registration to 3D settings-related events
#pragma region IADLX3DSettingsChangedHandling
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLX3DSettingsChangedHandling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DSettingsChangedHandling")

        /**
        *@page DOX_IADLX3DSettingsChangedHandling_Add3DSettingsEventListener Add3DSettingsEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when 3D Graphics settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add3DSettingsEventListener (@ref DOX_IADLX3DSettingsChangedListener* p3DSettingsChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,p3DSettingsChangedListener,@ref DOX_IADLX3DSettingsChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving 3D Graphics settings change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully registered, ADLX will call @ref DOX_IADLX3DSettingsChangedListener_On3DSettingsChanged method of the listener when 3D Graphics settings change.<br>
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLX3DSettingsChangedHandling_Remove3DSettingsEventListener.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Add3DSettingsEventListener (IADLX3DSettingsChangedListener* p3DSettingsChangedListener) = 0;

        /**
        *@page DOX_IADLX3DSettingsChangedHandling_Remove3DSettingsEventListener Remove3DSettingsEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from 3D Graphics settings event list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Remove3DSettingsEventListener (@ref DOX_IADLX3DSettingsChangedListener* p3DSettingsChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,p3DSettingsChangedListener,@ref DOX_IADLX3DSettingsChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving 3D Graphics settings change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLX3DSettingsChangedListener_On3DSettingsChanged method of the listener when 3D Graphics settings change.
        * The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Remove3DSettingsEventListener (IADLX3DSettingsChangedListener* p3DSettingsChangedListener) = 0;

    }; //IADLX3DSettingsChangedHandling
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DSettingsChangedHandling> IADLX3DSettingsChangedHandlingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DSettingsChangedHandling, L"IADLX3DSettingsChangedHandling")
typedef struct IADLX3DSettingsChangedHandling IADLX3DSettingsChangedHandling;

typedef struct IADLX3DSettingsChangedHandlingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DSettingsChangedHandling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DSettingsChangedHandling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DSettingsChangedHandling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLX3DSettingsChangedHandling interface
    ADLX_RESULT (ADLX_STD_CALL *Add3DSettingsEventListener)(IADLX3DSettingsChangedHandling* pThis, IADLX3DSettingsChangedListener* p3DSettingsChangedListener);
    ADLX_RESULT (ADLX_STD_CALL *Remove3DSettingsEventListener)(IADLX3DSettingsChangedHandling* pThis, IADLX3DSettingsChangedListener* p3DSettingsChangedListener);

} IADLX3DSettingsChangedHandlingVtbl;

struct IADLX3DSettingsChangedHandling { const IADLX3DSettingsChangedHandlingVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DSettingsChangedHandling

//3DSetting Services interface
#pragma region IADLX3DSettingsServices
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU;

    class ADLX_NO_VTABLE IADLX3DSettingsServices : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLX3DSettingsServices")

        /**
        *@page DOX_IADLX3DSettingsServices_GetAntiLag GetAntiLag
        *@ENG_START_DOX @brief Gets the reference-counted AMD Radeon™ Anti-Lag interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAntiLag (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DAntiLag** pp3DAntiLag)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DAntiLag,@ref DOX_IADLX3DAntiLag** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DAntiLag__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetAntiLag (IADLXGPU* pGPU, IADLX3DAntiLag** pp3DAntiLag) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetChill GetChill
        *@ENG_START_DOX @brief Gets the reference-counted AMD Radeon™ Chill interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetChill (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DChill** pp3DChill)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DChill,@ref DOX_IADLX3DChill** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DChill__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetChill (IADLXGPU* pGPU, IADLX3DChill** pp3DChill) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetBoost GetBoost
        *@ENG_START_DOX @brief Gets the reference-counted AMD Radeon™ Boost interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetBoost (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DBoost** pp3DBoost)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DBoost,@ref DOX_IADLX3DBoost** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DBoost__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetBoost (IADLXGPU* pGPU, IADLX3DBoost** pp3DBoost) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetImageSharpening GetImageSharpening
        *@ENG_START_DOX @brief Gets the reference-counted AMD Radeon Image Sharpening interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetImageSharpening (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DImageSharpening** pp3DImageSharpening)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DImageSharpening,@ref DOX_IADLX3DImageSharpening** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DImageSharpening__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetImageSharpening (IADLXGPU* pGPU, IADLX3DImageSharpening** pp3DImageSharpening) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetEnhancedSync GetEnhancedSync
        *@ENG_START_DOX @brief Gets the reference-counted AMD AMD Radeon™ Enhanced Sync interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetEnhancedSync (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DEnhancedSync** pp3DEnhancedSync)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DEnhancedSync,@ref DOX_IADLX3DEnhancedSync** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DEnhancedSync__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetEnhancedSync (IADLXGPU* pGPU, IADLX3DEnhancedSync** pp3DEnhancedSync) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetWaitForVerticalRefresh GetWaitForVerticalRefresh
        *@ENG_START_DOX @brief Gets the reference-counted Wait for Vertical Refresh interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetWaitForVerticalRefresh (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DWaitForVerticalRefresh** pp3DWaitForVerticalRefresh)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DWaitForVerticalRefresh,@ref DOX_IADLX3DWaitForVerticalRefresh** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DWaitForVerticalRefresh__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetWaitForVerticalRefresh (IADLXGPU* pGPU, IADLX3DWaitForVerticalRefresh** pp3DWaitForVerticalRefresh) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetFrameRateTargetControl GetFrameRateTargetControl
        *@ENG_START_DOX @brief Gets the reference-counted AMD Frame Rate Target Control interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFrameRateTargetControl (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DFrameRateTargetControl** pp3DFrameRateTargetControl)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DFrameRateTargetControl,@ref DOX_IADLX3DFrameRateTargetControl** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DFrameRateTargetControl__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFrameRateTargetControl (IADLXGPU* pGPU, IADLX3DFrameRateTargetControl** pp3DFrameRateTargetControl) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetAntiAliasing GetAntiAliasing
        *@ENG_START_DOX @brief Gets the reference-counted Anti-Aliasing interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAntiAliasing (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DAntiAliasing** pp3DAntiAliasing)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DAntiAliasing,@ref DOX_IADLX3DAntiAliasing** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DAntiAliasing__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetAntiAliasing (IADLXGPU* pGPU, IADLX3DAntiAliasing** pp3DAntiAliasing) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetMorphologicalAntiAliasing GetMorphologicalAntiAliasing
        *@ENG_START_DOX @brief Gets the reference-counted Morphological Anti-Aliasing interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMorphologicalAntiAliasing (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DMorphologicalAntiAliasing** pp3DMorphologicalAntiAliasing)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DMorphologicalAntiAliasing,@ref DOX_IADLX3DMorphologicalAntiAliasing** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DMorphologicalAntiAliasing__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMorphologicalAntiAliasing (IADLXGPU* pGPU, IADLX3DMorphologicalAntiAliasing** pp3DMorphologicalAntiAliasing) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetAnisotropicFiltering GetAnisotropicFiltering
        *@ENG_START_DOX @brief Gets the reference-counted Anisotropic Filtering interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAnisotropicFiltering (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DAnisotropicFiltering** pp3DAnisotropicFiltering)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DAnisotropicFiltering,@ref DOX_IADLX3DAnisotropicFiltering** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DAnisotropicFiltering__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetAnisotropicFiltering (IADLXGPU* pGPU, IADLX3DAnisotropicFiltering** pp3DAnisotropicFiltering) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetTessellation GetTessellation
        *@ENG_START_DOX @brief Gets the reference-counted Tessellation interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTessellation (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DTessellation** pp3DTessellation)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DTessellation,@ref DOX_IADLX3DTessellation** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DTessellation__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTessellation (IADLXGPU* pGPU, IADLX3DTessellation** pp3DTessellation) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetRadeonSuperResolution GetRadeonSuperResolution
        *@ENG_START_DOX @brief Gets the reference-counted Radeon™ Super Resolution interface. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetRadeonSuperResolution (@ref DOX_IADLX3DRadeonSuperResolution** pp3DRadeonSuperResolution)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,pp3DRadeonSuperResolution,@ref DOX_IADLX3DRadeonSuperResolution** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DRadeonSuperResolution__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetRadeonSuperResolution (IADLX3DRadeonSuperResolution** pp3DRadeonSuperResolution) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_GetResetShaderCache GetResetShaderCache
        *@ENG_START_DOX @brief Gets the reference-counted Reset Shader Cache interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetResetShaderCache (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLX3DResetShaderCache** pp3DResetShaderCache)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,pp3DResetShaderCache,@ref DOX_IADLX3DResetShaderCache** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DResetShaderCache__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetResetShaderCache (IADLXGPU* pGPU, IADLX3DResetShaderCache** pp3DResetShaderCache) = 0;

        /**
        *@page DOX_IADLX3DSettingsServices_Get3DSettingsChangedHandling Get3DSettingsChangedHandling
        *@ENG_START_DOX @brief Gets the reference-counted interface that allows registering and unregistering for notifications when 3D Graphics settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Get3DSettingsChangedHandling (@ref DOX_IADLX3DSettingsChangedHandling** pp3DSettingsChangedHandling)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,pp3DSettingsChangedHandling,@ref DOX_IADLX3DSettingsChangedHandling** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DSettingsChangedHandling__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "I3DSettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Get3DSettingsChangedHandling (IADLX3DSettingsChangedHandling** pp3DSettingsChangedHandling) = 0;

    };  //IADLX3DSettingsServices
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLX3DSettingsServices> IADLX3DSettingsServicesPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLX3DSettingsServices, L"IADLX3DSettingsServices")
typedef struct IADLX3DSettingsServices IADLX3DSettingsServices;

typedef struct IADLX3DSettingsServicesVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLX3DSettingsServices* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLX3DSettingsServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLX3DSettingsServices* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLX3DSettingsServices
    ADLX_RESULT (ADLX_STD_CALL *GetAntiLag)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DAntiLag** pp3DAntiLag);
    ADLX_RESULT (ADLX_STD_CALL *GetChill)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DChill** pp3DChill);
    ADLX_RESULT (ADLX_STD_CALL *GetBoost)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DBoost** pp3DBoost);
    ADLX_RESULT (ADLX_STD_CALL *GetImageSharpening)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DImageSharpening** pp3DImageSharpening);
    ADLX_RESULT (ADLX_STD_CALL *GetEnhancedSync)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DEnhancedSync** pp3DEnhancedSync);
    ADLX_RESULT (ADLX_STD_CALL *GetWaitForVerticalRefresh)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DWaitForVerticalRefresh** pp3DWaitForVerticalRefresh);
    ADLX_RESULT (ADLX_STD_CALL *GetFrameRateTargetControl)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DFrameRateTargetControl** pp3DFrameRateTargetControl);
    ADLX_RESULT (ADLX_STD_CALL *GetAntiAliasing)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DAntiAliasing** pp3DAntiAliasing);
    ADLX_RESULT (ADLX_STD_CALL *GetMorphologicalAntiAliasing)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DMorphologicalAntiAliasing** pp3DMorphologicalAntiAliasing);
    ADLX_RESULT (ADLX_STD_CALL *GetAnisotropicFiltering)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DAnisotropicFiltering** pp3DAnisotropicFiltering);
    ADLX_RESULT (ADLX_STD_CALL *GetTessellation)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DTessellation** pp3DTessellation);
    ADLX_RESULT (ADLX_STD_CALL *GetRadeonSuperResolution) (IADLX3DSettingsServices* pThis, IADLX3DRadeonSuperResolution** pp3DRadeonSuperResolution);
    ADLX_RESULT (ADLX_STD_CALL *GetResetShaderCache)(IADLX3DSettingsServices* pThis, IADLXGPU* pGPU, IADLX3DResetShaderCache** pp3DResetShaderCache);
    ADLX_RESULT (ADLX_STD_CALL *Get3DSettingsChangedHandling)(IADLX3DSettingsServices* pThis, IADLX3DSettingsChangedHandling** pp3DSettingsChangedHandling);
}IADLX3DSettingsServicesVtbl;

struct IADLX3DSettingsServices { const IADLX3DSettingsServicesVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLX3DSettingsServices

#endif //ADLX_I3DSETTINGS_H
