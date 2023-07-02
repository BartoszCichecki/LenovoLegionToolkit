// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDISPLAYGAMUT_H
#define ADLX_IDISPLAYGAMUT_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IDisplayGamut.h - Interfaces for ADLX Display Gamut functionality
#pragma region IADLXDisplayGamut interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGamut : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayGamut")

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCCIR709ColorSpace IsSupportedCCIR709ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CCIR709 is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCCIR709ColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR709 is returned. The variable is __true__ if the color space CCIR709 is supported.  The variable is __false__ if the color space CCIR709 is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR709 is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space CCIR709 is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The gamut color space CCIR709, also known as Rec.709, BT.709, and ITU 709, is a standard developed by ITU-R for image encoding and signal characteristics for high-definition television. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCCIR709ColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCCIR601ColorSpace IsSupportedCCIR601ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CCIR601 is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCCIR601ColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR601 is returned. The variable is __true__ if the color space CCIR601 is supported. The variable is __false__ if the color space CCIR601 is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR601 is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space CCIR601 is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The color space CCIR601 is a standard, originally issued in 1982 by the CCIR, for encoding interlaced analog video signals in digital video form and is also known as Rec.601 or BT.601. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCCIR601ColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedAdobeRgbColorSpace IsSupportedAdobeRgbColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space Adobe RGB is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedAdobeRgbColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space Adobe RGB is returned. The variable is __true__ if the color space Adobe RGB is supported. The variable is __false__ if the color space Adobe RGB is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space Adobe RGB is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space Adobe RGB is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The standard gamut Adobe RGB (1998) color space or opRGB was developed by Adobe Systems, Inc. in 1998. The color space was designed to encompass most of the colors achievable on CMYK color printers, but by using RGB primary colors on a device such as a computer display. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedAdobeRgbColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCIERgbColorSpace IsSupportedCIERgbColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CIERgb is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCIERgbColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *  @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CIERgb is returned. The variable is __true__ if the color space CIERgb is supported. The variable is __false__ if the color space CIERgb is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CIERgb is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space CIERgb is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The standard gamut CIERgb or standard RGB (red, green, blue) color space was co-operatively created by Microsoft and Hewlett-Packard (HP) in 1996 to be used on monitors, printers, and websites. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCIERgbColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCCIR2020ColorSpace IsSupportedCCIR2020ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space BT.2020 is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCCIR2020ColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *  @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR2020 is returned. The variable is __true__ if the color space CCIR2020 is supported. The variable is __false__ if the color space CCIR2020 is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR2020 is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space CCIR2020 is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details ITU-R Recommendation BT.2020, is commonly known by the abbreviations Rec.2020 or BT.2020. It defines various aspects of Ultra-High-Definition Television (UHDTV) with Standard Dynamic Range (SDR) and Wide Color Gamut (WCG), including picture resolutions, frame rates with progressive scan, bit depths, color primaries, RGB, and luma-chroma color representations, chroma subsampling, and opto-electronic transfer function. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCCIR2020ColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCustomColorSpace IsSupportedCustomColorSpace
        *@ENG_START_DOX @brief Checks if custom color space is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCustomColorSpace (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of custom color space is returned. The variable is __true__ if custom color space is supported. The variable is __false__ if custom color space is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of custom color space is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of custom color space is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The custom color space can be changed as required. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCustomColorSpace (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupported5000kWhitePoint IsSupported5000kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 5000k is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported5000kWhitePoint (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 5000k is returned. The variable is __true__ if the white point 5000k is supported. The variable is __false__ if the white point 5000k is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 5000k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 5000k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported5000kWhitePoint (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupported6500kWhitePoint IsSupported6500kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 6500k is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported6500kWhitePoint (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 6500k is returned. The variable is __true__ if the white point 6500k is supported. The variable is __false__ if the white point 6500k is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 6500k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 6500k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported6500kWhitePoint (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupported7500kWhitePoint IsSupported7500kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 7500k is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported7500kWhitePoint (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 7500k is returned. The variable is __true__ if the white point 7500k is supported. The variable is __false__ if the white point 7500k is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 7500k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 7500k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported7500kWhitePoint (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupported9300kWhitePoint IsSupported9300kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 9300k is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported9300kWhitePoint (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 9300k is returned. The variable is __true__ if the white point 9300k is supported. The variable is __false__ if the white point 9300k is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 9300k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 9300k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported9300kWhitePoint (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsSupportedCustomWhitePoint IsSupportedCustomWhitePoint
        *@ENG_START_DOX @brief Checks if custom white point is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCustomWhitePoint (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of custom white point is returned.The variable is __true__ if custom white point is supported. The variable is __false__ if custom white point is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of custom white point is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of custom white point is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCustomWhitePoint (adlx_bool* supported) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrent5000kWhitePoint IsCurrent5000kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 5000k is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrent5000kWhitePoint (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 5000k is returned. The variable is __true__ if white point 5000k is used. The variable is __false__ if white point 5000k is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 5000k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 5000k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrent5000kWhitePoint (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrent6500kWhitePoint IsCurrent6500kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 6500k is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrent6500kWhitePoint (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 6500k is returned.The variable is __true__ if white point 6500k is used. The variable is __false__ if white point 6500k is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 6500k is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of white point 6500k is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrent6500kWhitePoint (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrent7500kWhitePoint IsCurrent7500kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 7500k is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrent7500kWhitePoint (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 7500k is returned. The variable is __true__ if white point 7500k is used. The variable is __false__ if white point 7500k is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 7500k is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of white point 7500k is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrent7500kWhitePoint (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrent9300kWhitePoint IsCurrent9300kWhitePoint
        *@ENG_START_DOX @brief Checks if the standard white point 9300k is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrent9300kWhitePoint (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of white point 9300k is returned. The variable is __true__ if white point 9300k is used. The variable is __false__ if white point 9300k is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of white point 9300k is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of white point 9300k is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrent9300kWhitePoint (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCustomWhitePoint IsCurrentCustomWhitePoint
        *@ENG_START_DOX @brief Checks if a custom white point is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCustomWhitePoint (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of custom white point is returned. The variable is __true__ if a custom white point is used. The variable is __false__ if a custom white point is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of custom white point is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of custom white point is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCustomWhitePoint (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_GetWhitePoint GetWhitePoint
        *@ENG_START_DOX @brief Gets the white point coordinates (x, y) of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetWhitePoint (@ref ADLX_Point* point)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,point,@ref ADLX_Point* ,@ENG_START_DOX The pointer to a variable where the white point coordinates are returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the white point coordinates are successfully returned, __ADLX_OK__ is returned.<br>
        * If the white point coordinates are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetWhitePoint (ADLX_Point* point) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCCIR709ColorSpace IsCurrentCCIR709ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CCIR709 is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCCIR709ColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR709 is returned. The variable is __true__ if color space CCIR709 is used. The variable is __false__ if color space CCIR709 is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR709 is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of color space CCIR709 is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCCIR709ColorSpace (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCCIR601ColorSpace IsCurrentCCIR601ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CCIR601 is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCCIR601ColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR601 is returned. The variable is __true__ if color space CCIR601 is used. The variable is __false__ if color space CCIR601 is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR601 is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of color space CCIR601 is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCCIR601ColorSpace (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentAdobeRgbColorSpace IsCurrentAdobeRgbColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space Adobe RGB is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentAdobeRgbColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space Adobe RGB is returned. The variable is __true__ if color space Adobe RGB is used. The variable is __false__ if color space Adobe RGB is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space Adobe RGB is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of color space Adobe RGB is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentAdobeRgbColorSpace (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCIERgbColorSpace IsCurrentCIERgbColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space sRGB is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCIERgbColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space sRGB is returned. The variable is __true__ if color space sRGB is used. The variable is __false__ if color space sRGB is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space sRGB is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space sRGB is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCIERgbColorSpace (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCCIR2020ColorSpace IsCurrentCCIR2020ColorSpace
        *@ENG_START_DOX @brief Checks if the standard color space CCIR2020 is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCCIR2020ColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of color space CCIR709 is returned. The variable is __true__ if color space CCIR2020 is used. The variable is __false__ if color space CCIR2020 is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of color space CCIR709 is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of color space CCIR709 is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCCIR2020ColorSpace (adlx_bool* isSet) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_IsCurrentCustomColorSpace IsCurrentCustomColorSpace
        *@ENG_START_DOX @brief Checks if a custom color space is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentCustomColorSpace (adlx_bool* isSet)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],isSet,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of custom color space is returned. The variable is __true__ if a custom color space is used. The variable is __false__ if a custom color space is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of custom color space is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state of custom color space is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details For more information on color coordinates, refer to @ref ADLX_GamutColorSpace. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentCustomColorSpace (adlx_bool* isSet) const = 0;

        /**
       *@page DOX_IADLXDisplayGamut_GetGamutColorSpace GetGamutColorSpace
       *@ENG_START_DOX @brief Gets the color space coordinates of a display. @ENG_END_DOX
       *
       *@syntax
       *@codeStart
       * @ref ADLX_RESULT    GetGamutColorSpace (@ref ADLX_GamutColorSpace* gamutColorSpace)
       *@codeEnd
       *
       * @params
       * @paramrow{1.,[out] ,gamutColorSpace,@ref ADLX_GamutColorSpace* ,@ENG_START_DOX The pointer to a variable where the gamut color space are returned. @ENG_END_DOX}
       *
       *@retvalues
       *@ENG_START_DOX  If the color space coordinates are successfully returned, __ADLX_OK__ is returned.<br>
       * If the color space coordinates are not successfully returned, an error code is returned.<br>
       * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
       *
       *@detaileddesc
       *@ENG_START_DOX @details The gamut color space coordinates consist of (y, x) chromaticity coordinates for the red, green, and blue channels. @ENG_END_DOX
       *
       *@requirements
       *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
       *
       */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGamutColorSpace (ADLX_GamutColorSpace* gamutColorSpace) const = 0;

        /**
        *@page DOX_IADLXDisplayGamut_SetGamut_PW_PG SetGamut
        *@ENG_START_DOX @brief Sets a predefined white point for a predefined color space on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGamut (@ref ADLX_WHITE_POINT predefinedWhitePoint, @ref ADLX_GAMUT_SPACE predefinedGamutSpace)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,predefinedWhitePoint,@ref ADLX_WHITE_POINT ,@ENG_START_DOX The predefined white point. @ENG_END_DOX}
        * @paramrow{2.,[in] ,predefinedGamutSpace,@ref ADLX_GAMUT_SPACE ,@ENG_START_DOX The predefined color space. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the gamut settings are successfully set, __ADLX_OK__ is returned.<br>
        * If the gamut settings are not successfully set, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details For more information on white point coordinates, Refer to @ref ADLX_Point. <br>
        * For more information on color space coordinates, refer to @ref ADLX_GamutColorSpace. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGamut (ADLX_WHITE_POINT predefinedWhitePoint, ADLX_GAMUT_SPACE predefinedGamutSpace) = 0;

        /**
        *@page DOX_IADLXDisplayGamut_SetGamut_CW_PG SetGamut
        *@ENG_START_DOX @brief Sets a custom white point for a predefined color space on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGamut (@ref ADLX_RGB customWhitePoint, @ref ADLX_GAMUT_SPACE predefinedGamutSpace)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,customWhitePoint,@ref ADLX_RGB ,@ENG_START_DOX The custom white point. @ENG_END_DOX}
        * @paramrow{2.,[in] ,predefinedGamutSpace,@ref ADLX_GAMUT_SPACE ,@ENG_START_DOX The predefined color space. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the gamut settings are successfully set, __ADLX_OK__ is returned.<br>
        * If the gamut settings are not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details For more information on white point coordinates, refer to @ref ADLX_Point. <br>
        * For more information on color space coordinates, refer to @ref ADLX_GamutColorSpace. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGamut (ADLX_RGB customWhitePoint, ADLX_GAMUT_SPACE predefinedGamutSpace) = 0;

        /**
        *@page DOX_IADLXDisplayGamut_SetGamut_PW_CG SetGamut
        *@ENG_START_DOX @brief Sets a predefined white point for a custom color space on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGamut (@ref ADLX_WHITE_POINT predefinedWhitePoint, @ref ADLX_GamutColorSpace customGamut)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,predefinedWhitePoint,@ref ADLX_WHITE_POINT ,@ENG_START_DOX The predefined white point. @ENG_END_DOX}
        * @paramrow{2.,[in] ,customGamut,@ref ADLX_GamutColorSpace ,@ENG_START_DOX The custom color space. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the gamut settings are successfully set, __ADLX_OK__ is returned. <br>
        * If the gamut settings are not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details For more information on white point coordinates, refer to @ref ADLX_Point. <br>
        * For more information on color space coordinates, refer to @ref ADLX_GamutColorSpace. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGamut (ADLX_WHITE_POINT predefinedWhitePoint, ADLX_GamutColorSpace customGamut) = 0;

        /**
        *@page DOX_IADLXDisplayGamut_SetGamut_CW_CG SetGamut
        *@ENG_START_DOX @brief Sets a custom white point, for a custom color space on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetGamut (@ref ADLX_RGB customWhitePoint, @ref ADLX_GamutColorSpace customGamut)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,customWhitePoint,@ref ADLX_RGB ,@ENG_START_DOX The custom white point. @ENG_END_DOX}
        * @paramrow{2.,[in] ,customGamut,@ref ADLX_GamutColorSpace ,@ENG_START_DOX The custom color space. @ENG_END_DOX}
        *
        * @retvalues
        *@ENG_START_DOX  If the gamut settings are successfully set, __ADLX_OK__ is returned. <br>
        * If the gamut settings are not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details For more information on white point coordinates, Refer to @ref ADLX_Point. <br>
        * For more information on color space coordinates, refer to @ref ADLX_GamutColorSpace. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamut.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL SetGamut (ADLX_RGB customWhitePoint, ADLX_GamutColorSpace customGamut) = 0;
    };  //IADLXDisplayGamut
    //----------------------------------------------------------------------------------------------    
    typedef IADLXInterfacePtr_T<IADLXDisplayGamut> IADLXDisplayGamutPtr;
}
#else

ADLX_DECLARE_IID (IADLXDisplayGamut, L"IADLXDisplayGamut")
typedef struct IADLXDisplayGamut IADLXDisplayGamut;

typedef struct IADLXDisplayGamutVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)(IADLXDisplayGamut* pThis);
    adlx_long (ADLX_STD_CALL* Release)(IADLXDisplayGamut* pThis);
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)(IADLXDisplayGamut* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDisplayGamut
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCCIR709ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCCIR601ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedAdobeRgbColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCIERgbColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCCIR2020ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCustomColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL* IsSupported5000kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupported6500kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupported7500kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupported9300kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL* IsSupportedCustomWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* supported);

    ADLX_RESULT (ADLX_STD_CALL* IsCurrent5000kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrent6500kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrent7500kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrent9300kWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCustomWhitePoint)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* GetWhitePoint)(IADLXDisplayGamut* pThis, ADLX_Point* point);

    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCCIR709ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCCIR601ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentAdobeRgbColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCIERgbColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCCIR2020ColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* IsCurrentCustomColorSpace)(IADLXDisplayGamut* pThis, adlx_bool* isSet);
    ADLX_RESULT (ADLX_STD_CALL* GetGamutColorSpace)(IADLXDisplayGamut* pThis, ADLX_GamutColorSpace* gamutColorSpace);

    ADLX_RESULT (ADLX_STD_CALL* SetGamut_CW_CG)(IADLXDisplayGamut* pThis, ADLX_RGB customWhitePoint, ADLX_GamutColorSpace customGamut);
    ADLX_RESULT (ADLX_STD_CALL* SetGamut_PW_CG)(IADLXDisplayGamut* pThis, ADLX_WHITE_POINT predefinedWhitePoint, ADLX_GamutColorSpace customGamut);
    ADLX_RESULT (ADLX_STD_CALL* SetGamut_CW_PG)(IADLXDisplayGamut* pThis, ADLX_RGB customWhitePoint, ADLX_GAMUT_SPACE predefinedGamutSpace);
    ADLX_RESULT (ADLX_STD_CALL* SetGamut_PW_PG)(IADLXDisplayGamut* pThis, ADLX_WHITE_POINT predefinedWhitePoint, ADLX_GAMUT_SPACE predefinedGamutSpace);

} IADLXDisplayGamutVtbl;

struct IADLXDisplayGamut
{
    const IADLXDisplayGamutVtbl* pVtbl;
};
#endif
#pragma endregion IADLXDisplayGamut interface

#endif//ADLX_IDISPLAYGAMUT_H
