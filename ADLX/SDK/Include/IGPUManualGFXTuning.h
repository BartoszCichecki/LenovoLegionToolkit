//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------
#ifndef ADLX_IGPUMANUALGFXTUNING_H
#define ADLX_IGPUMANUALGFXTUNING_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IGPUManualGFXTunings.h - Interfaces for ADLX GPU Manual Graphics Tuning functionality

// Manual Graphics Tuning
#pragma region IADLXManualGraphicsTuning1
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualTuningStateList;
    class ADLX_NO_VTABLE IADLXManualGraphicsTuning1 : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualGraphicsTuning1")

        /**
        *@page DOX_IADLXManualGraphicsTuning1_GetGPUTuningRanges GetGPUTuningRanges
        *@ENG_START_DOX @brief Gets the frequency range and the voltage range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUTuningRanges (@ref ADLX_IntRange* frequencyRange, @ref ADLX_IntRange* voltageRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],frequencyRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the GPU frequency range (in MHz) is returned. @ENG_END_DOX}
        *@paramrow{2.,[out],voltageRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the GPU voltage range (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the frequency and voltage range are successfully returned, __ADLX_OK__ is returned.<br>
        * If the frequency and voltage range are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The frequency range and the voltage range are applicable to all the GPU states on this GPU. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs allow multiple GPU tuning states. Each GPU tuning state is represented by the GPU clock speed and the GPU voltage.<br/>
        * The GPU clock speed and the GPU voltage can be adjusted within their ranges.
        * @ENG_END_DOX
        *
		*@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUTuningRanges (ADLX_IntRange* frequencyRange, ADLX_IntRange* voltageRange) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning1_GetGPUTuningStates GetGPUTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of current GPU tuning states. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUTuningStates (@ref DOX_IADLXManualTuningStateList** ppGFXStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppGFXStates,@ref DOX_IADLXManualTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGFXStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the list of GPU tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of GPU tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.<br>
		*
		* Some GPUs allow multiple GPU tuning states. Each GPU tuning state is represented by the GPU clock speed and the GPU voltage. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUTuningStates (IADLXManualTuningStateList** ppGFXStates) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning1_GetEmptyGPUTuningStates GetEmptyGPUTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of empty GPU tuning states. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetEmptyGPUTuningStates (@ref DOX_IADLXManualTuningStateList** ppGFXStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppGFXStates,@ref DOX_IADLXManualTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGFXStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the list of empty GPU tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of empty GPU tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.<br>
        *
		* The frequency and voltage in the returned states are zero.
		*
		* Some GPUs allow multiple GPU tuning states. Each GPU tuning state is represented by the GPU clock speed and the GPU voltage. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetEmptyGPUTuningStates (IADLXManualTuningStateList** ppGFXStates) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning1_IsValidGPUTuningStates IsValidGPUTuningStates
        *@ENG_START_DOX @brief Checks if each GPU tuning state in a list is valid. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsValidGPUTuningStates (@ref DOX_IADLXManualTuningStateList* pGFXStates, adlx_int* errorIndex)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pGFXStates,@ref DOX_IADLXManualTuningStateList*,@ENG_START_DOX The pointer to the GPU states list interface. @ENG_END_DOX}
        *@paramrow{2.,[out],errorIndex,adlx_int*,@ENG_START_DOX The pointer to a variable where the invalid states index is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If __IsValidGPUTuningStates__ is successfully executed, __ADLX_OK__ is returned.<br>
        * If __IsValidGPUTuningStates__ is not successfully executed, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs allow multiple GPU tuning states. Each GPU tuning state is represented by the GPU clock speed and the GPU voltage.
        * @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details If the __*pGFXStates__ is valid then the method sets the __errorIndex__ to -1. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsValidGPUTuningStates (IADLXManualTuningStateList* pGFXStates, adlx_int* errorIndex) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning1_SetGPUTuningStates SetGPUTuningStates
        *@ENG_START_DOX @brief Sets the GPU tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGPUTuningStates (@ref DOX_IADLXManualTuningStateList* pGFXStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pGFXStates,@ref DOX_IADLXManualTuningStateList*,@ENG_START_DOX The pointer to the GPU states list interface. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU tuning states are successfully set, __ADLX_OK__ is returned.<br>
        * If the GPU tuning states are not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs allow multiple GPU tuning states. Each GPU tuning state is represented by the GPU clock speed and the GPU voltage.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGPUTuningStates (IADLXManualTuningStateList* pGFXStates) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualGraphicsTuning1> IADLXManualGraphicsTuning1Ptr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID(IADLXManualGraphicsTuning1, L"IADLXManualGraphicsTuning1")
typedef struct IADLXManualTuningStateList IADLXManualTuningStateList;
typedef struct IADLXManualGraphicsTuning1 IADLXManualGraphicsTuning1;

typedef struct IADLXManualGraphicsTuning1Vtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualGraphicsTuning1* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualGraphicsTuning1* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualGraphicsTuning1* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualGraphicsTuning1
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTuningRanges)(IADLXManualGraphicsTuning1* pThis, ADLX_IntRange* frequencyRange, ADLX_IntRange* voltageRange);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTuningStates)(IADLXManualGraphicsTuning1* pThis, IADLXManualTuningStateList** ppGFXStates);
    ADLX_RESULT (ADLX_STD_CALL *GetEmptyGPUTuningStates)(IADLXManualGraphicsTuning1* pThis, IADLXManualTuningStateList** ppGFXStates);
    ADLX_RESULT (ADLX_STD_CALL *IsValidGPUTuningStates)(IADLXManualGraphicsTuning1* pThis, IADLXManualTuningStateList* pGFXStates, adlx_int* errorIndex);
    ADLX_RESULT (ADLX_STD_CALL *SetGPUTuningStates)(IADLXManualGraphicsTuning1* pThis, IADLXManualTuningStateList* pGFXStates);
}IADLXManualGraphicsTuning1Vtbl;

struct IADLXManualGraphicsTuning1 { const IADLXManualGraphicsTuning1Vtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualGraphicsTuning1

// Manual Graphics Tuning
#pragma region IADLXManualGraphicsTuning2
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualGraphicsTuning2 : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualGraphicsTuning2")

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUMinFrequencyRange GetGPUMinFrequencyRange
        *@ENG_START_DOX @brief Gets the minimum frequency range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMinFrequencyRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the minimum frequency range (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum frequency range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the minimum frequency range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support minimum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUMinFrequencyRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUMinFrequency GetGPUMinFrequency
        *@ENG_START_DOX @brief Gets the current minimum frequency value on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMinFrequency (adlx_int* minFreq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],minFreq,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum frequency value (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum frequency value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the minimum frequency value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support minimum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUMinFrequency (adlx_int* minFreq) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_SetGPUMinFrequency SetGPUMinFrequency
        *@ENG_START_DOX @brief Sets the minimum frequency on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGPUMinFrequency (adlx_int minFreq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],minFreq,adlx_int,@ENG_START_DOX The new minimum frequency value (in MHz). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum frequency value is successfully set, __ADLX_OK__ is returned.<br>
        * If the minimum frequency value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support minimum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGPUMinFrequency (adlx_int minFreq) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUMaxFrequencyRange GetGPUMaxFrequencyRange
        *@ENG_START_DOX @brief Gets the maximum frequency range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMaxFrequencyRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the maximum frequency range (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the maximum frequency range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the maximum frequency range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support maximum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUMaxFrequencyRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUMaxFrequency GetGPUMaxFrequency
        *@ENG_START_DOX @brief Gets the current maximum frequency value on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMaxFrequency (adlx_int* maxFreq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],maxFreq,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum frequency value (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the maximum frequency value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the maximum frequency value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support maximum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUMaxFrequency (adlx_int* maxFreq) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_SetGPUMaxFrequency SetGPUMaxFrequency
        *@ENG_START_DOX @brief Sets the maximum frequency on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGPUMaxFrequency (adlx_int maxFreq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],maxFreq,adlx_int,@ENG_START_DOX The new maximum frequency (in MHz) value. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the maximum frequency value is successfully set, __ADLX_OK__ is returned.<br>
        * If the maximum frequency value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support maximum GPU clock speed adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGPUMaxFrequency (adlx_int maxFreq) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUVoltageRange GetGPUVoltageRange
        *@ENG_START_DOX @brief Gets the clock voltage range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUVoltageRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the clock voltage range (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the clock voltage range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the clock voltage range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support GPU voltage adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUVoltageRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_GetGPUVoltage GetGPUVoltage
        *@ENG_START_DOX @brief Gets the current clock voltage value on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUVoltage (adlx_int* volt)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],volt,adlx_int*,@ENG_START_DOX The pointer to a variable where the GPU voltage (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the clock voltage value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the clock voltage value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support GPU voltage adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUVoltage (adlx_int* volt) = 0;

        /**
        *@page DOX_IADLXManualGraphicsTuning2_SetGPUVoltage SetGPUVoltage
        *@ENG_START_DOX @brief Sets the clock voltage on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGPUVoltage (adlx_int volt)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],volt,adlx_int,@ENG_START_DOX The new GPU voltage (in mV). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the clock voltage value is successfully set, __ADLX_OK__ is returned.<br>
        * If the clock voltage value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support GPU voltage adjustment within a range.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualGFXTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGPUVoltage (adlx_int volt) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualGraphicsTuning2> IADLXManualGraphicsTuning2Ptr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualGraphicsTuning2, L"IADLXManualGraphicsTuning2")

typedef struct IADLXManualGraphicsTuning2 IADLXManualGraphicsTuning2;

typedef struct IADLXManualGraphicsTuning2Vtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualGraphicsTuning2* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualGraphicsTuning2* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualGraphicsTuning2* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualGraphicsTuning2
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMinFrequencyRange)(IADLXManualGraphicsTuning2* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMinFrequency)(IADLXManualGraphicsTuning2* pThis, adlx_int* minFreq);
    ADLX_RESULT (ADLX_STD_CALL *SetGPUMinFrequency)(IADLXManualGraphicsTuning2* pThis, adlx_int minFreq);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMaxFrequencyRange)(IADLXManualGraphicsTuning2* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMaxFrequency)(IADLXManualGraphicsTuning2* pThis, adlx_int* maxFreq);
    ADLX_RESULT (ADLX_STD_CALL *SetGPUMaxFrequency)(IADLXManualGraphicsTuning2* pThis, adlx_int maxFreq);

    ADLX_RESULT (ADLX_STD_CALL *GetGPUVoltageRange)(IADLXManualGraphicsTuning2* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUVoltage)(IADLXManualGraphicsTuning2* pThis, adlx_int* volt);
    ADLX_RESULT (ADLX_STD_CALL *SetGPUVoltage)(IADLXManualGraphicsTuning2* pThis, adlx_int volt);
}IADLXManualGraphicsTuning2Vtbl;
struct IADLXManualGraphicsTuning2 { const IADLXManualGraphicsTuning2Vtbl *pVtbl; };

#endif //__cplusplus
#pragma endregion IADLXManualGraphicsTuning2

#endif//ADLX_IGPUMANUALGFXTUNING_H
