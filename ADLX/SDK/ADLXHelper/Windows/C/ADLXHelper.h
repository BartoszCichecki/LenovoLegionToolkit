// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_ADLXFactory_h
#define ADLX_ADLXFactory_h
#pragma once

#include "../../../Include/ADLX.h"

/**
* @page page_cHelpADLXHelper_Initialize ADLXHelper_Initialize
* @ENG_START_DOX
* @brief Initializes ADLX with default parameters.<br>
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref ADLX_RESULT ADLXHelper_Initialize ()
* @codeEnd
* @params
* N/A
*
* @retvalues
* If __ADLXHelper_Initialize__ is successfully executed, __ADLX_OK__ is returned.<br>
* If __ADLXHelper_Initialize__ is not successfully executed, an error code is returned.<br>
* If ADLX was previously successfully initialized with any of the initialization functions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
* Refer to @ref ADLX_RESULT for success codes and error codes.
*
* @detaileddesc
* @ENG_START_DOX
* @details This function is used when an application does not use the ADL library and initializes ADLX with default parameters.<br>
* For more information for initializing ADLX with default parameters, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
ADLX_RESULT             ADLXHelper_Initialize();

/**
* @page page_cHelpADLXHelper_InitializeWithIncompatibleDriver ADLXHelper_InitializeWithIncompatibleDriver
* @ENG_START_DOX
* @brief Initializes ADLX with a legacy driver.<br>
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref ADLX_RESULT ADLXHelper_InitializeWithIncompatibleDriver ()
* @codeEnd
* @params
* N/A
*
* @retvalues
* If __ADLXHelper_InitializeWithIncompatibleDriver__ is successfully executed, __ADLX_OK__ is returned.<br>
* If __ADLXHelper_InitializeWithIncompatibleDriver__ is not successfully executed, an error code is returned.<br>
* If ADLX was previously successfully initialized with any of the initialization functions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
* Refer to @ref ADLX_RESULT for success codes and error codes.<br>
*
* @detaileddesc
* @ENG_START_DOX
* @details This function is used when an application does not use the ADL library and initializes ADLX to consider AMD GPUs using legacy AMD graphics driver.<br>
* For more information for initializing ADLX with a legacy driver, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
ADLX_RESULT             ADLXHelper_InitializeWithIncompatibleDriver();

/**
* @page page_cHelpADLXHelper_InitializeWithCallerAdl ADLXHelper_InitializeWithCallerAdl
* @ENG_START_DOX
* @brief Initializes ADLX with an ADL context.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref ADLX_RESULT ADLXHelper_InitializeWithCallerAdl (adlx_handle adlContext, @ref ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
* @codeEnd
* @params
* @paramrow{1.,[in],adlContext,adlx_handle,@ENG_START_DOX The ADL context. @ENG_END_DOX}
* @paramrow{2.,[in],adlMainMemoryFree,@ref ADLX_ADL_Main_Memory_Free,@ENG_START_DOX The callback handler of the memory deallocation function. @ENG_END_DOX}
*
* @retvalues
* If __ADLXHelper_InitializeWithCallerAdl__ is successfully executed, __ADLX_OK__ is returned.<br>
* If __ADLXHelper_InitializeWithCallerAdl__ is not successfully executed, an error code is returned.<br>
* If ADLX was previously successfully initialized with any of the Initialize versions, __ADLX_ALREADY_INITIALIZED__ is returned.<br>
* Refer to @ref ADLX_RESULT for success codes and error codes.<br>
*
* @detaileddesc
* @ENG_START_DOX
* @details
* This function is used when an application also uses ADL. A typical scenario is the application is transitioning from ADL to ADLX with some programming already completed with ADL.<br>
* In such case, the application shall pass the parameters in this function that corresponds to the current ADL initialization already in use, which are: an ADL context and a callback for the memory deallocation when ADL was initialized.<br>
* For more information for initializing ADLX with an ADL context, refer to @ref page_guide_use_ADLX "Using ADLX in an ADL application".<br>
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
ADLX_RESULT             ADLXHelper_InitializeWithCallerAdl (adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree);

/**
* @page page_cHelpADLXHelper_Terminate ADLXHelper_Terminate
* @ENG_START_DOX
* @brief Terminates ADLX and releases ADLX library.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref ADLX_RESULT ADLXHelper_Terminate ()
* @codeEnd
* @params
* N/A
*
* @retvalues
* If __ADLXHelper_Terminate__ is successfully executed, __ADLX_OK__ is returned.<br>
* If __ADLXHelper_Terminate__ is not successfully executed, an error code is returned.<br>
* Refer to @ref ADLX_RESULT for success codes and error codes.
*
* @detaileddesc
* @ENG_START_DOX
* @details
* Any interface obtained from ADLX that is not released becomes invalid.<br>
* Any attempt of calling ADLX interface after termination could result in errors such as exceptions or crashes.<br>
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
ADLX_RESULT             ADLXHelper_Terminate();

/**
* @page page_cHelpADLXHelper_GetSystemServices ADLXHelper_GetSystemServices
* @ENG_START_DOX
* @brief Gets the ADLX system interface.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref DOX_IADLXSystem* ADLXHelper_GetSystemServices ()
* @codeEnd
* @params
* N/A
*
* @retvalues
* If ADLX was successfully initialized before this function call, the @ref DOX_IADLXSystem interface is returned.<br>
* If ADLX was not successfully initialized, __nullptr__ is returned.
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
IADLXSystem*            ADLXHelper_GetSystemServices ();

/**
* @page page_cHelpADLXHelper_GetAdlMapping ADLXHelper_GetAdlMapping
* @ENG_START_DOX
* @brief Gets the ADL Mapping interface.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  @ref DOX_IADLMapping* ADLXHelper_GetAdlMapping ()
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
* @ENG_START_DOX
* @details __ADLXHelper_GetAdlMapping__ is used to convert data between ADL and ADLX in applications where ADLX was initialized with @ref page_cHelpADLXHelper_InitializeWithCallerAdl.
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
IADLMapping*            ADLXHelper_GetAdlMapping();

/**
* @page page_cHelpADLXHelper_QueryFullVersion ADLXHelper_QueryFullVersion
* @ENG_START_DOX
* @brief Gets the full version of ADLX.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  adlx_uint64 ADLXHelper_QueryFullVersion ()
* @codeEnd
* @params
* N/A
*
* @retvalues
* The full version of ADLX.
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
adlx_uint64             ADLXHelper_QueryFullVersion();

/**
* @page page_cHelpADLXHelper_QueryVersion ADLXHelper_QueryVersion
* @ENG_START_DOX
* @brief Gets the version of ADLX.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  const char* ADLXHelper_QueryVersion ();
* @codeEnd
* @params
* N/A
*
* @retvalues
* The version of ADLX.
* @requirements
* @DetailsTable{#include "ADLXHelper/Windows/C/ADLXHelper.h", @ADLX_First_Ver}
*/
const char*             ADLXHelper_QueryVersion();
#endif // __ADLXFactoryC_h__

