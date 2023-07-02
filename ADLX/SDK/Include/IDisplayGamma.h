//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDISPLAYGAMMA_H
#define ADLX_IDISPLAYGAMMA_H
#pragma once

#include "ADLXStructures.h"

//-------------------------------------------------------------------------------------------------
//IDisplayGamma.h - Interfaces for ADLX Display Gamma functionality

//The DisplayGamma object implements this interface
#pragma region IADLXDisplayGamma interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGamma : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayGamma")

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGammaRamp IsCurrentReGammaRamp
        *@ENG_START_DOX @brief Checks if re-gamma ramp is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGammaRamp (adlx_bool* isReGammaRamp)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsReGammaRamp,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of re-gamma ramp is returned. The variable is __true__ if re-gamma ramp is used. The variable is __false__ if re-gamma ramp is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of re-gamma ramp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of re-gamma ramp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGammaRamp (adlx_bool* isReGammaRamp) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentDeGammaRamp IsCurrentDeGammaRamp
        *@ENG_START_DOX @brief Checks if de-gamma ramp is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentDeGammaRamp (adlx_bool* isDeGammaRamp)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsDeGammaRamp,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of de-gamma ramp is returned. The variable is __true__ if de-gamma ramp is used. The variable is __false__ if de-gamma ramp is not used.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of de-gamma ramp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of de-gamma ramp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentDeGammaRamp (adlx_bool* isDeGammaRamp) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentRegammaCoefficient IsCurrentRegammaCoefficient
        *@ENG_START_DOX @brief Checks if re-gamma coefficient is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentRegammaCoefficient (adlx_bool* isRegammaCoeff)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsRegammaCoeff,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of re-gamma coefficient is returned. The variable is __true__ if re-gamma coefficient is used. The variable is __false__ if re-gamma coefficient is not used.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of re-gamma coefficient is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of re-gamma coefficient is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentRegammaCoefficient (adlx_bool* isRegammaCoeff) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_GetGammaRamp GetGammaRamp
        *@ENG_START_DOX @brief Gets the gamma ramp LUT on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGammaRamp (@ref ADLX_GammaRamp* lut)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,lut,@ref ADLX_GammaRamp*,@ENG_START_DOX The pointer to a variable where the gamma ramp LUT is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the gamma ramp LUT is successfully returned, __ADLX_OK__ is returned. <br>
        * If the gamma ramp LUT is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGammaRamp (ADLX_GammaRamp* lut) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_GetGammaCoefficient GetGammaCoefficient
        *@ENG_START_DOX @brief Gets the re-gamma coefficient settings on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGammaCoefficient (@ref ADLX_RegammaCoeff* coeff)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,coeff,@ref ADLX_RegammaCoeff* ,@ENG_START_DOX The pointer to a variable where the re-gamma coefficient settings are returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the re-gamma coefficient settings are successfully returned, __ADLX_OK__ is returned. <br>
        * If the re-gamma coefficient settings are not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGammaCoefficient (ADLX_RegammaCoeff* coeff) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsSupportedReGammaSRGB IsSupportedReGammaSRGB
        *@ENG_START_DOX @brief Checks if the sRGB re-gamma is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedReGammaSRGB (adlx_bool* isSupportedRegammaSRGB)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,isSupportedRegammaSRGB,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of sRGB re-gamma is returned. The variable is __true__ if sRGB re-gamma is supported. The variable is __false__ if sRGB re-gamma is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of sRGB re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of sRGB re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedReGammaSRGB (adlx_bool* isSupportedRegammaSRGB) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsSupportedReGammaBT709 IsSupportedReGammaBT709
        *@ENG_START_DOX @brief Checks if the BT709 re-gamma is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedReGammaBT709 (adlx_bool* isSupportedReGammaBT709)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsSupportedReGammaBT709,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of BT709 re-gamma is returned. The variable is __true__ if BT709 re-gamma is supported. The variable is __false__ if BT709 re-gamma is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of BT709 re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of BT709 re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedReGammaBT709 (adlx_bool* isSupportedReGammaBT709) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsSupportedReGammaPQ IsSupportedReGammaPQ
        *@ENG_START_DOX @brief Checks if the PQ re-gamma is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedReGammaPQ (adlx_bool* isSupportedReGammaPQ)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsSupportedReGammaPQ,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of PQ re-gamma is returned. The variable is __true__ if PQ re-gamma is supported. The variable is __false__ if PQ re-gamma is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of PQ re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of PQ re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedReGammaPQ (adlx_bool* isSupportedReGammaPQ) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsSupportedReGammaPQ2084Interim IsSupportedReGammaPQ2084Interim
        *@ENG_START_DOX @brief Checks if the PQ2084 re-gamma curve is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedReGammaPQ2084Interim (adlx_bool* isSupportedReGammaPQ2084Interim);
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsSupportedReGammaPQ2084Interim,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of PQ2084 re-gamma curve is returned. The variable is __true__ if PQ2084 re-gamma curve is supported. The variable is __false__ if PQ2084 re-gamma curve is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of PQ2084 re-gamma curve is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of PQ2084 re-gamma curve is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedReGammaPQ2084Interim (adlx_bool* isSupportedReGammaPQ2084Interim) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsSupportedReGamma36 IsSupportedReGamma36
        *@ENG_START_DOX @brief Checks if the 3.6 re-gamma is supported on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedReGamma36 (adlx_bool* isSupportedReGamma36)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsSupportedReGamma36,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of 3.6 re-gamma is returned. The variable is __true__ if 3.6 re-gamma is supported. The variable is __false__ if 3.6 re-gamma is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of 3.6 re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of 3.6 re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedReGamma36 (adlx_bool* isSupportedReGamma36) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGammaSRGB IsCurrentReGammaSRGB
        *@ENG_START_DOX @brief Checks if the sRGB re-gamma is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGammaSRGB (adlx_bool* isCurrentReGammaSRGB)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,IsCurrentReGammaSRGB,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of sRGB re-gamma is returned. The variable is __true__ if sRGB re-gamma is used. The variable is __false__ if sRGB re-gamma is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of sRGB re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of sRGB re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGammaSRGB (adlx_bool* isCurrentReGammaSRGB) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGammaBT709 IsCurrentReGammaBT709
        *@ENG_START_DOX @brief Checks if the BT709 re-gamma is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGammaBT709 (adlx_bool* isCurrentReGammaBT709)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],IsCurrentReGammaBT709,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of BT709 re-gamma is returned. The variable is __true__ if BT709 re-gamma is used. The variable is __false__ if BT709 re-gamma is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of BT709 re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of BT709 re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGammaBT709 (adlx_bool* isCurrentReGammaBT709) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGammaPQ IsCurrentReGammaPQ
        *@ENG_START_DOX @brief Checks if the PQ re-gamma is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGammaPQ (adlx_bool* isCurrentReGammaPQ)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],IsCurrentReGammaPQ,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of PQ re-gamma is returned. The variable is __true__ if PQ re-gamma is used. The variable is __false__ if PQ re-gamma is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of PQ re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of PQ re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGammaPQ (adlx_bool* isCurrentReGammaPQ) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGammaPQ2084Interim IsCurrentReGammaPQ2084Interim
        *@ENG_START_DOX @brief Checks if the PQ2084 re-gamma curve is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGammaPQ2084Interim (adlx_bool* isCurrentReGammaPQ2084Interim)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],IsCurrentReGammaPQ2084Interim,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of PQ2084 re-gamma curve is returned. The variable is __true__ if PQ2084 re-gamma curve is used. The variable is __false__ if PQ2084 re-gamma curve is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of PQ2084 re-gamma curve is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of PQ2084 re-gamma curve is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGammaPQ2084Interim (adlx_bool* isCurrentReGammaPQ2084Interim) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_IsCurrentReGamma36 IsCurrentReGamma36
        *@ENG_START_DOX @brief Checks if the 3.6 re-gamma is used by a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsCurrentReGamma36 (adlx_bool* isCurrentReGamma36)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],IsCurrentReGamma36,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of 3.6 re-gamma is returned. The variable is __true__ if 3.6 re-gamma is used. The variable is __false__ if 3.6 re-gamma is not used. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state of 3.6 re-gamma is successfully returned, __ADLX_OK__ is returned. <br>
        * If the state of 3.6 re-gamma is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsCurrentReGamma36 (adlx_bool* isCurrentReGamma36) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaSRGB SetReGammaSRGB
        *@ENG_START_DOX @brief Sets the sRGB re-gamma on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaSRGB ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the sRGB re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the sRGB re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaSRGB () = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaBT709 SetReGammaBT709
        *@ENG_START_DOX @brief Sets the BT709 re-gamma on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaBT709 ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the BT709 re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the BT709 re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaBT709 () = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaPQ SetReGammaPQ
        *@ENG_START_DOX @brief Sets the PQ re-gamma on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaPQ ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the PQ re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the PQ re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaPQ () = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaPQ2084Interim SetReGammaPQ2084Interim
        *@ENG_START_DOX @brief Sets the PQ2084 re-gamma curve on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaPQ2084Interim ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the PQ2084 re-gamma curve is successfully set, __ADLX_OK__ is returned. <br>
        * If the PQ2084 re-gamma curve is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaPQ2084Interim () = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGamma36 SetReGamma36
        *@ENG_START_DOX @brief Sets the 3.6 re-gamma on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGamma36 ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the 3.6 re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the 3.6 re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGamma36 () = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaCoefficient SetReGammaCoefficient
        *@ENG_START_DOX @brief Sets the re-gamma coefficient on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaCoefficient (@ref ADLX_RegammaCoeff coeff)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,coeff,@ref ADLX_RegammaCoeff ,@ENG_START_DOX The re-gamma coefficient. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the re-gamma coefficient is successfully set, __ADLX_OK__ is returned. <br>
        * If the re-gamma coefficient is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method sets the custom coefficient on a display.
        * The following is an example of a standard coefficient. @ENG_END_DOX
        *   coeff     | sRGB   | BT709    | Gamma2.2  | Gamma2.4  |P3     |
        * :---            | :---:  | :---:    | :---:  | :---:  |:---:     |
        * CoefficientA0  | 31308  | 180000   | 0         | 0         | 0     |
        * CoefficientA1  | 12920  | 4500     | 0         | 0         | 0     |
        * CoefficientA2  | 55     | 99       | 0         | 0         | 0     |
        * CoefficientA3  | 55     | 99       | 0         | 0         | 0     |
        * Gamma          | 2400   | 2200     | 2200      | 2400      | 2600  |
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaCoefficient (ADLX_RegammaCoeff coeff) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetDeGammaRampFile SetDeGammaRamp
        *@ENG_START_DOX @brief Sets the de-gamma on a display using a ramp file. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetDeGammaRamp (const char* path)
        *@codeEnd
        *
        * @params
		* @paramrow{1.,[in] ,pPath,const char* ,@ENG_START_DOX The zero-terminated string that specifies the path of the ramp file. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the de-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the de-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method sets the custom de-gamma on a display using a ramp file.
        * The following is an example of a ramp file. @ENG_END_DOX
        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        *       [R]            [G]          [B]
        * [0]    0,             0,           0
        * [1]   49836,         34885,        0
        * ...
        * [254] 58260,         58782,        0
        * [255] 65533,         56634,        0
        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetDeGammaRamp (const char* path) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetDeGammaRampMemory SetDeGammaRamp
        *@ENG_START_DOX @brief Sets the de-gamma on a display using a ramp buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetDeGammaRamp (@ref ADLX_GammaRamp gammaRamp)
        *@codeEnd
        *
        * @params
		* @paramrow{1.,[in] ,gammaRamp,@ref ADLX_GammaRamp ,@ENG_START_DOX The gamma ramp buffer. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the de-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the de-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetDeGammaRamp (ADLX_GammaRamp gammaRamp) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaRampFile SetReGammaRamp
        *@ENG_START_DOX @brief Sets the re-gamma on a display using a ramp file. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaRamp (const char* path)
        *@codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,pPath,const char* ,@ENG_START_DOX The zero-terminated string that specifies the path of the ramp file. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The method sets the custom re-gamma on a display using a ramp file.
        * The following is an example of a LUT file format, each acceptable value is a number between 0 and 65535. @ENG_END_DOX
        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        *       [R]            [G]          [B]
        * [0]    0,             0,           0
        * [1]   49836,         34885,        0
        * ...
        * [254] 58260,         58782,        0
        * [255] 65533,         56634,        0
        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaRamp (const char* path) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_SetReGammaRampMemory SetReGammaRamp
        *@ENG_START_DOX @brief Sets the re-gamma on a display using a ramp buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetReGammaRamp (@ref ADLX_GammaRamp gammaRamp)
        *@codeEnd
        *
        * @params
		* @paramrow{1.,[in] ,gammaRamp,@ref ADLX_GammaRamp ,@ENG_START_DOX The gamma ramp buffer. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the re-gamma is successfully set, __ADLX_OK__ is returned. <br>
        * If the re-gamma is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetReGammaRamp (ADLX_GammaRamp gammaRamp) = 0;

        /**
        *@page DOX_IADLXDisplayGamma_ResetGammaRamp ResetGammaRamp
        *@ENG_START_DOX @brief Resets the gamma ramp on a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    ResetGammaRamp ()
        *@codeEnd
        *
        * @params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the gamma ramp is successfully reset, __ADLX_OK__ is returned. <br>
        * If the gamma ramp is not successfully reset, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplayGamma.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL ResetGammaRamp () = 0;
    };  //IADLXDisplayGamma
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayGamma> IADLXDisplayGammaPtr;
}
#else
ADLX_DECLARE_IID (IADLXDisplayGamma, L"IADLXDisplayGamma")
typedef struct IADLXDisplayGamma IADLXDisplayGamma;

typedef struct IADLXDisplayGammaVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDisplayGamma* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDisplayGamma* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDisplayGamma* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDisplayGamma
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGammaRamp)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGammaRamp);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentDeGammaRamp)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentDeGammaRamp);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentRegammaCoefficient)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentRegammaCoefficient);
    ADLX_RESULT (ADLX_STD_CALL *GetGammaRamp)(IADLXDisplayGamma* pThis, ADLX_GammaRamp* lut);
    ADLX_RESULT (ADLX_STD_CALL *GetGammaCoefficient)(IADLXDisplayGamma* pThis, ADLX_RegammaCoeff* coeff);

    ADLX_RESULT (ADLX_STD_CALL *IsSupportedReGammaSRGB)(IADLXDisplayGamma* pThis, adlx_bool* isSupportedReGammaSRGB);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedReGammaBT709)(IADLXDisplayGamma* pThis, adlx_bool* isSupportedReGammaBT709);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedReGammaPQ)(IADLXDisplayGamma* pThis, adlx_bool* isSupportedReGammaPQ);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedReGammaPQ2084Interim)(IADLXDisplayGamma* pThis, adlx_bool* isSupportedReGammaPQ2084Interim);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedReGamma36)(IADLXDisplayGamma* pThis, adlx_bool* isSupportedReGamma36);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGammaSRGB)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGammaSRGB);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGammaBT709)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGammaBT709);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGammaPQ)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGammaPQ);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGammaPQ2084Interim)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGammaPQ2084Interim);
    ADLX_RESULT (ADLX_STD_CALL *IsCurrentReGamma36)(IADLXDisplayGamma* pThis, adlx_bool* isCurrentReGamma36);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaSRGB)(IADLXDisplayGamma* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaBT709)(IADLXDisplayGamma* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaPQ)(IADLXDisplayGamma* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaPQ2084Interim)(IADLXDisplayGamma* pThis);
    ADLX_RESULT (ADLX_STD_CALL *SetReGamma36)(IADLXDisplayGamma* pThis);

    ADLX_RESULT (ADLX_STD_CALL *SetReGammaCoefficient)(IADLXDisplayGamma* pThis, ADLX_RegammaCoeff coeff);
    ADLX_RESULT (ADLX_STD_CALL *SetDeGammaRamp_Memory)(IADLXDisplayGamma* pThis, ADLX_GammaRamp gammaRamp);
    ADLX_RESULT (ADLX_STD_CALL *SetDeGammaRamp_File)(IADLXDisplayGamma* pThis, const char* path);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaRamp_Memory)(IADLXDisplayGamma* pThis, ADLX_GammaRamp gammaRamp);
    ADLX_RESULT (ADLX_STD_CALL *SetReGammaRamp_File)(IADLXDisplayGamma* pThis, const char* path);
    ADLX_RESULT (ADLX_STD_CALL *ResetGammaRamp)(IADLXDisplayGamma* pThis);
} IADLXDisplayGammaVtbl;

struct IADLXDisplayGamma
{
    const IADLXDisplayGammaVtbl *pVtbl;
};
#endif
#pragma endregion IADLXDisplayGamma interface

#endif//ADLX_IDISPLAYGAMMA_H
