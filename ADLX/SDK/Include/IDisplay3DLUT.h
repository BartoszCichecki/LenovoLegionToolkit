//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDISPLAY3DLUT_H
#define ADLX_IDISPLAY3DLUT_H
#pragma once

#include "ADLXDefines.h"
#include "ADLXStructures.h"
//-------------------------------------------------------------------------------------------------
//IDisplays3DLUT.h - Interfaces for ADLX Display 3D LUT functionality

#pragma region IADLXDisplay3DLUT interface
#if defined (__cplusplus)

namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplay3DLUT : public IADLXInterface
    {

    public:
        ADLX_DECLARE_IID (L"IADLXDisplay3DLUT")

        /**
        *@page DOX_IADLXDisplay3DLUT_IsSupportedSCE IsSupportedSCE
        *@ENG_START_DOX @brief Checks if color enhancement is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedSCE (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color enhancement is returned. The variable is __true__ if color enhancement is supported. The variable is __false__ if color enhancement is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color enhancement is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color enhancement is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Some GPUs support built-in 3D LUT profiles for displays to improve and enhance game and application color vibrancy. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedSCE (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsSupportedSCEVividGaming IsSupportedSCEVividGaming
        *@ENG_START_DOX @brief Checks if the vivid gaming color enhancement preset is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedSCEVividGaming (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the vivid gaming preset is returned. The variable is __true__ if the vivid gaming preset is supported. The variable is __false__ if the vivid gaming preset is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the vivid gaming preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of the vivid gaming preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The vivid gaming preset enhances color saturation and vibrance. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedSCEVividGaming (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsCurrentSCEDisabled IsCurrentSCEDisabled
        *@ENG_START_DOX @brief Checks if the color enhancement presets are disabled on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentSCEDisabled (adlx_bool* sceDisabled)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],sceDisabled,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color enhancement is returned. The variable is __true__ if the color enhancement presets are disabled. The variable is __false__ if the color enhancement presets are not disabled. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color enhancement is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color enhancement is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentSCEDisabled (adlx_bool* sceDisabled) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsCurrentSCEVividGaming IsCurrentSCEVividGaming
        *@ENG_START_DOX @brief Checks if the vivid gaming color enhancement preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentSCEVividGaming (adlx_bool* vividGaming)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],vividGaming,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the vivid gaming preset is returned. The variable is __true__ if the vivid gaming preset is used. The variable is __false__ if the vivid gaming preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the vivid gaming preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of the vivid gaming preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The vivid gaming preset enhances color saturation and vibrance. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentSCEVividGaming (adlx_bool* vividGaming) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetSCEDisabled SetSCEDisabled
        *@ENG_START_DOX @brief Disables the color enhancement presets on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSCEDisabled ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the color enhancement presets are successfully disabled, __ADLX_OK__ is returned. <br>
        * If the color enhancement presets are not successfully disabled, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetSCEDisabled () = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetSCEVividGaming SetSCEVividGaming
        *@ENG_START_DOX @brief Sets the vivid gaming color enhancement preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSCEVividGaming ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the vivid gaming preset is successfully set, __ADLX_OK__ is returned. <br>
        * If the vivid gaming preset is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The vivid gaming preset enhances color saturation and vibrance. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetSCEVividGaming () = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsSupportedSCEDynamicContrast IsSupportedSCEDynamicContrast
        *@ENG_START_DOX @brief Checks if the Dynamic Contrast color enhancement preset is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedSCEDynamicContrast (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the Dynamic Contrast preset is returned. The variable is __true__ if the Dynamic Contrast preset is supported. The variable is __false__ if the Dynamic Contrast preset is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the Dynamic Contrast preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of the Dynamic Contrast preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * The Dynamic Contrast preset is designed to boost brightness low and mid tone areas while leaving other areas almost untouched.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedSCEDynamicContrast (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsCurrentSCEDynamicContrast IsCurrentSCEDynamicContrast
        *@ENG_START_DOX @brief Checks if the Dynamic Contrast color enhancement preset is used on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentSCEDynamicContrast (adlx_bool* dynamicContrast)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],dynamicContrast,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the Dynamic Contrast preset is returned. The variable is __true__ if the Dynamic Contrast preset is used. The variable is __false__ if the Dynamic Contrast preset is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of the Dynamic Contrast preset is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of the Dynamic Contrast preset is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * The Dynamic Contrast preset is designed to boost brightness low and mid tone areas while leaving other areas almost untouched.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsCurrentSCEDynamicContrast (adlx_bool* dynamicContrast) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetSCEDynamicContrastRange GetSCEDynamicContrastRange
        *@ENG_START_DOX @brief Gets the maximum contrast, minimum contrast, and step contrast of the Dynamic Contrast color enhancement preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSCEDynamicContrastRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the contrast range of the Dynamic Contrast color enhancement preset is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the contrast range is successfully returned, __ADLX_OK__ is returned. <br>
        * If the contrast range is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum contrast, minimum contrast, and step contrast of the Dynamic Contrast color enhancement preset are read only. @ENG_END_DOX
        *
        * @addinfo
        * The Dynamic Contrast preset is designed to boost brightness low and mid tone areas while leaving other areas almost untouched.
        * @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSCEDynamicContrastRange (ADLX_IntRange* range) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetSCEDynamicContrast GetSCEDynamicContrast
        *@ENG_START_DOX @brief Gets the contrast of the Dynamic Contrast color enhancement preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSCEDynamicContrast (adlx_int* contrast)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],contrast,adlx_int*,@ENG_START_DOX The pointer to a variable where the contrast of the Dynamic Contrast color enhancement preset is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the contrast is successfully returned, __ADLX_OK__ is returned. <br>
        * If the contrast is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * The Dynamic Contrast preset is designed to boost brightness low and mid tone areas while leaving other areas almost untouched.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSCEDynamicContrast (adlx_int* contrast) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetSCEDynamicContrast SetSCEDynamicContrast
        *@ENG_START_DOX @brief Sets the contrast of the Dynamic Contrast color enhancement preset on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSCEDynamicContrast (adlx_int contrast)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],contrast,adlx_int,@ENG_START_DOX The new contrast of the Dynamic Contrast color enhancement preset. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the contrast is successfully set, __ADLX_OK__ is returned. <br>
        * If the contrast is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        * @addinfo
        * The Dynamic Contrast preset is designed to boost brightness low and mid tone areas while leaving other areas almost untouched.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetSCEDynamicContrast (adlx_int contrast) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_IsSupportedUser3DLUT IsSupportedUser3DLUT
        *@ENG_START_DOX @brief Checks if 3D LUT customization is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedUser3DLUT (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of 3D LUT customization is returned. The variable is __true__ if 3D LUT customization is supported. The variable is __false__ if 3D LUT customization is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of 3D LUT customization is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of 3D LUT customization is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Some GPUs support custom 3D LUT panel calibration for eDP displays to improve and enhance game and application color vibrancy. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupportedUser3DLUT (adlx_bool* supported) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_ClearUser3DLUT ClearUser3DLUT
        *@ENG_START_DOX @brief Clears the custom 3D LUT for panel calibration on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    ClearUser3DLUT ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the custom 3D LUT is successfully cleared, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT is not successfully cleared, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL ClearUser3DLUT () = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetSDRUser3DLUT GetSDRUser3DLUT
        *@ENG_START_DOX @brief Gets the custom 3D LUT data suitable for the SDR mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSDRUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, @ref ADLX_3DLUT_COLORSPACE* colorSpace, @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION*,@ENG_START_DOX The pointer to a variable where the transfer function is returned. @ENG_END_DOX}
        *@paramrow{2.,[out] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE*,@ENG_START_DOX The pointer to a variable where the color space is returned. @ENG_END_DOX}
        *@paramrow{3.,[out] ,pointsNumber,adlx_int*,@ENG_START_DOX The pointer to a variable where the size of the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *@paramrow{4.,[out] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The pointer to a variable where the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the custom 3D LUT data is successfully returned, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetSDRUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT SetSDRUser3DLUT
        *@ENG_START_DOX @brief Sets the custom 3D LUT data suitable for the SDR mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSDRUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, @ref ADLX_3DLUT_COLORSPACE colorSpace, const @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION,@ENG_START_DOX The transfer function. @ENG_END_DOX}
        *@paramrow{2.,[in] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE,@ENG_START_DOX The color space. @ENG_END_DOX}
        *@paramrow{3.,[in] ,pointsNumber,adlx_int,@ENG_START_DOX The size of the custom 3D LUT data. @ENG_END_DOX}
        *@paramrow{4.,[in] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The custom 3D LUT buffer. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom 3D LUT data is successfully set, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        *@details To fill in the custom 3D LUT buffer use @ref DOX_IADLXDisplay3DLUT_GetUser3DLUTIndex.
        *@ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetSDRUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetHDRUser3DLUT GetHDRUser3DLUT
        *@ENG_START_DOX @brief Gets the custom 3D LUT data suitable for the HDR mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetHDRUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, @ref ADLX_3DLUT_COLORSPACE* colorSpace, @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION*,@ENG_START_DOX The pointer to a variable where the transfer function is returned. @ENG_END_DOX}
        *@paramrow{2.,[out] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE*,@ENG_START_DOX The pointer to a variable where the color space is returned. @ENG_END_DOX}
        *@paramrow{3.,[out] ,pointsNumber,adlx_int*,@ENG_START_DOX The pointer to a variable where the size of the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *@paramrow{4.,[out] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The pointer to a variable where the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom 3D LUT data is successfully returned, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetHDRUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT SetHDRUser3DLUT
        *@ENG_START_DOX @brief Sets the custom 3D LUT data suitable for the HDR mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetHDRUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, @ref ADLX_3DLUT_COLORSPACE colorSpace, const @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION,@ENG_START_DOX The transfer function. @ENG_END_DOX}
        *@paramrow{2.,[in] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE,@ENG_START_DOX The color space. @ENG_END_DOX}
        *@paramrow{3.,[in] ,pointsNumber,adlx_int,@ENG_START_DOX The size of the custom 3D LUT data. @ENG_END_DOX}
        *@paramrow{4.,[in] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The custom 3D LUT buffer. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom 3D LUT data is successfully set, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        *@details To fill in the custom 3D LUT buffer use @ref DOX_IADLXDisplay3DLUT_GetUser3DLUTIndex.
        *@ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetHDRUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetAllUser3DLUT GetAllUser3DLUT
        *@ENG_START_DOX @brief Gets the custom 3D LUT data suitable for both the SDR mode and the HDR mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAllUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, @ref ADLX_3DLUT_COLORSPACE* colorSpace, @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION*,@ENG_START_DOX The pointer to a variable where the transfer function is returned. @ENG_END_DOX}
        *@paramrow{2.,[out] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE*,@ENG_START_DOX The pointer to a variable where the color space is returned. @ENG_END_DOX}
        *@paramrow{3.,[out] ,pointsNumber,adlx_int*,@ENG_START_DOX The pointer to a variable where the size of the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *@paramrow{4.,[out] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The pointer to a variable where the custom 3D LUT buffer is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the custom 3D LUT data is successfully returned, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetAllUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_SetAllUser3DLUT SetAllUser3DLUT
        *@ENG_START_DOX @brief Sets the custom 3D LUT data suitable for both the SDR mode and the HRD mode of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetAllUser3DLUT (@ref ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, @ref ADLX_3DLUT_COLORSPACE colorSpace, const @ref ADLX_3DLUT_Data* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,transferFunction,@ref ADLX_3DLUT_TRANSFER_FUNCTION,@ENG_START_DOX The transfer function. @ENG_END_DOX}
        *@paramrow{2.,[in] ,colorSpace,@ref ADLX_3DLUT_COLORSPACE,@ENG_START_DOX The color space. @ENG_END_DOX}
        *@paramrow{3.,[in] ,pointsNumber,adlx_int,@ENG_START_DOX The size of the custom 3D LUT data. @ENG_END_DOX}
        *@paramrow{4.,[in] ,data,@ref ADLX_3DLUT_Data*,@ENG_START_DOX The custom 3D LUT buffer. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the custom 3D LUT data is successfully set, __ADLX_OK__ is returned. <br>
        * If the custom 3D LUT data is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        *@details To fill in the custom 3D LUT buffer use @ref DOX_IADLXDisplay3DLUT_GetUser3DLUTIndex.
        *@ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetAllUser3DLUT (ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data) = 0;

        /**
        *@page DOX_IADLXDisplay3DLUT_GetUser3DLUTIndex GetUser3DLUTIndex
        *@ENG_START_DOX @brief Gets the index in the 3D LUT buffer corresponding to a triplet of red, green, and blue values. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetUser3DLUTIndex (adlx_int lutSize, const @ref ADLX_UINT16_RGB* rgbCoordinate, adlx_int* index)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,lutSize,adlx_int,@ENG_START_DOX The size of the User 3D LUT. @ENG_END_DOX}
        *@paramrow{2.,[in] ,rgbCoordinate,@ref ADLX_UINT16_RGB*,@ENG_START_DOX The co-ordinates of a user 3D LUT. @ENG_END_DOX}
        *@paramrow{3.,[out] ,index,adlx_int*,@ENG_START_DOX The pointer to a variable where the index is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the index of the 3D LUT from a flat array is successfully returned, __ADLX_OK__ is returned. <br>
        * If the index of the 3D LUT from a flat array is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        *@details The 3D LUT table must contain between 5 and 17 points. Each point is defined by a triplet of red, green, and blue values. <br>
        *
        * GetUser3DLUTIndex is useful when constructing the 3D LUT buffer to set using @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT, @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT and @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT.
        *
        *@ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT sets a custom 3D LUT data for both the SDR mode and HDR mode of a display.<br>
        *
        * Setting a custom 3D LUT data suitable for the SDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT will delete the custom 3D LUT data for the HDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the HDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT. <br>
        *
        * Setting a custom 3D LUT data suitable for the HDR mode of the display with @ref DOX_IADLXDisplay3DLUT_SetHDRUser3DLUT will delete the custom 3D LUT data for the SDR mode that was previously created with @ref DOX_IADLXDisplay3DLUT_SetAllUser3DLUT. If a custom 3D LUT data suitable for the SDR mode is also required, it must be set with @ref DOX_IADLXDisplay3DLUT_SetSDRUser3DLUT.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplay3DLUT.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetUser3DLUTIndex (adlx_int lutSize, const ADLX_UINT16_RGB* rgbCoordinate, adlx_int* index) = 0;

    };  //IADLXDisplay3DLUT
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplay3DLUT> IADLXDisplay3DLUTPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplay3DLUT, L"IADLXDisplay3DLUT")
typedef struct IADLXDisplay3DLUT IADLXDisplay3DLUT;

typedef struct IADLXDisplay3DLUTVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplay3DLUT* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplay3DLUT* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplay3DLUT* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDisplay3DLUT
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedSCE)(IADLXDisplay3DLUT* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedSCEVividGaming)(IADLXDisplay3DLUT* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentSCEDisabled)(IADLXDisplay3DLUT* pThis, adlx_bool* sceDisabled);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentSCEVividGaming)(IADLXDisplay3DLUT* pThis, adlx_bool* vividGaming);

    ADLX_RESULT (ADLX_STD_CALL *SetSCEDisabled)(IADLXDisplay3DLUT* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetSCEVividGaming)(IADLXDisplay3DLUT* pThis);

    ADLX_RESULT (ADLX_STD_CALL* IsSupportedSCEDynamicContrast)(IADLXDisplay3DLUT* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentSCEDynamicContrast)(IADLXDisplay3DLUT* pThis, adlx_bool* dynamicContrast);
    ADLX_RESULT (ADLX_STD_CALL* GetSCEDynamicContrastRange)(IADLXDisplay3DLUT* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL* GetSCEDynamicContrast)(IADLXDisplay3DLUT* pThis, adlx_int* contrast);
    ADLX_RESULT (ADLX_STD_CALL* SetSCEDynamicContrast)(IADLXDisplay3DLUT* pThis, adlx_int contrast);

    ADLX_RESULT (ADLX_STD_CALL *IsSupportedUser3DLUT)(IADLXDisplay3DLUT* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *ClearUser3DLUT)(IADLXDisplay3DLUT* pThis);
    ADLX_RESULT (ADLX_STD_CALL *GetSDRUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *SetSDRUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *GetHDRUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *SetHDRUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *GetAllUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION* transferFunction, ADLX_3DLUT_COLORSPACE* colorSpace, adlx_int* pointsNumber, ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *SetAllUser3DLUT)(IADLXDisplay3DLUT* pThis, ADLX_3DLUT_TRANSFER_FUNCTION transferFunction, ADLX_3DLUT_COLORSPACE colorSpace, adlx_int pointsNumber, const ADLX_3DLUT_Data* data);
    ADLX_RESULT (ADLX_STD_CALL *GetUser3DLUTIndex)(IADLXDisplay3DLUT* pThis, adlx_int lutSize, const ADLX_UINT16_RGB* rgbCoordinate, adlx_int* index);
}IADLXDisplay3DLUTVtbl;

struct IADLXDisplay3DLUT
{
    const IADLXDisplay3DLUTVtbl *pVtbl;
};
#endif
#pragma endregion IADLXDisplay3DLUT interface

#endif//ADLX_IDISPLAY3DLUT_H
