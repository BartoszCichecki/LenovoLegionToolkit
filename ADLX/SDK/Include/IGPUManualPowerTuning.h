//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IGPUMANUALPOWERTUNING_H
#define ADLX_IGPUMANUALPOWERTUNING_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IGPUManualPowerTuning.h - Interfaces for ADLX GPU Manual Power Tuning functionality
// Manual Power Tuning
#pragma region IADLXManualPowerTuning
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualPowerTuning : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualPowerTuning")

        /**
        *@page DOX_IADLXManualPowerTuning_GetPowerLimitRange GetPowerLimitRange
        *@ENG_START_DOX @brief Gets the manual power tuning minimum power range, maximum power range, and step power range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetPowerLimitRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the manual power limit range (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the power limit range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the power limit range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetPowerLimitRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_GetPowerLimit GetPowerLimit
        *@ENG_START_DOX @brief Gets the current power limit of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetPowerLimit (adlx_int* curVal)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],curVal,adlx_int*,@ENG_START_DOX The pointer to a variable where the manual power limit value (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the power limit value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the power limit value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * A higher power limit increases performance headroom.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetPowerLimit (adlx_int* curVal) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_SetPowerLimit SetPowerLimit
        *@ENG_START_DOX @brief Sets the power limit of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetPowerLimit (adlx_int curVal)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],curVal,adlx_int,@ENG_START_DOX The new power limit value (in %) . @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the power limit value is successfully set, __ADLX_OK__ is returned.<br>
        * If the power limit value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * A higher power limit increases performance headroom.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetPowerLimit (adlx_int curVal) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_IsSupportedTDCLimit IsSupportedTDCLimit
        *@ENG_START_DOX
        *@brief Checks if Thermal Design Current (TDC) limit is supported on a GPU.
        *@details Thermal Design Current (TDC) functionality is not currently implemented in a production application. Usecase validation for these methods should be performed by application developers.
		*@ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedTDCLimit (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of TDC limit feature is returned. The variable is __true__ if TDC limit feature is supported. The variable is __false__ if TDC limit feature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the state of TDC limit feature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of TDC limit feature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        *@ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * A higher TDC limit increases performance headroom.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedTDCLimit(adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_GetTDCLimitRange GetTDCLimitRange
        *@ENG_START_DOX
        *@brief Gets the manual power tuning minimum Thermal Design Current (TDC) range, maximum TDC range, and step TDC range on a GPU.
        *@details Thermal Design Current (TDC) functionality is not currently implemented in a production application. Usecase validation for these methods should be performed by application developers.
		*@ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTDCLimitRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the manual TDC limit range (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the TDC limit range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the TDC limit range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTDCLimitRange(ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_GetTDCLimit GetTDCLimit
        *@ENG_START_DOX
        *@brief Gets the current Thermal Design Current (TDC) limit of a GPU.
        *@details Thermal Design Current (TDC) functionality is not currently implemented in a production application. Usecase validation for these methods should be performed by application developers.
		*@ENG_END_DOX
		*
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTDCLimit (adlx_int* curVal)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],curVal,adlx_int*,@ENG_START_DOX The pointer to a variable where the manual TDC limit value (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the TDC limit value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the TDC limit value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        *@ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * A higher TDC limit increases performance headroom.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTDCLimit(adlx_int* curVal) = 0;

        /**
        *@page DOX_IADLXManualPowerTuning_SetTDCLimit SetTDCLimit
        *@ENG_START_DOX
        *@brief Sets the Thermal Design Current (TDC) limit of a GPU.
        *@details Thermal Design Current (TDC) functionality is not currently implemented in a production application. Usecase validation for these methods should be performed by application developers.
		*@ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetTDCLimit (adlx_int curVal)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],curVal,adlx_int,@ENG_START_DOX The new TDC limit value (in %). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the TDC limit value is successfully set, __ADLX_OK__ is returned.<br>
        * If the TDC limit value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        *@ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * A higher TDC limit increases performance headroom.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualPowerTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetTDCLimit(adlx_int curVal) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualPowerTuning> IADLXManualPowerTuningPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualPowerTuning, L"IADLXManualPowerTuning")

typedef struct IADLXManualPowerTuning IADLXManualPowerTuning;

typedef struct IADLXManualPowerTuningVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualPowerTuning* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualPowerTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualPowerTuning* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualPowerTuning
    ADLX_RESULT (ADLX_STD_CALL *GetPowerLimitRange)(IADLXManualPowerTuning* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetPowerLimit)(IADLXManualPowerTuning* pThis, adlx_int* curVal);
    ADLX_RESULT (ADLX_STD_CALL *SetPowerLimit)(IADLXManualPowerTuning* pThis, adlx_int curVal);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedTDCLimit)(IADLXManualPowerTuning* pThis,adlx_bool* supported);
    ADLX_RESULT(ADLX_STD_CALL* GetTDCLimitRange)(IADLXManualPowerTuning* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT(ADLX_STD_CALL* GetTDCLimit)(IADLXManualPowerTuning* pThis, adlx_int* curVal);
    ADLX_RESULT(ADLX_STD_CALL* SetTDCLimit)(IADLXManualPowerTuning* pThis, adlx_int curVal);
}IADLXManualPowerTuningVtbl;

struct IADLXManualPowerTuning { const IADLXManualPowerTuningVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualPowerTuning

#endif//ADLX_IGPUMANUALPOWERTUNING_H
