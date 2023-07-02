//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IGPUPRESETTUNING_H
#define ADLX_IGPUPRESETTUNING_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IGPUPresetTuning.h - Interfaces for ADLX GPU Preset Tuning functionality

// Preset Tuning
#pragma region IADLXGPUPresetTuning
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUPresetTuning : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUPresetTuning")

        /**
        *@page DOX_IADLXGPUPresetTuning_IsSupportedPowerSaver IsSupportedPowerSaver
        *@ENG_START_DOX @brief Checks if the power saver tuning preset is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedPowerSaver (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of power saver is returned. The variable is __true__ if power saver is supported. The variable is __false__ if power saver is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of power saver is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of power saver is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        * @addinfo
        * @ENG_START_DOX
        * Use of the power saver tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedPowerSaver (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsSupportedQuiet IsSupportedQuiet
        *@ENG_START_DOX @brief Checks if the quiet tuning preset is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedQuiet (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of quiet is returned. The variable is __true__ if quiet is supported. The variable is __false__ if quiet is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of quiet is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of quiet is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The quiet tuning preset enables quiet operation with lowered power and fan settings.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedQuiet (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsSupportedBalanced IsSupportedBalanced
        *@ENG_START_DOX @brief Checks if the balanced tuning preset is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedBalanced (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the balanced of quiet is returned. The variable is __true__ if balanced is supported. The variable is __false__ if balanced is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of balanced is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of balanced is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The balanced tuning preset offers all-round performance with a balance of power, clocks, and fan settings. Use of the balanced tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedBalanced (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsSupportedTurbo IsSupportedTurbo
        *@ENG_START_DOX @brief Checks if the turbo tuning preset is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedTurbo (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of turbo is returned. The variable is __true__ if turbo is supported. The variable is __false__ if turbo is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of turbo is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of turbo is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The turbo tuning preset sets a higher power limit to allow more headroom for performance. Use of the turbo tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedTurbo (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsSupportedRage IsSupportedRage
        *@ENG_START_DOX @brief Checks if the rage tuning preset is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedRage (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of rage is returned. The variable is __true__ if rage is supported. The variable is __false__ if rage is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of rage is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of rage is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The rage tuning preset sets a higher power limit to allow more headroom for performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedRage (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsCurrentPowerSaver IsCurrentPowerSaver
        *@ENG_START_DOX @brief Checks if the power saver tuning preset is used on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentPowerSaver (adlx_bool* isPowerSaver)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isPowerSaver,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of power saver is returned. The variable is __true__ if power saver is applied. The variable is __false__ if power saver is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of power saver is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of power saver is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Use of the power saver tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentPowerSaver (adlx_bool* isPowerSaver) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsCurrentQuiet IsCurrentQuiet
        *@ENG_START_DOX @brief Checks if the quiet tuning preset is used on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentQuiet (adlx_bool* isQuiet)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isQuiet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of quiet is returned. The variable is __true__ if quiet is applied. The variable is __false__ if quiet is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of quiet is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of quiet is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The quiet tuning preset enables quiet operation with lowered power and fan settings.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentQuiet (adlx_bool* isQuiet) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsCurrentBalanced IsCurrentBalanced
        *@ENG_START_DOX @brief Checks if the balanced tuning preset is used on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentBalanced (adlx_bool* isBalanced)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isBalanced,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of balanced is returned. The variable is __true__ if balanced is applied. The variable is __false__ if balanced is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of balanced is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of balanced is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The balanced tuning preset offers all-round performance with a balance of power, clocks, and fan settings. Use of the balanced tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentBalanced (adlx_bool* isBalance) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsCurrentTurbo IsCurrentTurbo
        *@ENG_START_DOX @brief Checks if the turbo tuning preset is used on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentTurbo (adlx_bool* isTurbo)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isTurbo,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of turbo is returned. The variable is __true__ if turbo is applied. The variable is __false__ if turbo is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of turbo is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of turbo is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The turbo tuning preset sets a higher power limit to allow more headroom for performance. Use of the turbo preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentTurbo (adlx_bool* isTurbo) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_IsCurrentRage IsCurrentRage
        *@ENG_START_DOX @brief Checks if the rage tuning preset is used on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentRage (adlx_bool* isRage)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isRage,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of rage is returned. The variable is __true__ if rage is applied. The variable is __false__ if rage is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of rage is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of rage is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The rage tuning preset sets a higher power limit to allow more headroom for performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentRage (adlx_bool* isRage) = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_SetPowerSaver SetPowerSaver
        *@ENG_START_DOX @brief Sets the power saver tuning preset on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetPowerSaver ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the power saver is successfully applied, __ADLX_OK__ is returned.<br>
        * If the power saver is not successfully applied, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Use of the power saver tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetPowerSaver () = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_SetQuiet SetQuiet
        *@ENG_START_DOX @brief Sets the quiet tuning preset on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetQuiet ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the quiet is successfully applied, __ADLX_OK__ is returned.<br>
        * If the quiet is not successfully applied, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The quiet tuning preset enables quiet operation with lowered power and fan settings.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetQuiet () = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_SetBalanced SetBalanced
        *@ENG_START_DOX @brief Sets the balanced tuning preset on a GPU. @ENG_END_DOX
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
        *@ENG_START_DOX  If the balanced is successfully applied, __ADLX_OK__ is returned.<br>
        * If the balanced is not successfully applied, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The balanced tuning preset offers all-round performance with a balance of power, clocks, and fan settings. Use of the balanced tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetBalanced () = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_SetTurbo SetTurbo
        *@ENG_START_DOX @brief Sets the turbo tuning preset on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetTurbo ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the turbo is successfully applied, __ADLX_OK__ is returned.<br>
        * If the turbo is not successfully applied, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The turbo tuning preset sets a higher power limit to allow more headroom for performance. Use of the turbo tuning preset is limited to some GPUs.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetTurbo () = 0;

        /**
        *@page DOX_IADLXGPUPresetTuning_SetRage SetRage
        *@ENG_START_DOX @brief Sets the rage tuning preset on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetRage ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the rage is successfully applied, __ADLX_OK__ is returned.<br>
        * If the rage is not successfully applied, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The rage tuning preset sets a higher power limit to allow more headroom for performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUPresetTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetRage () = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUPresetTuning> IADLXGPUPresetTuningPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUPresetTuning, L"IADLXGPUPresetTuning")

typedef struct IADLXGPUPresetTuning IADLXGPUPresetTuning;

typedef struct IADLXGPUPresetTuningVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUPresetTuning* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUPresetTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUPresetTuning* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPUPresetTuning
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedPowerSaver)(IADLXGPUPresetTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedQuiet)(IADLXGPUPresetTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedBalanced)(IADLXGPUPresetTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedTurbo)(IADLXGPUPresetTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedRage)(IADLXGPUPresetTuning* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL *IsCurrentPowerSaver)(IADLXGPUPresetTuning* pThis, adlx_bool* isPowerSaver);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentQuiet)(IADLXGPUPresetTuning* pThis, adlx_bool* isQuiet);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentBalanced)(IADLXGPUPresetTuning* pThis, adlx_bool* isBalance);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentTurbo)(IADLXGPUPresetTuning* pThis, adlx_bool* isTurbo);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentRage)(IADLXGPUPresetTuning* pThis, adlx_bool* isRage);

    ADLX_RESULT (ADLX_STD_CALL *SetPowerSaver)(IADLXGPUPresetTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetQuiet)(IADLXGPUPresetTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetBalanced)(IADLXGPUPresetTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetTurbo)(IADLXGPUPresetTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetRage)(IADLXGPUPresetTuning* pThis);

}IADLXGPUPresetTuningVtbl;

struct IADLXGPUPresetTuning { const IADLXGPUPresetTuningVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUPresetTuning

#endif//ADLX_IGPUPRESETTUNING_H
