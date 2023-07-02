// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_ADLXHelper_h
#define ADLX_ADLXHelper_h

#pragma once

#include "../../../Include/ADLX.h"

class ADLXHelper
{
public:
    /**
    * @page page_cppHelpNew ADLXHelper
    * @ENG_START_DOX
    * @brief The constructor of the ADLXHelper class.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  ADLXHelper ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * An ADLXHelper object.
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    ADLXHelper ();

    /**
    * @page page_cppHelpDelete ~ADLXHelper
    * @ENG_START_DOX
    * @brief The destructor of the ADLXHelper class.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  ~ADLXHelper ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * N/A
    *
    * @detaileddesc
    * @ENG_START_DOX
    * @details This method calls @ref page_cppHelpTerminate.
    * @ENG_END_DOX
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    virtual ~ADLXHelper ();

    //Initialization. Either of these versions Must be called before any calls in this class

    /**
    * @page page_cppHelpInitializeWithCallerAdl InitializeWithCallerAdl
    * @ENG_START_DOX
    * @brief Initializes ADLX with an ADL context.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref ADLX_RESULT InitializeWithCallerAdl (adlx_handle adlContext, @ref ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
    * @codeEnd
    * @params
    * @paramrow{1.,[in],adlContext,adlx_handle,@ENG_START_DOX The ADL context. @ENG_END_DOX}
    * @paramrow{2.,[in],adlMainMemoryFree,@ref ADLX_ADL_Main_Memory_Free,@ENG_START_DOX The callback handler of the memory deallocation. @ENG_END_DOX}
    *
    * @retvalues
    * If __InitializeWithCallerAdl__ is successfully executed, __ADLX_OK__ is returned.<br>
    * If __InitializeWithCallerAdl__ is not successfully executed, an error code is returned.<br>
    * If ADLX was previously successfully initialized with any of the Initialize versions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    *
    * @detaileddesc
    * @ENG_START_DOX
    * @details
    * This method is used when an application also uses ADL. A typical scenario is the application is transitioning from ADL to ADLX with some programming already completed with ADL.<br>
    * In such case, the application shall pass the parameters in this method that corresponds to the current ADL initialization already in use, which are: an ADL context and a callback for the memory deallocation when ADL was initialized.<br>
    * For more information for initializing ADLX with an ADL context, refer to @ref page_guide_use_ADLX "Using ADLX in an ADL application".<br>
    * @ENG_END_DOX
    *
    * @requirements
    *
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    ADLX_RESULT InitializeWithCallerAdl (adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree);

    /**
    * @page page_cppHelpInitialize Initialize
    * @ENG_START_DOX
    * @brief Initializes ADLX with default parameters.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref ADLX_RESULT Initialize ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * If __Initialize__ is successfully executed, __ADLX_OK__ is returned.<br>
    * If __Initialize__ is not successfully executed, an error code is returned.<br>
    * If ADLX was previously successfully initialized with any of the initialization functions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    *
    * @detaileddesc
    * @details
    * This method is used when an application does not use the ADL library and initializes ADLX with default parameters.<br>
    * For more information for initializing ADLX with default parameters, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    ADLX_RESULT Initialize ();

    /**
    * @page page_cppHelpInitializeWithIncompatibleDriver InitializeWithIncompatibleDriver
    * @ENG_START_DOX
    * @brief Initializes ADLX with a legacy driver.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref ADLX_RESULT InitializeWithIncompatibleDriver ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * If __InitializeWithIncompatibleDriver__ is successfully executed, __ADLX_OK__ is returned.<br>
    * If __InitializeWithIncompatibleDriver__ is not successfully executed, an error code is returned.<br>
    * If ADLX was previously successfully initialized with any of the Initialize versions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    *
    * @detaileddesc
    * @details
    * This method is used when an application does not use the ADL library and initializes ADLX to consider AMD GPUs using legacy AMD graphics driver.<br>
    * For more information for initializing ADLX with a legacy driver, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    ADLX_RESULT InitializeWithIncompatibleDriver ();

    //Destruction.
    //WARNING: No outstanding interfaces from ADLX must exist when calling this method.
    //After this call they will be invalid and calls into them will result in access violation.

    /**
    * @page page_cppHelpTerminate Terminate
    * @ENG_START_DOX
    * @brief Terminates ADLX and releases ADLX library.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref ADLX_RESULT Terminate ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * If __Terminate__ is successfully executed, __ADLX_OK__ is returned.<br>
    * If __Terminate__ is not successfully executed, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.
    *
    * @detaileddesc
    * @details
    * Any interface obtained from ADLX that is not released becomes invalid.<br>
    * Any attempt of calling ADLX interface after termination could result in errors such as exceptions or crashes.<br>
    *
    * @requirements
    *
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    ADLX_RESULT Terminate ();

    //Returns the ADLX version

    /**
    * @page page_cppHelpQueryFullVersion QueryFullVersion
    * @ENG_START_DOX
    * @brief Gets the full version of ADLX.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  adlx_uint64 QueryFullVersion ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * The full version of ADLX.
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    adlx_uint64 QueryFullVersion ();

    /**
    * @page page_cppHelpQueryVersion QueryVersion
    * @ENG_START_DOX
    * @brief Gets the version of ADLX.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  const char* QueryVersion ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * The version of ADLX.
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    const char* QueryVersion();

    /**
    * @page page_cppHelpGetSystemServices GetSystemServices
    * @ENG_START_DOX
    * @brief Gets the ADLX system interface.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref DOX_IADLXSystem* GetSystemServices ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * If ADLX was successfully initialized before this method call, the @ref DOX_IADLXSystem interface is returned.<br>
    * If ADLX was not successfully initialized, __nullptr__ is returned.
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    adlx::IADLXSystem* GetSystemServices ();

    /**
    * @page page_cppHelpGetAdlMapping GetAdlMapping
    * @ENG_START_DOX
    * @brief Gets the ADL Mapping interface.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  @ref DOX_IADLMapping* GetAdlMapping ()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * If ADLX was successfully initialized with ADL, a valid pointer of the @ref DOX_IADLMapping interface is returned.<br>
    * If ADLX was initialized under any of the following circumstances, __nullptr__ is returned.<br>
    * - ADLX initialization was with other initialization methods.<br>
    * - ADLX initialization was failed.<br>
    * - ADLX initialization was not called.<br>
    *
    * @detaileddesc
    * @details
    * __GetAdlMapping__ is used to convert data between ADL and ADLX in applications where ADLX was initialized with @ref page_cppHelpInitializeWithCallerAdl.
    *
    * @requirements
    * @DetailsTable{#include "ADLXHelper/Windows/Cpp/ADLXHelper.h", @ADLX_First_Ver}
    */
    adlx::IADLMapping* GetAdlMapping ();

protected:

    //Handle to the ADLX dll
    adlx_handle m_hDLLHandle = nullptr;

    //Full Version of this ADLX instance
    adlx_uint64 m_ADLXFullVersion = 0;

    //Version of this ADLX instance
    const char* m_ADLXVersion = nullptr;

    //The ADLX system services interface
    adlx::IADLXSystem* m_pSystemServices = nullptr;

    //the ADL mapping interface
    adlx::IADLMapping* m_pAdlMapping = nullptr;

    //ADLX function - query full version
    ADLXQueryFullVersion_Fn m_fullVersionFn = nullptr;

    //ADLX function - query version
    ADLXQueryVersion_Fn m_versionFn = nullptr;

    //ADLX function - initialize with ADL
    ADLXInitializeWithCallerAdl_Fn m_initWithADLFn = nullptr;

    //ADLX function - initialize with incompatible driver
    ADLXInitialize_Fn m_initFnEx = nullptr;

    //ADLX function - initialize
    ADLXInitialize_Fn m_initFn = nullptr;

    //ADLX function - terminate
    ADLXTerminate_Fn m_terminateFn = nullptr;

    //Loads ADLX and finds the function pointers to the ADLX functions
    ADLX_RESULT LoadADLXDll ();

    //Initializes ADLX based on the  parameters
    ADLX_RESULT InitializePrivate (adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree, adlx_bool useIncompatibleDriver = false);
}; //class ADLXHelper

extern ::ADLXHelper g_ADLX;

#endif //ADLX_ADLXHelper_h