//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IGPUTUNING_H
#define ADLX_IGPUTUNING_H
#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"
#include "IChangedEvent.h"

//-------------------------------------------------------------------------------------------------
//IGPUTuning.h - Interfaces for ADLX GPU Tuning functionality

//Manual Tuning interface
#pragma region IADLXManualTuningState
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualTuningState : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualTuningState")

        /**
        * @page DOX_IADLXManualTuningState_GetFrequency GetFrequency
        * @ENG_START_DOX
        * @brief Gets the frequency in the manual tuning state on a GPU.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetFrequency (adlx_uint* value)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,value,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the frequency (in MHz) in the manual tuning state is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the frequency is successfully returned, __ADLX_OK__ is returned.<br>
        * If the frequency is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetFrequency (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualTuningState_SetFrequency SetFrequency
        *@ENG_START_DOX @brief Sets the frequency in the manual tuning state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetFrequency (adlx_int value)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,value,adlx_int ,@ENG_START_DOX The new frequency (in MHz) in the manual tuning state. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the frequency is successfully set, __ADLX_OK__ is returned.<br>
        * If the frequency is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetFrequency (adlx_int value) = 0;

        /**
        * @page DOX_IADLXManualTuningState_GetVoltage GetVoltage
        * @ENG_START_DOX
        * @brief Gets the voltage in the manual tuning state on a GPU.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetVoltage (adlx_uint* value)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,value,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the voltage (in mV) in the manual tuning state is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the voltage is successfully returned, __ADLX_OK__ is returned.<br>
        * If the voltage is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetVoltage (adlx_int* value) = 0;

        /**
        *@page DOX_IADLXManualTuningState_SetVoltage SetVoltage
        *@ENG_START_DOX @brief Sets the voltage in the manual tuning state on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetVoltage (adlx_int value)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,value,adlx_int ,@ENG_START_DOX The new voltage (in mV) in the manual tuning state. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the voltage is successfully set, __ADLX_OK__ is returned.<br>
        * If the voltage is not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetVoltage (adlx_int value) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualTuningState> IADLXManualTuningStatePtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualTuningState, L"IADLXManualTuningState")

typedef struct IADLXManualTuningState IADLXManualTuningState;

typedef struct IADLXManualTuningStateVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualTuningState* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualTuningState* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualTuningState* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXManualTuningState
    ADLX_RESULT (ADLX_STD_CALL *GetFrequency)(IADLXManualTuningState* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL *SetFrequency)(IADLXManualTuningState* pThis, adlx_int value);
    ADLX_RESULT (ADLX_STD_CALL *GetVoltage)(IADLXManualTuningState* pThis, adlx_int* value);
    ADLX_RESULT (ADLX_STD_CALL *SetVoltage)(IADLXManualTuningState* pThis, adlx_int value);
}IADLXManualTuningStateVtbl;

struct IADLXManualTuningState { const IADLXManualTuningStateVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualTuningState

#pragma region IADLXMemoryTimingDescription
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXMemoryTimingDescription : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXMemoryTimingDescription")

        /**
        * @page DOX_IADLXMemoryTimingDescription_GetDescription GetDescription
        * @ENG_START_DOX
        * @brief Gets the memory timing description.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetDescription (@ref ADLX_MEMORYTIMING_DESCRIPTION* description)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,description,@ref ADLX_MEMORYTIMING_DESCRIPTION* ,@ENG_START_DOX The pointer to a variable where the memory timing description is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the memory timing description is successfully returned, __ADLX_OK__ is returned.<br>
        * If the memory timing description is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT  ADLX_STD_CALL GetDescription (ADLX_MEMORYTIMING_DESCRIPTION* description) = 0;
    };
    typedef IADLXInterfacePtr_T<IADLXMemoryTimingDescription> IADLXMemoryTimingDescriptionPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXMemoryTimingDescription, L"IADLXMemoryTimingDescription")

typedef struct IADLXMemoryTimingDescription IADLXMemoryTimingDescription;

typedef struct IADLXMemoryTimingDescriptionVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXMemoryTimingDescription* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXMemoryTimingDescription* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXMemoryTimingDescription* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXMemoryTimingDescription
    ADLX_RESULT (ADLX_STD_CALL *GetDescription)(IADLXMemoryTimingDescription* pThis, ADLX_MEMORYTIMING_DESCRIPTION* description);
}IADLXMemoryTimingDescriptionVtbl;

struct IADLXMemoryTimingDescription { const IADLXMemoryTimingDescriptionVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXMemoryTimingDescription

//IADLXManualTuningState list interface
#pragma region IADLXManualTuningStateList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXManualTuningStateList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXManualTuningStateList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXManualTuningState::IID ())

        /**
        * @page DOX_IADLXManualTuningStateList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXManualTuningState** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXManualTuningState** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__.  @ENG_END_DOX}
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
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXManualTuningState** ppItem) = 0;

        /**
        *@page DOX_IADLXManualTuningStateList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXManualTuningState* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXManualTuningState* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
        * If the interface is not added to the end of the list, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXManualTuningState* pItem) = 0;
    };  //IADLXManualTuningStateList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXManualTuningStateList> IADLXManualTuningStateListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXManualTuningStateList, L"IADLXManualTuningStateList")
ADLX_DECLARE_ITEM_IID (IADLXManualTuningState, IID_IADLXManualTuningState ())

typedef struct IADLXManualTuningStateList IADLXManualTuningStateList;

typedef struct IADLXManualTuningStateListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXManualTuningStateList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXManualTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXManualTuningStateList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXManualTuningStateList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXManualTuningStateList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXManualTuningStateList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXManualTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXManualTuningStateList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXManualTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXManualTuningStateList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXManualTuningStateList* pThis, IADLXInterface* pItem);

    //IADLXManualTuningStateList
    ADLX_RESULT (ADLX_STD_CALL *At_ManualTuningStateList)(IADLXManualTuningStateList* pThis, const adlx_uint location, IADLXManualTuningState** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_ManualTuningStateList)(IADLXManualTuningStateList* pThis, IADLXManualTuningState* pItem);

}IADLXManualTuningStateListVtbl;

struct IADLXManualTuningStateList { const IADLXManualTuningStateListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXManualTuningStateList

#pragma region IADLXMemoryTimingDescriptionList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXMemoryTimingDescriptionList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXMemoryTimingDescriptionList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXMemoryTimingDescription::IID ())

        /**
        * @page DOX_IADLXMemoryTimingDescriptionList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXMemoryTimingDescription** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXMemoryTimingDescription** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__.  @ENG_END_DOX}
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
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXMemoryTimingDescription** ppItem) = 0;

        /**
        *@page DOX_IADLXMemoryTimingDescriptionList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXMemoryTimingDescription* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXMemoryTimingDescription* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
        * If the interface is not added to the end of the list, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXMemoryTimingDescription* pItem) = 0;
    };
    typedef IADLXInterfacePtr_T<IADLXMemoryTimingDescriptionList> IADLXMemoryTimingDescriptionListPtr;
}
#else //__cplusplus
ADLX_DECLARE_IID (IADLXMemoryTimingDescriptionList, L"IADLXMemoryTimingDescriptionList")
ADLX_DECLARE_ITEM_IID (IADLXMemoryTimingDescription, IID_IADLXMemoryTimingDescription ())

typedef struct IADLXMemoryTimingDescriptionList IADLXMemoryTimingDescriptionList;

typedef struct IADLXMemoryTimingDescriptionListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXMemoryTimingDescriptionList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXMemoryTimingDescriptionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXMemoryTimingDescriptionList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXMemoryTimingDescriptionList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXMemoryTimingDescriptionList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXMemoryTimingDescriptionList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXMemoryTimingDescriptionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXMemoryTimingDescriptionList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXMemoryTimingDescriptionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXMemoryTimingDescriptionList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXMemoryTimingDescriptionList* pThis, IADLXInterface* pItem);

    //IADLXMemoryTimingDescriptionList
    ADLX_RESULT (ADLX_STD_CALL *At_MemoryTimingDescriptionList)(IADLXMemoryTimingDescriptionList* pThis, const adlx_uint location, IADLXMemoryTimingDescription** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_MemoryTimingDescriptionList)(IADLXMemoryTimingDescriptionList* pThis, IADLXMemoryTimingDescription* pItem);

}IADLXMemoryTimingDescriptionListVtbl;

struct IADLXMemoryTimingDescriptionList { const IADLXMemoryTimingDescriptionListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXMemoryTimingDescriptionList

#pragma region IADLXGPUTuningChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU;
    class ADLX_NO_VTABLE IADLXGPUTuningChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUTuningChangedEvent")
        /**
        *@page DOX_IADLXGPUTuningChangedEvent_GetGPU GetGPU
        *@ENG_START_DOX @brief Gets the reference counted GPU interface on which the GPU tuning is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPU (@ref DOX_IADLXGPU** ppGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppGPU,@ref DOX_IADLXGPU** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPU__ to __nullptr__. @ENG_END_DOX}
        *
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
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLXGPUTuningChangedEvent_GetGPU returns the reference counted GPU interface used by all the methods in this interface to check if there are any changes in GPU tuning.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPU (IADLXGPU** ppGPU) = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsAutomaticTuningChanged IsAutomaticTuningChanged
        *@ENG_START_DOX @brief Checks if the automatic tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsAutomaticTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the automatic tuning settings are changed, __true__ is returned.<br>
        * If the automatic tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsAutomaticTuningChanged () = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsPresetTuningChanged IsPresetTuningChanged
        *@ENG_START_DOX @brief Checks if the preset tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsPresetTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the preset tuning settings are changed, __true__ is returned.<br>
        * If the preset tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsPresetTuningChanged () = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsManualGPUCLKTuningChanged IsManualGPUCLKTuningChanged
        *@ENG_START_DOX @brief Checks if the manual graphic clock tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsManualGPUCLKTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the manual graphic clock tuning settings are changed, __true__ is returned.<br>
        * If the manual graphic clock tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsManualGPUCLKTuningChanged () = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsManualVRAMTuningChanged IsManualVRAMTuningChanged
        *@ENG_START_DOX @brief Checks if the manual VRAM tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsManualVRAMTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the manual VRAM tuning settings are changed, __true__ is returned.<br>
        * If the manual VRAM tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsManualVRAMTuningChanged () = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsManualFanTuningChanged IsManualFanTuningChanged
        *@ENG_START_DOX @brief Checks if the manual fan tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsManualFanTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the manual fan tuning settings are changed, __true__ is returned.<br>
        * If the manual fan tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsManualFanTuningChanged () = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedEvent_IsManualPowerTuningChanged IsManualPowerTuningChanged
        *@ENG_START_DOX @brief Checks if the manual power tuning settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsManualPowerTuningChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the manual power tuning settings are changed, __true__ is returned.<br>
        * If the manual power tuning settings are not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the GPU, use @ref DOX_IADLXGPUTuningChangedEvent_GetGPU.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsManualPowerTuningChanged () = 0;
    }; //IADLXGPUTuningChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUTuningChangedEvent> IADLXGPUTuningChangedEventPtr;
} //namespace adlx
#else //__cplusplus
typedef struct IADLXGPU IADLXGPU;

ADLX_DECLARE_IID (IADLXGPUTuningChangedEvent, L"IADLXGPUTuningChangedEvent")
typedef struct IADLXGPUTuningChangedEvent IADLXGPUTuningChangedEvent;

typedef struct IADLXGPUTuningChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUTuningChangedEvent* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUTuningChangedEvent* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUTuningChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXGPUTuningChangedEvent* pThis);

    // IADLXAutomaticTuningChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL *GetGPU)(IADLXGPUTuningChangedEvent* pThis, IADLXGPU** ppGPU);

    adlx_bool (ADLX_STD_CALL *IsAutomaticTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsPresetTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsManualGPUCLKTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsManualVRAMTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsManualFanTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsManualPowerTuningChanged)(IADLXGPUTuningChangedEvent* pThis);
} IADLXGPUTuningChangedEventVtbl;

struct IADLXGPUTuningChangedEvent { const IADLXGPUTuningChangedEventVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUTuningChangedEvent

//GPU Tuning changed listener interface. To be implemented in application and passed in IADLXGPUTuningChangedHandling::IADLXGPUTuningChangedListener()
#pragma region IADLXGPUTuningChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUTuningChangedListener
    {
    public:
        /**
        *@page DOX_IADLXGPUTuningChangedListener_OnGPUTuningChanged OnGPUTuningChanged
        *@ENG_START_DOX @brief __OnGPUTuningChanged__ is called by ADLX when GPU tuning changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnGPUTuningChanged (@ref DOX_IADLXGPUTuningChangedEvent* pGPUTuningChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPUTuningChangedEvent,@ref DOX_IADLXGPUTuningChangedEvent* ,@ENG_START_DOX The pointer to a GPU tuning change event. @ENG_END_DOX}
        *
        *@retvalues
		*@ENG_START_DOX  If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  Once the application registers to the notifications with @ref DOX_IADLXGPUTuningChangedHandling_AddGPUTuningEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXGPUTuningChangedHandling_RemoveGPUTuningEventListener.
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the GPU tuning change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the GPU tuning change event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnGPUTuningChanged (IADLXGPUTuningChangedEvent* pGPUTuningChangedEvent) = 0;
    }; //IADLXGPUTuningChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXGPUTuningChangedListener IADLXGPUTuningChangedListener;

typedef struct IADLXGPUTuningChangedListenerVtbl
{
    // IADLXGPUTuningChangedListener interface
    adlx_bool (ADLX_STD_CALL *OnGPUTuningChanged)(IADLXGPUTuningChangedListener* pThis, IADLXGPUTuningChangedEvent* pGPUTuningChangedEvent);
} IADLXGPUTuningChangedListenerVtbl;

struct IADLXGPUTuningChangedListener { const IADLXGPUTuningChangedListenerVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUTuningChangedListener

//Interface that allows registration to gpu tuning-related events:
// - GPU Tuning List changed
// - Automatic Tuning changed
// - Preset Tuning changed
// - Manual Tuning changed
#pragma region IADLXGPUTuningChangedHandling
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUTuningChangedHandling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUTuningChangedHandling")
        /**
        *@page DOX_IADLXGPUTuningChangedHandling_AddGPUTuningEventListener AddGPUTuningEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the GPU tuning changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddGPUTuningEventListener (@ref DOX_IADLXGPUTuningChangedListener* pGPUTuningChangedListener);
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPUTuningChangedListener,@ref DOX_IADLXGPUTuningChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving GPU tuning change notifications. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully registered, ADLX will call @ref DOX_IADLXGPUTuningChangedListener_OnGPUTuningChanged method of the listener when GPU tuning changes.<br>
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXGPUTuningChangedHandling_RemoveGPUTuningEventListener.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddGPUTuningEventListener (IADLXGPUTuningChangedListener* pGPUTuningChangedListener) = 0;

        /**
        *@page DOX_IADLXGPUTuningChangedHandling_RemoveGPUTuningEventListener RemoveGPUTuningEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from the GPU tuning event list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveGPUTuningEventListener (@ref DOX_IADLXGPUTuningChangedListener* pGPUTuningChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPUTuningChangedListener,@ref DOX_IADLXGPUTuningChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving GPU tuning change notifications. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXGPUTuningChangedListener_OnGPUTuningChanged method of the listener when GPU tuning changes.
        * The application can discard the event listener instance. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveGPUTuningEventListener (IADLXGPUTuningChangedListener* pGPUTuningChangedListener) = 0;
    };  // IADLXGPUTuningChangedHandling
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUTuningChangedHandling> IADLXGPUTuningChangedHandlingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUTuningChangedHandling, L"IADLXGPUTuningChangedHandling")
typedef struct IADLXGPUTuningChangedHandling IADLXGPUTuningChangedHandling;

typedef struct IADLXGPUTuningChangedHandlingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUTuningChangedHandling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUTuningChangedHandling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUTuningChangedHandling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLXGPUTuningChangedHandling interface
    ADLX_RESULT (ADLX_STD_CALL *AddGPUTuningEventListener)(IADLXGPUTuningChangedHandling* pThis, IADLXGPUTuningChangedListener* pGPUTuningChangedListener);
    ADLX_RESULT (ADLX_STD_CALL *RemoveGPUTuningEventListener)(IADLXGPUTuningChangedHandling* pThis, IADLXGPUTuningChangedListener* pGPUTuningChangedListener);
} IADLXGPUTuningChangedHandlingVtbl;

struct IADLXGPUTuningChangedHandling { const IADLXGPUTuningChangedHandlingVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUTuningChangedHandling


//GPU Tuning Services interface
#pragma region IADLXGPUTuningServices
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUTuningServices : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUTuningServices")

        /**
        * @page DOX_IADLXGPUTuningServices_GetGPUTuningChangedHandling GetGPUTuningChangedHandling
        * @ENG_START_DOX
        * @brief Gets the reference counted interface that allows registering and unregistering for notifications when GPU tuning changes.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetGPUTuningChangedHandling (@ref DOX_IADLXGPUTuningChangedHandling** ppGPUTuningChangedHandling)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,ppGPUTuningChangedHandling,@ref DOX_IADLXGPUTuningChangedHandling** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPUTuningChangedHandling__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
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
        * @DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGPUTuningChangedHandling (IADLXGPUTuningChangedHandling** ppGPUTuningChangedHandling) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsAtFactory IsAtFactory
        *@ENG_START_DOX @brief Checks if the GPU tuning on a GPU is set to factory settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsAtFactory (@ref DOX_IADLXGPU* pGPU, adlx_bool* isFactory)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,isFactory,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of the GPU tuning is returned. The variable is __true__ if the GPU tuning is set to factory settings. The variable is __false__ if the GPU tuning is not set to factory settings. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the GPU tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of the GPU tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsAtFactory (IADLXGPU* pGPU, adlx_bool* isFactory) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_ResetToFactory ResetToFactory
        *@ENG_START_DOX @brief Reset the GPU tuning to factory settings. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    ResetToFactory (@ref DOX_IADLXGPU* pGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the factory settings are successfully reset, __ADLX_OK__ is returned.<br>
        * If the factory settings are not successfully reset, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method resets settings of Auto Tuning, Manual GPU Tuning, Manual Fan Tuning, Manual VRAM Tuning and Manual Power Tuning. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL ResetToFactory (IADLXGPU* pGPU) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedAutoTuning IsSupportedAutoTuning
        *@ENG_START_DOX @brief Checks if the automatic tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedAutoTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of automatic tuning is returned. The variable is __true__ if automatic tuning is supported. The variable is __false__ if automatic tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of automatic tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of automatic tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedAutoTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedPresetTuning IsSupportedPresetTuning
        *@ENG_START_DOX @brief Checks if the preset tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedPresetTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of preset tuning is returned. The variable is __true__ if preset tuning is supported. The variable is __false__ if preset tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of preset tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of preset tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedPresetTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedManualGFXTuning IsSupportedManualGFXTuning
        *@ENG_START_DOX @brief Checks if the manual graphic tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedManualGFXTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of manual graphic tuning is returned. The variable is __true__ if manual graphic tuning is supported. The variable is __false__ if manual graphic tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of manual graphic tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of manual graphic tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedManualGFXTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedManualVRAMTuning IsSupportedManualVRAMTuning
        *@ENG_START_DOX @brief Checks if the manual VRAM Tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedManualVRAMTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of manual VRAM tuning is returned. The variable is __true__ if manual VRAM tuning is supported. The variable is __false__ if manual VRAM tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of manual VRAM tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of manual VRAM tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedManualVRAMTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedManualFanTuning IsSupportedManualFanTuning
        *@ENG_START_DOX @brief Checks if the manual fan tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedManualFanTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of manual fan tuning is returned. The variable is __true__ if manual fan tuning is supported. The variable is __false__ if manual fan tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of manual fan tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of manual fan tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedManualFanTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_IsSupportedManualPowerTuning IsSupportedManualPowerTuning
        *@ENG_START_DOX @brief Checks if the manual power tuning is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedManualPowerTuning (@ref DOX_IADLXGPU* pGPU, adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of manual power tuning is returned. The variable is __true__ if manual power tuning is supported. The variable is __false__ if manual power tuning is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of manual power tuning is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of manual power tuning is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedManualPowerTuning (IADLXGPU* pGPU, adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetAutoTuning GetAutoTuning
        *@ENG_START_DOX @brief Gets the reference counted automatic tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAutoTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppAutoTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppAutoTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppAutoTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method returns an @ref DOX_IADLXInterface interface. To get the @ref DOX_IADLXGPUAutoTuning interface, which is the automatic tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the @ref DOX_IADLXGPUAutoTuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed.<br> @ENG_END_DOX
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetAutoTuning (IADLXGPU* pGPU, IADLXInterface** ppAutoTuning) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetPresetTuning GetPresetTuning
        *@ENG_START_DOX @brief Gets the reference counted preset tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetPresetTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppPresetTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppPresetTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppPresetTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method returns an @ref DOX_IADLXInterface interface. To get the @ref DOX_IADLXGPUPresetTuning interface, which is the preset tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the @ref DOX_IADLXGPUPresetTuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed.<br> @ENG_END_DOX
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetPresetTuning (IADLXGPU* pGPU, IADLXInterface** ppPresetTuning) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetManualGFXTuning GetManualGFXTuning
        *@ENG_START_DOX @brief Gets the reference counted manual graphics tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetManualGFXTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppManualGFXTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppManualGFXTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppManualGFXTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
		*@detaileddesc
        *@ENG_START_DOX @details Different GPUs support different manual graphics tuning functionality.<br>
        * ADLX provides the @ref DOX_IADLXManualGraphicsTuning1 interface or the @ref DOX_IADLXManualGraphicsTuning2 interface to obtain the supported manual graphics tuning interface.<br>
        * To get the manual graphics tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the manual graphics tuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed. @ENG_END_DOX
		*
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetManualGFXTuning (IADLXGPU* pGPU, IADLXInterface** ppManualGFXTuning) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetManualVRAMTuning GetManualVRAMTuning
        *@ENG_START_DOX @brief Gets the reference counted manual VRAM tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetManualVRAMTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppManualVRAMTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppManualVRAMTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppManualVRAMTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
		*@detaileddesc
        *@ENG_START_DOX @details Different GPUs support different manual VRAM tuning functionality.<br>
        * ADLX provides the @ref DOX_IADLXManualVRAMTuning1 interface or the @ref DOX_IADLXManualVRAMTuning2 interface to obtain the supported manual VRAM tuning interface.<br>
        * To get the manual VRAM tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the manual VRAM tuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
		*@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetManualVRAMTuning (IADLXGPU* pGPU, IADLXInterface** ppManualVRAMTuning) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetManualFanTuning GetManualFanTuning
        *@ENG_START_DOX @brief Gets the reference counted manual fan tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetManualFanTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppManualFanTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppManualFanTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppManualFanTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *@detaileddesc
        *@ENG_START_DOX @details The method returns an @ref DOX_IADLXInterface interface. To get the @ref DOX_IADLXManualFanTuning interface, which is the manual fan tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the @ref DOX_IADLXManualFanTuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetManualFanTuning (IADLXGPU* pGPU, IADLXInterface** ppManualFanTuning) = 0;

        /**
        *@page DOX_IADLXGPUTuningServices_GetManualPowerTuning GetManualPowerTuning
        *@ENG_START_DOX @brief Gets the reference counted manual power tuning interface of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetManualPowerTuning (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXInterface** ppManualPowerTuning)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppManualPowerTuning,@ref DOX_IADLXInterface** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppManualPowerTuning__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method returns an @ref DOX_IADLXInterface interface. To get the @ref DOX_IADLXManualPowerTuning interface, which is the manual power tuning interface, the application must call @ref DOX_IADLXInterface_QueryInterface in the returned @ref DOX_IADLXInterface.<br>
        * The @ref DOX_IADLXInterface interface and the @ref DOX_IADLXManualPowerTuning interface must be discarded with @ref DOX_IADLXInterface_Release when they are no longer needed.<br> @ENG_END_DOX
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetManualPowerTuning (IADLXGPU* pGPU, IADLXInterface** ppManualPowerTuning) = 0;
    };  //IADLXGPUTuningServices
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUTuningServices> IADLXGPUTuningServicesPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUTuningServices, L"IADLXGPUTuningServices")
typedef struct IADLXGPUTuningServices IADLXGPUTuningServices;

typedef struct IADLXGPUTuningServicesVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUTuningServices* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUTuningServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUTuningServices* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPUTuningServices
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTuningChangedHandling)(IADLXGPUTuningServices* pThis, IADLXGPUTuningChangedHandling** ppGPUTuningChangedHandling);
    ADLX_RESULT (ADLX_STD_CALL *IsAtFactory)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* isFactory);
    ADLX_RESULT (ADLX_STD_CALL *ResetToFactory)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU);

    ADLX_RESULT (ADLX_STD_CALL *IsSupportedAutoTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedPresetTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedManualGFXTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedManualVRAMTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedManualFanTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedManualPowerTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL *GetAutoTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppAutoTuning);
    ADLX_RESULT (ADLX_STD_CALL *GetPresetTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppPresetTuning);
    ADLX_RESULT (ADLX_STD_CALL *GetManualGFXTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppManualGFXTuning);
    ADLX_RESULT (ADLX_STD_CALL *GetManualVRAMTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppManualVRAMTuning);
    ADLX_RESULT (ADLX_STD_CALL *GetManualFanTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppManualFanTuning);
    ADLX_RESULT (ADLX_STD_CALL *GetManualPowerTuning)(IADLXGPUTuningServices* pThis, IADLXGPU* pGPU, IADLXInterface** ppManualPowerTuning);
}IADLXGPUTuningServicesVtbl;

struct IADLXGPUTuningServices { const IADLXGPUTuningServicesVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUTuningServices

#endif//ADLX_IGPUTUNING_H
