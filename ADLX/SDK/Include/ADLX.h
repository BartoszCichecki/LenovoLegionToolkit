//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_H
#define ADLX_H
#pragma once

#include "ADLXDefines.h"
#include "ISystem.h"

#pragma region dll name
#if defined(_WIN32)
#if defined(_M_AMD64)
/**
* @def ADLX_DLL_NAMEW
* @ingroup ADLXMacro
* @ENG_START_DOX Unicode name for 64-bit ADLX dll @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAMEW                      L"amdadlx64.dll"
*  @codeEnd
*/
#define ADLX_DLL_NAMEW                      L"amdadlx64.dll"
/**
* @def ADLX_DLL_NAMEA
* @ingroup ADLXMacro
* @ENG_START_DOX ANSI name for 64-bit ADLX dll @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAMEA                      "amdadlx64.dll"
*  @codeEnd
*/
#define ADLX_DLL_NAMEA                      "amdadlx64.dll"
#else       //_M_AMD64
/**
* @def ADLX_DLL_NAMEW
* @ingroup ADLXMacro
* @ENG_START_DOX Unicode name for 32-bit ADLX dll @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAMEW                      L"amdadlx32.dll"
*  @codeEnd
*/
#define ADLX_DLL_NAMEW                      L"amdadlx32.dll"
/**
* @def ADLX_DLL_NAMEA
* @ingroup ADLXMacro
* @ENG_START_DOX ANSI name for 32-bit ADLX dll @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAMEA                      "amdadlx32.dll"
*  @codeEnd
*/
#define ADLX_DLL_NAMEA                      "amdadlx32.dll"
#endif      //_M_AMD64

#if defined(UNICODE)
/**
* @def ADLX_DLL_NAME
* @ingroup ADLXMacro
* @ENG_START_DOX ADLX dll name in Unicode applications @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAME                       @ref ADLX_DLL_NAMEW
*  @codeEnd
*/
#define ADLX_DLL_NAME                       ADLX_DLL_NAMEW
#else       //UNICODE
/**
* @def ADLX_DLL_NAME
* @ingroup ADLXMacro
* @ENG_START_DOX ADLX dll name in ANSI applications @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_DLL_NAME                       @ref ADLX_DLL_NAMEA
*  @codeEnd
*/
#define ADLX_DLL_NAME                       ADLX_DLL_NAMEA
#endif      //UNICODE
#endif      //_WIN32
#pragma endregion dll name

#pragma region ADLX callbacks
typedef ADLX_RESULT(ADLX_CDECL_CALL* ADLXQueryFullVersion_Fn)(adlx_uint64* fullVersion);
typedef ADLX_RESULT(ADLX_CDECL_CALL* ADLXQueryVersion_Fn)(const char** version);

/**
* @typedef ADLX_ADL_Main_Memory_Free
* @ingroup ADLXDefs
* @ENG_START_DOX The typedef of ADLX_ADL_Main_Memory_Free function. @ENG_END_DOX
* @definition
*  @codeStart
*   typedef void (ADLX_STD_CALL* ADLX_ADL_Main_Memory_Free)(void** buffer)
*  @codeEnd
*/
typedef void (ADLX_STD_CALL* ADLX_ADL_Main_Memory_Free)(void** buffer);
#pragma endregion ADLX callbacks

#pragma region C-style functions
/**
 * @def ADLX_QUERY_FULL_VERSION_FUNCTION_NAME
 * @ingroup ADLXMacro
 * @ENG_START_DOX The function name of QueryFullVersion @ENG_END_DOX
 * @definition
 *  @codeStart
 *   \#define ADLX_QUERY_FULL_VERSION_FUNCTION_NAME                    "ADLXQueryFullVersion"
 *  @codeEnd
 */
#define ADLX_QUERY_FULL_VERSION_FUNCTION_NAME                    "ADLXQueryFullVersion"

/**
 * @def ADLX_QUERY_VERSION_FUNCTION_NAME
 * @ingroup ADLXMacro
 * @ENG_START_DOX The function name of QueryVersion @ENG_END_DOX
 * @definition
 *  @codeStart
 *   \#define ADLX_QUERY_VERSION_FUNCTION_NAME            "ADLXQueryVersion"
 *  @codeEnd
 */
#define ADLX_QUERY_VERSION_FUNCTION_NAME            "ADLXQueryVersion"

 /**
  * @def ADLX_INIT_FUNCTION_NAME
  * @ingroup ADLXMacro
  * @ENG_START_DOX The function name of ADLXInitialize @ENG_END_DOX
  * @definition
  *  @codeStart
  *   \#define ADLX_INIT_FUNCTION_NAME                             "ADLXInitialize"
  *  @codeEnd
  */
#define ADLX_INIT_FUNCTION_NAME                             "ADLXInitialize"

  /**
   * @def ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME
   * @ingroup ADLXMacro
   * @ENG_START_DOX The function name of ADLXInitializeWithIncompatibleDriver @ENG_END_DOX
   * @definition
   *  @codeStart
   *   \#define ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME          "ADLXInitializeWithIncompatibleDriver"
   *  @codeEnd
   */
#define ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME          "ADLXInitializeWithIncompatibleDriver"

/**
* @def ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME
* @ingroup ADLXMacro
* @ENG_START_DOX The function name of ADLXInitializeWithCallerAdl @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME             "ADLXInitializeWithCallerAdl"
*  @codeEnd
*/
#define ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME             "ADLXInitializeWithCallerAdl"

/**
* @def ADLX_TERMINATE_FUNCTION_NAME
* @ingroup ADLXMacro
* @ENG_START_DOX The function name of ADLXTerminate @ENG_END_DOX
* @definition
*  @codeStart
*   \#define ADLX_TERMINATE_FUNCTION_NAME                        "ADLXTerminate"
*  @codeEnd
*/
#define ADLX_TERMINATE_FUNCTION_NAME                        "ADLXTerminate"

#if defined(__cplusplus)

extern "C"
{
    /**
    * @page page_ADLXQueryFullVersion_Fn ADLXQueryFullVersion_Fn
    * @ENG_START_DOX
    * @brief A pointer to the function to query the full version of ADLX.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryFullVersion_Fn)(adlx_uint64* fullVersion)
    * @codeEnd
    * @params
    * @paramrow{1.,[out],fullVersion,adlx_uint64*,@ENG_START_DOX The pointer to a variable where the full version of ADLX is returned. @ENG_END_DOX}
    *
    * @retvalues
    * @ENG_START_DOX
    * If the full version is successfully returned, __ADLX_OK__ is returned.<br>
    * If the full version is not successfully returned, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    * @ENG_END_DOX
    * @detaileddesc
    * The pointer of the function is returned by the @ref adlx_get_proc_address using the @ref ADLX_QUERY_FULL_VERSION_FUNCTION_NAME as the function name.
    * @requirements
    * @DetailsTable{#include "ADLX.h", @ADLX_First_Ver}
    */

    /**
    * @typedef ADLXQueryFullVersion_Fn
    * @ingroup ADLXDefs
    * The typedef of QueryFullVersion function.
    * @definition
    *  @codeStart
    *   typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *QueryFullVersion_Fn)(adlx_uint64* fullVersion)
    *  @codeEnd
    */
    typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryFullVersion_Fn)(adlx_uint64* fullVersion);

    /**
    * @page page_ADLXQueryVersion_Fn ADLXQueryVersion_Fn
    * @ENG_START_DOX
    * @brief A pointer to the function to query the version of ADLX.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryVersion_Fn)(const char** version)
    * @codeEnd
    * @params
    * @paramrow{1.,[out],version,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the version of ADLX is returned. @ENG_END_DOX}
    *
    * @retvalues
    * @ENG_START_DOX
    * If the version is successfully returned, __ADLX_OK__ is returned.<br>
    * If the version is not successfully returned, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    * @ENG_END_DOX
    * @detaileddesc
    * The pointer of the function is returned by the @ref adlx_get_proc_address using the @ref ADLX_QUERY_VERSION_FUNCTION_NAME as the function name.
    * @requirements
    * @DetailsTable{#include "ADLX.h", @ADLX_First_Ver}
    */

    /**
    * @typedef ADLXQueryVersion_Fn
    * @ingroup ADLXDefs
    * The typedef of QueryVersion function.
    * @definition
    *  @codeStart
    *   typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *QueryVersion_Fn)(const char** version)
    *  @codeEnd
    */
    typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryVersion_Fn)(const char** version);

    /**
    * @page page_ADLXInitialize_Fn ADLXInitialize_Fn
    * @ENG_START_DOX
    * @brief A pointer to the function to initialize ADLX with default parameters or a pointer to the function to initialize ADLX with a legacy driver.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *  typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitialize_Fn)(adlx_uint64 version, @ref DOX_IADLXSystem** ppSystem)
    * @codeEnd
    * @params
    * @paramrow{1.,[in],version,adlx_uint64,@ENG_START_DOX The version of ADLX. @ENG_END_DOX}
    * @paramrow{1.,[out],ppSystem,@ref DOX_IADLXSystem**,@ENG_START_DOX The address of a pointer to the ADLX system interface. If ADLX initialization failed\, the method sets the dereferenced address __*ppSystem__ to __nullptr__. @ENG_END_DOX}
    *
    * @retvalues
    * @ENG_START_DOX
    * If ADLX was successfully initialized, __ADLX_OK__ is returned.<br>
    * If ADLX was not successfully initialized, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    * @ENG_END_DOX
    * @detaileddesc
    * The pointer of the function is returned by the @ref adlx_get_proc_address using the @ref ADLX_INIT_FUNCTION_NAME or @ref ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME as the function name.
    * @requirements
    * @DetailsTable{#include "ADLX.h", @ADLX_First_Ver}
    */

    /**
    * @typedef ADLXInitialize_Fn
    * @ingroup ADLXDefs
    * The typedef of ADLXInitialize function.
    * @definition
    *  @codeStart
    *  typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitialize_Fn)(adlx_uint64 version, @ref DOX_IADLXSystem** ppSystem)
    *  @codeEnd
    */
    typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitialize_Fn)(adlx_uint64 version, adlx::IADLXSystem** ppSystem);

    /**
    * @page page_ADLXInitializeWithCallerAdl_Fn ADLXInitializeWithCallerAdl_Fn
    * @ENG_START_DOX
    * @brief A pointer to the function to initialize ADLX with an ADL context.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *   typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitializeWithCallerAdl_Fn)(adlx_uint64 version, @ref DOX_IADLXSystem** ppSystem, @ref DOX_IADLMapping** ppAdlMapping, adlx_handle adlContext, @ref ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
    * @codeEnd
    * @params
    * @paramrow{1.,[in],version,adlx_uint64,@ENG_START_DOX The version of ADLX. @ENG_END_DOX}
    * @paramrow{2.,[out],ppSystem,@ref DOX_IADLXSystem**,@ENG_START_DOX The address of a pointer to the ADLX system interface. If ADLX initialization failed\, the method sets the dereferenced address __*ppSystem__ to __nullptr__. @ENG_END_DOX}
    * @paramrow{3.,[out],ppAdlMapping,@ref DOX_IADLMapping**,@ENG_START_DOX The address of a pointer to the ADLX mapping interface. If ADLX initialization failed\, the method sets the dereferenced address __*ppAdlMapping__ to __nullptr__. @ENG_END_DOX}
    * @paramrow{4.,[in],adlContext,adlx_handle,@ENG_START_DOX The ADL context. @ENG_END_DOX}
    * @paramrow{5.,[in],adlMainMemoryFree,@ref ADLX_ADL_Main_Memory_Free,@ENG_START_DOX The callback handle of the memory deallocation function. @ENG_END_DOX}
    *
    * @retvalues
    * @ENG_START_DOX
    * If ADLX was successfully initialized, __ADLX_OK__ is returned.<br>
    * If ADLX was not successfully initialized, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    * @ENG_END_DOX
    * @detaileddesc
    * The pointer of the function is returned by the @ref adlx_get_proc_address using the @ref ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME as the function name.
    * @requirements
    * @DetailsTable{#include "ADLX.h", @ADLX_First_Ver}
    */

    /**
    * @typedef ADLXInitializeWithCallerAdl_Fn
    * @ingroup ADLXDefs
    * The typedef of ADLXInitializeWithCallerAdl function.
    * @definition
    *  @codeStart
    *   typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitializeWithCallerAdl_Fn)(adlx_uint64 version, @ref DOX_IADLXSystem** ppSystem, @ref DOX_IADLMapping** ppAdlMapping, adlx_handle adlContext, @ref ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
    *  @codeEnd
    */
    typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitializeWithCallerAdl_Fn)(adlx_uint64 version, adlx::IADLXSystem** ppSystem, adlx::IADLMapping** ppAdlMapping, adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree);

    /**
    * @page page_ADLXTerminate_Fn ADLXTerminate_Fn
    * @ENG_START_DOX
    * @brief A pointer to the function to terminate ADLX.
    * @ENG_END_DOX
    *
    * @syntax
    * @codeStart
    *   typedef @ref ADLX_RESULT (ADLX_CDECL_CALL *ADLXTerminate_Fn)()
    * @codeEnd
    * @params
    * N/A
    *
    * @retvalues
    * @ENG_START_DOX
    * If the function is successfully executed, __ADLX_OK__ is returned.<br>
    * If the function is not successfully executed, an error code is returned.<br>
    * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
    * @ENG_END_DOX
    * @detaileddesc
    * The pointer of the function is returned by the @ref adlx_get_proc_address using the @ref ADLX_TERMINATE_FUNCTION_NAME as the function name.
    * @requirements
    * @DetailsTable{#include "ADLX.h", @ADLX_First_Ver}
    */

    /**
    * @typedef ADLXTerminate_Fn
    * @ingroup ADLXDefs
    * The typedef of ADLXTerminate function.
    * @definition
    *  @codeStart
    *   typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXTerminate_Fn)()
    *  @codeEnd
    */
    typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXTerminate_Fn)();
}
#else
typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryFullVersion_Fn)(adlx_uint64* fullVersion);
typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXQueryVersion_Fn)(const char** version);
typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitialize_Fn)(adlx_uint64 version, IADLXSystem** ppSystem);
typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXInitializeWithCallerAdl_Fn)(adlx_uint64 version, IADLXSystem** ppSystem, IADLMapping** ppAdlMapping, adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree);
typedef ADLX_RESULT (ADLX_CDECL_CALL *ADLXTerminate_Fn)();
#endif

#pragma endregion C-style functions

#endif  //ADLX_H