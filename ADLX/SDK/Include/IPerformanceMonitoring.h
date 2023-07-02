//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IPERFORMANCEMONITORING_H
#define ADLX_IPERFORMANCEMONITORING_H
#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"

//-------------------------------------------------------------------------------------------------
//IPerformanceMonitoring.h - Interfaces for ADLX Performance Monitoring functionality
#pragma region IADLXGPUMetricsSupport
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUMetricsSupport : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUMetricsSupport")

        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUUsage IsSupportedGPUUsage
        *@ENG_START_DOX @brief Checks if the GPU usage metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUUsage (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU usage metric reporting is returned. The variable is __true__ if the GPU usage metric reporting is supported\. The variable is __false__ if the GPU usage metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU usage metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU usage metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUUsage (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUClockSpeed IsSupportedGPUClockSpeed
        *@ENG_START_DOX @brief Checks if the GPU clock speed metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUClockSpeed (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU clock speed metric reporting is returned. The variable is __true__ if the GPU clock speed metric reporting is supported\. The variable is __false__ if the GPU clock speed metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU clock speed metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU clock speed metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUClockSpeed (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUVRAMClockSpeed IsSupportedGPUVRAMClockSpeed
        *@ENG_START_DOX @brief Checks if the GPU memory clock speed metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUVRAMClockSpeed (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU memory clock speed metric reporting is returned. The variable is __true__ if the GPU memory clock speed metric reporting is supported\. The variable is __false__ if the GPU memory clock speed metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU memory clock speed metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU memory clock speed metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUVRAMClockSpeed (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUTemperature IsSupportedGPUTemperature
        *@ENG_START_DOX @brief Checks if the GPU temperature metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUTemperature (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU temperature metric reporting is returned. The variable is __true__ if the GPU temperature metric reporting is supported\. The variable is __false__ if the GPU temperature metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU temperature metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU temperature metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUTemperature (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUHotspotTemperature IsSupportedGPUHotspotTemperature
        *@ENG_START_DOX @brief Checks if the GPU hotspot temperature metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUHotspotTemperature (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU temperature metric reporting is returned. The variable is __true__ if the GPU hotspot temperature metric reporting is supported\. The variable is __false__ if the GPU hotspot temperature metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU hotspot temperature metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU hotspot temperature metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUHotspotTemperature (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUPower IsSupportedGPUPower
        *@ENG_START_DOX @brief Checks if the GPU power metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUPower (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU power metric reporting is returned. The variable is __true__ if the GPU power metric reporting is supported\. The variable is __false__ if the GPU power metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU power metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU power metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUPower (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUTotalBoardPower IsSupportedGPUTotalBoardPower
        *@ENG_START_DOX @brief Checks if the GPU total board power metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUTotalBoardPower (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU total board power metric reporting is returned. The variable is __true__ if the GPU total board power metric reporting is supported\. The variable is __false__ if the GPU total board power metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU total board power metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU total board power metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUTotalBoardPower(adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUFanSpeed IsSupportedGPUFanSpeed
        *@ENG_START_DOX @brief Checks if the GPU fan speed metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUFanSpeed (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU fan speed metric reporting is returned. The variable is __true__ if the GPU fan speed metric reporting is supported\. The variable is __false__ if the GPU fan speed metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU fan speed metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU fan speed metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUFanSpeed (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUVRAM IsSupportedGPUVRAM
        *@ENG_START_DOX @brief Checks if the GPU VRAM usage metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUVRAM (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU VRAM usage metric reporting is returned. The variable is __true__ if the GPU VRAM usage metric reporting is supported. The variable is __false__ if the GPU VRAM usage metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU VRAM usage metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU VRAM usage metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUVRAM (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_IsSupportedGPUVoltage IsSupportedGPUVoltage
        *@ENG_START_DOX @brief Checks if the GPU voltage metric reporting is supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedGPUVoltage (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of GPU voltage metric reporting is returned. The variable is __true__ if the GPU voltage metric reporting is supported. The variable is __false__ if the GPU voltage metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of GPU voltage metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of GPU voltage metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedGPUVoltage (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUUsageRange GetGPUUsageRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU usage on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUUsageRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU usage (in %) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU usage (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU usage range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU usage range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU usage are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUUsageRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUClockSpeedRange GetGPUClockSpeedRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU clock speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUClockSpeedRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU clock speed (in MHz) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU clock speed (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU clock speed range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU clock speed range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU clock speed are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUClockSpeedRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUVRAMClockSpeedRange GetGPUVRAMClockSpeedRange
        *@ENG_START_DOX @brief Gets the minimum and maximum VRAM clock speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUVRAMClockSpeedRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum VRAM clock speed (in MHz) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum VRAM clock speed (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the VRAM clock speed range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the VRAM clock speed range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum VRAM clock speed are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUVRAMClockSpeedRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUTemperatureRange GetGPUTemperatureRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU temperature on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUTemperatureRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU temperature (in °C) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU temperature (in °C) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU temperature range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU temperature range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU temperature are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUTemperatureRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUHotspotTemperatureRange GetGPUHotspotTemperatureRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU hotspot temperature on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUHotspotTemperatureRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU hotspot temperature (in °C) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU hotspot temperature (in °C) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU hotspot temperature range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU hotspot temperature range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU hotspot temperature are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUHotspotTemperatureRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUPowerRange GetGPUPowerRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU power consumption on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUPowerRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU power consumption (in W) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU power consumption (in W) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU power consumption range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU power consumption range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU power consumption are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUPowerRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUFanSpeedRange GetGPUFanSpeedRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU fan speed on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUFanSpeedRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU fan speed (in RPM) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU fan speed (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU fan speed range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU fan speed range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU fan speed are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUFanSpeedRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUVRAMRange GetGPUVRAMRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU memory on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUVRAMRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU memory (in MB) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU memory (in MB) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU memory range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU memory range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU memory are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUVRAMRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUVoltageRange GetGPUVoltageRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU voltage on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUVoltageRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU voltage (in mV) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU voltage (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU voltage range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU voltage range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU voltage are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUVoltageRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXGPUMetricsSupport_GetGPUTotalBoardPowerRange GetGPUTotalBoardPowerRange
        *@ENG_START_DOX @brief Gets the minimum and maximum GPU total board power consumption on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUTotalBoardPowerRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum GPU total board power consumption (in W) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum GPU total board power consumption (in W) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU total board power consumption range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the GPU total board power consumption range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum GPU total board power consumption are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUTotalBoardPowerRange (adlx_int* minValue, adlx_int* maxValue) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUMetricsSupport> IADLXGPUMetricsSupportPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUMetricsSupport, L"IADLXGPUMetricsSupport")

typedef struct IADLXGPUMetricsSupport IADLXGPUMetricsSupport;

typedef struct IADLXGPUMetricsSupportVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUMetricsSupport* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUMetricsSupport* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUMetricsSupport* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPUMetricsSupport
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUUsage)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUClockSpeed)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUVRAMClockSpeed)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUTemperature)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUHotspotTemperature)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUPower)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUTotalBoardPower)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUFanSpeed)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUVRAM)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedGPUVoltage)(IADLXGPUMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUUsageRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUClockSpeedRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUVRAMClockSpeedRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTemperatureRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUHotspotTemperatureRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUPowerRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUFanSpeedRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUVRAMRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUVoltageRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTotalBoardPowerRange)(IADLXGPUMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
}IADLXGPUMetricsSupportVtbl;

struct IADLXGPUMetricsSupport { const IADLXGPUMetricsSupportVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUMetricsSupport

#pragma region IADLXSystemMetricsSupport
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXSystemMetricsSupport : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXSystemMetricsSupport")

        /**
        *@page DOX_IADLXSystemMetricsSupport_IsSupportedCPUUsage IsSupportedCPUUsage
        *@ENG_START_DOX @brief Checks if the CPU usage metric reporting is supported by the system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedCPUUsage (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of CPU usage metric reporting is returned. The variable is __true__ if the CPU usage metric reporting is supported\. The variable is __false__ if the CPU usage metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of CPU usage metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of CPU usage metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedCPUUsage (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXSystemMetricsSupport_IsSupportedSystemRAM IsSupportedSystemRAM
        *@ENG_START_DOX @brief Checks if the RAM usage metric reporting is supported by the system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedSystemRAM (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of system RAM usage metric reporting is returned. The variable is __true__ if the system RAM usage metric reporting is supported. The variable is __false__ if the system RAM usage metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of system RAM usage metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of system RAM usage metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedSystemRAM (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXSystemMetricsSupport_IsSupportedSmartShift IsSupportedSmartShift
        *@ENG_START_DOX @brief Checks if the SmartShift metric reporting is supported by the system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupportedSmartShift (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out],supported,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of SmartShift metric reporting is returned. The variable is __true__ if the SmartShift metric reporting is supported\. The variable is __false__ if the SmartShift metric reporting is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the state of SmartShift metric reporting is successfully returned, __ADLX_OK__ is returned. <br>
        *If the state of SmartShift metric reporting is not successfully returned, an error code is returned. <br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupportedSmartShift (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXSystemMetricsSupport_GetCPUUsageRange GetCPUUsageRange
        *@ENG_START_DOX @brief Gets the minimum and maximum CPU usage of a system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCPUUsageRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum CPU usage (in %) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum CPU usage (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the CPU usage range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the CPU usage range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum CPU usage are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCPUUsageRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXSystemMetricsSupport_GetSystemRAMRange GetSystemRAMRange
        *@ENG_START_DOX @brief Gets the minimum and maximum system RAM of a system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSystemRAMRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum system RAM (in MB) is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum system RAM (in MB) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the system RAM range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the system RAM range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum system RAM are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSystemRAMRange (adlx_int* minValue, adlx_int* maxValue) = 0;
        /**
        *@page DOX_IADLXSystemMetricsSupport_GetSmartShiftRange GetSmartShiftRange
        *@ENG_START_DOX @brief Gets the minimum and maximum SmartShift value of a system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSmartShiftRange (adlx_int* minValue, adlx_int* maxValue)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],minValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the minimum SmartShift value is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],maxValue,adlx_int*,@ENG_START_DOX The pointer to a variable where the maximum SmartShift value is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the SmartShift value range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the SmartShift value range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The minimum and maximum SmartShift value are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSmartShiftRange (adlx_int* minValue, adlx_int* maxValue) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXSystemMetricsSupport> IADLXSystemMetricsSupportPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXSystemMetricsSupport, L"IADLXSystemMetricsSupport")

typedef struct IADLXSystemMetricsSupport IADLXSystemMetricsSupport;

typedef struct IADLXSystemMetricsSupportVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXSystemMetricsSupport* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXSystemMetricsSupport* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXSystemMetricsSupport* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXSystemMetricsSupport
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedCPUUsage)(IADLXSystemMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedSystemRAM)(IADLXSystemMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *IsSupportedSmartShift)(IADLXSystemMetricsSupport* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *GetCPUUsageRange)(IADLXSystemMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetSystemRAMRange)(IADLXSystemMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
    ADLX_RESULT (ADLX_STD_CALL *GetSmartShiftRange)(IADLXSystemMetricsSupport* pThis, adlx_int* minValue, adlx_int* maxValue);
}IADLXSystemMetricsSupportVtbl;

struct IADLXSystemMetricsSupport { const IADLXSystemMetricsSupportVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXSystemMetricsSupport

#pragma region IADLXGPUMetrics
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUMetrics : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUMetrics")
        /**
        *@page DOX_IADLXGPUMetrics_TimeStamp TimeStamp
        *@ENG_START_DOX @brief Gets the timestamp of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TimeStamp (adlx_int64* ms)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ms,adlx_int64* ,@ENG_START_DOX The pointer to a variable where the timestamp (in ms) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the timestamp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the timestamp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The timestamp is the duration (in ms) from the system epoch time to the time when the metric sample was acquired. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TimeStamp (adlx_int64* ms) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUUsage GPUUsage
        *@ENG_START_DOX @brief Gets the GPU usage of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUUsage (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the CPU usage (in %) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU usage is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU usage is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUUsage (adlx_double* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUClockSpeed GPUClockSpeed
        *@ENG_START_DOX @brief Gets the GPU clock speed of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUClockSpeed (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the GPU clock speed (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU clock speed is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU clock speed is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUClockSpeed (adlx_int* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUVRAMClockSpeed GPUVRAMClockSpeed
        *@ENG_START_DOX @brief Gets the VRAM clock speed of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUVRAMClockSpeed (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX GPU Memory The pointer to a variable where the VRAM clock speed (in MHz) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the VRAM clock speed is successfully returned, __ADLX_OK__ is returned. <br>
        * If the VRAM clock speed is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUVRAMClockSpeed (adlx_int* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUTemperature GPUTemperature
        *@ENG_START_DOX @brief Gets the GPU temperature of a GPU metric sample.
        *@details GPUTemperature reports the average temperature measured at the edge of the die of the GPU. This is sometimes referred to as ‘Edge Temperature’. 
		*@ENG_END_DOX
		*
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUTemperature (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the GPU temperature (in °C) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU temperature is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU temperature is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
		*  @ENG_START_DOX Related method: @ref DOX_IADLXGPUMetrics_GPUHotspotTemperature @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUTemperature (adlx_double* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUHotspotTemperature GPUHotspotTemperature
        *@ENG_START_DOX
        *@brief Gets the GPU hotspot temperature of a GPU metric sample.
		*@details GPUHotspotTemperature reports the highest temperature measured on the die of the GPU from a collection of junction temperature sensors on the die. This is sometimes referred to as ‘Junction Temperature’.
		*@ENG_END_DOX
		*
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUHotspotTemperature (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the GPU hotspot temperature (in °C) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU hotspot temperature is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU hotspot temperature is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
		*  @ENG_START_DOX Related method: @ref DOX_IADLXGPUMetrics_GPUTemperature @ENG_END_DOX
		*
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUHotspotTemperature (adlx_double* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUPower GPUPower
        *@ENG_START_DOX @brief Gets the GPU power consumption of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUPower (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the GPU power consumption (in W) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU power is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU power is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUPower (adlx_double* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUTotalBoardPower GPUTotalBoardPower
        *@ENG_START_DOX @brief Gets the GPU total board power consumption of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUTotalBoardPower (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the GPU total board power consumption (in W) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU total board power is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU total board power is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@addinfo
        * @ENG_START_DOX The method returns power of all components on the board including VRAM. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUTotalBoardPower(adlx_double* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUFanSpeed GPUFanSpeed
        *@ENG_START_DOX @brief Gets the GPU fan speed of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUFanSpeed (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the GPU fan speed (in RPM) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the GPU fan speed is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU fan speed is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUFanSpeed (adlx_int* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUVRAM GPUVRAM
        *@ENG_START_DOX @brief Gets the dedicated GPU memory of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUVRAM (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the dedicated GPU memory (in MB) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the dedicated GPU memory is successfully returned, __ADLX_OK__ is returned. <br>
        * If the dedicated GPU memory is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUVRAM (adlx_int* data) = 0;
        /**
        *@page DOX_IADLXGPUMetrics_GPUVoltage GPUVoltage
        *@ENG_START_DOX @brief Gets the GPU voltage of a GPU metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GPUVoltage (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the GPU voltage (in mV) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX If the GPU voltage is successfully returned, __ADLX_OK__ is returned. <br>
        * If the GPU voltage is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GPUVoltage (adlx_int* data) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUMetrics> IADLXGPUMetricsPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUMetrics, L"IADLXGPUMetrics")

typedef struct IADLXGPUMetrics IADLXGPUMetrics;

typedef struct IADLXGPUMetricsVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUMetrics* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUMetrics* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUMetrics* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPUMetrics
    ADLX_RESULT (ADLX_STD_CALL *TimeStamp)(IADLXGPUMetrics* pThis, adlx_int64* ms);
    ADLX_RESULT (ADLX_STD_CALL *GPUUsage)(IADLXGPUMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUClockSpeed)(IADLXGPUMetrics* pThis, adlx_int* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUVRAMClockSpeed)(IADLXGPUMetrics* pThis, adlx_int* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUTemperature)(IADLXGPUMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUHotspotTemperature)(IADLXGPUMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUPower)(IADLXGPUMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUTotalBoardPower)(IADLXGPUMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUFanSpeed)(IADLXGPUMetrics* pThis, adlx_int* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUVRAM)(IADLXGPUMetrics* pThis, adlx_int* data);
    ADLX_RESULT (ADLX_STD_CALL *GPUVoltage)(IADLXGPUMetrics* pThis, adlx_int* data);
}IADLXGPUMetricsVtbl;

struct IADLXGPUMetrics { const IADLXGPUMetricsVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUMetrics

#pragma region IADLXGPUMetricsList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUMetricsList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUMetricsList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXGPUMetrics::IID ())

        /**
        * @page DOX_IADLXGPUMetricsList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXGPUMetrics** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXGPUMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned. <br>
        * If the location is not within the list bounds, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
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
        * @DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXGPUMetrics** ppItem) = 0;

        /**
        *@page DOX_IADLXGPUMetricsList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXGPUMetrics* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXGPUMetrics* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is added successfully to the end of the list, __ADLX_OK__ is returned. <br>
        * If the interface is not added to the end of the list, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXGPUMetrics* pItem) = 0;
    };  //IADLXGPUMetricsList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUMetricsList> IADLXGPUMetricsListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXGPUMetricsList, L"IADLXGPUMetricsList")
ADLX_DECLARE_ITEM_IID (IADLXGPUMetrics, IID_IADLXGPUMetrics ())

typedef struct IADLXGPUMetricsList IADLXGPUMetricsList;

typedef struct IADLXGPUMetricsListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUMetricsList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUMetricsList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXGPUMetricsList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXGPUMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXGPUMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXGPUMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXGPUMetricsList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXGPUMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXGPUMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXGPUMetricsList* pThis, IADLXInterface* pItem);

    //IADLXGPUMetricsList
    ADLX_RESULT (ADLX_STD_CALL *At_GPUMetricsList)(IADLXGPUMetricsList* pThis, const adlx_uint location, IADLXGPUMetrics** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_GPUMetricsList)(IADLXGPUMetricsList* pThis, IADLXGPUMetrics* pItem);

}IADLXGPUMetricsListVtbl;

struct IADLXGPUMetricsList { const IADLXGPUMetricsListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXGPUMetricsList

#pragma region IADLXSystemMetrics
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXSystemMetrics : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXSystemMetrics")
        /**
        *@page DOX_IADLXSystemMetrics_TimeStamp TimeStamp
        *@ENG_START_DOX @brief Gets the timestamp of a system metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TimeStamp (adlx_int64* ms)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ms,adlx_int64* ,@ENG_START_DOX The pointer to a variable where the timestamp (in ms) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the timestamp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the timestamp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The timestamp is the duration (in ms) from the system epoch time to the time when the metric sample was acquired. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TimeStamp (adlx_int64* ms) = 0;
        /**
        *@page DOX_IADLXSystemMetrics_CPUUsage CPUUsage
        *@ENG_START_DOX @brief Gets the CPU usage of a system metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    CPUUsage (adlx_double* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_double* ,@ENG_START_DOX The pointer to a variable where the CPU usage (in %) is returned.   @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the CPU usage is successfully returned, __ADLX_OK__ is returned. <br>
        * If the CPU usage is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL CPUUsage (adlx_double* data) = 0;
        /**
        *@page DOX_IADLXSystemMetrics_SystemRAM SystemRAM
        *@ENG_START_DOX @brief Gets the system RAM of a system metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SystemRAM (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the system RAM (in MB) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the system RAM is successfully returned, __ADLX_OK__ is returned. <br>
        * If the system RAM is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SystemRAM (adlx_int* data) = 0;
        /**
        *@page DOX_IADLXSystemMetrics_SmartShift SmartShift
        *@ENG_START_DOX @brief Gets the SmartShift value of a system metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SmartShift (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the SmartShift value is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the SmartShift value is successfully returned, __ADLX_OK__ is returned. <br>
        * If the SmartShift value is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The value returned by SmartShift is an integer number which is within -100 to +100 range.<br>
        * A negative value indicates that the power is shifted to the CPU. <br>
        * A positive value indicates that the power is shifted to the GPU. <br>
        * Considering zero as the state with no power shift, the larger the deviation from zero, the larger the power shift. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  AMD SmartShift helps boost notebook performance by dynamically shifting the power between the CPU and the GPU depending on the workload. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SmartShift (adlx_int* data) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXSystemMetrics> IADLXSystemMetricsPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXSystemMetrics, L"IADLXSystemMetrics")

typedef struct IADLXSystemMetrics IADLXSystemMetrics;

typedef struct IADLXSystemMetricsVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXSystemMetrics* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXSystemMetrics* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXSystemMetrics* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXSystemMetrics
    ADLX_RESULT (ADLX_STD_CALL *TimeStamp)(IADLXSystemMetrics* pThis, adlx_int64* ms);
    ADLX_RESULT (ADLX_STD_CALL *CPUUsage)(IADLXSystemMetrics* pThis, adlx_double* data);
    ADLX_RESULT (ADLX_STD_CALL *SystemRAM)(IADLXSystemMetrics* pThis, adlx_int* data);
    ADLX_RESULT (ADLX_STD_CALL *SmartShift)(IADLXSystemMetrics* pThis, adlx_int* data);
}IADLXSystemMetricsVtbl;

struct IADLXSystemMetrics { const IADLXSystemMetricsVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXSystemMetrics

#pragma region IADLXSystemMetricsList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXSystemMetricsList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXSystemMetricsList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXSystemMetrics::IID ())

        /**
        * @page DOX_IADLXSystemMetricsList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXSystemMetrics** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXSystemMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned. <br>
        * If the location is not within the list bounds, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
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
        * @DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXSystemMetrics** ppItem) = 0;

        /**
        *@page DOX_IADLXSystemMetricsList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXSystemMetrics* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXSystemMetrics* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is added successfully to the end of the list, __ADLX_OK__ is returned. <br>
        * If the interface is not added to the end of the list, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXSystemMetrics* pItem) = 0;
    };  //IADLXSystemMetricsList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXSystemMetricsList> IADLXSystemMetricsListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXSystemMetricsList, L"IADLXSystemMetricsList")
ADLX_DECLARE_ITEM_IID (IADLXSystemMetrics, IID_IADLXSystemMetrics ())

typedef struct IADLXSystemMetricsList IADLXSystemMetricsList;

typedef struct IADLXSystemMetricsListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXSystemMetricsList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXSystemMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXSystemMetricsList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXSystemMetricsList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXSystemMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXSystemMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXSystemMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXSystemMetricsList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXSystemMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXSystemMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXSystemMetricsList* pThis, IADLXInterface* pItem);

    //IADLXSystemMetricsList
    ADLX_RESULT (ADLX_STD_CALL *At_SystemMetricsList)(IADLXSystemMetricsList* pThis, const adlx_uint location, IADLXSystemMetrics** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_SystemMetricsList)(IADLXSystemMetricsList* pThis, IADLXSystemMetrics* pItem);

}IADLXSystemMetricsListVtbl;

struct IADLXSystemMetricsList { const IADLXSystemMetricsListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXSystemMetricsList

#pragma region IADLXFPS
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXFPS : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXFPS")
        /**
        *@page DOX_IADLXFPS_TimeStamp TimeStamp
        *@ENG_START_DOX @brief Gets the timestamp of an FPS metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TimeStamp (adlx_int64* ms)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ms,adlx_int64* ,@ENG_START_DOX The pointer to a variable where the timestamp (in ms) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the timestamp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the timestamp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The timestamp is the duration (in ms) from the system epoch time to the time when the metric sample was acquired. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TimeStamp (adlx_int64* ms) = 0;
        /**
        *@page DOX_IADLXFPS_FPS FPS
        *@ENG_START_DOX @brief Gets the FPS when this metric set was acquired. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    FPS (adlx_int* data)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,data,adlx_int* ,@ENG_START_DOX The pointer to a variable where the FPS is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the FPS is successfully returned, __ADLX_OK__ is returned. <br>
        * If the FPS is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL FPS (adlx_int* data) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXFPS> IADLXFPSPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXFPS, L"IADLXFPS")

typedef struct IADLXFPS IADLXFPS;

typedef struct IADLXFPSVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXFPS* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXFPS* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXFPS* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXFPS
    ADLX_RESULT (ADLX_STD_CALL *TimeStamp)(IADLXFPS* pThis, adlx_int64* ms);
    ADLX_RESULT (ADLX_STD_CALL *FPS)(IADLXFPS* pThis, adlx_int* data);
}IADLXFPSVtbl;

struct IADLXFPS { const IADLXFPSVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXFPS

#pragma region IADLXFPSList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXFPSList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXFPSList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXFPS::IID ())

        /**
        * @page DOX_IADLXFPSList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXFPS** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXFPS** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned. <br>
        * If the location is not within the list bounds, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
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
        * @DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXFPS** ppItem) = 0;

        /**
        *@page DOX_IADLXFPSList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXFPS* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXFPS* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is added successfully to the end of the list, __ADLX_OK__ is returned. <br>
        * If the interface is not added to the end of the list, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXFPS* pItem) = 0;
    };  //IADLXFPSList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXFPSList> IADLXFPSListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXFPSList, L"IADLXFPSList")
ADLX_DECLARE_ITEM_IID (IADLXFPS, IID_IADLXFPS ())

typedef struct IADLXFPSList IADLXFPSList;

typedef struct IADLXFPSListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXFPSList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXFPSList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXFPSList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXFPSList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXFPSList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXFPSList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXFPSList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXFPSList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXFPSList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXFPSList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXFPSList* pThis, IADLXInterface* pItem);

    //IADLXFPSList
    ADLX_RESULT (ADLX_STD_CALL *At_FPSList)(IADLXFPSList* pThis, const adlx_uint location, IADLXFPS** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_FPSList)(IADLXFPSList* pThis, IADLXFPS* pItem);

}IADLXFPSListVtbl;

struct IADLXFPSList { const IADLXFPSListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXFPSList

#pragma region IADLXAllMetrics
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU;
    class ADLX_NO_VTABLE IADLXAllMetrics : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXAllMetrics")
        /**
        *@page DOX_IADLXAllMetrics_TimeStamp TimeStamp
        *@ENG_START_DOX @brief Gets the timestamp of a metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TimeStamp (adlx_int64* ms)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ms,adlx_int64* ,@ENG_START_DOX The pointer to a variable where the timestamp (in ms) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the timestamp is successfully returned, __ADLX_OK__ is returned. <br>
        * If the timestamp is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The timestamp is the duration (in ms) from the system epoch time to the time when the metric sample was acquired. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TimeStamp (adlx_int64* ms) = 0;
        /**
        *@page DOX_IADLXAllMetrics_GetSystemMetrics GetSystemMetrics
        *@ENG_START_DOX @brief Gets the reference counted system metrics interface of a metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSystemMetrics (@ref DOX_IADLXSystemMetrics** ppSystemMetrics)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out] ,ppSystemMetrics,@ref DOX_IADLXSystemMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppSystemMetrics__ to __nullptr__.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSystemMetrics (IADLXSystemMetrics** ppSystemMetrics) = 0;
        /**
        *@page DOX_IADLXAllMetrics_GetFPS GetFPS
        *@ENG_START_DOX @brief Gets the reference counted FPS metrics interface of a metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFPS (@ref DOX_IADLXFPS** ppFPS)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out] ,ppFPS,@ref DOX_IADLXFPS** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppFPS__ to __nullptr__.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetFPS (IADLXFPS** ppFPS) = 0;
        /**
        *@page DOX_IADLXAllMetrics_GetGPUMetrics GetGPUMetrics
        *@ENG_START_DOX @brief Gets the reference counted GPU metrics interface of a metric sample. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMetrics (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXGPUMetrics** ppGPUMetrics)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to a variable where IADLXGPU interface is returned. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppGPUMetrics,@ref DOX_IADLXGPUMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPUMetrics__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUMetrics (IADLXGPU* pGPU, IADLXGPUMetrics** ppGPUMetrics) = 0;
    };
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXAllMetrics> IADLXAllMetricsPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXAllMetrics, L"IADLXAllMetrics")

typedef struct IADLXAllMetrics IADLXAllMetrics;

typedef struct IADLXAllMetricsVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXAllMetrics* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXAllMetrics* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXAllMetrics* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXAllMetrics
    ADLX_RESULT (ADLX_STD_CALL *TimeStamp)(IADLXAllMetrics* pThis, adlx_int64* ms);
    ADLX_RESULT (ADLX_STD_CALL *GetSystemMetrics)(IADLXAllMetrics* pThis, IADLXSystemMetrics** metrics);
    ADLX_RESULT (ADLX_STD_CALL *GetFPS)(IADLXAllMetrics* pThis, IADLXFPS** metrics);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMetrics)(IADLXAllMetrics* pThis, IADLXGPU* pGPU, IADLXGPUMetrics** metrics);
}IADLXAllMetricsVtbl;

struct IADLXAllMetrics { const IADLXAllMetricsVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXAllMetrics

#pragma region IADLXAllMetricsList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXAllMetricsList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXAllMetricsList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXAllMetrics::IID ())

        /**
        * @page DOX_IADLXAllMetricsList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXAllMetrics** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXAllMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned. <br>
        * If the location is not within the list bounds, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
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
        * @DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXAllMetrics** ppItem) = 0;

        /**
        *@page DOX_IADLXAllMetricsList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXAllMetrics* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXAllMetrics* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is added successfully to the end of the list, __ADLX_OK__ is returned. <br>
        * If the interface is not added to the end of the list, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXAllMetrics* pItem) = 0;
    };  //IADLXAllMetricsList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXAllMetricsList> IADLXAllMetricsListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXAllMetricsList, L"IADLXAllMetricsList")
ADLX_DECLARE_ITEM_IID (IADLXAllMetrics, IID_IADLXAllMetrics ())

typedef struct IADLXAllMetricsList IADLXAllMetricsList;

typedef struct IADLXAllMetricsListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXAllMetricsList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXAllMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXAllMetricsList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXAllMetricsList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXAllMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXAllMetricsList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXAllMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXAllMetricsList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXAllMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXAllMetricsList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXAllMetricsList* pThis, IADLXInterface* pItem);

    //IADLXAllMetricsList
    ADLX_RESULT (ADLX_STD_CALL *At_AllMetricsList)(IADLXAllMetricsList* pThis, const adlx_uint location, IADLXAllMetrics** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_AllMetricsList)(IADLXAllMetricsList* pThis, IADLXAllMetrics* pItem);

}IADLXAllMetricsListVtbl;

struct IADLXAllMetricsList { const IADLXAllMetricsListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXAllMetricsList

#pragma region IADLXPerformanceMonitoringServices
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXPerformanceMonitoringServices : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID(L"IADLXPerformanceMonitoringServices")
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetSamplingIntervalRange GetSamplingIntervalRange
        *@ENG_START_DOX @brief Gets the maximum sampling interval, minimum sampling interval, and step sampling interval for the performance monitoring. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSamplingIntervalRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the sampling interval range (in millisecond) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sampling interval range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the sampling interval range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum sampling interval, minimum sampling interval, and step sampling interval are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSamplingIntervalRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_SetSamplingInterval SetSamplingInterval
        *@ENG_START_DOX @brief Sets the sampling interval for the performance monitoring. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetSamplingInterval (adlx_int intervalMs)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,intervalMs,adlx_int ,@ENG_START_DOX The sampling interval (in millisecond). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sampling interval is successfully set, __ADLX_OK__ is returned. <br>
        * If the sampling interval is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The sampling interval is the time gap between two samples.
        * Obtain the sampling interval range with @ref DOX_IADLXPerformanceMonitoringServices_GetSamplingIntervalRange. The default sampling interval is 100 ms.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetSamplingInterval (adlx_int askedIntervalMs) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetSamplingInterval GetSamplingInterval
        *@ENG_START_DOX @brief Gets the sampling interval for performance monitoring. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSamplingInterval (adlx_int* intervalMs)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,intervalMs,adlx_int* ,@ENG_START_DOX The pointer to a variable where the sampling interval (in millisecond) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the sampling interval is successfully returned, __ADLX_OK__ is returned. <br>
        * If the sampling interval is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The sampling interval is the time gap between two samples.
        * Obtain the sampling interval range with @ref DOX_IADLXPerformanceMonitoringServices_GetSamplingIntervalRange. The default sampling interval is 1000 ms.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSamplingInterval (adlx_int* intervalMs) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetMaxPerformanceMetricsHistorySizeRange GetMaxPerformanceMetricsHistorySizeRange
        *@ENG_START_DOX @brief Gets the maximum size, minimum size, and step size for the performance monitoring buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMaxPerformanceMetricsHistorySizeRange (@ref ADLX_IntRange* range)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],range,@ref ADLX_IntRange*,@ENG_START_DOX The pointer to a variable where the buffer size range (in second) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the buffer size range is successfully returned, __ADLX_OK__ is returned.<br>
        * If the buffer size range is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The maximum buffer size, minimum buffer size, and step buffer size are read only. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetMaxPerformanceMetricsHistorySizeRange (ADLX_IntRange* range) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_SetMaxPerformanceMetricsHistorySize SetMaxPerformanceMetricsHistorySize
        *@ENG_START_DOX @brief Sets the duration of the performance monitoring buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    SetMaxPerformanceMetricsHistorySize (adlx_int sizeSec)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,sizeSec,adlx_int ,@ENG_START_DOX The buffer duration (in second). @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the buffer duration is successfully set, __ADLX_OK__ is returned. <br>
        * If the buffer duration is not successfully set, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Obtain the buffer duration range with @ref DOX_IADLXPerformanceMonitoringServices_GetMaxPerformanceMetricsHistorySizeRange.
        * The default buffer duration is 100 s. If the monitoring continues longer than the buffer duration, the old performance samples are discarded to give buffer space to the latest samples.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SetMaxPerformanceMetricsHistorySize (adlx_int sizeSec) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetMaxPerformanceMetricsHistorySize GetMaxPerformanceMetricsHistorySize
        *@ENG_START_DOX @brief Gets the duration of the performance monitoring buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetMaxPerformanceMetricsHistorySize (adlx_int* sizeSec)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,sizeSec,adlx_int* ,@ENG_START_DOX The pointer to a variable where the buffer duration (in second) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the buffer duration is successfully returned, __ADLX_OK__ is returned. <br>
        * If the buffer duration is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Obtain the buffer duration range with @ref DOX_IADLXPerformanceMonitoringServices_GetMaxPerformanceMetricsHistorySizeRange.
        * The default buffer duration is 100 s. If the monitoring continues longer than the buffer duration, the old performance samples are discarded to give buffer space to the latest samples.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetMaxPerformanceMetricsHistorySize (adlx_int* sizeSec) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_ClearPerformanceMetricsHistory ClearPerformanceMetricsHistory
        *@ENG_START_DOX @brief Clears the buffer for performance monitoring. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    ClearPerformanceMetricsHistory ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the buffer of performance monitoring is successfully cleared, __ADLX_OK__ is returned. <br>
        * If the buffer of performance monitoring is not successfully cleared, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL ClearPerformanceMetricsHistory () = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetCurrentPerformanceMetricsHistorySize GetCurrentPerformanceMetricsHistorySize
        *@ENG_START_DOX @brief Gets the duration of the metrics history from the performance monitoring buffer. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentPerformanceMetricsHistorySize (adlx_int* sizeSec)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,sizeSec,adlx_int* ,@ENG_START_DOX The pointer to a variable where the duration (in second) is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the duration is successfully returned, __ADLX_OK__ is returned. <br>
        * If the duration is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The buffer duration ranges between 50 s and 100 s. The default size is 100 s.
        * If the monitoring continues longer than the buffer duration, the old performance samples are discarded to give buffer space to the new samples. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentPerformanceMetricsHistorySize (adlx_int* sizeSec) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_StartPerformanceMetricsTracking StartPerformanceMetricsTracking
        *@ENG_START_DOX @brief Increases the count for the performance metrics tracking. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    StartPerformanceMetricsTracking ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the count is successfully increased, __ADLX_OK__ is returned. <br>
        * If the count is not successfully increased, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details ADLX reserves one buffer for the performance data. A count is used to keep a track of the calls to the __StartPerformanceMetricsTracking__. The count is increased by one every time the tracking starts. <br>
        * By default, the count is zero. The monitoring starts when the first call is made and the count increases by one. <br>
        * If the monitoring continues longer than the buffer duration, the old performance samples are discarded to give buffer space to the new samples.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL StartPerformanceMetricsTracking () = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_StopPerformanceMetricsTracking StopPerformanceMetricsTracking
        *@ENG_START_DOX @brief Decreases the count for the performance metrics tracking. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    StopPerformanceMetricsTracking ()
        *@codeEnd
        *
        *@params
        * N/A
        *
        *@retvalues
        *@ENG_START_DOX  If the count is successfully decreased, __ADLX_OK__ is returned. <br>
        * If the count is not successfully decreased, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details ADLX reserves one buffer for the performance data. A count is used to keep a track of the calls to the __StopPerformanceMetricsTracking__. <br>
        * When the __StopPerformanceMetricsTracking__ is called, the count is decreased by one. When the count reaches zero, the monitoring stops.<br>
        * If the monitoring continues longer than the buffer duration, the old performance samples are discarded to give buffer space to the new samples.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL StopPerformanceMetricsTracking () = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetAllMetricsHistory GetAllMetricsHistory
        *@ENG_START_DOX @brief Gets the reference counted list of all the metrics in a time interval. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetAllMetricsHistory (adlx_int startMs, adlx_int stopMs, @ref DOX_IADLXAllMetricsList** ppMetricsList)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,startMs,adlx_int ,@ENG_START_DOX The start time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{1.,[in] ,stopMs,adlx_int ,@ENG_START_DOX The stop time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{1.,[out] ,ppMetricsList,@ref DOX_IADLXAllMetricsList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Use the __startMs__ and __stopMs__ to specify the time intervals for fetching the history.
        * - To get the reference counted list of all the performance metric samples from the performance monitoring buffer, specify both the __startMs__ and __stopMs__ as zero.<br>
        * - To get the reference counted list of the performance metric samples from A ms ago to the present time, specify __startMs__ as A ms and __stopMs__ as zero. <br>
        * - To get the reference counted list of the performance metric samples from A ms ago to B ms ago, specify __startMs__ as A ms and __stopMs__ as B ms. <br>
        *
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. <br>
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetAllMetricsHistory (adlx_int startMs, adlx_int stopMs, IADLXAllMetricsList** ppMetricsList) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetGPUMetricsHistory GetGPUMetricsHistory
        *@ENG_START_DOX @brief Gets the reference counted list of GPU metrics in a time interval of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUMetricsHistory (@ref DOX_IADLXGPU* pGPU, adlx_int startMs, adlx_int stopMs, @ref DOX_IADLXGPUMetricsList** ppMetricsList)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface.  @ENG_END_DOX}
        * @paramrow{2.,[in] ,startMs,adlx_int ,@ENG_START_DOX The start time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{3.,[in] ,stopMs,adlx_int ,@ENG_START_DOX The stop time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{4.,[out] ,ppMetricsList,@ref DOX_IADLXGPUMetricsList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Use the __startMs__ and __stopMs__ to specify the time intervals for fetching the history.
        * - To get the reference counted list of all the performance metric samples from the performance monitoring buffer, specify both the __startMs__ and __stopMs__ as zero.<br>
        * - To get the reference counted list of the performance metric samples from A ms ago to the present time, specify __startMs__ as A ms and __stopMs__ as zero. <br>
        * - To get the reference counted list of the performance metric samples from A ms ago to B ms ago, specify __startMs__ as A ms and __stopMs__ as B ms. <br>
        *
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. <br>
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUMetricsHistory (IADLXGPU* pGPU, adlx_int startMs, adlx_int stopMs, IADLXGPUMetricsList** ppMetricsList) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetSystemMetricsHistory GetSystemMetricsHistory
        *@ENG_START_DOX @brief Gets the reference counted list of system metrics in a time interval. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSystemMetricsHistory (adlx_int startMs, adlx_int stopMs, @ref DOX_IADLXSystemMetricsList** ppMetricsList)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,startMs,adlx_int ,@ENG_START_DOX The start time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{2.,[in] ,stopMs,adlx_int ,@ENG_START_DOX The stop time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{3.,[out] ,ppMetricsList,@ref DOX_IADLXSystemMetricsList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Use the __startMs__ and __stopMs__ to specify the time intervals for fetching the history.
        * - To get the reference counted list of all the performance metric samples from the performance monitoring buffer, specify both the __startMs__ and __stopMs__ as zero.<br>
        * - To get the reference counted list of the performance metric samples from A ms ago to the present time, specify __startMs__ as A ms and __stopMs__ as zero. <br>
        * - To get the reference counted list of the performance metric samples from A ms ago to B ms ago, specify __startMs__ as A ms and __stopMs__ as B ms. <br>
        *
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. <br>
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSystemMetricsHistory (adlx_int startMs, adlx_int stopMs, IADLXSystemMetricsList** ppMetricsList) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetFPSHistory GetFPSHistory
        *@ENG_START_DOX @brief Gets the reference counted list of FPS metrics in a time interval. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFPSHistory (adlx_int startMs, adlx_int stopMs, @ref DOX_IADLXFPSList** ppMetricsList)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,startMs,adlx_int ,@ENG_START_DOX The start time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{2.,[in] ,stopMs,adlx_int ,@ENG_START_DOX The stop time of the time interval (in millisecond). @ENG_END_DOX}
        * @paramrow{3.,[out] ,ppMetricsList,@ref DOX_IADLXFPSList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsList__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Use the __startMs__ and __stopMs__ to specify the time intervals for fetching the history.
        * - To get the reference counted list of all the performance metric samples from the performance monitoring buffer, specify both the __startMs__ and __stopMs__ as zero.<br>
        * - To get the reference counted list of the performance metric samples from A ms ago to the present time, specify __startMs__ as A ms and __stopMs__ as zero. <br>
        * - To get the reference counted list of the performance metric samples from A ms ago to B ms ago, specify __startMs__ as A ms and __stopMs__ as B ms. <br>
        *
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. <br>
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ The FPS metric is only available while a 3D graphics application or game runs in exclusive full screen mode.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetFPSHistory (adlx_int startMs, adlx_int stopMs, IADLXFPSList** ppMetricsList) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetCurrentAllMetrics GetCurrentAllMetrics
        *@ENG_START_DOX @brief Gets the reference counted  @ref DOX_IADLXAllMetrics interface for the current metric set. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentAllMetrics (@ref DOX_IADLXAllMetrics** ppMetrics)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppMetrics,@ref DOX_IADLXAllMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetrics__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the current interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentAllMetrics (IADLXAllMetrics** ppMetrics) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetCurrentGPUMetrics GetCurrentGPUMetrics
        *@ENG_START_DOX @brief Gets the reference counted  @ref DOX_IADLXGPUMetrics interface for the current metric set of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentGPUMetrics (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXGPUMetrics** ppMetrics)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface.  @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppMetrics,@ref DOX_IADLXGPUMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetrics__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentGPUMetrics (IADLXGPU* pGPU, IADLXGPUMetrics** ppMetrics) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetCurrentSystemMetrics GetCurrentSystemMetrics
        *@ENG_START_DOX @brief Gets the reference counted  @ref DOX_IADLXSystemMetrics interface for the current metric set. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentSystemMetrics (@ref DOX_IADLXSystemMetrics** ppMetrics)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppMetrics,@ref DOX_IADLXSystemMetrics** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetrics__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentSystemMetrics (IADLXSystemMetrics** ppMetrics) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetCurrentFPS GetCurrentFPS
        *@ENG_START_DOX @brief Gets the reference counted  @ref DOX_IADLXFPS interface for the current FPS metric. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCurrentFPS (@ref DOX_IADLXFPS** ppMetrics)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppMetrics,@ref DOX_IADLXFPS** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetrics__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ The FPS metric is only available while a 3D graphics application or game runs in exclusive full screen mode.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCurrentFPS (IADLXFPS** ppMetrics) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetSupportedGPUMetrics GetSupportedGPUMetrics
        *@ENG_START_DOX @brief Gets the reference counted interface for discovering what performance metrics are supported on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSupportedGPUMetrics (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXGPUMetricsSupport** ppMetricsSupported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppMetricsSupported,@ref DOX_IADLXGPUMetricsSupport** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsSupported__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSupportedGPUMetrics (IADLXGPU* pGPU, IADLXGPUMetricsSupport** ppMetricsSupported) = 0;
        /**
        *@page DOX_IADLXPerformanceMonitoringServices_GetSupportedSystemMetrics GetSupportedSystemMetrics
        *@ENG_START_DOX @brief Gets the reference counted interface for discovering what performance metrics are supported on the system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetSupportedSystemMetrics (@ref DOX_IADLXSystemMetricsSupport** ppMetricsSupported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppMetricsSupported,@ref DOX_IADLXSystemMetricsSupport** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppMetricsSupported__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IPerformanceMonitoring.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetSupportedSystemMetrics (IADLXSystemMetricsSupport** ppMetricsSupported) = 0;

    };  //IADLXPerformanceMonitoringServices
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXPerformanceMonitoringServices> IADLXPerformanceMonitoringServicesPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXPerformanceMonitoringServices, L"IADLXPerformanceMonitoringServices")
typedef struct IADLXPerformanceMonitoringServices IADLXPerformanceMonitoringServices;

typedef struct IADLXPerformanceMonitoringServicesVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXPerformanceMonitoringServices* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXPerformanceMonitoringServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXPerformanceMonitoringServices* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXPerformanceMonitoringServices
    ADLX_RESULT (ADLX_STD_CALL *GetSamplingIntervalRange)(IADLXPerformanceMonitoringServices* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *SetSamplingInterval)(IADLXPerformanceMonitoringServices* pThis, adlx_int intervalMs);
    ADLX_RESULT (ADLX_STD_CALL *GetSamplingInterval)(IADLXPerformanceMonitoringServices* pThis, adlx_int* intervalMs);
    ADLX_RESULT (ADLX_STD_CALL *GetMaxPerformanceMetricsHistorySizeRange)(IADLXPerformanceMonitoringServices* pThis, ADLX_IntRange* range);
    ADLX_RESULT (ADLX_STD_CALL *SetMaxPerformanceMetricsHistorySize)(IADLXPerformanceMonitoringServices* pThis, adlx_int sizeSec);
    ADLX_RESULT (ADLX_STD_CALL *GetMaxPerformanceMetricsHistorySize)(IADLXPerformanceMonitoringServices* pThis, adlx_int* sizeSec);
    ADLX_RESULT (ADLX_STD_CALL *ClearPerformanceMetricsHistory)(IADLXPerformanceMonitoringServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentPerformanceMetricsHistorySize)(IADLXPerformanceMonitoringServices* pThis, adlx_int* sizeSec);
    ADLX_RESULT (ADLX_STD_CALL *StartPerformanceMetricsTracking)(IADLXPerformanceMonitoringServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *StopPerformanceMetricsTracking)(IADLXPerformanceMonitoringServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *GetAllMetricsHistory)(IADLXPerformanceMonitoringServices* pThis, adlx_int startMs, adlx_int stopMs, IADLXAllMetricsList** ppMetricsList);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUMetricsHistory)(IADLXPerformanceMonitoringServices* pThis, IADLXGPU* pGPU, adlx_int startMs, adlx_int stopMs, IADLXGPUMetricsList** ppMetricsList);
    ADLX_RESULT (ADLX_STD_CALL *GetSystemMetricsHistory)(IADLXPerformanceMonitoringServices* pThis, adlx_int startMs, adlx_int stopMs, IADLXSystemMetricsList** ppMetricsList);
    ADLX_RESULT (ADLX_STD_CALL *GetFPSHistory)(IADLXPerformanceMonitoringServices* pThis, adlx_int startMs, adlx_int stopMs, IADLXFPSList** ppMetricsList);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentAllMetrics)(IADLXPerformanceMonitoringServices* pThis, IADLXAllMetrics** ppMetrics);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentGPUMetrics)(IADLXPerformanceMonitoringServices* pThis, IADLXGPU* pGPU, IADLXGPUMetrics** ppMetrics);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentSystemMetrics)(IADLXPerformanceMonitoringServices* pThis, IADLXSystemMetrics** ppMetrics);
    ADLX_RESULT (ADLX_STD_CALL *GetCurrentFPS)(IADLXPerformanceMonitoringServices* pThis, IADLXFPS** ppMetrics);
    ADLX_RESULT (ADLX_STD_CALL *GetSupportedGPUMetrics)(IADLXPerformanceMonitoringServices* pThis, IADLXGPU* pGPU, IADLXGPUMetricsSupport** ppMetricsSupported);
    ADLX_RESULT (ADLX_STD_CALL *GetSupportedSystemMetrics)(IADLXPerformanceMonitoringServices* pThis, IADLXSystemMetricsSupport** ppMetricsSupported);
}IADLXPerformanceMonitoringServicesVtbl;

struct IADLXPerformanceMonitoringServices { const IADLXPerformanceMonitoringServicesVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXPerformanceMonitoringServices

#endif//ADLX_IPERFORMANCEMONITORING_H
