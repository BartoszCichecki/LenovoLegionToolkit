//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_DEFINES_H
#define ADLX_DEFINES_H
#pragma once

#if defined(_WIN32)
#include <Windows.h>
#endif

#include <stdio.h>
#include <stdint.h>
#include "ADLXVersion.h"

//-------------------------------------------------------------------------------------------------
//Version stuff
#pragma region version

#define ADLX_MAKE_FULL_VER(VERSION_MAJOR, VERSION_MINOR, VERSION_RELEASE, VERSION_BUILD_NUM)    ( ((adlx_uint64)(VERSION_MAJOR) << 48ull) | ((adlx_uint64)(VERSION_MINOR) << 32ull) | ((adlx_uint64)(VERSION_RELEASE) << 16ull)  | (adlx_uint64)(VERSION_BUILD_NUM))
#define ADLX_FULL_VERSION ADLX_MAKE_FULL_VER(ADLX_VER_MAJOR, ADLX_VER_MINOR, ADLX_VER_RELEASE, ADLX_VER_BUILD_NUM)

#define ADLX_VERSION_STR ADLX_VER_MAJOR##.##ADLX_VER_MINOR
#pragma endregion version

//-------------------------------------------------------------------------------------------------
//ADLX Platform - platform-specific defines, such as types and C/C++ declarations
#pragma region platform
/**
  * @defgroup typedefs ADLX Primitive Data Types
  * @ENG_START_DOX
  * @brief This section provides definitions for ADLX primitive data types.
  * @ENG_END_DOX
  * 
  * @requirements
  * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
  *
  * ADLX Type       | Description
  * ----:           | :----
  * adlx_int64      | @ENG_START_DOX adlx_int64 is typedef of int64_t @ENG_END_DOX
  * adlx_int32      | @ENG_START_DOX adlx_int32 is typedef of int32_t @ENG_END_DOX
  * adlx_int16      | @ENG_START_DOX adlx_int16 is typedef of int16_t @ENG_END_DOX
  * adlx_int8       | @ENG_START_DOX adlx_int8 is typedef of int8_t @ENG_END_DOX
  * adlx_uint64     | @ENG_START_DOX adlx_uint64 is typedef of uint64_t @ENG_END_DOX
  * adlx_uint32     | @ENG_START_DOX adlx_uint32 is typedef of uint32_t @ENG_END_DOX
  * adlx_uint16     | @ENG_START_DOX adlx_uint16 is typedef of uint16_t @ENG_END_DOX
  * adlx_uint8      | @ENG_START_DOX adlx_uint8 is typedef of uint8_t @ENG_END_DOX
  * adlx_size       | @ENG_START_DOX adlx_size is typedef of size_t @ENG_END_DOX
  * adlx_handle     | @ENG_START_DOX adlx_handle is typedef of void* @ENG_END_DOX
  * adlx_double     | @ENG_START_DOX adlx_double is typedef of double @ENG_END_DOX
  * adlx_float      | @ENG_START_DOX adlx_float is typedef of float @ENG_END_DOX
  * adlx_void       | @ENG_START_DOX adlx_void is typedef of void @ENG_END_DOX
  * adlx_int        | @ENG_START_DOX adlx_int is typedef of adlx_int32 @ENG_END_DOX
  * adlx_uint       | @ENG_START_DOX adlx_uint is typedef of adlx_uint32 @ENG_END_DOX
  * adlx_ulong      | @ENG_START_DOX adlx_ulong is typedef of unsigned long @ENG_END_DOX
  * adlx_bool       | @ENG_START_DOX adlx_bool is typedef of bool @ENG_END_DOX
  */
#pragma region ADLX simple data types
typedef     int64_t             adlx_int64;
typedef     int32_t             adlx_int32;
typedef     int16_t             adlx_int16;
typedef     int8_t              adlx_int8;

typedef     uint64_t            adlx_uint64;
typedef     uint32_t            adlx_uint32;
typedef     uint16_t            adlx_uint16;
typedef     uint8_t             adlx_uint8;
typedef     size_t              adlx_size;

typedef     void*               adlx_handle;
typedef     double              adlx_double;
typedef     float               adlx_float;

typedef     void                adlx_void;

typedef     long                adlx_long;
typedef     adlx_int32          adlx_int;
typedef     unsigned long       adlx_ulong;
typedef     adlx_uint32         adlx_uint;

typedef     uint8_t             adlx_byte;

#if defined(__cplusplus)
typedef     bool                adlx_bool;
#else
typedef     adlx_uint8           adlx_bool;
#define     true                1
#define     false               0
#endif
#pragma endregion ADLX simple data types

//Calling standards
#pragma region Export declarations

#if defined(_WIN32)
    //  Microsoft
#define ADLX_CORE_LINK          __declspec(dllexport)
#define ADLX_STD_CALL           __stdcall
#define ADLX_CDECL_CALL         __cdecl
#define ADLX_FAST_CALL          __fastcall
#define ADLX_INLINE              __inline
#define ADLX_FORCEINLINE         __forceinline
#define ADLX_NO_VTABLE          __declspec(novtable)
#else
#define EXPORT
#define IMPORT
#pragma warning Unknown dynamic link import/export semantics.
#endif

#pragma endregion Export declarations

//Various platform APIs
#pragma region Platform APIs

/**
 * @page adlx_atomic_inc adlx_atomic_inc
 * @ENG_START_DOX
 * @brief An atomic increment of a variable in multithreading environments.
 * @ENG_END_DOX
 *
 * @syntax
 * @codeStart
 *  adlx_long    adlx_atomic_inc (adlx_long* x)
 * @codeEnd
 * @params
 * @paramrow{1.,[in],x,adlx_long*,@ENG_START_DOX The pointer to the variable to be incremented. @ENG_END_DOX}
 *
 * @retvalues
 * @ENG_START_DOX
 * The incremented value.
 * @ENG_END_DOX
 * @requirements
 * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
 */
adlx_long   ADLX_CDECL_CALL adlx_atomic_inc (adlx_long* x);

/**
 * @page adlx_atomic_dec adlx_atomic_dec
 * @ENG_START_DOX
 * @brief An atomic decrement of a variable in multithreading environments.
 * @ENG_END_DOX
 *
 * @syntax
 * @codeStart
 *  adlx_long    adlx_atomic_dec (adlx_long* x)
 * @codeEnd
 * @params
 * @paramrow{1.,[in],x,adlx_long*,@ENG_START_DOX The pointer to the variable to be decremented. @ENG_END_DOX}
 *
 * @retvalues
 * @ENG_START_DOX
 * The decremented value.
 * @ENG_END_DOX
 * @requirements
 * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
 */
adlx_long   ADLX_CDECL_CALL adlx_atomic_dec (adlx_long* x);


/**
 * @page adlx_load_library adlx_load_library
 * @ENG_START_DOX
 * @brief Loads a module into the address space of the calling process.
 * @ENG_END_DOX
 *
 * @syntax
 * @codeStart
 *  adlx_handle    adlx_load_library (const TCHAR* filename)
 * @codeEnd
 * @params
 * @paramrow{1.,[in],filename,const TCHAR*,@ENG_START_DOX The zero-terminated string that specifies the file name of the module to be loaded. @ENG_END_DOX}
 *
 * @retvalues
 * @ENG_START_DOX
 * If the module is successfully loaded, a handle to the loaded module is returned.<br>
 * If the module is not successfully loaded, __nullptr__ is returned.<br>
 * @ENG_END_DOX
 *
 * @detaileddesc
 * @ENG_START_DOX
 * @details
 * Use this function to load ADLX in your application. Specify the __filename__ parameter as @ref ADLX_DLL_NAME.
 * @ENG_END_DOX
 *
 * @requirements
 * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
 */
adlx_handle ADLX_CDECL_CALL adlx_load_library (const TCHAR* filename);

/**
* @page adlx_free_library adlx_free_library
* @ENG_START_DOX
* @brief Frees a loaded module.
* @ENG_END_DOX
*
* @syntax
* @codeStart
*  int    adlx_free_library (adlx_handle module)
* @codeEnd
* @params
* @paramrow{1.,[in],module,adlx_handle,@ENG_START_DOX The handle to the loaded module. @ENG_END_DOX}
*
* @retvalues
* @ENG_START_DOX
* If the module is successfully unloaded, a non-zero value is returned.<br>
* If the module is not successfully unloaded, zero is returned.<br>
* @ENG_END_DOX
*
* @detaileddesc
* @ENG_START_DOX
* @details
* When ADLX is successfully freed with this function, ADLX is unloaded from the address space of the calling process and the handle to the module is no longer valid.
* @ENG_END_DOX
*
* @requirements
* @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
*/
int         ADLX_CDECL_CALL adlx_free_library (adlx_handle module);

/**
 * @page adlx_get_proc_address adlx_get_proc_address
 * @ENG_START_DOX
 * @brief Retrieves the address of a function from a module.
 * @ENG_END_DOX
 *
 * @syntax
 * @codeStart
 *  void*    adlx_get_proc_address (adlx_handle module, const char* procName)
 * @codeEnd
 * @params
 * @paramrow{1.,[in],module,adlx_handle,@ENG_START_DOX The handle to the module. @ENG_END_DOX}
 * @paramrow{2.,[in],procName,const char*,@ENG_START_DOX The zero-terminated string that specifies the name of the function. @ENG_END_DOX}
 *
 * @retvalues
 * @ENG_START_DOX
 * If the function was found, the address of the function is returned.<br>
 * If the function was not found, __nullptr__ is returned.<br>
 * @ENG_END_DOX
 *
 * @detaileddesc
 * @ENG_START_DOX
 * @details
 * Use this function to load ADLX functions from the ADLX module. The name of the ADLX function can be one of values: @ref ADLX_QUERY_FULL_VERSION_FUNCTION_NAME, @ref ADLX_QUERY_VERSION_FUNCTION_NAME, @ref ADLX_INIT_FUNCTION_NAME, @ref ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME, @ref ADLX_TERMINATE_FUNCTION_NAME.<br>
 * Parameter: procName | Description  | Return Value
 * -|-|-
 * @ref ADLX_QUERY_FULL_VERSION_FUNCTION_NAME | The function to query the full version of ADLX. | @ref page_ADLXQueryFullVersion_Fn|
 * @ref ADLX_QUERY_VERSION_FUNCTION_NAME | The function to query the version of ADLX. | @ref page_ADLXQueryVersion_Fn|
 * @ref ADLX_INIT_FUNCTION_NAME | The function to initialize ADLX with default parameters. | @ref page_ADLXInitialize_Fn |
 * @ref ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME | The function to initialize ADLX with a legacy driver. | @ref page_ADLXInitialize_Fn |
 * @ref ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME |  The function to initialize ADLX with an ADL context. | @ref page_ADLXInitializeWithCallerAdl_Fn|
 * @ref ADLX_TERMINATE_FUNCTION_NAME | The function to terminate ADLX. | @ref page_ADLXTerminate_Fn |
 * @ENG_END_DOX
 *
 * @requirements
 * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
 */
void*       ADLX_CDECL_CALL adlx_get_proc_address (adlx_handle module, const char* procName);

#pragma endregion Platform APIs

#pragma endregion platform
/**   @file */
//-------------------------------------------------------------------------------------------------
//ADLX data types
#pragma region ADLX data types

#pragma region ADLX_RESULT
/**
 * @enum ADLX_RESULT
 * @ingroup enumerations
 * @ENG_START_DOX
 * @brief Indicates the result returned from an ADLX function or from an ADLX method.
 * @ENG_END_DOX
 */
typedef enum
{
    ADLX_OK = 0,                    /**< @ENG_START_DOX This result indicates success. @ENG_END_DOX */
    ADLX_ALREADY_ENABLED,           /**< @ENG_START_DOX This result indicates that the asked action is already enabled. @ENG_END_DOX */
    ADLX_ALREADY_INITIALIZED,       /**< @ENG_START_DOX This result indicates that ADLX has a unspecified type of initialization. @ENG_END_DOX */
    ADLX_FAIL,                      /**< @ENG_START_DOX This result indicates an unspecified failure. @ENG_END_DOX */
    ADLX_INVALID_ARGS,              /**< @ENG_START_DOX This result indicates that the arguments are invalid. @ENG_END_DOX */
    ADLX_BAD_VER,                   /**< @ENG_START_DOX This result indicates that the asked version is incompatible with the current version. @ENG_END_DOX */
    ADLX_UNKNOWN_INTERFACE,         /**< @ENG_START_DOX This result indicates that an unknown interface was asked. @ENG_END_DOX */
    ADLX_TERMINATED,                /**< @ENG_START_DOX This result indicates that the calls were made in an interface after ADLX was terminated. @ENG_END_DOX */
    ADLX_ADL_INIT_ERROR,            /**< @ENG_START_DOX This result indicates that the ADL initialization failed. @ENG_END_DOX */
    ADLX_NOT_FOUND,                 /**< @ENG_START_DOX This result indicates that the item is not found. @ENG_END_DOX */
    ADLX_INVALID_OBJECT,            /**< @ENG_START_DOX This result indicates that the method was called into an invalid object. @ENG_END_DOX */
    ADLX_ORPHAN_OBJECTS,            /**< @ENG_START_DOX This result indicates that ADLX was terminated with outstanding ADLX objects. Any interface obtained from ADLX points to invalid memory and calls in their methods will result in unexpected behavior. @ENG_END_DOX */
    ADLX_NOT_SUPPORTED,             /**< @ENG_START_DOX This result indicates that the asked feature is not supported. @ENG_END_DOX */
    ADLX_PENDING_OPERATION,         /**< @ENG_START_DOX This result indicates a failure due to an operation currently in progress. @ENG_END_DOX */
    ADLX_GPU_INACTIVE               /**< @ENG_START_DOX This result indicates that the GPU is inactive. @ENG_END_DOX */
} ADLX_RESULT;

/**
 * @def ADLX_SUCCEEDED
 * @ingroup ADLXMacro
 * @ENG_START_DOX Checks if the result code passed as parameter indicates a successful operation. @ENG_END_DOX
 * @definition
 *  @codeStart
 *   \#define ADLX_SUCCEEDED(x) (<b>ADLX_OK</b>  == (x) || <b>ADLX_ALREADY_ENABLED</b> == (x) || <b>ADLX_ALREADY_INITIALIZED</b> == (x))
 *  @codeEnd
*/
#define ADLX_SUCCEEDED(x) (ADLX_OK == (x) || ADLX_ALREADY_ENABLED == (x) || ADLX_ALREADY_INITIALIZED == (x))

 /**
  * @def ADLX_FAILED
  * @ingroup ADLXMacro
  * @ENG_START_DOX Checks if the result code passed as parameter indicates an unsuccessful operation. @ENG_END_DOX
  * @definition
  *  @codeStart
  *   \#define ADLX_FAILED(x) (<b>ADLX_OK</b> != (x)  && <b>ADLX_ALREADY_ENABLED</b> != (x) && <b>ADLX_ALREADY_INITIALIZED</b> != (x))
  *  @codeEnd
 */
#define ADLX_FAILED(x) (ADLX_OK != (x)  && ADLX_ALREADY_ENABLED != (x) && ADLX_ALREADY_INITIALIZED != (x))

#pragma endregion ADLX_RESULT

#pragma region ADLX_HG_TYPE
/**
 * @enum ADLX_HG_TYPE
 * @ingroup enumerations
 * @ENG_START_DOX
 * @brief Indicates the type of Hybrid Graphic.
 * @ENG_END_DOX
 */
typedef enum
{
    NONE = 0,                       /**< @ENG_START_DOX This is not a Hybrid Graphics system. @ENG_END_DOX */
    AMD,                            /**< @ENG_START_DOX This is an AMD integrated GPU. @ENG_END_DOX */
    OTHER,                          /**< @ENG_START_DOX This is a non-AMD integrated GPU. @ENG_END_DOX */
} ADLX_HG_TYPE;
#pragma endregion ADLX_HG_TYPE

#pragma region ADLX_ASIC_FAMILY_TYPE
/**
 * @enum ADLX_ASIC_FAMILY_TYPE
 * @ingroup enumerations
 * @ENG_START_DOX
 * @brief Indicates the ASIC family type.
 * @ENG_END_DOX
 */
typedef enum
{
    ASIC_UNDEFINED = 0,             /**< @ENG_START_DOX The ASIC family type is not defined. @ENG_END_DOX */
    ASIC_RADEON,                    /**< @ENG_START_DOX The ASIC family type is discrete. @ENG_END_DOX */
    ASIC_FIREPRO,                   /**< @ENG_START_DOX The ASIC family type is Firepro. @ENG_END_DOX */
    ASIC_FIREMV,                    /**< @ENG_START_DOX The ASIC family type is FireMV. @ENG_END_DOX */
    ASIC_FIRESTREAM,                /**< @ENG_START_DOX The ASIC family type is FireStream. @ENG_END_DOX */
    ASIC_FUSION,                    /**< @ENG_START_DOX The ASIC family type is Fusion. @ENG_END_DOX */
    ASIC_EMBEDDED,                  /**< @ENG_START_DOX The ASIC family type is Embedded. @ENG_END_DOX */
} ADLX_ASIC_FAMILY_TYPE;
#pragma endregion ADLX_ASIC_FAMILY_TYPE

#pragma region ADLX_GPU_TYPE
/**
 * @enum ADLX_GPU_TYPE
 * @ingroup enumerations
 * @ENG_START_DOX
 * @brief Indicates the GPU type.
 * @ENG_END_DOX
 */
typedef enum
{
    GPUTYPE_UNDEFINED = 0,          /**< @ENG_START_DOX The GPU type is unknown. @ENG_END_DOX */
    GPUTYPE_INTEGRATED,             /**< @ENG_START_DOX The GPU type is an integrated GPU. @ENG_END_DOX */
    GPUTYPE_DISCRETE,               /**< @ENG_START_DOX The GPU type is a discrete GPU. @ENG_END_DOX */
} ADLX_GPU_TYPE;
#pragma endregion ADLX_GPU_TYPE

#pragma region ADLX_DISPLAY_CONNECTOR_TYPE
/**
 *  @enum ADLX_DISPLAY_CONNECTOR_TYPE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the display connector type.
 *  @ENG_END_DOX
 */
typedef enum
{
    DISPLAY_CONTYPE_UNKNOWN = 0,                /**< @ENG_START_DOX The display connector type is unknown. @ENG_END_DOX */
    DISPLAY_CONTYPE_VGA,                        /**< @ENG_START_DOX The display connector type is VGA. @ENG_END_DOX */
    DISPLAY_CONTYPE_DVI_D,                      /**< @ENG_START_DOX The display connector type is DVI-D. @ENG_END_DOX */
    DISPLAY_CONTYPE_DVI_I,                      /**< @ENG_START_DOX The display connector type is DVI-I. @ENG_END_DOX */
    DISPLAY_CONTYPE_CVDONGLE_NTSC,           /**< @ENG_START_DOX The display connector type is NTSC. @ENG_END_DOX */
    DISPLAY_CONTYPE_CVDONGLE_JPN,            /**< @ENG_START_DOX The display connector type is JPN. @ENG_END_DOX */
    DISPLAY_CONTYPE_CVDONGLE_NONI2C_JPN,     /**< @ENG_START_DOX The display connector type is NONI2C-JPN. @ENG_END_DOX */
    DISPLAY_CONTYPE_CVDONGLE_NONI2C_NTSC,    /**< @ENG_START_DOX The display connector type is NONI2C-NTSC. @ENG_END_DOX */
    DISPLAY_CONTYPE_PROPRIETARY,                /**< @ENG_START_DOX The display connector type is PROPRIETARY. @ENG_END_DOX */
    DISPLAY_CONTYPE_HDMI_TYPE_A,                /**< @ENG_START_DOX The display connector type is HDMI A. @ENG_END_DOX */
    DISPLAY_CONTYPE_HDMI_TYPE_B,                /**< @ENG_START_DOX The display connector type is HDMI B. @ENG_END_DOX */
    DISPLAY_CONTYPE_SVIDEO,                     /**< @ENG_START_DOX The display connector type is SVIDEO. @ENG_END_DOX */
    DISPLAY_CONTYPE_COMPOSITE,                  /**< @ENG_START_DOX The display connector type is COMPOSITE. @ENG_END_DOX */
    DISPLAY_CONTYPE_RCA_3COMPONENT,             /**< @ENG_START_DOX The display connector type is RCA. @ENG_END_DOX */
    DISPLAY_CONTYPE_DISPLAYPORT,                /**< @ENG_START_DOX The display connector type is DISPLAYPORT. @ENG_END_DOX */
    DISPLAY_CONTYPE_EDP,                        /**< @ENG_START_DOX The display connector type is EDP. @ENG_END_DOX */
    DISPLAY_CONTYPE_WIRELESSDISPLAY,            /**< @ENG_START_DOX The display connector type is WIRELESSDISPLAY. @ENG_END_DOX */
    DISPLAY_CONTYPE_USB_TYPE_C                  /**< @ENG_START_DOX The display connector type is USB Type-C. @ENG_END_DOX */
}ADLX_DISPLAY_CONNECTOR_TYPE;
#pragma endregion ADLX_DISPLAY_CONNECTOR_TYPE

#pragma region ADLX_DISPLAY_TYPE
/**
 *  @enum ADLX_DISPLAY_TYPE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the display type.
 *  @ENG_END_DOX
 */
typedef enum
{
    DISPLAY_TYPE_UNKOWN = 0,          /**< @ENG_START_DOX The display type is an unknown display. @ENG_END_DOX */
    DISPLAY_TYPE_MONITOR,             /**< @ENG_START_DOX The display type is a monitor display. @ENG_END_DOX */
    DISPLAY_TYPE_TELEVISION,          /**< @ENG_START_DOX The display type is a TV display. @ENG_END_DOX */
    DISPLAY_TYPE_LCD_PANEL,           /**< @ENG_START_DOX The display type is an LCD display. @ENG_END_DOX */
    DISPLAY_TYPE_DIGITAL_FLAT_PANEL,  /**< @ENG_START_DOX The display type is a DFP display. @ENG_END_DOX */
    DISPLAY_TYPE_COMPONENT_VIDEO,     /**< @ENG_START_DOX The display type is a component video display. @ENG_END_DOX */
    DISPLAY_TYPE_PROJECTOR            /**< @ENG_START_DOX The display type is a projector display. @ENG_END_DOX */
}ADLX_DISPLAY_TYPE;
#pragma endregion ADLX_DISPLAY_TYPE

#pragma region ADLX_DISPLAY_SCAN_TYPE
/**
 *  @enum ADLX_DISPLAY_SCAN_TYPE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the display scan type.
 *  @ENG_END_DOX
 */
typedef enum
{
    PROGRESSIVE = 0,                /**< @ENG_START_DOX The display scan type is progressive mode. @ENG_END_DOX */
    INTERLACED                      /**< @ENG_START_DOX The display scan type is interlaced mode. @ENG_END_DOX */
} ADLX_DISPLAY_SCAN_TYPE;
#pragma endregion ADLX_DISPLAY_SCAN_TYPE

#pragma region ADLX_DISPLAY_TIMING_POLARITY
/**
 *  @enum ADLX_DISPLAY_TIMING_POLARITY
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Display timing polarity
 *  @ENG_END_DOX
 */
typedef enum
{
    POSITIVE = 0,   /**< @ENG_START_DOX Positive Polarity @ENG_END_DOX */
    NEGATIVE        /**< @ENG_START_DOX Negative Polarity @ENG_END_DOX */
} ADLX_DISPLAY_TIMING_POLARITY;

#pragma endregion ADLX_DISPLAY_TIMING_POLARITY

#pragma region ADLX_DISPLAY_TIMING_LIMITATION
/**
 *  @enum ADLX_DISPLAY_TIMING_LIMITATION
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Display timing limitation
 *  @ENG_END_DOX
 */
typedef enum
{
    PIXEL_CLOCK_MAX = 650000, /**< @ENG_START_DOX Maximum pixel clock @ENG_END_DOX */
    PIXEL_CLOCK_MIN = 0,      /**< @ENG_START_DOX Minimum pixel clock @ENG_END_DOX */
    REFRESH_RATE_MAX = 200,   /**< @ENG_START_DOX Maximum refresh rate @ENG_END_DOX */
    REFRESH_RATE_MIN = 1,     /**< @ENG_START_DOX Minimum refresh rate @ENG_END_DOX */
    RESOLUTION_MAX = 9999,   /**< @ENG_START_DOX Maximum resolution @ENG_END_DOX */
    RESOLUTION_MIN = 1       /**< @ENG_START_DOX Minimum resolution @ENG_END_DOX */
} ADLX_DISPLAY_TIMING_LIMITATION;

#pragma endregion ADLX_DISPLAY_TIMING_LIMITATION

#pragma region ADLX_USER_3DLUT_SIZE
/**
* @def MAX_USER_3DLUT_NUM_POINTS
* @ingroup ADLXMacro
* @ENG_START_DOX Constant value used to set size of user 3D LUT data @ENG_END_DOX
* @definition
*  @codeStart
*   \#define MAX_USER_3DLUT_NUM_POINTS     17
*  @codeEnd
*/

#define MAX_USER_3DLUT_NUM_POINTS 17

#pragma endregion ADLX_USER_3DLUT_SIZE

#pragma region ADLX_GAMUT_SPACE
/**
 *  @enum ADLX_GAMUT_SPACE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the predefined gamut space.
 *  @ENG_END_DOX
 */
typedef enum
{
    GAMUT_SPACE_CCIR_709 = 0,       /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_CCIR_709. @ENG_END_DOX */
    GAMUT_SPACE_CCIR_601,           /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_CCIR_601. @ENG_END_DOX */
    GAMUT_SPACE_ADOBE_RGB,          /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_ADOBE_RGB. @ENG_END_DOX */
    GAMUT_SPACE_CIE_RGB,            /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_CIE_RGB. @ENG_END_DOX */
    GAMUT_SPACE_CCIR_2020,          /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_CCIR_2020. @ENG_END_DOX */
    GAMUT_SPACE_CUSTOM              /**< @ENG_START_DOX The predefined gamut space is GAMUT_SPACE_CUSTOM. @ENG_END_DOX */
} ADLX_GAMUT_SPACE;
#pragma endregion ADLX_GAMUT_SPACE

#pragma region ADLX_WHITE_POINT
/**
 *  @enum ADLX_WHITE_POINT
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the standardized white point setting.
 *  @ENG_END_DOX
 */
typedef enum
{
    WHITE_POINT_5000K = 0,          /**< @ENG_START_DOX The white point setting is 5000k. @ENG_END_DOX */
    WHITE_POINT_6500K,              /**< @ENG_START_DOX The white point setting is 6500k. @ENG_END_DOX */
    WHITE_POINT_7500K,              /**< @ENG_START_DOX The white point setting is 7500k. @ENG_END_DOX */
    WHITE_POINT_9300K,              /**< @ENG_START_DOX The white point setting is 9300k. @ENG_END_DOX */
    WHITE_POINT_CUSTOM              /**< @ENG_START_DOX The white point setting is customized. @ENG_END_DOX */
} ADLX_WHITE_POINT;
#pragma endregion ADLX_WHITE_POINT

#pragma region ADLX_GAMMA_TYPE
/**
 *  @enum ADLX_GAMMA_TYPE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the gamma type.
 *  @ENG_END_DOX
 */
typedef enum
{
    UNKNOW = 0,                 /**< @ENG_START_DOX The gamma type is unknown. @ENG_END_DOX */
    DEGAMMA_RAMP,               /**< @ENG_START_DOX The gamma type is Degamma Ramp way. @ENG_END_DOX */
    REGAMMA_RAMP,               /**< @ENG_START_DOX The gamma type is Regamma Ramp way. @ENG_END_DOX */
    DEGAMMA_COEFFICIENTS,       /**< @ENG_START_DOX The gamma type is Degamma coefficients way. @ENG_END_DOX */
    REGAMMA_COEFFICIENTS        /**< @ENG_START_DOX The gamma type is Regamma coefficients way. @ENG_END_DOX */
} ADLX_GAMMA_TYPE;
#pragma endregion ADLX_GAMMA_TYPE

#pragma region ADLX_ORIENTATION
/**
 *  @enum ADLX_ORIENTATION
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the orientation.
 *  @ENG_END_DOX
 */
typedef enum
{
    ORIENTATION_LANDSCAPE = 0,              /**< @ENG_START_DOX The orientation is landscape. @ENG_END_DOX */
    ORIENTATION_PORTRAIT = 90,              /**< @ENG_START_DOX The orientation is Portrait. @ENG_END_DOX */
    ORIENTATION_LANDSCAPE_FLIPPED = 180,    /**< @ENG_START_DOX The orientation is landscape (flipped). @ENG_END_DOX */
    ORIENTATION_PORTRAIT_FLIPPED = 270,     /**< @ENG_START_DOX the orientation is Portrait (flipped). @ENG_END_DOX */
} ADLX_ORIENTATION;
#pragma endregion ADLX_ORIENTATION

#pragma region ADLX_DESKTOP_TYPE
/**
 *  @enum ADLX_DESKTOP_TYPE
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Types of desktops in respect to display composition
 *  @ENG_END_DOX
 */
typedef enum {
    DESKTOP_SINGLE = 0,     /**< @ENG_START_DOX Single display desktop: one display showing the entire desktop @ENG_END_DOX */
    DESKTOP_DUPLCATE = 1,   /**< @ENG_START_DOX Duplicate desktop: two or mode displays each show the entire desktop @ENG_END_DOX */
    DESKTOP_EYEFINITY = 2,  /**< @ENG_START_DOX AMD Eyefinity desktop: two or mode displays each show a portion of the desktop @ENG_END_DOX */
} ADLX_DESKTOP_TYPE;
#pragma endregion ADLX_DESKTOP_TYPE

#pragma region ADLX_LOG_SEVERITY
/** @enum ADLX_LOG_SEVERITY
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the severity level for ADLX logs.
 * @ENG_END_DOX
 *
 */
typedef enum
{
    LDEBUG = 0,      /**< @ENG_START_DOX The log captures errors, warnings and debug information. @ENG_END_DOX */
    LWARNING,        /**< @ENG_START_DOX The log captures errors and warnings. @ENG_END_DOX */
    LERROR,          /**< @ENG_START_DOX The log captures errors. @ENG_END_DOX */
} ADLX_LOG_SEVERITY;
#pragma endregion ADLX_LOG_SEVERITY

#pragma region ADLX_LOG_DESTINATION
/** @enum ADLX_LOG_DESTINATION
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the location of the log traces that are generated from the internal code execution of ADLX.
 * @ENG_END_DOX
 *
 */
typedef enum
{
    LOCALFILE = 0,      /**< @ENG_START_DOX The log destination is a file. @ENG_END_DOX */
    DBGVIEW,            /**< @ENG_START_DOX The log destination is the application debugger. @ENG_END_DOX */
    APPLICATION,        /**< @ENG_START_DOX The log destination is the application. @ENG_END_DOX */
} ADLX_LOG_DESTINATION;
#pragma endregion ADLX_LOG_DESTINATION

#pragma region ADLX_SCALE_MODE
/** @enum ADLX_SCALE_MODE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the methods to stretch and position the image to fit on the display.
 * @ENG_END_DOX
 *
 */
typedef enum
{
    PRESERVE_ASPECT_RATIO = 0,      /**< @ENG_START_DOX The scale mode preserves aspect ratio. @ENG_END_DOX */
    FULL_PANEL,                     /**< @ENG_START_DOX The scale mode extends to full panel. @ENG_END_DOX */
    CENTERED                        /**< @ENG_START_DOX The scale mode is centered on screen. @ENG_END_DOX */
} ADLX_SCALE_MODE;

#pragma endregion ADLX_SCALE_MODE

#pragma region ADLX_COLOR_DEPTH
/** @enum ADLX_COLOR_DEPTH
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the color/bit depth, which is the number of bits used to indicate the color of a single pixel.
 * @ENG_END_DOX
 *
 */
typedef enum
{
    BPC_UNKNOWN = 0,
    BPC_6,      /**< @ENG_START_DOX A color component/pixel with 6 bits @ENG_END_DOX */
    BPC_8,      /**< @ENG_START_DOX A color component/pixel with 8 bits @ENG_END_DOX */
    BPC_10,     /**< @ENG_START_DOX A color component/pixel with 10 bits @ENG_END_DOX */
    BPC_12,     /**< @ENG_START_DOX A color component/pixel with 12 bits @ENG_END_DOX */
    BPC_14,     /**< @ENG_START_DOX A color component/pixel with 14 bits @ENG_END_DOX */
    BPC_16      /**< @ENG_START_DOX A color component/pixel with 16 bits @ENG_END_DOX */
} ADLX_COLOR_DEPTH;

#pragma endregion ADLX_COLOR_DEPTH

#pragma region ADLX_PIXEL_FORMAT
/** @enum ADLX_PIXEL_FORMAT
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the pixel format to encode images for the display.
 * @ENG_END_DOX
 *
 */
typedef enum
{
    FORMAT_UNKNOWN = 0,/**< @ENG_START_DOX The pixel format is unknown. @ENG_END_DOX */
    RGB_444_FULL,      /**< @ENG_START_DOX The pixel format is RGB 4:4:4 PC Standard (Full RGB). @ENG_END_DOX */
    YCBCR_444,         /**< @ENG_START_DOX The pixel format is YCbCr 4:4:4. @ENG_END_DOX */
    YCBCR_422,         /**< @ENG_START_DOX The pixel format is YCbCr 4:2:2. @ENG_END_DOX */
    RGB_444_LIMITED,   /**< @ENG_START_DOX The pixel format is RGB 4:4:4 Studio (Limited RGB). @ENG_END_DOX */
    YCBCR_420          /**< @ENG_START_DOX The pixel format is YCbCr 4:2:0 Pixel Format. @ENG_END_DOX */
} ADLX_PIXEL_FORMAT;

#pragma endregion ADLX_PIXEL_FORMAT

#pragma region ADLX_TIMING_STANDARD

/** @enum ADLX_TIMING_STANDARD
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Timing standard for custom resolution
 * @ENG_END_DOX
 *
 */
typedef enum
{
    CVT = 0,               /**< @ENG_START_DOX Coordinated Video Timings(CVT) VESA standard for generating and defining the display timings. @ENG_END_DOX */
    CVT_RB,                /**< @ENG_START_DOX Coordinated Video Timings-Reduced Blanking(CVT-RB).Reduces horizontal and vertical blanking
                                periods and allows a lower pixel clock rate and higher frame rates. @ENG_END_DOX */
    GTF,                   /**< @ENG_START_DOX Generalized Timing Formula(GTF). A method of generating industry standard display timings. @ENG_END_DOX */
    DMT,                   /**< @ENG_START_DOX Display Monitor Timmings(DMT). VESA standard that lists pre-defined display timings for various resolutions and display sizes. @ENG_END_DOX */
    MANUAL                 /**< @ENG_START_DOX Manual control @ENG_END_DOX */
} ADLX_TIMING_STANDARD;

#pragma endregion ADLX_TIMING_STANDARD

#pragma region ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE
/** @enum ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the modes of VSync settings.
 * @ENG_END_DOX
 */
typedef enum
{
    WFVR_ALWAYS_OFF = 0,                       /**< @ENG_START_DOX VSync is always off. @ENG_END_DOX */
    WFVR_OFF_UNLESS_APP_SPECIFIES,               /**< @ENG_START_DOX VSync is off, unless specified by the application. @ENG_END_DOX */
    WFVR_ON_UNLESS_APP_SPECIFIES,                /**< @ENG_START_DOX VSync is on, unless specified by application. @ENG_END_DOX */
    WFVR_ALWAYS_ON,                            /**< @ENG_START_DOX VSync is always on. @ENG_END_DOX */
} ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE;
#pragma endregion ADLX_WAIT_FOR_VERTICAL_REFRESH_MODE

#pragma region ADLX_ANTI_ALIASING_MODE
/** @enum ADLX_ANTI_ALIASING_MODE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the anti-aliasing mode.
 * @ENG_END_DOX
 */
typedef enum
{
    AA_MODE_USE_APP_SETTINGS = 0,               /**< @ENG_START_DOX The anti-aliasing mode uses application settings. @ENG_END_DOX */
    AA_MODE_ENHANCE_APP_SETTINGS,               /**< @ENG_START_DOX The anti-aliasing mode enhances the application settings. @ENG_END_DOX */
    AA_MODE_OVERRIDE_APP_SETTINGS,              /**< @ENG_START_DOX The anti-aliasing mode overrides the application settings. @ENG_END_DOX */
} ADLX_ANTI_ALIASING_MODE;
#pragma endregion ADLX_ANTI_ALIASING_MODE

#pragma region ADLX_ANTI_ALIASING_LEVEL
/** @enum ADLX_ANTI_ALIASING_LEVEL
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the anti-aliasing level.
 * @ENG_END_DOX
 */
typedef enum
{
    AA_LEVEL_INVALID = 0,       /**< @ENG_START_DOX The anti-aliasing level is invalid. @ENG_END_DOX */
    AA_LEVEL_2X = 2,            /**< @ENG_START_DOX The anti-aliasing level is 2X. @ENG_END_DOX */
    AA_LEVEL_2XEQ = 3,          /**< @ENG_START_DOX The anti-aliasing level is 2XEQ. @ENG_END_DOX */
    AA_LEVEL_4X = 4,            /**< @ENG_START_DOX The anti-aliasing level is 4X. @ENG_END_DOX */
    AA_LEVEL_4XEQ = 5,          /**< @ENG_START_DOX The anti-aliasing level is 4XEQ. @ENG_END_DOX */
    AA_LEVEL_8X = 8,            /**< @ENG_START_DOX The anti-aliasing level is 8X. @ENG_END_DOX */
    AA_LEVEL_8XEQ = 9,          /**< @ENG_START_DOX The anti-aliasing level is 8XEQ. @ENG_END_DOX */
} ADLX_ANTI_ALIASING_LEVEL;
#pragma endregion ADLX_ANTI_ALIASING_LEVEL

#pragma region ADLX_ANTI_ALIASING_METHOD
/** @enum ADLX_ANTI_ALIASING_METHOD
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the anti-aliasing method.
 * @ENG_END_DOX
 */
typedef enum
{
    AA_METHOD_MULTISAMPLING = 0,            /**< @ENG_START_DOX The multi-sampling method. @ENG_END_DOX */
    AA_METHOD_ADAPTIVE_MULTISAMPLING,       /**< @ENG_START_DOX The adaptive multi-sampling method. @ENG_END_DOX */
    AA_METHOD_SUPERSAMPLING,                /**< @ENG_START_DOX The super-sampling method. @ENG_END_DOX */
} ADLX_ANTI_ALIASING_METHOD;
#pragma endregion ADLX_ANTI_ALIASING_METHOD

#pragma region ADLX_ANISOTROPIC_FILTERING_LEVEL
/** @enum ADLX_ANISOTROPIC_FILTERING_LEVEL
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the Anisotropic Filtering level.
 * @ENG_END_DOX
 */
typedef enum
{
    AF_LEVEL_INVALID = 0,    /**< @ENG_START_DOX The Anisotropic Filtering level is invalid. @ENG_END_DOX */
    AF_LEVEL_X2 = 2,         /**< @ENG_START_DOX The Anisotropic Filtering level is 2X. @ENG_END_DOX */
    AF_LEVEL_X4 = 4,         /**< @ENG_START_DOX The Anisotropic Filtering level is 4X. @ENG_END_DOX */
    AF_LEVEL_X8 = 8,         /**< @ENG_START_DOX The Anisotropic Filtering level is 8X. @ENG_END_DOX */
    AF_LEVEL_X16 = 16,       /**< @ENG_START_DOX The Anisotropic Filtering level is 16X. @ENG_END_DOX */
} ADLX_ANISOTROPIC_FILTERING_LEVEL;
#pragma endregion ADLX_ANISOTROPIC_FILTERING_LEVEL

#pragma region ADLX_TESSELLATION_MODE
/** @enum ADLX_TESSELLATION_MODE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the tessellation setting on a GPU.
 * @ENG_END_DOX
 */
typedef enum
{
    T_MODE_AMD_OPTIMIZED = 0,           /**< @ENG_START_DOX This mode uses AMD optimization. @ENG_END_DOX */
    T_MODE_USE_APP_SETTINGS,            /**< @ENG_START_DOX This mode uses application settings. @ENG_END_DOX */
    T_MODE_OVERRIDE_APP_SETTINGS,       /**< @ENG_START_DOX This mode uses override application settings. @ENG_END_DOX */
} ADLX_TESSELLATION_MODE;
#pragma endregion ADLX_TESSELLATION_MODE

#pragma region ADLX_TESSELLATION_LEVEL
/** @enum ADLX_TESSELLATION_LEVEL
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the tessellation level on a GPU.
 * @ENG_END_DOX
 */
typedef enum
{
    T_LEVEL_OFF = 1,        /**< @ENG_START_DOX The tessellation level is Off. @ENG_END_DOX */
    T_LEVEL_2X = 2,         /**< @ENG_START_DOX The tessellation level is 2X. @ENG_END_DOX */
    T_LEVEL_4X = 4,         /**< @ENG_START_DOX The tessellation level is 4X. @ENG_END_DOX */
    T_LEVEL_6X = 6,         /**< @ENG_START_DOX The tessellation level is 6X. @ENG_END_DOX */
    T_LEVEL_8X = 8,         /**< @ENG_START_DOX The tessellation level is 8X. @ENG_END_DOX */
    T_LEVEL_16X = 16,       /**< @ENG_START_DOX The tessellation level is 16X. @ENG_END_DOX */
    T_LEVEL_32X = 32,       /**< @ENG_START_DOX The tessellation level is 32X. @ENG_END_DOX */
    T_LEVEL_64X = 64,       /**< @ENG_START_DOX The tessellation level is 64X. @ENG_END_DOX */
} ADLX_TESSELLATION_LEVEL;
#pragma endregion ADLX_TESSELLATION_LEVEL

#pragma region ADLX_MEMORYTIMING_DESCRIPTION
/**
 *  @enum ADLX_MEMORYTIMING_DESCRIPTION
 *  @ingroup enumerations
 *  @ENG_START_DOX
 *  @brief Indicates the priority of the log entry.
 *  @ENG_END_DOX
 */
typedef enum
{
    MEMORYTIMING_DEFAULT = 0,               /**< @ENG_START_DOX The memory timing is default. @ENG_END_DOX */
    MEMORYTIMING_FAST_TIMING,                /**< @ENG_START_DOX The memory timing is fast timing. @ENG_END_DOX */
    MEMORYTIMING_FAST_TIMING_LEVEL_2,          /**< @ENG_START_DOX The memory timing is fast timing level 2. @ENG_END_DOX */
    MEMORYTIMING_AUTOMATIC,                 /**< @ENG_START_DOX The memory timing is automatic. @ENG_END_DOX */
    MEMORYTIMING_MEMORYTIMING_LEVEL_1,        /**< @ENG_START_DOX The memory timing is level 1. @ENG_END_DOX */
    MEMORYTIMING_MEMORYTIMING_LEVEL_2,        /**< @ENG_START_DOX The memory timing is level 2. @ENG_END_DOX */
} ADLX_MEMORYTIMING_DESCRIPTION;
#pragma endregion ADLX_MEMORYTIMING_DESCRIPTION


#pragma region ADLX_I2C_LINE
/** @enum ADLX_I2C_LINE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the I2C line options.
 * @ENG_END_DOX
 */
typedef enum
{
    I2C_LINE_OEM = 1,       /**< @ENG_START_DOX The I2C line is OEM. @ENG_END_DOX */
    I2C_LINE_OD_CONTROL,    /**< @ENG_START_DOX The I2C line is Over Driver Control. @ENG_END_DOX */
    I2C_LINE_OEM2,          /**< @ENG_START_DOX The I2C line is OEM2. @ENG_END_DOX */
    I2C_LINE_OEM3,          /**< @ENG_START_DOX The I2C line is OEM3. @ENG_END_DOX */
    I2C_LINE_OEM4,          /**< @ENG_START_DOX The I2C line is OEM4. @ENG_END_DOX */
    I2C_LINE_OEM5,          /**< @ENG_START_DOX The I2C line is OEM5. @ENG_END_DOX */
    I2C_LINE_OEM6           /**< @ENG_START_DOX The I2C line is OEM6. @ENG_END_DOX */
} ADLX_I2C_LINE;
#pragma endregion ADLX_I2C_LINE

#pragma region ADLX_SYNC_ORIGIN
/** @enum ADLX_SYNC_ORIGIN
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the origin of an event.
 * @ENG_END_DOX
*/
typedef enum
{
    SYNC_ORIGIN_UNKNOWN = 1,   /**< @ENG_START_DOX The event has an unknown trigger. @ENG_END_DOX */
    SYNC_ORIGIN_INTERNAL,      /**< @ENG_START_DOX The event is triggered by a change in settings using ADLX in this application. @ENG_END_DOX */
    SYNC_ORIGIN_EXTERNAL       /**< @ENG_START_DOX The event is triggered by a change in settings using ADLX in another application. @ENG_END_DOX */
} ADLX_SYNC_ORIGIN;
#pragma endregion ADLX_SYNC_ORIGIN

#pragma region ADLX_3DLUT_TRANSFER_FUNCTION
/** @enum ADLX_3DLUT_TRANSFER_FUNCTION
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the User 3D LUT transfer function.
 * @ENG_END_DOX
 */
 typedef enum
{
    TF_SRGB = 1,        /**< @ENG_START_DOX The transfer function is sRGB. @ENG_END_DOX */
    TF_PQ,              /**< @ENG_START_DOX The transfer function is PQ. @ENG_END_DOX */
    TF_G22              /**< @ENG_START_DOX The transfer function is G22. @ENG_END_DOX */
} ADLX_3DLUT_TRANSFER_FUNCTION;
#pragma endregion ADLX_3DLUT_TRANSFER_FUNCTION

#pragma region ADLX_3DLUT_COLORSPACE
/** @enum ADLX_3DLUT_COLORSPACE
 *  @ingroup enumerations
 * @ENG_START_DOX
 *  @brief Indicates the custom 3D LUT color space.
 * @ENG_END_DOX
 */
typedef enum
{
    CS_SRGB = 1,        /**< @ENG_START_DOX The color space is sRGB. @ENG_END_DOX */
    CS_BT2020,          /**< @ENG_START_DOX The color space is BT2020. @ENG_END_DOX */
    CS_DCIP3,           /**< @ENG_START_DOX The color space is DCIP3. @ENG_END_DOX */
    CS_NATIVE           /**< @ENG_START_DOX The color space is native. @ENG_END_DOX */
} ADLX_3DLUT_COLORSPACE;
#pragma endregion ADLX_3DLUT_COLORSPACE

#pragma endregion ADLX data types

//-------------------------------------------------------------------------------------------------
//definitions for IADLXInterface
#pragma region ADLX_DECLARE_IID
//------------------------------------------------------------------------
//IID's
#if defined(__cplusplus)
#define ADLX_DECLARE_IID(X) static ADLX_INLINE const wchar_t* IID()  { return X; }
#define ADLX_IS_IID(X, Y) (!wcscmp (X, Y))

#define ADLX_DECLARE_ITEM_IID(X) static ADLX_INLINE const wchar_t* ITEM_IID()  { return X; }
#else //__cplusplus
#define ADLX_IS_IID(X, Y) (!wcscmp (X, Y))
#define ADLX_DECLARE_IID(name,X) static const wchar_t* IID_##name(void)  { return X; }

#define ADLX_DECLARE_ITEM_IID(name,X) static const wchar_t* ITEM_IID_##name(void)  { return X; }
#endif //__cplusplus
#pragma endregion ADLX_DECLARE_IID

//------------------------------------------------------------------------
//All ref-counted interfaces derive from this interface
#pragma region IADLXInterface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXInterface")
        /**
        * @page DOX_IADLXInterface_Acquire Acquire
        * @ENG_START_DOX
        * @brief Increments the reference count for an ADLX interface.
        * @ENG_END_DOX
        * @syntax
        * @codeStart
        *  adlx_long    Acquire ()
        * @codeEnd
        *
        * @params
        * N/A
        *
        * @retvalues
        * @ENG_START_DOX
        * Returns the new reference count.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details __Acquire__ must be used when a copy of the interface pointer is needed.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call the __Acquire__ because smart pointers call it in their internal implementation.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_long         ADLX_STD_CALL Acquire () = 0;
        /**
         * @page DOX_IADLXInterface_Release Release
         * @ENG_START_DOX
         * @brief Decrements the reference count for an ADLX interface.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  adlx_long    Release ()
         * @codeEnd
         *
         * @params
         * N/A
         *
         * @retvalues
         * @ENG_START_DOX
         * Returns the new reference count.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __Release__ must be called when an interface is no longer needed. It decrements the reference counting and returns the new reference count. When the reference count reaches zero, the object behind this interface will self-destruct and free its memory and resources.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++, when using ADLX interfaces as smart pointers, there is no need to call the __Release__ because smart pointers call it in their internal implementation.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
         *
         */
        virtual adlx_long  ADLX_STD_CALL Release () = 0;

        /**
         * @page DOX_IADLXInterface_QueryInterface QueryInterface
         * @ENG_START_DOX
         * @brief Retrieves reference counted interfaces to an object.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    QueryInterface (const wchar_t* interfaceId, void** ppInterface)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,interfaceId,const wchar_t*,@ENG_START_DOX The identifier of the interface being requested. @ENG_END_DOX}
         * @paramrow{2.,[out],ppInterface,void**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppInterface__ to __nullptr__. @ENG_END_DOX}
         *
         *
         * @retvalues
         * @ENG_START_DOX
         * If __QueryInterface__ is successfully called, __ADLX_OK__ is returned. <br>
         * If the requested interface is not supported, __ADLX_UNKNOWN_INTERFACE__ is returned. <br>
         * If the __*ppInterface__ parameter is __nullptr__, __ADLX_INVALID_ARGS__ is returned. <br>
         * If __QueryInterface__ is not successfully called, an error code is returned. <br>
         * Refer @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details To ensure ADLX SDK backwards and forward compatibility the ADLX interfaces are locked, they do not change in subsequent versions of the driver. When an ADLX interface needs another functionality, an extension interface is provided. The extension interface is obtained from the initial interface using __QueryInterface__. The documentation for the extension interface shows how to obtain it.<br>
         * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when no longer needed.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call __QueryInterface__  and @ref DOX_IADLXInterface_Release because the smart pointer calls them internally.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include "ADLXDefines.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT       ADLX_STD_CALL QueryInterface (const wchar_t* interfaceId, void** ppInterface) = 0;
    };  //IADLXInterface
}   // namespace adlx
//----------------------------------------------------------------------------------------------
#else //__cplusplus
ADLX_DECLARE_IID (IADLXInterface, L"IADLXInterface")
typedef struct IADLXInterface IADLXInterface;

typedef struct IADLXInterfaceVtbl
{
    // IADLXInterface interface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXInterface* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXInterface* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXInterface* pThis, const wchar_t* interfaceId, void** ppInterface);
} IADLXInterfaceVtbl;

struct IADLXInterface
{
    const IADLXInterfaceVtbl *pVtbl;
};

#endif //__cplusplus
#pragma endregion IADLXInterface

//------------------------------------------------------------------------
//Template for ADLX smart pointer for interfaces derived from IADLXInterface
#pragma region IADLXInterfacePtr
#if defined(__cplusplus)
namespace adlx
{
    template<class _Interf>
    class IADLXInterfacePtr_T
    {
    private:
        _Interf* m_pInterf;

        void InternalAcquire ()
        {
            if (m_pInterf != nullptr)
            {
                m_pInterf->Acquire ();
            }
        }
        void InternalRelease ()
        {
            if (m_pInterf != nullptr)
            {
                m_pInterf->Release ();
            }
        }
    public:
        IADLXInterfacePtr_T () : m_pInterf (nullptr)
        {}

        IADLXInterfacePtr_T (const IADLXInterfacePtr_T<_Interf>& p) : m_pInterf (p.m_pInterf)
        {
            InternalAcquire ();
        }

        IADLXInterfacePtr_T (_Interf* pInterface) : m_pInterf (pInterface)
        {
            InternalAcquire ();
        }

        template<class _OtherInterf>
        explicit IADLXInterfacePtr_T (const IADLXInterfacePtr_T<_OtherInterf>& cp) : m_pInterf (nullptr)
        {
            void* pInterf = nullptr;
            if ((cp == NULL) || (cp->QueryInterface (_Interf::IID (), &pInterf) != ADLX_OK))
            {
                pInterf = nullptr;
            }
            m_pInterf = static_cast<_Interf*>(pInterf);
        }

        template<class _OtherInterf>
        explicit IADLXInterfacePtr_T (_OtherInterf* cp) : m_pInterf (nullptr)
        {
            void* pInterf = nullptr;
            if ((cp == nullptr) || (cp->QueryInterface (_Interf::IID (), &pInterf) != ADLX_OK))
            {
                pInterf = nullptr;
            }
            m_pInterf = static_cast<_Interf*>(pInterf);
        }

        ~IADLXInterfacePtr_T ()
        {
            InternalRelease ();
        }

        IADLXInterfacePtr_T& operator=(_Interf* pInterface)
        {
            if (m_pInterf != pInterface)
            {
                _Interf* pOldInterface = m_pInterf;
                m_pInterf = pInterface;
                InternalAcquire ();
                if (pOldInterface != nullptr)
                {
                    pOldInterface->Release ();
                }
            }
            return *this;
        }

        IADLXInterfacePtr_T& operator=(const IADLXInterfacePtr_T<_Interf>& cp)
        {
            return operator=(cp.m_pInterf);
        }

        void Attach (_Interf* pInterface)
        {
            InternalRelease ();
            m_pInterf = pInterface;
        }

        _Interf* Detach ()
        {
            _Interf* const pOld = m_pInterf;
            m_pInterf = NULL;
            return pOld;
        }
        void Release ()
        {
            InternalRelease ();
            m_pInterf = NULL;
        }

        operator _Interf*() const
        {
            return m_pInterf;
        }

        _Interf& operator*() const
        {
            return *m_pInterf;
        }

        // Returns the address of the interface pointer contained in this
        // class. This is required for initializing from C-style factory function to
        // avoid getting an incorrect ref count at the beginning.

        _Interf** operator&()
        {
            InternalRelease ();
            m_pInterf = 0;
            return &m_pInterf;
        }

        _Interf* operator->() const
        {
            return m_pInterf;
        }

        bool operator==(const IADLXInterfacePtr_T<_Interf>& p)
        {
            return (m_pInterf == p.m_pInterf);
        }

        bool operator==(_Interf* p)
        {
            return (m_pInterf == p);
        }

        bool operator!=(const IADLXInterfacePtr_T<_Interf>& p)
        {
            return !(operator==(p));
        }
        bool operator!=(_Interf* p)
        {
            return !(operator==(p));
        }

        _Interf* GetPtr ()
        {
            return m_pInterf;
        }

        const _Interf* GetPtr () const
        {
            return m_pInterf;
        }
    }; //IADLXInterfacePtr_T

    typedef IADLXInterfacePtr_T<IADLXInterface> IADLXInterfacePtr;
}   // namespace adlx
#endif //__cplusplus
#pragma endregion IADLXInterfacePtr

#endif //ADLX_DEFINES_H