//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------------
//ISystem.h - Interfaces for ADLX System-level functionality

#ifndef ADLX_ISYSTEM_H
#define ADLX_ISYSTEM_H
#pragma once

#include "ADLXDefines.h"
#include "ICollections.h"

//Interfaces for GPU Info
#pragma region IADLXGPU interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPU")

        /**
        * @page DOX_IADLXGPU_VendorId VendorId
        * @ENG_START_DOX
        * @brief Gets the vendor id of a GPU.
        * @ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    VendorId (const char** vendorId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],vendorId,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the vendor id is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the vendor id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the vendor id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The vendor id is a predetermined value programmed into the GPU at the point of manufacturing and consists of four hexadecimal digits.
        * @ENG_END_DOX
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the vendor id beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the vendor id.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL VendorId (const char** vendorId) = 0;
        /**
         * @page DOX_IADLXGPU_ASICFamilyType ASICFamilyType
         * @ENG_START_DOX
         * @brief Gets the ASIC family type of a GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    ASICFamilyType (@ref ADLX_ASIC_FAMILY_TYPE* asicFamilyType)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],asicFamilyType,@ref ADLX_ASIC_FAMILY_TYPE*,@ENG_START_DOX The pointer to a variable where the ASIC family type is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the ASIC family type is successfully returned, __ADLX_OK__ is returned.<br>
         * If the ASIC family type is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL ASICFamilyType (ADLX_ASIC_FAMILY_TYPE* asicFamilyType) const = 0;
        /**
         * @page DOX_IADLXGPU_Type Type
         * @ENG_START_DOX
         * @brief Gets the type of a GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    Type (@ref ADLX_GPU_TYPE* gpuType)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],gpuType,@ref ADLX_GPU_TYPE*,@ENG_START_DOX The pointer to a variable where the GPU type is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the GPU type is successfully returned, __ADLX_OK__ is returned.<br>
         * If the GPU type is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The GPU type can be categorized into Integrated, Discrete, and Unknown.
         * @ENG_END_DOX
         *
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL Type (ADLX_GPU_TYPE* gpuType) const = 0;
        /**
         * @page DOX_IADLXGPU_IsExternal IsExternal
         * @ENG_START_DOX
         * @brief Checks if a GPU is an external or internal GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    IsExternal (adlx_bool* isExternal)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],isExternal,adlx_bool*,@ENG_START_DOX The pointer to a variable where the state of the GPU is returned. The variable is __true__ if the GPU is external. The variable is __false__ if the GPU is internal.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If __IsExternal__ is successfully executed, __ADLX_OK__ is returned.<br>
         * If __IsExternal__ is not successfully executed, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL IsExternal (adlx_bool* isExternal) const = 0;
        /**
         * @page DOX_IADLXGPU_Name Name
         * @ENG_START_DOX
         * @brief Gets the name of a GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    Name (const char** name)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],name,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the name of the GPU is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the name is successfully returned, __ADLX_OK__ is returned.<br>
         * If the name is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * The returned memory buffer is valid within a lifetime of the @ref DOX_IADLXGPU interface.<br>
         * If the application uses the name beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the name.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL Name (const char** name) const = 0;
        /**
         * @page DOX_IADLXGPU_DriverPath DriverPath
         * @ENG_START_DOX
         * @brief Gets the driver registry path of a GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    DriverPath (const char** driverPath)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],driverPath,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the driver registry path of a GPU is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the driver path is successfully returned, __ADLX_OK__ is returned.<br>
         * If the driver path is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * The returned memory buffer is valid within a lifetime of the @ref DOX_IADLXGPU interface.<br>
         * If the application uses the driver path beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the driver path.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL DriverPath (const char** driverPath) const = 0;
        /**
         * @page DOX_IADLXGPU_PNPString PNPString
         * @ENG_START_DOX
         * @brief Gets the PNP string of a GPU.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    PNPString (const char** pnpString)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],pnpString,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the PNP string of a GPU is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the PNP string is successfully returned, __ADLX_OK__ is returned.<br>
         * If the PNP string is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * The returned memory buffer is valid within a lifetime of the @ref DOX_IADLXGPU interface.<br>
         * If the application uses the PNP string beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the PNP string.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL PNPString (const char** pnpString) const = 0;
        /**
         * @page DOX_IADLXGPU_HasDesktops HasDesktops
         * @ENG_START_DOX
         * @brief Checks if a GPU drives any desktops.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    HasDesktops (adlx_bool* hasDesktops)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out],hasDesktops,adlx_bool*,@ENG_START_DOX The pointer to a variable to check if a GPU drives any desktops is returned. The variable is __true__ if the GPU drives desktops. The variable is __false__ if the GPU does not drive any desktop. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If __HasDesktops__ is successfully executed, __ADLX_OK__ is returned.<br>
         * If __HasDesktops__ is not successfully executed, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL HasDesktops (adlx_bool* hasDesktops) const = 0;
        /**
        *@page DOX_IADLXGPU_TotalVRAM TotalVRAM
        *@ENG_START_DOX @brief Gets the total VRAM size of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TotalVRAM (adlx_uint* vramMB)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,vramMB,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the total VRAM size is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the total VRAM size is successfully returned, __ADLX_OK__ is returned.<br>
        * If the total VRAM size is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The total VRAM size is in MB. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TotalVRAM (adlx_uint* vramMB) = 0;
        /**
        * @page DOX_IADLXGPU_VRAMType VRAMType
        * @ENG_START_DOX@brief Gets the VRAM type of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    VRAMType(const char** type)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],type,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the VRAM type of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the VRAM type is successfully returned, __ADLX_OK__ is returned.<br>
        * If the VRAM type is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the VRAM type beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the VRAM type.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL VRAMType(const char** type) = 0;
        /**
        * @page DOX_IADLXGPU_BIOSInfo BIOSInfo
        * @ENG_START_DOX@brief Gets the BIOS info of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    BIOSInfo(const char** partNumber, const char** version, const char** date)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],partNumber,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the BIOS part number of the GPU is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],version,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the BIOS version of the GPU is returned. @ENG_END_DOX}
        * @paramrow{3.,[out],date,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the BIOS date of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the BIOS part number, BIOS version and BIOS date are successfully returned, __ADLX_OK__ is returned.<br>
        * If the BIOS part number, BIOS version and BIOS date are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * The returned memory buffers are valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the BIOS part number, BIOS version and BIOS date beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the BIOS part number, BIOS version and BIOS date.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL BIOSInfo(const char** partNumber, const char** version, const char** date) = 0;
        /**
        * @page DOX_IADLXGPU_DeviceId DeviceId
        * @ENG_START_DOX@brief Gets the device id of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    DeviceId(const char** deviceId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],deviceId,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the device id of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the device id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the device id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        * 
        * @detaileddesc
        * @ENG_START_DOX
        * @details The device id is a predetermined value programmed into the GPU at the point of manufacturing and consists of four hexadecimal digits.
        * @ENG_END_DOX
        * 
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the device id beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the device id.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL DeviceId(const char** deviceId) = 0;
        /**
        * @page DOX_IADLXGPU_RevisionId RevisionId
        * @ENG_START_DOX@brief Gets the revision id of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    RevisionId(const char** revisionId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],revisionId,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the revision id of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the revision id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the revision id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the revision id beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the revision id.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RevisionId(const char** revisionId) = 0;
        /**
        * @page DOX_IADLXGPU_SubSystemId SubSystemId
        * @ENG_START_DOX@brief Gets the subsystem id of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    SubSystemId(const char** subSystemId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],subSystemId,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the subsystem id of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the subsystem id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the subsystem id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the subsystem id beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the subsystem id.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SubSystemId(const char** subSystemId) = 0;
        /**
        * @page DOX_IADLXGPU_SubSystemVendorId SubSystemVendorId
        * @ENG_START_DOX@brief Gets the subsystem vendor id of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    SubSystemVendorId(const char** subSystemVendorId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],subSystemVendorId,const char**,@ENG_START_DOX The pointer to a zero-terminated string where the subsystem vendor id of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the subsystem vendor id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the subsystem vendor id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The subsystem vendor id is a predetermined value programmed into the GPU at the point of manufacturing and consists of four hexadecimal digits.
        * @ENG_END_DOX
        * 
        * @addinfo
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXGPU interface.<br>
        * If the application uses the subsystem vendor id beyond the lifetime of the @ref DOX_IADLXGPU interface, the application must make a copy of the subsystem vendor id.
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL SubSystemVendorId(const char** subSystemVendorId) = 0;
        /**
        * @page DOX_IADLXGPU_UniqueId UniqueId
        * @ENG_START_DOX@brief Gets the unique id of a GPU.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    UniqueId(adlx_int* uniqueId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],uniqueId,adlx_int*,@ENG_START_DOX The pointer to a variable where the unique id of the GPU is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the unique id is successfully returned, __ADLX_OK__ is returned.<br>
        * If the unique id is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL UniqueId(adlx_int* uniqueId) = 0;
    };  //IADLXGPU
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPU> IADLXGPUPtr;
}   //namespace adlx
#else
ADLX_DECLARE_IID (IADLXGPU, L"IADLXGPU");
typedef struct IADLXGPU IADLXGPU;

typedef struct IADLXGPUVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPU* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPU* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPU* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXGPU
    ADLX_RESULT (ADLX_STD_CALL *VendorId)(IADLXGPU* pThis, const char** vendorId);
    ADLX_RESULT (ADLX_STD_CALL *ASICFamilyType)(IADLXGPU* pThis, ADLX_ASIC_FAMILY_TYPE* asicFamilyType);
    ADLX_RESULT (ADLX_STD_CALL *Type)(IADLXGPU* pThis, ADLX_GPU_TYPE* gpuType);
    ADLX_RESULT (ADLX_STD_CALL *IsExternal)(IADLXGPU* pThis, adlx_bool* isExternal);
    ADLX_RESULT (ADLX_STD_CALL *Name)(IADLXGPU* pThis, const char** gpuName);
    ADLX_RESULT (ADLX_STD_CALL *DriverPath)(IADLXGPU* pThis, const char** driverPath);
    ADLX_RESULT (ADLX_STD_CALL *PNPString)(IADLXGPU* pThis, const char** pnpString);
    ADLX_RESULT (ADLX_STD_CALL *HasDesktops)(IADLXGPU* pThis, adlx_bool* hasDesktops);
    ADLX_RESULT (ADLX_STD_CALL *TotalVRAM)(IADLXGPU* pThis, adlx_uint* vramMB);
    ADLX_RESULT (ADLX_STD_CALL *VRAMType)(IADLXGPU* pThis, const char** type);
    ADLX_RESULT (ADLX_STD_CALL *BIOSInfo)(IADLXGPU* pThis, const char** partNumber, const char** version, const char** date);
    ADLX_RESULT (ADLX_STD_CALL *DeviceId)(IADLXGPU* pThis, const char** deviceId);
    ADLX_RESULT (ADLX_STD_CALL *RevisionId)(IADLXGPU* pThis, const char** revisionId);
    ADLX_RESULT (ADLX_STD_CALL *SubSystemId)(IADLXGPU* pThis, const char** subSystemId);
    ADLX_RESULT (ADLX_STD_CALL *SubSystemVendorId)(IADLXGPU* pThis, const char** subSystemVendorId);
    ADLX_RESULT (ADLX_STD_CALL *UniqueId)(IADLXGPU* pThis, adlx_int* uniqueId);
} IADLXGPUVtbl;

struct IADLXGPU
{
    const IADLXGPUVtbl *pVtbl;
};
#endif
#pragma endregion IADLXGPU interface

#pragma region IADLXGPUList interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXGPU::IID ())

        /**
        * @page DOX_IADLXGPUList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXGPU** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface.  @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXGPU** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the location is within the list bounds, __ADLX_OK__ is returned.<br>
        * If the location is not within the list bounds, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
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
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL At (const adlx_uint location, IADLXGPU** ppItem) = 0;
        /**
         * @page DOX_IADLXGPUList_Add_Back Add_Back
         * @ENG_START_DOX
         * @brief Adds an interface to the end of a list.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXGPU* pItem)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pItem,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the interface to be added to the list.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
         * If the interface is not added to the end of the list, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details
         * @ENG_END_DOX
         *
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL Add_Back (IADLXGPU* pItem) = 0;
    };  //IADLXGPUList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUList> IADLXGPUListPtr;
}   //namespace adlx
#else
ADLX_DECLARE_IID (IADLXGPUList, L"IADLXGPUList")
ADLX_DECLARE_ITEM_IID (IADLXGPU, IID_IADLXGPU ())

typedef struct IADLXGPUList IADLXGPUList;

typedef struct IADLXGPUListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXGPUList* pThis);
    adlx_uint8 (ADLX_STD_CALL *Empty)(IADLXGPUList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXGPUList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXGPUList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXGPUList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXGPUList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXGPUList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXGPUList* pThis, IADLXInterface* pItem);

    //IADLXGPUList
    ADLX_RESULT (ADLX_STD_CALL *At_GPUList)(IADLXGPUList* pThis, const adlx_uint location, IADLXGPU** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_GPUList)(IADLXGPUList* pThis, IADLXGPU* pItem);

} IADLXGPUListVtbl;

struct IADLXGPUList
{
    const IADLXGPUListVtbl *pVtbl;
};

#endif
#pragma endregion IADLXGPUList interface

#pragma region IADLXGPUsChangedHandling interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPUsEventListener
    {
    public:
        /**
        *@page DOX_IADLXGPUsEventListener_OnGPUListChanged OnGPUListChanged
        *@ENG_START_DOX @brief The __OnGPUListChanged__ is called by ADLX when the GPU list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnGPUListChanged (@ref DOX_IADLXGPUList* pNewGPUs)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,pNewGPUs,@ref DOX_IADLXGPUList* ,@ENG_START_DOX The pointer to the new GPU list. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  Once the application registers to the notifications with @ref DOX_IADLXGPUsChangedHandling_AddGPUsListEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXGPUsChangedHandling_RemoveGPUsListEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the new GPU list with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the new GPU list with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnGPUListChanged (IADLXGPUList* pNewGPUs) = 0;
    };

    class ADLX_NO_VTABLE IADLXGPUsChangedHandling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXGPUsChangedHandling")
        /**
        *@page DOX_IADLXGPUsChangedHandling_AddGPUsListEventListener AddGPUsListEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the GPU list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddGPUsListEventListener (@ref DOX_IADLXGPUsEventListener* pListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pListener,@ref DOX_IADLXGPUsEventListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the GPU list change notifications. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT  for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully registered, ADLX will call @ref DOX_IADLXGPUsEventListener_OnGPUListChanged of the listener when the GPU list changes.<br>
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXGPUsChangedHandling_RemoveGPUsListEventListener.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddGPUsListEventListener(IADLXGPUsEventListener* pListener) = 0;
       /**
        *@page DOX_IADLXGPUsChangedHandling_RemoveGPUsListEventListener RemoveGPUsListEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the GPU list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveGPUsListEventListener (@ref DOX_IADLXGPUsEventListener* pListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pListener,@ref DOX_IADLXGPUsEventListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the GPU list change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT  for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXGPUsEventListener_OnGPUListChanged method of the listener when the GPU list changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveGPUsListEventListener(IADLXGPUsEventListener* pListener) = 0;
    };

    //------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXGPUsChangedHandling> IADLXGPUsChangedHandlingPtr;
}
#else
ADLX_DECLARE_IID (IADLXGPUsChangedHandling, L"IADLXGPUsChangedHandling")
typedef struct IADLXGPUsEventListener IADLXGPUsEventListener;

typedef struct IADLXGPUsEventListenerVtbl
{
    // IADLXGPUsEventListener interface
    adlx_bool (ADLX_STD_CALL *OnGPUListChanged)(IADLXGPUsEventListener* pThis, IADLXGPUList* pNewGPUs);
} IADLXGPUsEventListenerVtbl;

struct IADLXGPUsEventListener
{
    const IADLXGPUsEventListenerVtbl *pVtbl;
};

typedef struct IADLXGPUsChangedHandling IADLXGPUsChangedHandling;

typedef struct IADLXGPUsChangedHandlingVtbl
{
    // IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXGPUsChangedHandling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXGPUsChangedHandling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXGPUsChangedHandling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLXGPUsChangedHandling
    ADLX_RESULT (ADLX_STD_CALL *AddGPUsListEventListener)(IADLXGPUsChangedHandling* pThis, IADLXGPUsEventListener* pListener);
    ADLX_RESULT (ADLX_STD_CALL *RemoveGPUsListEventListener)(IADLXGPUsChangedHandling* pThis, IADLXGPUsEventListener* pListener);

} IADLXGPUsChangedHandlingVtbl;

struct IADLXGPUsChangedHandling
{
    const IADLXGPUsChangedHandlingVtbl *pVtbl;
};

#endif
#pragma endregion IADLXGPUsChangedHandling interface

//IADLXSystem is a singleton interface, should not be deleted
#pragma region IADLXSystem interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDesktopServices;
    class ADLX_NO_VTABLE IADLXDisplayServices;
    class ADLX_NO_VTABLE IADLXLog;
    class ADLX_NO_VTABLE IADLX3DSettingsServices;
    class ADLX_NO_VTABLE IADLXGPUTuningServices;
    class ADLX_NO_VTABLE IADLXPerformanceMonitoringServices;
    class ADLX_NO_VTABLE IADLXI2C;

    class ADLX_NO_VTABLE IADLXSystem
    {
    public:
        /**
         * @page DOX_IADLXSystem_HybridGraphicsType HybridGraphicsType
         * @ENG_START_DOX
         * @brief Gets the hybrid graphics type of the system.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    HybridGraphicsType (@ref ADLX_HG_TYPE* hgType)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,hgType,@ref ADLX_HG_TYPE* ,@ENG_START_DOX The pointer to a variable where the hybrid graphics type is returned.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the hybrid graphics type is successfully returned, __ADLX_OK__ is returned. <br>
		 * If the hybrid graphics type is not successfully returned, an error code is returned. <br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The hybrid graphics type can be used to discover if the system is a hybrid graphics platform.
		 * Hybrid graphics platforms share cross-adapter resources between a discrete GPU and an integrated GPU. Applications can run on either GPU depending on their needs. The operating system and the AMD display driver together determine which GPU an application should run on.
		 *
		 * On an AMD hybrid graphics platform, the integrated GPU can be an AMD integrated GPU or a non-AMD integrated GPU.
		 *
         * @ENG_END_DOX
         *
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL HybridGraphicsType (ADLX_HG_TYPE* hgType) = 0;
        /**
         * @page DOX_IADLXSystem_GetGPUs GetGPUs
         * @ENG_START_DOX
         * @brief Gets the reference counted list of AMD GPUs.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetGPUs (@ref DOX_IADLXGPUList** ppGPUs)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,ppGPUs,@ref DOX_IADLXGPUList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPUs__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details For more information about the AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
		 * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         *@addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
		 *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUs (IADLXGPUList** ppGPUs) = 0;
        /**
         * @page DOX_IADLXSystem_QueryInterface QueryInterface
         * @ENG_START_DOX
         * @brief Gets reference counted extension interfaces to @ref DOX_IADLXSystem.
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
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL QueryInterface (const wchar_t* interfaceId, void** ppInterface) = 0;
        /**
         * @page DOX_IADLXSystem_GetDisplaysServices GetDisplaysServices
         * @ENG_START_DOX
         * @brief Gets the reference counted main interface to the @ref DOX_IADLXDisplayServices "Display" domain.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetDisplaysServices (@ref DOX_IADLXDisplayServices** ppDispServices)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,ppDispServices,@ref DOX_IADLXDisplayServices**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDispServices__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
		 * @addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplaysServices(IADLXDisplayServices** ppDispServices) = 0;
        /**
         * @page DOX_IADLXSystem_GetDesktopsServices GetDesktopsServices
         * @ENG_START_DOX
         * @brief Gets the reference counted main interface to the @ref DOX_IADLXDesktopServices "Desktop" domain.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetDesktopsServices (@ref DOX_IADLXDesktopServices** ppDeskServices)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,ppDeskServices,@ref DOX_IADLXDesktopServices**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address  __*ppDeskServices__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
		 *
		 * @addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetDesktopsServices(IADLXDesktopServices** ppDeskServices) = 0;

        /**
         * @page DOX_IADLXSystem_GetGPUsChangedHandling GetGPUsChangedHandling
         * @ENG_START_DOX
         * @brief Gets the reference counted interface to allow registering and unregistering for notifications when the GPU list changes.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetGPUsChangedHandling (@ref DOX_IADLXGPUsChangedHandling** ppGPUsChangedHandling)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,ppGPUsChangedHandling,@ref DOX_IADLXGPUsChangedHandling**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address  __*ppGPUsChangedHandling__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
		 *@addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
		 *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUsChangedHandling (IADLXGPUsChangedHandling** ppGPUsChangedHandling) = 0;
        /**
        * @page DOX_IADLXSystem_EnableLog EnableLog
        * @ENG_START_DOX
        * @brief Enables logging in ADLX.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    EnableLog (@ref ADLX_LOG_DESTINATION mode, @ref ADLX_LOG_SEVERITY severity, @ref DOX_IADLXLog* pLogger, const wchar_t* fileName)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,mode,@ref ADLX_LOG_DESTINATION,@ENG_START_DOX The log destination. @ENG_END_DOX}
        * @paramrow{2.,[in] ,severity,@ref ADLX_LOG_SEVERITY,@ENG_START_DOX The logging severity. @ENG_END_DOX}
        * @paramrow{3.,[in] ,pLogger,@ref DOX_IADLXLog*,@ENG_START_DOX The pointer to the log interface to receive log messages from ADLX. @ENG_END_DOX}
        * @paramrow{4.,[in] ,fileName,const wchar_t*,@ENG_START_DOX The zero-terminated string that specifies the path of the log file. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If __EnableLog__ is successfully executed, __ADLX_OK__ is returned. <br>
		* If __EnableLog__ is not successfully executed, an error code is returned. <br>
		* Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details By default, ADLX logging is disabled.
		* Use __EnableLog__ to configure log destination and severity.
		*
        * - To configure the log destination into a file, specify the __mode__ parameter as __LOCALFILE__ and provide the file path in the __filename__ parameter. The __pLogger__ parameter must be __nullptr__. <br>
		* - To configure the log destination into the output window of the debugger, specify the __mode__ parameter as __DBGVIEW__. The __filename__ and __pLogger__ parameters must be __nullptr__.<br>
		* - To configure the log destination to be sent as a string into the application, specify the __mode__ parameter as __APPLICATION__.  Implement the @ref DOX_IADLXLog interface  in the application and pass a pointer to an instance of that interface into the __pLogger__ parameter. The log instance must exist until ADLX is terminated. The __filename__ parameter must be __nullptr__.<br>
		* - To obtain the error messages in the ADLX execution code, specify the __severity__ parameter as __LERROR__.<br>
		* - To obtain the error and warning messages in the ADLX execution code, specify the __severity__ parameter as __LWARNING__.<br>
		* - To obtain error, warning, and debug tracing messages in the ADLX execution code, specify the __severity__ parameter as __LDEBUG__.
		* @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL EnableLog (ADLX_LOG_DESTINATION mode, ADLX_LOG_SEVERITY severity, IADLXLog* pLogger, const wchar_t* fileName) = 0;

        /**
         * @page DOX_IADLXSystem_Get3DSettingsServices Get3DSettingsServices
         * @ENG_START_DOX
         * @brief Gets the reference counted main interface to the @ref DOX_IADLX3DSettingsServices "3D Graphics" domain.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    Get3DSettingsServices (@ref DOX_IADLX3DSettingsServices** pp3DSettingsServices)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,pp3DSettingsServices,@ref DOX_IADLX3DSettingsServices**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*pp3DSettingsServices__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
	     * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
		 *@addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
		 *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL Get3DSettingsServices (IADLX3DSettingsServices** pp3DSettingsServices) = 0;

        /**
         * @page DOX_IADLXSystem_GetGPUTuningServices GetGPUTuningServices
         * @ENG_START_DOX
         * @brief Gets the reference counted main interface to the @ref DOX_IADLXGPUTuningServices "GPU Tuning" domain.
         * @ENG_END_DOX
         *
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetGPUTuningServices (@ref DOX_IADLXGPUTuningServices** ppGPUTuningServices)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out] ,ppGPUTuningServices,@ref DOX_IADLXGPUTuningServices**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPUTuningServices__ to __nullptr__. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
		 * If the interface is not successfully returned, an error code is returned.<br>
		 * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
		 *
		 *@addinfo
		 *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUTuningServices (IADLXGPUTuningServices** ppGPUTuningServices) = 0;

        /**
        *@page DOX_IADLXSystem_GetPerformanceMonitoringServices GetPerformanceMonitoringServices
        *@ENG_START_DOX @brief Gets the reference counted main interface to the @ref DOX_IADLXPerformanceMonitoringServices "Performance Monitoring" domain. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetPerformanceMonitoringServices (@ref DOX_IADLXPerformanceMonitoringServices** ppPerformanceMonitoringServices)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppPerformanceMonitoringServices,@ref DOX_IADLXPerformanceMonitoringServices** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppPerformanceMonitoringServices__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetPerformanceMonitoringServices (IADLXPerformanceMonitoringServices** ppPerformanceMonitoringServices) = 0;

        /**
        *@page DOX_IADLXSystem_TotalSystemRAM TotalSystemRAM
        *@ENG_START_DOX @brief Gets the size of the total RAM on this system. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TotalSystemRAM (adlx_uint* ramMB)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ramMB,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the total system RAM size is returned\, in MB. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TotalSystemRAM (adlx_uint* ramMB) = 0;

        /**
         *@page DOX_IADLXSystem_GetI2C GetI2C
         *@ENG_START_DOX @brief Gets the reference counted @ref DOX_IADLXI2C "I2C" interface of a GPU. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT    GetI2C (@ref DOX_IADLXGPU* pGPU, @ref DOX_IADLXI2C** ppI2C)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface. @ENG_END_DOX}
         *@paramrow{2.,[out] ,ppI2C,@ref DOX_IADLXI2C** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppI2C__ to __nullptr__. @ENG_END_DOX}
         *
         *@retvalues
         *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned. <br>
         * If the interface is not successfully returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
         *
         *@detaileddesc
         *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetI2C (IADLXGPU* pGPU, IADLXI2C** ppI2C) = 0;

    };  //IADLXSystem
}   //namespace adlx
#else

typedef struct IADLXDesktopServices IADLXDesktopServices;
typedef struct IADLXDisplayServices IADLXDisplayServices;
typedef struct IADLXLog IADLXLog;
typedef struct IADLX3DSettingsServices IADLX3DSettingsServices;
typedef struct IADLXSystem IADLXSystem;
typedef struct IADLXGPUTuningServices IADLXGPUTuningServices;
typedef struct IADLXPerformanceMonitoringServices IADLXPerformanceMonitoringServices;
typedef struct IADLXI2C IADLXI2C;

typedef struct IADLXSystemVtbl
{
    // IADLXSystem interface
    ADLX_RESULT (ADLX_STD_CALL *GetHybridGraphicsType)(IADLXSystem* pThis, ADLX_HG_TYPE* hgType);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUs)(IADLXSystem* pThis, IADLXGPUList** ppGPUs);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXSystem* pThis, const wchar_t* interfaceId, void** ppInterface);
    ADLX_RESULT (ADLX_STD_CALL *GetDisplaysServices)(IADLXSystem* pThis, IADLXDisplayServices** ppDispServices);
    ADLX_RESULT (ADLX_STD_CALL *GetDesktopsServices)(IADLXSystem* pThis, IADLXDesktopServices** ppDeskServices);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUsChangedHandling)(IADLXSystem* pThis, IADLXGPUsChangedHandling** ppGPUsChangedHandling);
    ADLX_RESULT (ADLX_STD_CALL *EnableLog)(IADLXSystem* pThis, ADLX_LOG_DESTINATION mode, ADLX_LOG_SEVERITY severity, IADLXLog* pLogger, const wchar_t* fileName);
    ADLX_RESULT (ADLX_STD_CALL *Get3DSettingsServices)(IADLXSystem* pThis, IADLX3DSettingsServices** pp3DSettingsServices);
    ADLX_RESULT (ADLX_STD_CALL *GetGPUTuningServices)(IADLXSystem* pThis, IADLXGPUTuningServices** ppGPUTuningServices);
    ADLX_RESULT (ADLX_STD_CALL *GetPerformanceMonitoringServices)(IADLXSystem* pThis, IADLXPerformanceMonitoringServices** ppPerformanceMonitoringServices);
    ADLX_RESULT (ADLX_STD_CALL *TotalSystemRAM)(IADLXSystem* pThis, adlx_uint* ramMB);
    ADLX_RESULT (ADLX_STD_CALL *GetI2C)(IADLXSystem* pThis, IADLXGPU* pGPU, IADLXI2C** ppI2C);
} IADLXSystemVtbl;

struct IADLXSystem
{
    const IADLXSystemVtbl *pVtbl;
};
#endif
#pragma endregion IADLXSystem interface

//IADLMapping is a singleton interface, should not be deleted
#pragma region IADLMapping interface
#if defined (__cplusplus)
namespace adlx
{
    //Interface used to interface between ADL and ADLX. This is useful in applications that use both ADL and ADLX,
    //where ADLX was initialized with the instance of ADL that is initialized by the application.
    //In such case the application might need to use both ADL and ADLX on the same GPU or display.
    //This interface provides conversion both ways between ADL and ADLX GPU and display identity
    class ADLX_NO_VTABLE IADLXDisplay;
    class ADLX_NO_VTABLE IADLXDesktop;

    class ADLX_NO_VTABLE IADLMapping
    {
    public:
        /**
         * @page DOX_IADLMapping_GetADLXGPUFromBdf GetADLXGPUFromBdf
         * @ENG_START_DOX
         * @brief Gets the reference counted @ref DOX_IADLXGPU interface corresponding to the GPU with the specified PCI bus number, device number, and function number.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetADLXGPUFromBdf (adlx_int bus, adlx_int device, adlx_int function, @ref DOX_IADLXGPU** ppGPU)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,bus,adlx_int,@ENG_START_DOX The unique PCIE bus number of the requested GPU. @ENG_END_DOX}
         * @paramrow{2.,[in] ,device,adlx_int,@ENG_START_DOX The device number of the requested GPU. @ENG_END_DOX}
         * @paramrow{3.,[in] ,function,adlx_int,@ENG_START_DOX The function number of the requested GPU. @ENG_END_DOX}
         * @paramrow{4.,[out],ppGPU,@ref DOX_IADLXGPU**,@ENG_START_DOX The address of the pointer to the returned interface. If the GPU was not found\, the method sets the dereferenced address __*ppGPU__ to __nullptr__.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXGPU is successfully returned, __ADLX_OK__ is returned. <br>
         * If the @ref DOX_IADLXGPU is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __GetADLXGPUFromBdf__ is used when an ADLX method must be called for a GPU obtained from ADL where an __AdapterInfo__ structure is available.<br>
         * The PCI bus number, device number, and function number parameters correspond to ADL __AdapterInfo.iBusNumber__, __AdapterInfo.iDeviceNumber__, and __AdapterInfo.iFunctionNumber__ for that GPU.<br>
         * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetADLXGPUFromBdf (adlx_int bus, adlx_int device, adlx_int function, IADLXGPU** ppGPU) = 0;
        /**
         * @page DOX_IADLMapping_GetADLXGPUFromAdlAdapterIndex GetADLXGPUFromAdlAdapterIndex
         * @ENG_START_DOX
         * @brief Gets the reference counted @ref DOX_IADLXGPU interface corresponding to the GPU with the specified ADL adapter index.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetADLXGPUFromAdlAdapterIndex (adlx_int adlAdapterIndex, @ref DOX_IADLXGPU** ppGPU)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in],adlAdapterIndex,adlx_int,@ENG_START_DOX The ADL adapter index for the requested GPU.  @ENG_END_DOX}
         * @paramrow{2.,[out],ppGPU,@ref DOX_IADLXGPU**,@ENG_START_DOX The address of the pointer to the returned interface. If the GPU was not found\, the method sets the dereferenced address __*ppGPU__ to __nullptr__.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXGPU is successfully returned, __ADLX_OK__ is returned. <br>
         * If the @ref DOX_IADLXGPU is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __GetADLXGPUFromAdlAdapterIndex__ is used when an  ADLX method must be called for a GPU obtained from ADL for which an ADL adapter index is available. <br>
         * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetADLXGPUFromAdlAdapterIndex (adlx_int adlAdapterIndex, IADLXGPU** ppGPU) = 0;
        /**
         * @page DOX_IADLMapping_BdfFromADLXGPU BdfFromADLXGPU
         * @ENG_START_DOX
         * @brief Gets the PCI bus number, device number, and function number corresponding to the GPU with the specified @ref DOX_IADLXGPU interface.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    BdfFromADLXGPU (@ref DOX_IADLXGPU* ppGPU, adlx_int* bus, adlx_int* device, adlx_int* function)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface.  @ENG_END_DOX}
         * @paramrow{2.,[out] ,bus,adlx_int* ,@ENG_START_DOX The pointer to a variable where the unique PCIE bus number of the requested GPU is returned.  @ENG_END_DOX}
         * @paramrow{3.,[out] ,device,adlx_int* ,@ENG_START_DOX The pointer to a variable where the device number of the requested GPU is returned.  @ENG_END_DOX}
         * @paramrow{4.,[out] ,function,adlx_int* ,@ENG_START_DOX The pointer to a variable where function number of the requested GPU is returned.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If bus, device, function are returned correctly, __ADLX_OK__ is returned.<br>
         * If bus, device, function are not returned correctly, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __BdfFromADLXGPU__ is used when an ADL function must be called for a GPU obtained from ADLX, and the GPU's PCI bus, device, and function are required for that ADL function.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL BdfFromADLXGPU (IADLXGPU* pGPU, adlx_int* bus, adlx_int* device, adlx_int* function) = 0;
        /**
         * @page DOX_IADLMapping_AdlAdapterIndexFromADLXGPU AdlAdapterIndexFromADLXGPU
         * @ENG_START_DOX
         * @brief Gets the ADL Adapter index corresponding to the GPU with the specified @ref DOX_IADLXGPU interface.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    AdlAdapterIndexFromADLXGPU (@ref DOX_IADLXGPU* ppGPU, adlx_int* adlAdapterIndex)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pGPU,@ref DOX_IADLXGPU* ,@ENG_START_DOX The pointer to the GPU interface.  @ENG_END_DOX}
         * @paramrow{2.,[out] ,adlAdapterIndex,adlx_int* ,@ENG_START_DOX The pointer to a variable where ADL adapter index of the requested GPU is returned.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the ADL adapter index is successfully returned, __ADLX_OK__ is returned. <br>
         * If the ADL Adapter index is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __AdlAdapterIndexFromADLXGPU__ is used when an ADL function must be called for a GPU obtained from ADLX, and the ADL adapter index is required for that ADL function.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL AdlAdapterIndexFromADLXGPU (IADLXGPU* pGPU, adlx_int* adlAdapterIndex) = 0;
        /**
         * @page DOX_IADLMapping_GetADLXDisplayFromADLIds GetADLXDisplayFromADLIds
         * @ENG_START_DOX
         * @brief Gets the reference counted @ref DOX_IADLXDisplay interface corresponding to the display with the specified ADL adapter index, display index, PCI bus number, device number, and function number.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetADLXDisplayFromADLIds (adlx_int adapterIndex, adlx_int displayIndex, adlx_int bus, adlx_int device, adlx_int function, @ref DOX_IADLXDisplay** ppDisplay)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,adapterIndex,adlx_int ,@ENG_START_DOX The ADL adapter index of the GPU where the requested display is connected. @ENG_END_DOX}
         * @paramrow{2.,[in] ,displayIndex,adlx_int ,@ENG_START_DOX The ADL logical display index of the requested display. @ENG_END_DOX}
         * @paramrow{3.,[in] ,bus,adlx_int ,@ENG_START_DOX The unique PCIE bus number of the GPU where the requested display is connected.  @ENG_END_DOX}
         * @paramrow{4.,[in] ,device,adlx_int ,@ENG_START_DOX The device number of the GPU where the requested display is connected. @ENG_END_DOX}
         * @paramrow{5.,[in] ,function,adlx_int ,@ENG_START_DOX The device number of the GPU where the requested display is connected. @ENG_END_DOX}
         * @paramrow{6.,[out] ,ppDisplay,@ref DOX_IADLXDisplay**,@ENG_START_DOX The address of the pointer to the returned interface. If the display was not found\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXDisplay interface is successfully returned, __ADLX_OK__ is returned. <br>
         * If the @ref DOX_IADLXDisplay interface is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __GetADLXDisplayFromADLIds__ is used when an ADLX method must be called for a display obtained from ADL. The ADL adapter index corresponds to the GPU to which the display is connected. <br>
         * The display index corresponds to the __ADLDisplayID.iDisplayLogicalIndex__ field for the display. <br>
         * The PCI bus number, device number, and function number parameters correspond to ADL __AdapterInfo.iBusNumber__, __AdapterInfo.iDeviceNumber__, and __AdapterInfo.iFunctionNumber__ for the GPU where that display is connected.<br>
         * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
         * @ENG_END_DOX
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetADLXDisplayFromADLIds (adlx_int adapterIndex, adlx_int displayIndex, adlx_int bus, adlx_int device, adlx_int function, IADLXDisplay** ppDisplay) = 0;
        /**
         * @page DOX_IADLMapping_ADLIdsFromADLXDisplay ADLIdsFromADLXDisplay
         * @ENG_START_DOX
         * @brief Gets the ADL Adapter index, display index, the PCI bus number, device number, and function number corresponding to the display with the specified @ref DOX_IADLXDisplay interface.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    ADLIdsFromADLXDisplay (@ref DOX_IADLXDisplay* pDisplay, adlx_int* adapterIndex, adlx_int* displayIndex, adlx_int* bus, adlx_int* device, adlx_int* function)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pDisplay, @ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
         * @paramrow{2.,[out] ,adapterIndex, adlx_int* ,@ENG_START_DOX The pointer to a variable where the ADL adapter index of the GPU that drives the requested display is returned. @ENG_END_DOX}
         * @paramrow{3.,[out] ,displayIndex, adlx_int* ,@ENG_START_DOX The pointer to a variable where the ADL logical display index of the GPU that drives the requested display is returned. @ENG_END_DOX}
         * @paramrow{4.,[out] ,bus,adlx_int*,@ENG_START_DOX The pointer to a variable where the unique PCIE bus number of the GPU that drives the requested display is returned. @ENG_END_DOX}
         * @paramrow{5.,[out] ,device,adlx_int*,@ENG_START_DOX The pointer to a variable where the device number of the GPU that drives the requested display is returned. @ENG_END_DOX}
         * @paramrow{6.,[out] ,function,adlx_int*,@ENG_START_DOX The pointer to a variable where the function number of the GPU that drives the requested display is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXDisplay interface is successfully returned, __ADLX_OK__ is returned. <br>
         * If the @ref DOX_IADLXDisplay  interface is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __ADLIdsFromADLXDisplay__ is used when an ADL function must be called for a display obtained from ADLX, or for the GPU where the display is connected.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL ADLIdsFromADLXDisplay (IADLXDisplay* pDisplay, adlx_int* adapterIndex, adlx_int* displayIndex, adlx_int* bus, adlx_int* device, adlx_int* function) = 0;
        /**
         * @page DOX_IADLMapping_GetADLXDesktopFromADLIds GetADLXDesktopFromADLIds
         * @ENG_START_DOX
         * @brief Gets the reference counted @ref DOX_IADLXDesktop interface corresponding to the desktop with the specified ADL adapter index, VidPnSource ID, the PCI bus number, device number, and function number.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    GetADLXDesktopFromADLIds (adlx_int adapterIndex, adlx_int VidPnSource, adlx_int bus, adlx_int device, adlx_int function, @ref DOX_IADLXDesktop** ppDesktop)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,adapterIndex,adlx_int ,@ENG_START_DOX The ADL adapter index of the GPU where the requested desktop is instantiated. @ENG_END_DOX}
         * @paramrow{2.,[in] ,VidPnSourceID,adlx_int ,@ENG_START_DOX The zero-based identification number of the video present source in a path of a video present network (VidPN) topology of the requested desktop. @ENG_END_DOX}
         * @paramrow{3.,[in] ,bus,adlx_int ,@ENG_START_DOX The unique PCIE bus number of the GPU where the requested desktop is instantiated. @ENG_END_DOX}
         * @paramrow{4.,[in] ,device,adlx_int ,@ENG_START_DOX The device number of the GPU where the requested desktop is instantiated. @ENG_END_DOX}
         * @paramrow{5.,[in] ,function,adlx_int ,@ENG_START_DOX The function number of the GPU where the requested desktop is instantiated. @ENG_END_DOX}
         * @paramrow{6.,[out] ,ppDesktop,@ref DOX_IADLXDesktop**,@ENG_START_DOX The address of the pointer to the returned interface. If the desktop was not found\, the method sets the dereferenced address __*ppDesktop__ to __nullptr__.  @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXDesktop interface is successfully returned, __ADLX_OK__ is returned. <br>
         * If the arguments  are incorrect __ADLX_INVALID_ARGS__ s returned. <br>
         * If @ref DOX_IADLXDesktop interface is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __GetADLXDesktopFromADLIds__ is used when and ADLX method must be called from a desktop obtained from ADL. The adapter index corresponds to the GPU where the desktop is instantiated.<br>
         * The PCI bus number, device number, and function number parameters correspond to ADL __AdapterInfo.iBusNumber__, __AdapterInfo.iDeviceNumber__, and __AdapterInfo.iFunctionNumber__ of the  GPU where that desktop is instantiated.<br>
         * The VidPnSource ID is obtained from __D3DKMT_OPENADAPTERFROMGDIDISPLAYNAME.VidPnSourceId__ field for this desktop.<br>
         * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
         * @ENG_END_DOX
         *
         * @addinfo
         * @ENG_START_DOX
         * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetADLXDesktopFromADLIds (adlx_int adapterIndex, adlx_int VidPnSourceId, adlx_int bus, adlx_int device, adlx_int function, IADLXDesktop** ppDesktop) = 0;
        /**
         * @page DOX_IADLMapping_ADLIdsFromADLXDesktop ADLIdsFromADLXDesktop
         * @ENG_START_DOX
         * @brief Gets the ADL Adapter index, Vid source ID, the PCIE bus number, device number, and function number corresponding to desktop with the specified @ref DOX_IADLXDesktop interface.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    ADLIdsFromADLXDesktop (@ref DOX_IADLXDesktop* pDesktop, adlx_int* adapterIndex, adlx_int* VidPnSourceId, adlx_int* bus, adlx_int* device, adlx_int* function)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[in] ,pDesktop, @ref DOX_IADLXDesktop* ,@ENG_START_DOX The pointer to the desktop interface. @ENG_END_DOX}
         * @paramrow{2.,[out] ,adapterIndex, adlx_int* ,@ENG_START_DOX The pointer to a variable where the ADL adapter index of the GPU that drives the requested desktop is returned. @ENG_END_DOX}
         * @paramrow{3.,[out] ,VidPnSourceId, adlx_int* ,@ENG_START_DOX The pointer to a variable where the zero-based identification number of the video present network (VidPN) topology of the requested desktop is returned. @ENG_END_DOX}
         * @paramrow{4.,[out] ,bus,adlx_int*,@ENG_START_DOX The pointer to a variable where the unique PCIE bus number of the GPU that drives the requested desktop is returned. @ENG_END_DOX}
         * @paramrow{5.,[out] ,device,adlx_int*,@ENG_START_DOX The pointer to a variable where the device number of the GPU that drives the requested desktop is returned. @ENG_END_DOX}
         * @paramrow{6.,[out] ,function,adlx_int*,@ENG_START_DOX The pointer to a variable where the function number of the GPU that drives the requested desktop is returned. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * If the @ref DOX_IADLXDesktop interface is successfully returned, __ADLX_OK__ is returned. <br>
         * If the @ref DOX_IADLXDesktop interface  is not returned, an error code is returned. <br>
         * Refer to @ref ADLX_RESULT for success codes and error codes
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details __ADLIdsFromADLXDesktop__ is used when an ADL function must be called for a desktop obtained from ADLX, or for the GPU that drives the desktop.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL ADLIdsFromADLXDesktop (IADLXDesktop* pDesktop, adlx_int* adapterIndex, adlx_int* VidPnSourceId, adlx_int* bus, adlx_int* device, adlx_int* function) = 0;
    }; // IADLMapping
}   //namespace adlx
#else
typedef struct IADLXDisplay IADLXDisplay;
typedef struct IADLXDesktop IADLXDesktop;
typedef struct IADLMapping IADLMapping;

typedef struct IADLMappingVtbl
{
    //Gets the IADLXGPU object corresponding to a GPU with given bus, device and function number
    ADLX_RESULT (ADLX_STD_CALL *GetADLXGPUFromBdf) (IADLMapping* pThis, adlx_int bus, adlx_int device, adlx_int function, IADLXGPU** ppGPU);
    //Gets the IADLXGPU object corresponding to a GPU with given ADL adapter index
    ADLX_RESULT (ADLX_STD_CALL *GetADLXGPUFromAdlAdapterIndex) (IADLMapping* pThis, adlx_int adlAdapterIndex, IADLXGPU** ppGPU);
    //Gets the bus, device and function number corresponding to the given IADLXGPU
    ADLX_RESULT (ADLX_STD_CALL *BdfFromADLXGPU) (IADLMapping* pThis, IADLXGPU* pGPU, adlx_int* bus, adlx_int* device, adlx_int* function);
    //Gets the ADL Adapter index corresponding to the given IADLXGPU
    ADLX_RESULT (ADLX_STD_CALL *AdlAdapterIndexFromADLXGPU) (IADLMapping* pThis, IADLXGPU* pGPU, adlx_int* adlAdapterIndex);
    //Gets the display object corresponding to the give ADL ids
    ADLX_RESULT (ADLX_STD_CALL *GetADLXDisplayFromADLIds) (IADLMapping* pThis, adlx_int adapterIndex, adlx_int displayIndex, adlx_int bus, adlx_int device, adlx_int function, IADLXDisplay** ppDisplay);
    //Gets ADL ids corresponding to the display object
    ADLX_RESULT (ADLX_STD_CALL *ADLIdsFromADLXDisplay) (IADLMapping* pThis, IADLXDisplay* pDisplay, adlx_int* adapterIndex, adlx_int* displayIndex, adlx_int* bus, adlx_int* device, adlx_int* function);
    //Gets the desktop object corresponding to the give ADL ids
    ADLX_RESULT (ADLX_STD_CALL *GetADLXDesktopFromADLIds) (IADLMapping* pThis, adlx_int adapterIndex, adlx_int VidPnSourceId, adlx_int bus, adlx_int device, adlx_int function, IADLXDesktop** ppDesktop);
    //Gets ADL ids corresponding to the desktop object
    ADLX_RESULT (ADLX_STD_CALL *ADLIdsFromADLXDesktop) (IADLMapping* pThis, IADLXDesktop* pDesktop, adlx_int* adapterIndex, adlx_int* VidPnSourceId, adlx_int* bus, adlx_int* device, adlx_int* function);

} IADLMappingVtbl;

struct IADLMapping
{
    const IADLMappingVtbl *pVtbl;
};
#endif
#pragma endregion IADLMapping interface

#endif  //ADLX_ISYSTEM_H