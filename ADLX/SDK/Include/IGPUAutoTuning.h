//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IGPUAUTOTUNING_H
#define ADLX_IGPUAUTOTUNING_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IGPUAutoTuning.h - Interfaces for ADLX GPU Auto Tuning functionality

//Interface with information on GPU Tuning changes on a GPU. ADLX passes this to application that registered for GPU Tuning changed event in the IADLXAutomaticTuningChangedListener::OnAutomaticTuningChanged()
#pragma region IADLXGPUAutoTuningCompleteEvent

#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUAutoTuningCompleteEvent : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUAutoTuningCompleteEvent")

        /**
        *@page DOX_IADLXGPUAutoTuningCompleteEvent_IsUndervoltGPUCompleted IsUndervoltGPUCompleted
        *@ENG_START_DOX @brief Checks if the GPU undervolting is completed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsUndervoltGPUCompleted ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU undervolting is completed, __true__ is returned.<br>
        * If the GPU undervolting is not completed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsUndervoltGPUCompleted () = 0;

        /**
        *@page DOX_IADLXGPUAutoTuningCompleteEvent_IsOverclockGPUCompleted IsOverclockGPUCompleted
        *@ENG_START_DOX @brief Checks if the GPU overclocking is completed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsOverclockGPUCompleted ()
        *@codeEnd
        *
        *@params
        *N/A
        *
         *@retvalues
        *@ENG_START_DOX  If the GPU overclocking is completed, __true__ is returned.<br>
        * If the GPU overclocking is not completed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsOverclockGPUCompleted () = 0;

        /**
        *@page DOX_IADLXGPUAutoTuningCompleteEvent_IsOverclockVRAMCompleted IsOverclockVRAMCompleted
        *@ENG_START_DOX @brief Checks if the VRAM overclocking is completed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsOverclockVRAMCompleted ()
        *@codeEnd
        *
        *@params
        *N/A
        *
         *@retvalues
        *@ENG_START_DOX  If the VRAM undervolting is completed, __true__ is returned.<br>
        * If the VRAM undervolting is not completed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsOverclockVRAMCompleted () = 0;
    }; //IADLXGPUAutoTuningCompleteEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUAutoTuningCompleteEvent> IADLXGPUAutoTuningCompleteEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUAutoTuningCompleteEvent, L"IADLXGPUAutoTuningCompleteEvent")
typedef struct IADLXGPUAutoTuningCompleteEvent IADLXGPUAutoTuningCompleteEvent;

typedef struct IADLXGPUAutoTuningCompleteEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUAutoTuningCompleteEvent* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUAutoTuningCompleteEvent* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUAutoTuningCompleteEvent* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLXGPUAutoTuningCompleteEvent interface
    adlx_bool (ADLX_STD_CALL *IsUndervoltGPUCompleted)(IADLXGPUAutoTuningCompleteEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsOverclockGPUCompleted)(IADLXGPUAutoTuningCompleteEvent* pThis);
    adlx_bool (ADLX_STD_CALL *IsOverclockVRAMCompleted)(IADLXGPUAutoTuningCompleteEvent* pThis);
} IADLXGPUAutoTuningCompleteEventVtbl;

struct IADLXGPUAutoTuningCompleteEvent { const IADLXGPUAutoTuningCompleteEventVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUAutoTuningCompleteEvent

//GPU Auto Tuning complete listener interface. To be implemented in application and passed in IADLXGPUTuningChangedHandling::IADLXGPUTuningChangedListener()
#pragma region IADLXGPUAutoTuningCompleteListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUAutoTuningCompleteListener
    {
    public:
        /**
        *@page DOX_IADLXGPUAutoTuningCompleteListener_OnGPUAutoTuningComplete OnGPUAutoTuningComplete
        *@ENG_START_DOX @brief The __OnGPUAutoTuningComplete__ is called by ADLX when GPU tuning completes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnGPUAutoTuningComplete (@ref DOX_IADLXGPUAutoTuningCompleteEvent* pGPUAutoTuningCompleteEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pGPUAutoTuningCompleteEvent,@ref DOX_IADLXGPUAutoTuningCompleteEvent* ,@ENG_START_DOX The pointer to a GPU tuning complete event. @ENG_END_DOX}
        *
        *
        *@retvalues
		*@ENG_START_DOX  If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  Once the application registers to the notifications with @ref DOX_IADLXGPUAutoTuning_StartUndervoltGPU, @ref DOX_IADLXGPUAutoTuning_StartOverclockGPU, and @ref DOX_IADLXGPUAutoTuning_StartOverclockVRAM, ADLX will call this method when GPU tuning completes.
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the GPU tuning complete event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the GPU tuning complete event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IGPUTuning.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnGPUAutoTuningComplete (IADLXGPUAutoTuningCompleteEvent* pGPUAutoTuningCompleteEvent) = 0;
    }; //IADLXGPUAutoTuningCompleteListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXGPUAutoTuningCompleteListener IADLXGPUAutoTuningCompleteListener;

typedef struct IADLXGPUAutoTuningCompleteListenerVtbl
{
    // IADLXGPUAutoTuningCompleteListener interface
    adlx_bool (ADLX_STD_CALL *OnGPUAutoTuningComplete)(IADLXGPUAutoTuningCompleteListener* pThis, IADLXGPUAutoTuningCompleteEvent* pGPUAutoTuningCompleteEvent);
} IADLXGPUAutoTuningCompleteListenerVtbl;

struct IADLXGPUAutoTuningCompleteListener { const IADLXGPUAutoTuningCompleteListenerVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUAutoTuningCompleteListener

// Automatic Tuning
#pragma region IADLXGPUAutoTuning
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUAutoTuning : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUAutoTuning")
            /**
            *@page DOX_IADLXGPUAutoTuning_IsSupportedUndervoltGPU IsSupportedUndervoltGPU
            *@ENG_START_DOX @brief Checks if the GPU undervolting tuning profile is supported on a GPU. @ENG_END_DOX
            *
            *@syntax
            *@codeStart
            * @ref ADLX_RESULT    IsSupportedUndervoltGPU (adlx_bool* supported)
            *@codeEnd
            *
            *@params
            *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU undervolting is returned. The variable is __true__ if GPU undervolting is supported. The variable is __false__ if GPU undervolting is not supported. @ENG_END_DOX}
            *
            *@retvalues
            *@ENG_START_DOX  If the state of GPU undervolting is successfully returned, __ADLX_OK__ is returned.<br>
            * If the state of GPU undervolting is not successfully returned, an error code is returned.<br>
            * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
            *
            * @addinfo
            * @ENG_START_DOX
            * GPU undervolting reduces voltage and maintains clock speed to improve performance per watt.
            * @ENG_END_DOX
            *
            *@requirements
            *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
            *
            */
            virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedUndervoltGPU (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_IsSupportedOverclockGPU IsSupportedOverclockGPU
        *@ENG_START_DOX @brief Checks if the GPU overclocking tuning profile is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedOverclockGPU (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU overclocking is returned. The variable is __true__ if GPU overclocking is supported. The variable is __false__ if GPU overclocking is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU overclocking is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of GPU overclocking is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic GPU overclocking uses an overclocking algorithm to improve GPU performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedOverclockGPU (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_IsSupportedOverclockVRAM IsSupportedOverclockVRAM
        *@ENG_START_DOX @brief Checks if the VRAM overclocking tuning profile is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedOverclockVRAM (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of VRAM overclocking is returned. The variable is __true__ if VRAM overclocking is supported. The variable is __false__ if VRAM overclocking is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of VRAM overclocking is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of VRAM overclocking is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic VRAM overclocking uses an overclocking algorithm to improve video memory performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedOverclockVRAM (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_IsCurrentUndervoltGPU IsCurrentUndervoltGPU
        *@ENG_START_DOX @brief Checks if the GPU undervolting tuning profile is currently enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentUndervoltGPU (adlx_bool* isUndervoltGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isUndervoltGPU,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU undervolting is returned. The variable is __true__ if GPU undervolting is applied. The variable is __false__ if GPU undervolting is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU undervolting is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of GPU undervolting is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * GPU undervolting reduces voltage and maintains clock speed to improve performance per watt.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentUndervoltGPU (adlx_bool* isUndervoltGPU) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_IsCurrentOverclockGPU IsCurrentOverclockGPU
        *@ENG_START_DOX @brief Checks if the GPU overclocking tuning profile is currently enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentOverclockGPU (adlx_bool* isOverclockGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isOverclockGPU,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU overclocking is returned. The variable is __true__ if GPU overclocking is applied. The variable is __false__ if GPU overclocking is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of GPU overclocking is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of GPU overclocking is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic GPU overclocking uses an overclocking algorithm to improve GPU performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentOverclockGPU (adlx_bool* isOverclockGPU) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_IsCurrentOverclockVRAM IsCurrentOverclockVRAM
        *@ENG_START_DOX @brief Checks if the VRAM overclocking tuning profile is currently enabled on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentOverclockVRAM (adlx_bool* isOverclockVRAM)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],isOverclockVRAM,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of VRAM overclocking is returned. The variable is __true__ if VRAM overclocking is applied. The variable is __false__ if VRAM overclocking is not applied. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of VRAM overclocking is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of VRAM overclocking is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic VRAM overclocking uses an overclocking algorithm to improve video memory performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentOverclockVRAM (adlx_bool* isOverclockVRAM) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_StartUndervoltGPU StartUndervoltGPU
        *@ENG_START_DOX @brief Starts GPU undervolting on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    StartUndervoltGPU (@ref DOX_IADLXGPUAutoTuningCompleteListener* pCompleteListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pCompleteListener,@ref DOX_IADLXGPUAutoTuningCompleteListener*,@ENG_START_DOX The pointer to a GPU tuning complete listener interface. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU undervolting is successfully started, __ADLX_OK__ is returned.<br>
        * If the GPU undervolting is not successfully started, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
        *The __StartUndervoltGPU__ method triggers an asynchronous execution for the autotuning and returns immediately. When the autotuning is completed, ADLX calls @ref DOX_IADLXGPUAutoTuningCompleteListener_OnGPUAutoTuningComplete in the GPU tuning complete listener. After the event is raised, @ref DOX_IADLXGPUAutoTuning_IsCurrentUndervoltGPU returns __true__. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * GPU undervolting reduces voltage and maintains clock speed to improve performance per watt.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL StartUndervoltGPU (IADLXGPUAutoTuningCompleteListener* pCompleteListener) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_StartOverclockGPU StartOverclockGPU
        *@ENG_START_DOX @brief Starts GPU overclocking on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    StartOverclockGPU (@ref DOX_IADLXGPUAutoTuningCompleteListener* pCompleteListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pCompleteListener,@ref DOX_IADLXGPUAutoTuningCompleteListener*,@ENG_START_DOX The pointer to a GPU tuning complete listener interface. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU overclocking is successfully started, __ADLX_OK__ is returned.<br>
        * If the GPU overclocking is not successfully started, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
		* The __StartOverclockGPU__ method triggers an asynchronous execution for the autotuning and returns immediately. When the autotuning is completed, ADLX calls @ref DOX_IADLXGPUAutoTuningCompleteListener_OnGPUAutoTuningComplete in the GPU tuning complete listener. After the event is raised, @ref DOX_IADLXGPUAutoTuning_IsCurrentOverclockGPU returns __true__. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic GPU overclocking uses an overclocking algorithm to improve GPU performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL StartOverclockGPU (IADLXGPUAutoTuningCompleteListener* pCompleteListener) = 0;

        /**
        *@page DOX_IADLXGPUAutoTuning_StartOverclockVRAM StartOverclockVRAM
        *@ENG_START_DOX @brief Start VRAM overclocking on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    StartOverclockVRAM (@ref DOX_IADLXGPUAutoTuningCompleteListener* pCompleteListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pCompleteListener,@ref DOX_IADLXGPUAutoTuningCompleteListener*,@ENG_START_DOX The pointer to a GPU tuning complete listener interface. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the VRAM overclocking is successfully started, __ADLX_OK__ is returned.<br>
        * If the VRAM overclocking is not successfully started, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
        * The __StartOverclockVRAM__ method triggers an asynchronous execution for the autotuning and returns immediately. When the autotuning is completed, ADLX calls @ref DOX_IADLXGPUAutoTuningCompleteListener_OnGPUAutoTuningComplete in the GPU tuning complete listener. After the event is raised, @ref DOX_IADLXGPUAutoTuning_IsCurrentOverclockVRAM returns __true__. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * Automatic VRAM overclocking uses an overclocking algorithm to improve video memory performance.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IGPUAutoTuning.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL StartOverclockVRAM (IADLXGPUAutoTuningCompleteListener* pCompleteListener) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUAutoTuning> IADLXGPUAutoTuningPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUAutoTuning, L"IADLXGPUAutoTuning")

typedef struct IADLXGPUAutoTuning IADLXGPUAutoTuning;

typedef struct IADLXGPUAutoTuningVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUAutoTuning* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUAutoTuning* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUAutoTuning* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPUAutoTuning
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedUndervoltGPU)(IADLXGPUAutoTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedOverclockGPU)(IADLXGPUAutoTuning* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedOverclockVRAM)(IADLXGPUAutoTuning* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL *IsCurrentUndervoltGPU)(IADLXGPUAutoTuning* pThis, adlx_bool* isUndervoltGPU);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentOverclockGPU)(IADLXGPUAutoTuning* pThis, adlx_bool* isOverclockGPU);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentOverclockVRAM)(IADLXGPUAutoTuning* pThis, adlx_bool* isOverclockVRAM);

    ADLX_RESULT (ADLX_STD_CALL *StartUndervoltGPU)(IADLXGPUAutoTuning* pThis, IADLXGPUAutoTuningCompleteListener* pCompleteListener);
    ADLX_RESULT (ADLX_STD_CALL *StartOverclockGPU)(IADLXGPUAutoTuning* pThis, IADLXGPUAutoTuningCompleteListener* pCompleteListener);
    ADLX_RESULT (ADLX_STD_CALL *StartOverclockVRAM)(IADLXGPUAutoTuning* pThis, IADLXGPUAutoTuningCompleteListener* pCompleteListener);

}IADLXGPUAutoTuningVtbl;

struct IADLXGPUAutoTuning { const IADLXGPUAutoTuningVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUAutoTuning

#endif//ADLX_IGPUAUTOTUNING_H
