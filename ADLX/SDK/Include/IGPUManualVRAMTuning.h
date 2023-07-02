//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------
#ifndef ADLX_IGPUMANUALVRAMTUNING_H
#define ADLX_IGPUMANUALVRAMTUNING_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IGPUManualVRAMTuning.h - Interfaces for ADLX GPU manual VRAM Tuning functionality

// Manual VRAM Tuning, common verion
#pragma region IADLXManualVRAMTuning1
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualTuningStateList;
    class ADLX_NO_VTABLE IADLXMemoryTimingDescriptionList;
    class ADLX_NO_VTABLE IADLXManualVRAMTuning1 : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualVRAMTuning1")

        /**
        *@page DOX_IADLXManualVRAMTuning1_IsSupportedMemoryTiming IsSupportedMemoryTiming
        *@ENG_START_DOX @brief Checks if the memory timing is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedMemoryTiming (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of memory timing is returned. The variable is __true__ if memory timing is supported. The variable is __false__ if memory timing is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of memory timing is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of memory timing is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedMemoryTiming (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_GetSupportedMemoryTimingDescriptionList GetSupportedMemoryTimingDescriptionList
        *@ENG_START_DOX @brief Gets the reference counted list of the supported memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSupportedMemoryTimingDescriptionList (@ref DOX_IADLXMemoryTimingDescriptionList** ppDescriptionList)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppDescriptionList,@ref DOX_IADLXMemoryTimingDescriptionList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDescriptionList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of the supported memory timing description is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of the supported memory timing description is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
		*
		* Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSupportedMemoryTimingDescriptionList (IADLXMemoryTimingDescriptionList** ppDescriptionList) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_GetMemoryTimingDescription GetMemoryTimingDescription
        *@ENG_START_DOX @brief Gets the current memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMemoryTimingDescription (@ref ADLX_MEMORYTIMING_DESCRIPTION* description)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],description,@ref ADLX_MEMORYTIMING_DESCRIPTION*,@ENG_START_DOX The pointer to a variable where the memory timing description is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the memory timing description is successfully returned, __ADLX_OK__ is returned.<br>
        * If the memory timing description is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMemoryTimingDescription (ADLX_MEMORYTIMING_DESCRIPTION* description) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_SetMemoryTimingDescription SetMemoryTimingDescription
        *@ENG_START_DOX @brief Sets the memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMemoryTimingLevel (@ref ADLX_MEMORYTIMING_DESCRIPTION description)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],description,@ref ADLX_MEMORYTIMING_DESCRIPTION,@ENG_START_DOX The new memory timing description. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the memory timing description is successfully set, __ADLX_OK__ is returned.<br>
        * If the memory timing description is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMemoryTimingDescription (ADLX_MEMORYTIMING_DESCRIPTION description) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_GetVRAMTuningRanges GetVRAMTuningRanges
        *@ENG_START_DOX @brief Gets the VRAM frequency range and the VRAM voltage range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetVRAMTuningRanges (@ref ADLX_IntRange* frequencyRange, @ref ADLX_IntRange* voltageRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],frequencyRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the frequency range (in MHz) is returned. @ENG_END_DOX}
        *@paramrow{2.,[out],voltageRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the voltage range (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the frequency and voltage range are successfully returned, __ADLX_OK__ is returned.<br>
        * If the frequency and voltage range are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The VRAM frequency range and the VRAM voltage range are applicable to all the VRAM states on this GPU. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetVRAMTuningRanges (ADLX_IntRange* frequencyRange, ADLX_IntRange* voltageRange) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_GetVRAMTuningStates GetVRAMTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of current VRAM tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetVRAMTuningStates (@ref DOX_IADLXManualTuningStateList** ppVRAMStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppVRAMStates,@ref DOX_IADLXManualTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppVRAMStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of VRAM tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of VRAM tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetVRAMTuningStates (IADLXManualTuningStateList** ppVRAMStates) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_GetEmptyVRAMTuningStates GetEmptyVRAMTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of empty VRAM tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetEmptyVRAMTuningStates (@ref DOX_IADLXManualTuningStateList** ppVRAMStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppVRAMStates,@ref DOX_IADLXManualTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppVRAMStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of empty VRAM tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of empty VRAM tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  All the value in this list are zero.
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetEmptyVRAMTuningStates (IADLXManualTuningStateList** ppVRAMStates) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_IsValidVRAMTuningStates IsValidVRAMTuningStates
        *@ENG_START_DOX @brief Checks the validity of listed VRAM tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsValidVRAMTuningStates (@ref DOX_IADLXManualTuningStateList* pVRAMStates, adlx_int* errorIndex)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pVRAMStates,@ref DOX_IADLXManualTuningStateList*,@ENG_START_DOX The pointer to the VRAM states list interface. @ENG_END_DOX}
        *@paramrow{2.,[out],errorIndex,adlx_int*,@ENG_START_DOX The pointer to a variable where the invalid states index is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If __IsValidVRAMTuningStates__ is successfully executed, __ADLX_OK__ is returned.<br>
        * If __IsValidVRAMTuningStates__ is not successfully executed, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details If the __*pVRAMStates__ is valid then the method sets the __errorIndex__ to -1. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsValidVRAMTuningStates (IADLXManualTuningStateList* pVRAMStates, adlx_int* errorIndex) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning1_SetVRAMTuningStates SetVRAMTuningStates
        *@ENG_START_DOX @brief Sets listed VRAM states to the tuning state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetVRAMTuningStates (@ref DOX_IADLXManualTuningStateList* pVRAMStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pVRAMStates,@ref DOX_IADLXManualTuningStateList*,@ENG_START_DOX The pointer to the VRAM states list interface. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the VRAM tuning states are successfully set, __ADLX_OK__ is returned.<br>
        * If the VRAM tuning states are not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetVRAMTuningStates (IADLXManualTuningStateList* pVRAMStates) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualVRAMTuning1> IADLXManualVRAMTuning1Ptr;
} //namespace adlx
#else //__cplusplus

ADLX_DECLARE_IID (IADLXManualVRAMTuning1, L"IADLXManualVRAMTuning1")
typedef struct IADLXManualVRAMTuning1 IADLXManualVRAMTuning1;
typedef struct IADLXManualTuningStateList IADLXManualTuningStateList;
typedef struct IADLXMemoryTimingDescriptionList IADLXMemoryTimingDescriptionList;
typedef struct IADLXManualVRAMTuning1Vtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualVRAMTuning1* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualVRAMTuning1* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualVRAMTuning1* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualVRAMTuning1
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedMemoryTiming)(IADLXManualVRAMTuning1* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetSupportedMemoryTimingDescriptionList)(IADLXManualVRAMTuning1* pThis, IADLXMemoryTimingDescriptionList** ppDescriptionList);
    ADLX_RESULT (ADLX_STD_CALL *GetMemoryTimingDescription)(IADLXManualVRAMTuning1* pThis, ADLX_MEMORYTIMING_DESCRIPTION* description);
    ADLX_RESULT (ADLX_STD_CALL *SetMemoryTimingDescription)(IADLXManualVRAMTuning1* pThis, ADLX_MEMORYTIMING_DESCRIPTION description);

    ADLX_RESULT (ADLX_STD_CALL *GetVRAMTuningRanges)(IADLXManualVRAMTuning1* pThis, ADLX_IntRange* frequencyRange, ADLX_IntRange* voltageRange);
    ADLX_RESULT (ADLX_STD_CALL *GetVRAMTuningStates)(IADLXManualVRAMTuning1* pThis, IADLXManualTuningStateList** ppVRAMStates);
    ADLX_RESULT (ADLX_STD_CALL *GetEmptyVRAMTuningStates)(IADLXManualVRAMTuning1* pThis, IADLXManualTuningStateList** ppVRAMStates);
    ADLX_RESULT (ADLX_STD_CALL *IsValidVRAMTuningStates)(IADLXManualVRAMTuning1* pThis, IADLXManualTuningStateList* pVRAMStates, adlx_int* errorIndex);
    ADLX_RESULT (ADLX_STD_CALL *SetVRAMTuningStates)(IADLXManualVRAMTuning1* pThis, IADLXManualTuningStateList* pVRAMStates);
}IADLXManualVRAMTuning1Vtbl;

struct IADLXManualVRAMTuning1 { const IADLXManualVRAMTuning1Vtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualVRAMTuning1

// Manual VRAM Tuning2 for Navi2X
#pragma region IADLXManualVRAMTuning2
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualVRAMTuning2 : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualVRAMTuning2")

        /**
        *@page DOX_IADLXManualVRAMTuning2_IsSupportedMemoryTiming IsSupportedMemoryTiming
        *@ENG_START_DOX @brief Checks if the memory timing is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedMemoryTiming (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of memory timing is returned. The variable is __true__ if memory timing is supported. The variable is __false__ if memory timing is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of memory timing is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of memory timing is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedMemoryTiming (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_GetSupportedMemoryTimingDescriptionList GetSupportedMemoryTimingDescriptionList
        *@ENG_START_DOX @brief Gets the reference counted list of the supported memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSupportedMemoryTimingDescriptionList (@ref DOX_IADLXMemoryTimingDescriptionList** ppDescriptionList)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppDescriptionList,@ref DOX_IADLXMemoryTimingDescriptionList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDescriptionList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of the supported memory timing description is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of the supported memory timing description is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
		*
		* Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSupportedMemoryTimingDescriptionList (IADLXMemoryTimingDescriptionList** ppDescriptionList) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_GetMemoryTimingDescription GetMemoryTimingDescription
        *@ENG_START_DOX @brief Gets the current memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMemoryTimingDescription (@ref ADLX_MEMORYTIMING_DESCRIPTION* description)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],description,@ref ADLX_MEMORYTIMING_DESCRIPTION*,@ENG_START_DOX The pointer to a variable where the memory timing description is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the memory timing description is successfully returned, __ADLX_OK__ is returned.<br>
        * If the memory timing description is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMemoryTimingDescription (ADLX_MEMORYTIMING_DESCRIPTION* description) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_SetMemoryTimingDescription SetMemoryTimingDescription
        *@ENG_START_DOX @brief Sets the memory timing description on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMemoryTimingLevel (@ref ADLX_MEMORYTIMING_DESCRIPTION description)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],description,@ref ADLX_MEMORYTIMING_DESCRIPTION,@ENG_START_DOX The new memory timing description. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the memory timing description is successfully set, __ADLX_OK__ is returned.<br>
        * If the memory timing description is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support memory timing presets for VRAM latency control to adjust the video memory clock speed.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMemoryTimingDescription (ADLX_MEMORYTIMING_DESCRIPTION description) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_GetMaxVRAMFrequencyRange GetMaxVRAMFrequencyRange
        *@ENG_START_DOX @brief Gets the maximum VRAM frequency range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMaxVRAMFrequencyRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the max frequency range (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the max frequency range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the max frequency range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMaxVRAMFrequencyRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_GetMaxVRAMFrequency GetMaxVRAMFrequency
        *@ENG_START_DOX @brief Gets the current maximum frequency value of a VRAM on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMaxVRAMFrequency (adlx_int* freq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],freq,adlx_int*,@ENG_START_DOX The pointer to a variable where the max frequency value (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the max frequency value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the max frequency value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMaxVRAMFrequency (adlx_int* freq) = 0;

        /**
        *@page DOX_IADLXManualVRAMTuning2_SetMaxVRAMFrequency SetMaxVRAMFrequency
        *@ENG_START_DOX @brief Sets the maximum VRAM frequency value on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMaxVRAMFrequency (adlx_int freq)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],freq,adlx_int,@ENG_START_DOX The new max frequency value (in MHz). @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the max frequency value is successfully set, __ADLX_OK__ is returned.<br>
        * If the max frequency value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualVRAMTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMaxVRAMFrequency (adlx_int freq) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualVRAMTuning2> IADLXManualVRAMTuning2Ptr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualVRAMTuning2, L"IADLXManualVRAMTuning2")
typedef struct IADLXManualVRAMTuning2 IADLXManualVRAMTuning2;
typedef struct IADLXManualVRAMTuning2Vtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualVRAMTuning2* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualVRAMTuning2* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualVRAMTuning2* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualVRAMTuning2
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedMemoryTiming)(IADLXManualVRAMTuning2* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetSupportedMemoryTimingDescriptionList)(IADLXManualVRAMTuning2* pThis, IADLXMemoryTimingDescriptionList** ppDescriptionList);
    ADLX_RESULT (ADLX_STD_CALL *GetMemoryTimingDescription)(IADLXManualVRAMTuning2* pThis, ADLX_MEMORYTIMING_DESCRIPTION* description);
    ADLX_RESULT (ADLX_STD_CALL *SetMemoryTimingDescription)(IADLXManualVRAMTuning2* pThis, ADLX_MEMORYTIMING_DESCRIPTION description);

    ADLX_RESULT (ADLX_STD_CALL *GetMaxVRAMFrequencyRange)(IADLXManualVRAMTuning2* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetMaxVRAMFrequency)(IADLXManualVRAMTuning2* pThis, adlx_int* freq);
    ADLX_RESULT (ADLX_STD_CALL *SetMaxVRAMFrequency)(IADLXManualVRAMTuning2* pThis, adlx_int freq);
}IADLXManualVRAMTuning2Vtbl;

struct IADLXManualVRAMTuning2 { const IADLXManualVRAMTuning2Vtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualVRAMTuning2

#endif//ADLX_IGPUMANUALVRAMTUNING_H
