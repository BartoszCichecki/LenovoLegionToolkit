//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IGPUMANUALFANTUNING_H
#define ADLX_IGPUMANUALFANTUNING_H
#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"

//-------------------------------------------------------------------------------------------------
//IGPUManualFanTuning.h - Interfaces for ADLX GPU Manual Fan Tuning functionality

//Manual Tuning interface
#pragma region IADLXManualFanTuningState
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualFanTuningState : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualFanTuningState")

        /**
        * @page DOX_IADLXManualFanTuningState_GetFanSpeed GetFanSpeed
        * @ENG_START_DOX
        * @brief Gets the fan speed in the manual fan tuning state.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetFanSpeed (adlx_int* value)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,value,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the fan speed (in %) is returned. @ENG_END_DOX}
        *
        * @retvalues
		* @ENG_START_DOX
        * If the fan speed is successfully returned, __ADLX_OK__ is returned.<br>
        * If the fan speed is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFanSpeed (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualFanTuningState_SetFanSpeed SetFanSpeed
        *@ENG_START_DOX @brief Sets the fan speed in the manual fan tuning state. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetFanSpeed (adlx_int value)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,value,adlx_int ,@ENG_START_DOX The new fan speed value (in %). @ENG_END_DOX}
        *
        * @retvalues
		* @ENG_START_DOX
        * If the fan speed is successfully set, __ADLX_OK__ is returned.<br>
        * If the fan speed is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Method for applying fan speed for the fan tuning state. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetFanSpeed (adlx_int value) = 0;

        /**
        * @page DOX_IADLXManualFanTuningState_GetTemperature GetTemperature
        * @ENG_START_DOX
        * @brief Gets the temperature in the manual fan tuning state.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetTemperature (adlx_uint* value)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,value,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the temperature (in °C) is returned. @ENG_END_DOX}
        *
        * @retvalues
		* @ENG_START_DOX
        * If the temperature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the temperature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTemperature (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualFanTuningState_SetTemperature SetTemperature
        *@ENG_START_DOX @brief Sets the temperature in the manual fan tuning state. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetTemperature (adlx_int value)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,value,adlx_int ,@ENG_START_DOX The new temperature value (in °C). @ENG_END_DOX}
        *
        * @retvalues
		* @ENG_START_DOX
        * If the temperature is successfully set, __ADLX_OK__ is returned.<br>
        * If the temperature is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetTemperature (adlx_int value) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualFanTuningState> IADLXManualFanTuningStatePtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualFanTuningState, L"IADLXManualFanTuningState")

typedef struct IADLXManualFanTuningState IADLXManualFanTuningState;

typedef struct IADLXManualFanTuningStateVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualFanTuningState* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualFanTuningState* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualFanTuningState* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualFanTuningState
    ADLX_RESULT (ADLX_STD_CALL *GetFanSpeed)(IADLXManualFanTuningState* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL *SetFanSpeed)(IADLXManualFanTuningState* pThis, adlx_int value);
    ADLX_RESULT (ADLX_STD_CALL *GetTemperature)(IADLXManualFanTuningState* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL *SetTemperature)(IADLXManualFanTuningState* pThis, adlx_int value);
}IADLXManualFanTuningStateVtbl;

struct IADLXManualFanTuningState { const IADLXManualFanTuningStateVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualFanTuningState

//IADLXManualFanTuningState list interface
#pragma region IADLXManualFanTuningStateList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualFanTuningStateList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualFanTuningStateList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXManualFanTuningState::IID ())

        /**
        * @page DOX_IADLXManualFanTuningStateList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXManualFanTuningState** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXManualFanTuningState** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned.<br>
        * If the location is not within the list bounds, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
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
        * @DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXManualFanTuningState** ppItem) = 0;
        /**
        *@page DOX_IADLXManualFanTuningStateList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXManualFanTuningState* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXManualFanTuningState* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
        * If the interface is not added to the end of the list, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXManualFanTuningState* pItem) = 0;
    };  //IADLXManualFanTuningStateList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualFanTuningStateList> IADLXManualFanTuningStateListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualFanTuningStateList, L"IADLXManualFanTuningStateList")
ADLX_DECLARE_ITEM_IID (IADLXManualFanTuningState, IID_IADLXManualFanTuningState ())

typedef struct IADLXManualFanTuningStateList IADLXManualFanTuningStateList;

typedef struct IADLXManualFanTuningStateListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualFanTuningStateList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualFanTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualFanTuningStateList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXManualFanTuningStateList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXManualFanTuningStateList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXManualFanTuningStateList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXManualFanTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXManualFanTuningStateList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXManualFanTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXManualFanTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXManualFanTuningStateList* pThis, IADLXInterface* pItem);

    //IADLXManualFanTuningStateList
    ADLX_RESULT (ADLX_STD_CALL *At_ManualFanTuningStateList)(IADLXManualFanTuningStateList* pThis, const adlx_uint location, IADLXManualFanTuningState** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_ManualFanTuningStateList)(IADLXManualFanTuningStateList* pThis, IADLXManualFanTuningState* pItem);

}IADLXManualFanTuningStateListVtbl;

struct IADLXManualFanTuningStateList { const IADLXManualFanTuningStateListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualFanTuningStateList

// Manual FAN Tuning
#pragma region IADLXManualFanTuning
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualFanTuning : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualFanTuning")

        /**
        *@page DOX_IADLXManualFanTuning_GetFanTuningRanges GetFanTuningRanges
        *@ENG_START_DOX @brief Gets the fan speed range and the temperature range on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFanTuningRanges (@ref ADLX_IntRange* speedRange, @ref ADLX_IntRange* temperatureRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],speedRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the fan speed range (in %) is returned. @ENG_END_DOX}
        *@paramrow{2.,[out],temperatureRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the temperature range (in °C) is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the fan speed and temperature range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the fan speed and temperature range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The fan speed range and the temperature range are applicable to all GPU states on a GPU.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFanTuningRanges (ADLX_IntRange* speedRange, ADLX_IntRange* temperatureRange) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetFanTuningStates GetFanTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of current GPU fan tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFanTuningStates (@ref DOX_IADLXManualFanTuningStateList** ppStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppStates,@ref DOX_IADLXManualFanTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of GPU fan tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of GPU fan tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFanTuningStates (IADLXManualFanTuningStateList** ppStates) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetEmptyFanTuningStates GetEmptyFanTuningStates
        *@ENG_START_DOX @brief Gets the reference counted list of empty GPU fan tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetEmptyFanTuningStates (@ref DOX_IADLXManualFanTuningStateList** ppStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],ppStates,@ref DOX_IADLXManualFanTuningStateList**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppStates__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the list of empty GPU fan tuning states is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of empty GPU fan tuning states is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetEmptyFanTuningStates (IADLXManualFanTuningStateList** ppStates) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_IsValidFanTuningStates IsValidFanTuningStates
        *@ENG_START_DOX @brief Checks if each GPU fan tuning state in a list is valid on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsValidFanTuningStates (@ref DOX_IADLXManualFanTuningStateList* pStates, adlx_int* errorIndex)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pStates,@ref DOX_IADLXManualFanTuningStateList*,@ENG_START_DOX The pointer to the GPU fan tuning states list interface. @ENG_END_DOX}
        *@paramrow{2.,[out],errorIndex,adlx_int*,@ENG_START_DOX The pointer to a variable where the invalid states index is returned. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If __IsValidFanTuningStates__ is successfully executed, __ADLX_OK__ is returned.<br>
        * If __IsValidFanTuningStates__ is not successfully executed, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details If the __*pStates__ is valid then the method sets the __errorIndex__ to -1. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsValidFanTuningStates (IADLXManualFanTuningStateList* pStates, adlx_int* errorIndex) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_SetFanTuningStates SetFanTuningStates
        *@ENG_START_DOX @brief Sets the fan tuning states on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetFanTuningStates (@ref DOX_IADLXManualFanTuningStateList* pStates)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pStates,@ref DOX_IADLXManualFanTuningStateList*,@ENG_START_DOX The pointer to the GPU states list interface. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the GPU fan tuning states are successfully set, __ADLX_OK__ is returned.<br>
        * If the GPU fan tuning states are not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetFanTuningStates (IADLXManualFanTuningStateList* pStates) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_IsSupportedZeroRPM IsSupportedZeroRPM
        *@ENG_START_DOX @brief Checks if zero RPM is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedZeroRPM (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of zero RPM feature is returned. The variable is __true__ if zero RPM feature is supported. The variable is __false__ if zero RPM feature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of zero RPM feature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of zero RPM feature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Zero RPM enables quiet operation when the GPU is under a light load and speeds up the fans when the GPU load and temperature increases.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedZeroRPM (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetZeroRPMState GetZeroRPMState
        *@ENG_START_DOX @brief Checks if zero RPM is currently activated on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetZeroRPMState (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of zero RPM is returned. The variable is __true__ if zero RPM is enabled. The variable is __false__ if zero RPM is not enabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of zero RPM is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of zero RPM is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Zero RPM enables quiet operation when the GPU is under a light load and speeds up the fans when the GPU load and temperature increases.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetZeroRPMState (adlx_bool* isSet) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_SetZeroRPMState SetZeroRPMState
        *@ENG_START_DOX @brief Enables or disables zero RPM on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetZeroRPMState (adlx_bool set)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],isSet,adlx_bool,@ENG_START_DOX The new zero RPM state. Set __true__ to enable zero RPM. Set __false__ to disable zero RPM. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of zero RPM is successfully set, __ADLX_OK__ is returned.<br>
        * If the state of zero RPM is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Zero RPM enables quiet operation when the GPU is under a light load and speeds up the fans when the GPU load and temperature increases.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetZeroRPMState (adlx_bool set) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_IsSupportedMinAcousticLimit IsSupportedMinAcousticLimit
        *@ENG_START_DOX @brief Checks if the minimum acoustic limit is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedMinAcousticLimit (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of min acoustic limit feature is returned. The variable is __true__ if min acoustic limit feature is supported. The variable is __false__ if min acoustic limit feature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of min acoustic limit feature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of min acoustic limit feature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support Minimum Acoustic Limit adjustments (in MHz).
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedMinAcousticLimit (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetMinAcousticLimitRange GetMinAcousticLimitRange
        *@ENG_START_DOX @brief Gets the maximum value, minimum value, and step for the minimum acoustic limit on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMinAcousticLimitRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the min acoustic limit range (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the min acoustic limit range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the min acoustic limit range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support Minimum Acoustic Limit adjustments (in MHz).
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMinAcousticLimitRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetMinAcousticLimit GetMinAcousticLimit
        *@ENG_START_DOX @brief Gets the current minimum acoustic limit on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMinAcousticLimit (adlx_int* value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],value,adlx_int*,@ENG_START_DOX The pointer to a variable where the min acoustic limit value (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the min acoustic limit value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the min acoustic limit value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support Minimum Acoustic Limit adjustments (in MHz).
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMinAcousticLimit (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_SetMinAcousticLimit SetMinAcousticLimit
        *@ENG_START_DOX @brief Sets the minimum acoustic limit on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMinAcousticLimit (adlx_int value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],value,adlx_int,@ENG_START_DOX The new min acoustic limit (in MHz). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the min acoustic limit value is successfully set, __ADLX_OK__ is returned.<br>
        * If the min acoustic limit value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support Minimum Acoustic Limit adjustments (in MHz).
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMinAcousticLimit (adlx_int value) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_IsSupportedMinFanSpeed IsSupportedMinFanSpeed
        *@ENG_START_DOX @brief Checks if the minimum fan speed is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedMinFanSpeed (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of minimum fan speed feature is returned. The variable is __true__ if minimum fan speed feature is supported. The variable is __false__ if minimum fan speed feature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of minimum fan speed feature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of minimum fan speed feature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedMinFanSpeed (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetMinFanSpeedRange GetMinFanSpeedRange
        *@ENG_START_DOX @brief Gets the maximum value, minimum value, and step for the minimum fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMinFanSpeedRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the minimum fan speed range (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum fan speed range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the minimum fan speed range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMinFanSpeedRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetMinFanSpeed GetMinFanSpeed
        *@ENG_START_DOX @brief Gets the current minimum fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMinFanSpeed (adlx_int* value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],value,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum fan speed value (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum fan speed value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the minimum fan speed value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Some GPUs support minimum fan speed adjustments (in MHz).
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetMinFanSpeed (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_SetMinFanSpeed SetMinFanSpeed
        *@ENG_START_DOX @brief Sets the minimum fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMinFanSpeed (adlx_int value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],value,adlx_int,@ENG_START_DOX The new minimum fan speed (in RPM). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the minimum fan speed value is successfully set, __ADLX_OK__ is returned.<br>
        * If the minimum fan speed value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetMinFanSpeed (adlx_int value) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_IsSupportedTargetFanSpeed IsSupportedTargetFanSpeed
        *@ENG_START_DOX @brief Checks if the target fan speed is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedTargetFanSpeed (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of target fan speed feature is returned. The variable is __true__ if target fan speed feature is supported. The variable is __false__ if target fan speed feature is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of target fan speed feature is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of target fan speed feature is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedTargetFanSpeed (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetTargetFanSpeedRange GetTargetFanSpeedRange
        *@ENG_START_DOX @brief Gets the maximum value, minimum value, and step for the target fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTargetFanSpeedRange (@ref ADLX_IntRange* tuningRange)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],tuningRange,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the target fan speed range (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the target fan speed range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the target fan speed range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTargetFanSpeedRange (ADLX_IntRange* tuningRange) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_GetTargetFanSpeed GetTargetFanSpeed
        *@ENG_START_DOX @brief Gets the current target fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetTargetFanSpeed (adlx_int* value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],value,adlx_int*,@ENG_START_DOX The pointer to a variable where the target fan speed value (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the target fan speed value is successfully returned, __ADLX_OK__ is returned.<br>
        * If the target fan speed value is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetTargetFanSpeed (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualFanTuning_SetTargetFanSpeed SetTargetFanSpeed
        *@ENG_START_DOX @brief Sets the target fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetTargetFanSpeed (adlx_int value)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],value,adlx_int,@ENG_START_DOX The new target fan speed (in RPM). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the target fan speed value is successfully set, __ADLX_OK__ is returned.<br>
        * If the target fan speed value is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUManualFanTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetTargetFanSpeed (adlx_int value) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualFanTuning> IADLXManualFanTuningPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualFanTuning, L"IADLXManualFanTuning")

typedef struct IADLXManualFanTuning IADLXManualFanTuning;

typedef struct IADLXManualFanTuningVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualFanTuning* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualFanTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualFanTuning* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualFanTuningState
    ADLX_RESULT (ADLX_STD_CALL *GetFanTuningRanges)(IADLXManualFanTuning* pThis, ADLX_IntRange* speedRange, ADLX_IntRange* temperatureRange);
    ADLX_RESULT (ADLX_STD_CALL *GetFanTuningStates)(IADLXManualFanTuning* pThis, IADLXManualFanTuningStateList** ppStates);
    ADLX_RESULT (ADLX_STD_CALL *GetEmptyFanTuningStates)(IADLXManualFanTuning* pThis, IADLXManualFanTuningStateList** ppStates);
    ADLX_RESULT (ADLX_STD_CALL *IsValidFanTuningStates)(IADLXManualFanTuning* pThis, IADLXManualFanTuningStateList* pStates, adlx_int* errorIndex);
    ADLX_RESULT (ADLX_STD_CALL *SetFanTuningStates)(IADLXManualFanTuning* pThis, IADLXManualFanTuningStateList* pStates);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedZeroRPM)(IADLXManualFanTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetZeroRPMState)(IADLXManualFanTuning* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL *SetZeroRPMState)(IADLXManualFanTuning* pThis, adlx_bool set);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedMinAcousticLimit)(IADLXManualFanTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetMinAcousticLimitRange)(IADLXManualFanTuning* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL *GetMinAcousticLimit)(IADLXManualFanTuning* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL *SetMinAcousticLimit)(IADLXManualFanTuning* pThis, adlx_int value);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedMinFanSpeed)(IADLXManualFanTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* GetMinFanSpeedRange)(IADLXManualFanTuning* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL* GetMinFanSpeed)(IADLXManualFanTuning* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL* SetMinFanSpeed)(IADLXManualFanTuning* pThis, adlx_int value);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedTargetFanSpeed)(IADLXManualFanTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* GetTargetFanSpeedRange)(IADLXManualFanTuning* pThis, ADLX_IntRange* tuningRange);
    ADLX_RESULT (ADLX_STD_CALL* GetTargetFanSpeed)(IADLXManualFanTuning* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL* SetTargetFanSpeed)(IADLXManualFanTuning* pThis, adlx_int value);
}IADLXManualFanTuningVtbl;

struct IADLXManualFanTuning { const IADLXManualFanTuningVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualFanTuning

#endif//ADLX_IGPUMANUALFANTUNING_H