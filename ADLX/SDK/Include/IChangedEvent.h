//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_CHANGEDE_EVENT_H
#define ADLX_CHANGEDE_EVENT_H 
#pragma once

#include "ADLXDefines.h"

#pragma region IADLXChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXChangedEvent : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID(L"IADLXChangedEvent")

         /**
        *@page DOX_IADLXChangedEvent_GetOrigin GetOrigin
        *@ENG_START_DOX @brief Gets the origin of an event. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_SYNC_ORIGIN    GetOrigin ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event is triggered by a change in settings using ADLX in this application, __SYNC_ORIGIN_INTERNAL__ is returned. <br>
        * If the event is triggered by a change in settings using ADLX in another application, __SYNC_ORIGIN_EXTERNAL__ is returned. <br>
        * If the event has an unknown trigger, __SYNC_ORIGIN_UNKNOWN__ is returned. <br>
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IADLXChangedEvent.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_SYNC_ORIGIN   ADLX_STD_CALL GetOrigin() = 0;
    };  //IADLXList
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXChangedEvent> IADLXChangedEventPtr;
}   // namespace adlx
#else
ADLX_DECLARE_IID(IADLXChangedEvent, L"IADLXChangedEvent")
typedef struct IADLXChangedEvent IADLXChangedEvent;
typedef struct IADLXChangedEventVtbl
{
    //IADLXInterface
    adlx_long(ADLX_STD_CALL* Acquire)(IADLXChangedEvent* pThis);
    adlx_long(ADLX_STD_CALL* Release)(IADLXChangedEvent* pThis);
    ADLX_RESULT(ADLX_STD_CALL* QueryInterface)(IADLXChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXChangedEvent* pThis);
   
}IADLXChangedEventVtbl;

struct IADLXChangedEvent
{
    const IADLXChangedEventVtbl* pVtbl;
};

#endif
#pragma endregion IADLXChangedEvent

#endif //ADLX_CHANGEDE_EVENT_H