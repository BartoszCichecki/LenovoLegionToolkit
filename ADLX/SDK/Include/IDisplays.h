//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDISPLAYS_H
#define ADLX_IDISPLAYS_H
#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"
#include "IChangedEvent.h"

//-------------------------------------------------------------------------------------------------
//IDisplays.h - Interfaces for ADLX Display Information functionality

//Display interface
#pragma region IADLXDisplay
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXGPU;
    class ADLX_NO_VTABLE IADLXDisplayGamut;
    class ADLX_NO_VTABLE IADLXDisplayGamma;
    class ADLX_NO_VTABLE IADLXDisplay3DLUT;
    class ADLX_NO_VTABLE IADLXDisplayFreeSync;
    class ADLX_NO_VTABLE IADLXDisplayVSR;
    class ADLX_NO_VTABLE IADLXDisplayGPUScaling;
    class ADLX_NO_VTABLE IADLXDisplayScalingMode;
    class ADLX_NO_VTABLE IADLXDisplayIntegerScaling;
    class ADLX_NO_VTABLE IADLXDisplayColorDepth;
    class ADLX_NO_VTABLE IADLXDisplayPixelFormat;
    class ADLX_NO_VTABLE IADLXDisplayCustomColor;
    class ADLX_NO_VTABLE IADLXDisplayHDCP;
    class ADLX_NO_VTABLE IADLXDisplayCustomResolution;
    class ADLX_NO_VTABLE IADLXDisplayVariBright;
    class ADLX_NO_VTABLE IADLXDisplay : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplay")

        /**
        * @page DOX_IADLXDisplay_ManufacturerID ManufacturerID
        * @ENG_START_DOX
        * @brief Gets the manufacturer id of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    ManufacturerID (adlx_uint* manufacturerID)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,manufacturerID,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the manufacturer id is returned. A valid id is greater than zero. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the manufacturer id is successfully returned, __ADLX_OK__ is returned. <br>
        * If the manufacturer id is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The manufacturer id is a predetermined value programmed into the display at the time of manufacturing. A valid manufacturer id is an integer greater than zero. <br>
        * If zero is returned this means the display manufacturer id is invalid.
        * @ENG_END_DOX
        *
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL ManufacturerID (adlx_uint* manufacturerID) const = 0;

        /**
        * @page DOX_IADLXDisplay_DisplayType DisplayType
        * @ENG_START_DOX
        * @brief Gets the type of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    DisplayType (@ref ADLX_DISPLAY_TYPE* displayType)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,displayType,@ref ADLX_DISPLAY_TYPE* ,@ENG_START_DOX The pointer to a variable where the display type is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the type of this display is successfully returned, __ADLX_OK__ is returned. <br>
        * If the type of this display is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.

        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The display type can be used to discover if the display is a TV, a Digital Flat Panel, or any other kind of display.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL DisplayType (ADLX_DISPLAY_TYPE* displayType) const = 0;

        /**
        * @page DOX_IADLXDisplay_ConnectorType ConnectorType
        * @ENG_START_DOX
        * @brief Gets the connector type of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    ConnectorType (@ref ADLX_DISPLAY_CONNECTOR_TYPE* connectType)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,connectType,@ref ADLX_DISPLAY_CONNECTOR_TYPE* ,@ENG_START_DOX The pointer to a variable where the connector type is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the connector type is successfully returned, __ADLX_OK__ is returned. <br>
        * If the connector type is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The connector type can be used to discover if the display is connected to the GPU using HDMI, DisplayPort or any other kind of display interface.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL ConnectorType (ADLX_DISPLAY_CONNECTOR_TYPE* connectType) const = 0;

        /**
        * @page DOX_IADLXDisplay_Name Name
        * @ENG_START_DOX
        * @brief Gets the name of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    Name (const char** displayName)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,displayName,const char** ,@ENG_START_DOX The pointer to a zero-terminated string where the display name is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the name of this display is successfully returned, __ADLX_OK__ is returned. <br>
        * If the name of this display is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The display name is a predetermined value programmed into the display at the time of manufacturing.
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXDisplay interface. If the application uses the display name beyond the  lifetime of the IADLXDisplay @ref DOX_IADLXDisplay interface, the application must make a copy of the display name.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Name (const char** displayName) const = 0;

        /**
        * @page DOX_IADLXDisplay_EDID EDID
        * @ENG_START_DOX
        * @brief Gets the virtual EDID (Extended Display Identification Data) of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    EDID (const char** edid)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,edid,const char** ,@ENG_START_DOX The pointer to a zero-terminated buffer where the display EDID data is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the virtual EDID is successfully returned, __ADLX_OK__ is returned. <br>
        * If the virtual EDID is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details EDID is a standardized metadata programmed into the display device by the manufacturer and is used for interoperability by video sources to discover the capabilities and information about the display device. VESA (Video Electronics Standards Association) defines the standard for the EDID data.
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * The returned memory buffer is valid within the lifetime of the @ref DOX_IADLXDisplay interface. If the application uses the EDID beyond the  lifetime of the IADLXDisplay @ref DOX_IADLXDisplay interface, the application must make a copy of the EDID.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL EDID (const char** edid) const = 0;

        /**
        * @page DOX_IADLXDisplay_NativeResolution NativeResolution
        * @ENG_START_DOX
        * @brief Gets the native resolution of a  display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    NativeResolution (adlx_int* maxHResolution, adlx_int* maxVResolution)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,maxHResolution,adlx_int* ,@ENG_START_DOX The pointer to a variable where the horizontal resolution of the display is returned. @ENG_END_DOX}
        * @paramrow{2.,[out] ,maxVResolution,adlx_int* ,@ENG_START_DOX The pointer to a variable where the vertical resolution of the display is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the native resolution is successfully returned, __ADLX_OK__ is returned. <br>
        * If the resolution is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The  native resolution of the display is shown in the Windows Advanced display settings as Active signal resolution.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL NativeResolution (adlx_int* maxHResolution, adlx_int* maxVResolution) const = 0;

        /**
        * @page DOX_IADLXDisplay_RefreshRate RefreshRate
        * @ENG_START_DOX
        * @brief Gets the refresh rate of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    RefreshRate (adlx_double* refreshRate)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,refreshRate,adlx_double* ,@ENG_START_DOX The pointer to a variable where the refresh rate of the display (in Hz) is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the refresh rate is successfully returned, __ADLX_OK__ is returned. <br>
        * If the refresh rate is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The refresh rate returns the number of times per second the display shows a new image.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RefreshRate (adlx_double* refreshRate) const = 0;

        /**
        * @page DOX_IADLXDisplay_PixelClock PixelClock
        * @ENG_START_DOX
        * @brief Gets the pixel clock of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    PixelClock (adlx_uint* pixelClock)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,pixelClock,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the pixel clock of the display (in KHz) is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the pixel clock is successfully returned, __ADLX_OK__ is returned. <br>
        * If the pixel clock is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The pixel clock rate is the speed at which pixels are transmitted over a video signal such as HDMI or DVI, to fit a full frame of pixels in one refresh cycle.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL PixelClock (adlx_uint* pixelClock) const = 0;

        /**
        * @page DOX_IADLXDisplay_ScanType ScanType
        * @ENG_START_DOX
        * @brief Gets the scan type of a display.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    ScanType (@ref ADLX_DISPLAY_SCAN_TYPE* scanType)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,scanType,@ref ADLX_DISPLAY_SCAN_TYPE* ,@ENG_START_DOX The pointer to a variable where the scan type of the display is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the scan type is successfully returned, __ADLX_OK__ is returned. <br>
        * If the scan type is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details The scan type can be progressive or interlaced.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL ScanType (ADLX_DISPLAY_SCAN_TYPE* scanType) const = 0;

        /**
        *@page DOX_IADLXDisplay_GetGPU GetGPU
        *@ENG_START_DOX @brief Gets the reference counted GPU interface of a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPU (@ref DOX_IADLXGPU** ppGPU)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppGPU,@ref DOX_IADLXGPU** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPU__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the reference counted GPU interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the reference counted GPU interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPU (IADLXGPU** ppGPU) = 0;

        /**
        * @page DOX_IADLXDisplay_UniqueId UniqueId
        * @ENG_START_DOX@brief Gets the unique id of a Display.@ENG_END_DOX
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    UniqueId(adlx_size* uniqueId)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out],uniqueId,adlx_size*,@ENG_START_DOX The pointer to a variable where the unique id of the Display is returned. @ENG_END_DOX}
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
        virtual ADLX_RESULT ADLX_STD_CALL UniqueId(adlx_size* uniqueId) = 0;
    };  //IADLXDisplay
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplay> IADLXDisplayPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplay, L"IADLXDisplay")
typedef struct IADLXDisplayGamut IADLXDisplayGamut;
typedef struct IADLXDisplayGamma IADLXDisplayGamma;
typedef struct IADLXDisplay3DLUT IADLXDisplay3DLUT;
typedef struct IADLXGPU IADLXGPU;

typedef struct IADLXDisplay IADLXDisplay;

typedef struct IADLXDisplayVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplay* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplay* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplay* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXDisplay
    ADLX_RESULT (ADLX_STD_CALL* ManufacturerID)( IADLXDisplay* pThis, adlx_uint* manufacturerID );
    ADLX_RESULT (ADLX_STD_CALL* DisplayType)( IADLXDisplay* pThis, ADLX_DISPLAY_TYPE* displayType );
    ADLX_RESULT (ADLX_STD_CALL* ConnectorType)( IADLXDisplay* pThis, ADLX_DISPLAY_CONNECTOR_TYPE* connectType );
    ADLX_RESULT (ADLX_STD_CALL* Name)( IADLXDisplay* pThis, const char** displayName );
    ADLX_RESULT (ADLX_STD_CALL* EDID)( IADLXDisplay* pThis, const char** edid );
    ADLX_RESULT (ADLX_STD_CALL* NativeResolution)( IADLXDisplay* pThis, adlx_int* maxHResolution, adlx_int* maxVResolution );
    ADLX_RESULT (ADLX_STD_CALL* RefreshRate)( IADLXDisplay* pThis, adlx_double* refreshRate );
    ADLX_RESULT (ADLX_STD_CALL* PixelClock)( IADLXDisplay* pThis, adlx_uint* pixelClock );
    ADLX_RESULT (ADLX_STD_CALL* ScanType)( IADLXDisplay* pThis, ADLX_DISPLAY_SCAN_TYPE* scanType );
    ADLX_RESULT (ADLX_STD_CALL* GetGPU)( IADLXDisplay* pThis, IADLXGPU** ppGPU );
    ADLX_RESULT (ADLX_STD_CALL* UniqueId)( IADLXDisplay* pThis, adlx_size* uniqueId );
}IADLXDisplayVtbl;

struct IADLXDisplay { const IADLXDisplayVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplay

//Display list interface
#pragma region IADLXDisplayList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXDisplay::IID ())

        /**
        * @page DOX_IADLXDisplayList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXDisplay** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXDisplay** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
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
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXDisplay** ppItem) = 0;

        /**
        *@page DOX_IADLXDisplayList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXDisplay* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
        * If the interface is not added to the end of the list, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXDisplay* pItem) = 0;
    };  //IADLXDisplayList
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayList> IADLXDisplayListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayList, L"IADLXDisplayList")
ADLX_DECLARE_ITEM_IID (IADLXDisplay, IID_IADLXDisplay ())

typedef struct IADLXDisplayList IADLXDisplayList;

typedef struct IADLXDisplayListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplayList* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplayList* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplayList* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXList
    adlx_uint (ADLX_STD_CALL* Size)( IADLXDisplayList* pThis );
    adlx_bool (ADLX_STD_CALL* Empty)( IADLXDisplayList* pThis );
    adlx_uint (ADLX_STD_CALL* Begin)( IADLXDisplayList* pThis );
    adlx_uint (ADLX_STD_CALL* End)( IADLXDisplayList* pThis );
    ADLX_RESULT (ADLX_STD_CALL* At)( IADLXDisplayList* pThis, const adlx_uint location, IADLXInterface** ppItem );
    ADLX_RESULT (ADLX_STD_CALL* Clear)( IADLXDisplayList* pThis );
    ADLX_RESULT (ADLX_STD_CALL* Remove_Back)( IADLXDisplayList* pThis );
    ADLX_RESULT (ADLX_STD_CALL* Add_Back)( IADLXDisplayList* pThis, IADLXInterface* pItem );

    //IADLXDisplayList
    ADLX_RESULT (ADLX_STD_CALL* At_DisplayList)( IADLXDisplayList* pThis, const adlx_uint location, IADLXDisplay** ppItem );
    ADLX_RESULT (ADLX_STD_CALL* Add_Back_DisplayList)( IADLXDisplayList* pThis, IADLXDisplay* pItem );

}IADLXDisplayListVtbl;

struct IADLXDisplayList { const IADLXDisplayListVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayList

//Display List changed listener interface. To be implemented in application and passed in IADLXDisplayChangedHandling::AddDisplayListEventListener()
#pragma region IADLXDisplayListChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayListChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDisplayListChangedListener_OnDisplayListChanged OnDisplayListChanged
        *@ENG_START_DOX @brief The __OnDisplayListChanged__ method is called by ADLX when the display list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDisplayListChanged (@ref DOX_IADLXDisplayList* pNewDisplay)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,pNewDisplay,@ref DOX_IADLXDisplayList* ,@ENG_START_DOX The pointer to the new display list. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * Once the application registers to the notifications with @ref DOX_IADLXDisplayChangedHandling_AddDisplayListEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayListEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the new display list with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the new display list with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDisplayListChanged (IADLXDisplayList* pNewDisplay) = 0;
    }; //IADLXDisplayListChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDisplayListChangedListener IADLXDisplayListChangedListener;

typedef struct IADLXDisplayListChangedListenerVtbl
{
    // IDisplayEventListener interface
    adlx_bool (ADLX_STD_CALL* OnDisplayListChanged)( IADLXDisplayListChangedListener* pThis, IADLXDisplayList* pNewDisplay );

} IADLXDisplayListChangedListenerVtbl;

struct IADLXDisplayListChangedListener { const IADLXDisplayListChangedListenerVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayListChangedListener

//Interface with information on gamut changes on a display. ADLX passes this to application that registered for Gamut changed event in the IADLXDisplayGamutChangedListener::OnDisplayGamutChanged()
#pragma region IADLXDisplayGamutChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGamutChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayGamutChangedEvent")

        /**
        *@page DOX_IADLXDisplayGamutChangedEvent_GetDisplay GetDisplay
        *@ENG_START_DOX @brief Gets the reference counted display interface on which the gamut setting is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetDisplay (@ref DOX_IADLXDisplay** ppDisplay)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppDisplay,@ref DOX_IADLXDisplay** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the display interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the display inferface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLXDisplayGamutChangedEvent_GetDisplay returns the reference counted display used by all the methods in this interface to check if there are any changes.
        *@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplay (IADLXDisplay** ppDisplay) = 0;

        /**
        *@page DOX_IADLXDisplayGamutChangedEvent_IsWhitePointChanged IsWhitePointChanged
        *@ENG_START_DOX @brief Checks if the white point of the display gamut is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsWhitePointChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the white point is changed, __true__ is returned.<br>
        * If the white point is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGamutChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsWhitePointChanged () = 0;

        /**
        *@page DOX_IADLXDisplayGamutChangedEvent_IsColorSpaceChanged IsColorSpaceChanged
        *@ENG_START_DOX @brief Checks if the color space of the display gamut is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsColorSpaceChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the color space is changed, __true__ is returned.<br>
        * If the color space is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGamutChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsColorSpaceChanged () = 0;
    }; //IADLXDisplayGamutChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayGamutChangedEvent> IADLXDisplayGamutChangedEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayGamutChangedEvent, L"IADLXDisplayGamutChangedEvent")
typedef struct IADLXDisplayGamutChangedEvent IADLXDisplayGamutChangedEvent;

typedef struct IADLXDisplayGamutChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplayGamutChangedEvent* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplayGamutChangedEvent* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplayGamutChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXDisplayGamutChangedEvent* pThis);

    // IADLXDisplayGamutChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL* GetDisplay)( IADLXDisplayGamutChangedEvent* pThis, IADLXDisplay** ppDisplay );
    adlx_bool (ADLX_STD_CALL* IsWhitePointChanged)( IADLXDisplayGamutChangedEvent* pThis );
    adlx_bool (ADLX_STD_CALL* IsColorSpaceChanged)( IADLXDisplayGamutChangedEvent* pThis );

} IADLXDisplayGamutChangedEventVtbl;

struct IADLXDisplayGamutChangedEvent { const IADLXDisplayGamutChangedEventVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayGamutChangedEvent

//Display Gamut changed listener interface. To be implemented in application and passed in IADLXDisplayChangedHandling::AddDisplayGamutEventListener()
#pragma region IADLXDisplayGamutChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGamutChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDisplayGamutChangedListener_OnDisplayGamutChanged OnDisplayGamutChanged
        *@ENG_START_DOX @brief The __OnDisplayGamutChanged__ method is called by ADLX when the display gamut changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDisplayGamutChanged (@ref DOX_IADLXDisplayGamutChangedEvent* pDisplayGamutChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGamutChangedEvent,@ref DOX_IADLXDisplayGamutChangedEvent* ,@ENG_START_DOX The pointer to the event. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * Once the application registers to the notifications with @ref DOX_IADLXDisplayChangedHandling_AddDisplayGamutEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayGamutEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the gamut change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the gamut change event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDisplayGamutChanged (IADLXDisplayGamutChangedEvent* pDisplayGamutChangedEvent) = 0;
    }; //IADLXDisplayGamutChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDisplayGamutChangedListener IADLXDisplayGamutChangedListener;

typedef struct IADLXDisplayGamutChangedListenerVtbl
{
    // IADLXDisplayGamutChangedListener interface
    adlx_bool (ADLX_STD_CALL* OnDisplayGamutChanged)( IADLXDisplayGamutChangedListener* pThis, IADLXDisplayGamutChangedEvent* pDisplayGamutChangedEvent );

} IADLXDisplayGamutChangedListenerVtbl;

struct IADLXDisplayGamutChangedListener { const IADLXDisplayGamutChangedListenerVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayGamutChangedListener

//Interface with information on gamma changes on a display. ADLX passes this to application that registered for Gamma changed event in the IADLXDisplayGammaChangedListener::OnDisplayGammaChanged()
#pragma region IADLXDisplayGammaChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGammaChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayGammaChangedEvent")

        /**
        *@page DOX_IADLXDisplayGammaChangedEvent_GetDisplay GetDisplay
        *@ENG_START_DOX @brief Gets the reference counted display interface on which the gamma setting is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetDisplay (@ref DOX_IADLXDisplay** ppDisplay)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppDisplay,@ref DOX_IADLXDisplay** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the display interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the display inferface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLXDisplayGammaChangedEvent_GetDisplay returns the reference counted display used by all the methods in this interface to check if there are any changes.
        *@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplay (IADLXDisplay** ppDisplay) = 0;

        /**
        *@page DOX_IADLXDisplayGammaChangedEvent_IsGammaRampChanged IsGammaRampChanged
        *@ENG_START_DOX @brief Checks if the gamma ramp of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsGammaRampChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the gamma ramp is changed, __true__ is returned.<br>
        * If the gamma ramp is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGammaChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsGammaRampChanged () = 0;

        /**
        *@page DOX_IADLXDisplayGammaChangedEvent_IsGammaCoefficientChanged IsGammaCoefficientChanged
        *@ENG_START_DOX @brief Checks if the gamma coefficient of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsGammaCoefficientChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the gamma coefficient is changed, __true__ is returned.<br>
        * If the gamma coefficient is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGammaChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsGammaCoefficientChanged () = 0;

        /**
        *@page DOX_IADLXDisplayGammaChangedEvent_IsReGammaChanged IsReGammaChanged
        *@ENG_START_DOX @brief Checks if the re-gamma of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsReGammaChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the re-gamma is changed, __true__ is returned.<br>
        * If the re-gamma is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGammaChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsReGammaChanged () = 0;

        /**
        *@page DOX_IADLXDisplayGammaChangedEvent_IsDeGammaChanged IsDeGammaChanged
        *@ENG_START_DOX @brief Checks if the de-gamma of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsDeGammaChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the de-gamma is changed, __true__ is returned.<br>
        * If the de-gamma is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplayGammaChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsDeGammaChanged () = 0;
    }; //IADLXDisplayGammaChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayGammaChangedEvent> IADLXDisplayGammaChangedEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayGammaChangedEvent, L"IADLXDisplayGammaChangedEvent")
typedef struct IADLXDisplayGammaChangedEvent IADLXDisplayGammaChangedEvent;

typedef struct IADLXDisplayGammaChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplayGammaChangedEvent* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplayGammaChangedEvent* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplayGammaChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXDisplayGammaChangedEvent* pThis);

    // IADLXDisplayGammaChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL* GetDisplay)( IADLXDisplayGammaChangedEvent* pThis, IADLXDisplay** ppDisplay );
    adlx_bool (ADLX_STD_CALL* IsGammaRampChanged)( IADLXDisplayGammaChangedEvent* pThis );
    adlx_bool (ADLX_STD_CALL* IsGammaCoefficientChanged)( IADLXDisplayGammaChangedEvent* pThis );
    adlx_bool (ADLX_STD_CALL* IsReGammaChanged)( IADLXDisplayGammaChangedEvent* pThis );
    adlx_bool (ADLX_STD_CALL* IsDeGammaChanged)( IADLXDisplayGammaChangedEvent* pThis );

} IADLXDisplayGammaChangedEventVtbl;

struct IADLXDisplayGammaChangedEvent { const IADLXDisplayGammaChangedEventVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayGammaChangedEvent

//Display Gamma changed listener interface. To be implemented in application and passed in IADLXDisplayChangedHandling::AddDisplayGammaEventListener()
#pragma region IADLXDisplayGammaChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayGammaChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDisplayGammaChangedListener_OnDisplayGammaChanged OnDisplayGammaChanged
        *@ENG_START_DOX @brief The __OnDisplayGammaChanged__ method is called by ADLX when the display gamma changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDisplayGammaChanged (@ref DOX_IADLXDisplayGammaChangedEvent* pDisplayGammaChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGammaChangedEvent,@ref DOX_IADLXDisplayGammaChangedEvent* ,@ENG_START_DOX The pointer to the event. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * Once the application registers to the notifications with @ref DOX_IADLXDisplayChangedHandling_AddDisplayGammaEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayGammaEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the gamma change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the gamma change event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDisplayGammaChanged (IADLXDisplayGammaChangedEvent* pDisplayGammaChangedEvent) = 0;
    }; //IADLXDisplayGammaChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDisplayGammaChangedListener IADLXDisplayGammaChangedListener;

typedef struct IADLXDisplayGammaChangedListenerVtbl
{
    // IADLXDisplayGammaChangedListener interface
    adlx_bool (ADLX_STD_CALL* OnDisplayGammaChanged)( IADLXDisplayGammaChangedListener* pThis, IADLXDisplayGammaChangedEvent* pDisplayGammaChangedEvent );

} IADLXDisplayGammaChangedListenerVtbl;

struct IADLXDisplayGammaChangedListener { const IADLXDisplayGammaChangedListenerVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayGammaChangedListener

//Interface with information on 3D LUT changes on a display. ADLX passes this to application that registered for 3D LUT changed event in the IADLXDisplay3DLUTChangedListener::OnDisplay3DLUTChanged()
#pragma region IADLXDisplay3DLUTChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplay3DLUTChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplay3DLUTChangedEvent")

        /**
        *@page DOX_IADLXDisplay3DLUTChangedEvent_GetDisplay GetDisplay
        *@ENG_START_DOX @brief Gets the reference counted display interface on which the 3D LUT is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetDisplay (@ref DOX_IADLXDisplay** ppDisplay)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,ppDisplay,@ref DOX_IADLXDisplay** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the display interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the display inferface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLXDisplay3DLUTChangedEvent_GetDisplay returns the reference counted display used by all the methods in this interface to check if there are any changes.
        *@ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplay (IADLXDisplay** ppDisplay) = 0;
        /**
        *@page DOX_IADLXDisplay3DLUTChangedEvent_IsSCEChanged IsSCEChanged
        *@ENG_START_DOX @brief Checks if the color enhancement of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsSCEChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the color enhancement is changed, __true__ is returned.<br>
        * If the color enhancement is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplay3DLUTChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsSCEChanged() = 0;
        /**
        *@page DOX_IADLXDisplay3DLUTChangedEvent_IsCustom3DLUTChanged IsCustom3DLUTChanged
        *@ENG_START_DOX @brief Checks if the custom 3D LUT for panel calibration of a display is changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustom3DLUTChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the custom 3D LUT is changed, __true__ is returned.<br>
        * If the custom 3D LUT is not changed, __false__ is returned.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplay3DLUTChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustom3DLUTChanged() = 0;
    }; //IADLXDisplay3DLUTChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplay3DLUTChangedEvent> IADLXDisplay3DLUTChangedEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplay3DLUTChangedEvent, L"IADLXDisplay3DLUTChangedEvent")
typedef struct IADLXDisplay3DLUTChangedEvent IADLXDisplay3DLUTChangedEvent;

typedef struct IADLXDisplay3DLUTChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplay3DLUTChangedEvent* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplay3DLUTChangedEvent* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplay3DLUTChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXDisplay3DLUTChangedEvent* pThis);

    // IADLXDisplay3DLUTChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL* GetDisplay)( IADLXDisplay3DLUTChangedEvent* pThis, IADLXDisplay** ppDisplay );
    adlx_bool (ADLX_STD_CALL* IsSCEChanged)( IADLXDisplay3DLUTChangedEvent* pThis );
    adlx_bool (ADLX_STD_CALL* IsCustom3DLUTChanged)( IADLXDisplay3DLUTChangedEvent* pThis );
} IADLXDisplay3DLUTChangedEventVtbl;

struct IADLXDisplay3DLUTChangedEvent { const IADLXDisplay3DLUTChangedEventVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplay3DLUTChangedEvent

//Display 3D LUT changed listener interface. To be implemented in application and passed in IADLXDisplayChangedHandling::AddDisplay3DLUTEventListener()
#pragma region IADLXDisplay3DLUTChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplay3DLUTChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDisplay3DLUTChangedListener_OnDisplay3DLUTChanged OnDisplay3DLUTChanged
        *@ENG_START_DOX @brief The __OnDisplay3DLUTChanged__ method is called by ADLX when the display 3D LUT changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDisplay3DLUTChanged (@ref DOX_IADLXDisplay3DLUTChangedEvent* pDisplay3DLUTChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay3DLUTChangedEvent,@ref DOX_IADLXDisplay3DLUTChangedEvent* ,@ENG_START_DOX The pointer to the event. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * Once the application registers to the notifications with @ref DOX_IADLXDisplayChangedHandling_AddDisplay3DLUTEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplay3DLUTEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the 3D LUT change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the 3D LUT change event with @ref DOX_IADLXInterface_Release.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDisplay3DLUTChanged (IADLXDisplay3DLUTChangedEvent* pDisplay3DLUTChangedEvent) = 0;
    }; //IADLXDisplay3DLUTChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDisplay3DLUTChangedListener IADLXDisplay3DLUTChangedListener;

typedef struct IADLXDisplay3DLUTChangedListenerVtbl
{
    // IADLXDisplayGammaChangedListener interface
    adlx_bool (ADLX_STD_CALL* OnDisplay3DLUTChanged)( IADLXDisplay3DLUTChangedListener* pThis, IADLXDisplay3DLUTChangedEvent* pDisplay3DLUTChangedEvent );

} IADLXDisplay3DLUTChangedListenerVtbl;

struct IADLXDisplay3DLUTChangedListener { const IADLXDisplay3DLUTChangedListenerVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplay3DLUTChangedListener

#pragma region IADLXDisplaySettingsChangedEvent
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplay;

    class ADLX_NO_VTABLE IADLXDisplaySettingsChangedEvent : public IADLXChangedEvent
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplaySettingsChangedEvent")

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_GetDisplay GetDisplay
        *@ENG_START_DOX @brief Gets the reference counted display interface on which settings are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetDisplay (@ref DOX_IADLXDisplay **ppDisplay)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppDisplay,@ref DOX_IADLXDisplay**, @ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the display interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the display interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. <br>
        * __Note:__ @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay returns the reference counted display used by all the methods in this interface to check if there are any changes.
        * @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "displaySetting.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplay (IADLXDisplay** ppDisplay) = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsFreeSyncChanged IsFreeSyncChanged
        *@ENG_START_DOX @brief Checks if the AMD FreeSync™ settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsFreeSyncChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the AMD FreeSync settings are changed, __true__ is returned.<br>
        * If the AMD FreeSync settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsFreeSyncChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsVSRChanged IsVSRChanged
        *@ENG_START_DOX @brief Checks if the AMD Virtual Super Resolution settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsVSRChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the Virtual Super Resolution settings are changed, __true__ is returned. <br>
        * If the Virtual Super Resolution settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsVSRChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsGPUScalingChanged IsGPUScalingChanged
        *@ENG_START_DOX @brief Checks if the GPU scaling settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsGPUScalingChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the GPU scaling settings are changed, __true__ is returned. <br>
        * If the GPU scaling settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        * @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsGPUScalingChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsScalingModeChanged IsScalingModeChanged
        *@ENG_START_DOX @brief Checks if the scaling mode settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsScalingModeChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the scaling mode settings are changed, __true__ is returned.<br>
        * If the scaling mode settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsScalingModeChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsIntegerScalingChanged IsIntegerScalingChanged
        *@ENG_START_DOX @brief Checks if the Integer Display Scaling settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsIntegerScalingChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the Integer Display Scaling settings are changed, __true__ is returned. <br>
        * If the Integer Display Scaling settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsIntegerScalingChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsColorDepthChanged IsColorDepthChanged
        *@ENG_START_DOX @brief Checks if the color format settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsColorDepthChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the color format settings are changed, __true__ is returned. <br>
        * If the color format settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsColorDepthChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsPixelFormatChanged IsPixelFormatChanged
        *@ENG_START_DOX @brief Checks if the pixel format settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsPixelFormatChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the pixel format settings are changed, __true__ is returned.<br>
        * If the pixel format settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsPixelFormatChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsHDCPChanged IsHDCPChanged
        *@ENG_START_DOX @brief Checks if the HDCP settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsHDCPChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the HDCP settings are changed, __true__ is returned. <br>
        * If the HDCP settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsHDCPChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomColorHueChanged IsCustomColorHueChanged
        *@ENG_START_DOX @brief Checks if the hue settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomColorHueChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the hue settings are changed, __true__ is returned. <br>
        * If the hue settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomColorHueChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomColorSaturationChanged IsCustomColorSaturationChanged
        *@ENG_START_DOX @brief Checks if the saturation settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomColorSaturationChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the saturation settings are changed, __true__ is returned. <br>
        * If the saturation settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomColorSaturationChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomColorBrightnessChanged IsCustomColorBrightnessChanged
        *@ENG_START_DOX @brief Checks if the brightness settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomColorBrightnessChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the brightness settings are changed, __true__ is returned. <br>
        * If the brightness settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomColorBrightnessChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomColorTemperatureChanged IsCustomColorTemperatureChanged
        *@ENG_START_DOX @brief Checks if the color temperature settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomColorTemperatureChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the color temperature settings are changed, __true__ is returned. <br>
        * If the color temperature settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomColorTemperatureChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomColorContrastChanged IsCustomColorContrastChanged
        *@ENG_START_DOX @brief Checks if the color contrast settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomColorContrastChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the color contrast settings are changed, __true__ is returned. <br>
        * If the color contrast settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomColorContrastChanged () = 0;

        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsCustomResolutionChanged IsCustomResolutionChanged
        *@ENG_START_DOX @brief Checks if the resolution settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsCustomResolutionChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the custom resolution settings are changed, __true__ is returned. <br>
        * If the custom resolution settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsCustomResolutionChanged () = 0;
        /**
        *@page DOX_IADLXDisplaySettingsChangedEvent_IsVariBrightChanged IsVariBrightChanged
        *@ENG_START_DOX @brief Checks if the Vari-Bright settings of the display are changed. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    IsVariBrightChanged ()
        *@codeEnd
        *
        *@params
        *N/A
        *
        *@retvalues
        *@ENG_START_DOX
        * If the Vari-Bright settings are changed, __true__ is returned. <br>
        * If the Vari-Bright settings are not changed, __false__ is returned. @ENG_END_DOX
        *
        *
        *@addinfo
        *@ENG_START_DOX
        * __Note:__ To obtain the display, use @ref DOX_IADLXDisplaySettingsChangedEvent_GetDisplay.
        *@ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool   ADLX_STD_CALL IsVariBrightChanged () = 0;
    }; //IADLXDisplaySettingsChangedEvent
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplaySettingsChangedEvent> IADLXDisplaySettingsChangedEventPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplaySettingsChangedEvent, L"IADLXDisplaySettingsChangedEvent")
typedef struct IADLXDisplaySettingsChangedEvent IADLXDisplaySettingsChangedEvent;

typedef struct IADLXDisplaySettingsChangedEventVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_long (ADLX_STD_CALL* Release)(IADLXDisplaySettingsChangedEvent* pThis);
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)(IADLXDisplaySettingsChangedEvent* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXChangedEvent
    ADLX_SYNC_ORIGIN(ADLX_STD_CALL* GetOrigin)(IADLXDisplaySettingsChangedEvent* pThis);

    // IADLXDisplaySettingsChangedEvent interface
    ADLX_RESULT (ADLX_STD_CALL* GetDisplay)(IADLXDisplaySettingsChangedEvent* pThis, IADLXDisplay** ppDisplay);
    adlx_bool (ADLX_STD_CALL* IsFreeSyncChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsVSRChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsGPUScalingChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsScalingModeChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsIntegerScalingChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsColorDepthChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsPixelFormatChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsHDCPChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomColorHueChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomColorSaturationChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomColorBrightnessChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomColorTemperatureChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomColorContrastChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsCustomResolutionChanged)(IADLXDisplaySettingsChangedEvent* pThis);
    adlx_bool (ADLX_STD_CALL* IsVariBrightChanged)(IADLXDisplaySettingsChangedEvent* pThis);

} IADLXDisplaySettingsChangedEventVtbl;

struct IADLXDisplaySettingsChangedEvent { const IADLXDisplaySettingsChangedEventVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplaySettingsChangedEvent

#pragma region IADLXDisplaySettingsChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplaySettingsChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDisplaySettingsChangedListener_OnDisplaySettingsChanged OnDisplaySettingsChanged
        *@ENG_START_DOX @brief The __OnDisplaySettingsChanged__ method is called by ADLX when the display settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDisplaySettingsChanged (@ref DOX_IADLXDisplaySettingsChangedEvent* pDisplaySettingChangedEvent)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplaySettingChangedEvent,@ref DOX_IADLXDisplaySettingsChangedEvent* ,@ENG_START_DOX The pointer to the display settings change event. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * Once the application registers to the notifications with @ref DOX_IADLXDisplayChangedHandling_AddDisplaySettingsEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplaySettingsEventListener.
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the display settings change event with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the display settings change event with @ref DOX_IADLXInterface_Release. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IdisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDisplaySettingsChanged (IADLXDisplaySettingsChangedEvent* pDisplaySettingChangedEvent) = 0;
    }; //IADLXDisplaySettingsChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDisplaySettingsChangedListener IADLXDisplaySettingsChangedListener;

typedef struct IADLXDisplaySettingsChangedListenerVtbl
{
    // IADLXDisplaySettingsChangedListener interface
    adlx_bool (ADLX_STD_CALL* OnDisplaySettingsChanged)(IADLXDisplaySettingsChangedListener* pThis, IADLXDisplaySettingsChangedEvent* pDisplaySettingChangedEvent);

} IADLXDisplaySettingsChangedListenerVtbl;

struct IADLXDisplaySettingsChangedListener { const IADLXDisplaySettingsChangedListenerVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplaySettingsChangedListener


//Interface that allows registration to display-related events: Display List changed, Display Gamut changed, Display Gamma changed and Display 3D LUT changed
#pragma region IADLXDisplayChangedHandling
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayChangedHandling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayChangedHandling")

        /**
        *@page DOX_IADLXDisplayChangedHandling_AddDisplayListEventListener AddDisplayListEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the display list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDisplayListEventListener (@ref DOX_IADLXDisplayListChangedListener* pDisplayListChangedListener);
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayListChangedListener,@ref DOX_IADLXDisplayListChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the display list change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDisplayListChangedListener_OnDisplayListChanged method of the listener when the display list changes.
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayListEventListener. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDisplayListEventListener (IADLXDisplayListChangedListener* pDisplayListChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_RemoveDisplayListEventListener RemoveDisplayListEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the display list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDisplayListEventListener (@ref DOX_IADLXDisplayListChangedListener* pDisplayListChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayListChangedListener,@ref DOX_IADLXDisplayListChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the display list change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDisplayListChangedListener_OnDisplayListChanged method of the listener when the display list changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDisplayListEventListener (IADLXDisplayListChangedListener* pDisplayListChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_AddDisplayGamutEventListener AddDisplayGamutEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the display gamut changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDisplayGamutEventListener (@ref DOX_IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGamutChangedListener,@ref DOX_IADLXDisplayGamutChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the display gamut change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDisplayGamutChangedListener_OnDisplayGamutChanged method of the listener when the display gamut changes.
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayGamutEventListener. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDisplayGamutEventListener (IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_RemoveDisplayGamutEventListener RemoveDisplayGamutEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the display gamut changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDisplayGamutEventListener (@ref DOX_IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGamutChangedListener,@ref DOX_IADLXDisplayGamutChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the display gamut change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDisplayGamutChangedListener_OnDisplayGamutChanged method of the listener when the display gamut changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDisplayGamutEventListener (IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_AddDisplayGammaEventListener AddDisplayGammaEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the display gamma changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDisplayGammaEventListener (@ref DOX_IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGammaChangedListener,@ref DOX_IADLXDisplayGammaChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the display gamma change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDisplayGammaChangedListener_OnDisplayGammaChanged method of the listener when the display gamma changes.
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplayGammaEventListener. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDisplayGammaEventListener (IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_RemoveDisplayGammaEventListener RemoveDisplayGammaEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the display gamma changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDisplayGammaEventListener (@ref DOX_IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplayGammaChangedListener,@ref DOX_IADLXDisplayGammaChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the display gamma change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDisplayGammaChangedListener_OnDisplayGammaChanged method of the listener when the display gamma changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDisplayGammaEventListener (IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_AddDisplay3DLUTEventListener AddDisplay3DLUTEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the display 3D LUT changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDisplay3DLUTEventListener (@ref DOX_IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay3DLUTChangedListener,@ref DOX_IADLXDisplay3DLUTChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the display 3D LUT change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDisplay3DLUTChangedListener_OnDisplay3DLUTChanged method of the listener when the display 3D LUT changes.
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplay3DLUTEventListener. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDisplay3DLUTEventListener (IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_RemoveDisplay3DLUTEventListener RemoveDisplay3DLUTEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the display 3D LUT changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDisplay3DLUTEventListener (@ref DOX_IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay3DLUTChangedListener,@ref DOX_IADLXDisplay3DLUTChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the display 3D LUT change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDisplay3DLUTChangedListener_OnDisplay3DLUTChanged method of the listener when the display 3D LUT changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDisplay3DLUTEventListener (IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener) = 0;
        /**
        *@page DOX_IADLXDisplayChangedHandling_AddDisplaySettingsEventListener AddDisplaySettingsEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the display settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDisplaySettingsEventListener (@ref DOX_IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in], pDisplaySettingsChangedListener,@ref DOX_IADLXDisplaySettingsChangedListener*, @ENG_START_DOX The pointer to the event listener interface to register for receiving display settings change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDisplaySettingsChangedListener_OnDisplaySettingsChanged method of the listener when display settings change.<br>
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDisplayChangedHandling_RemoveDisplaySettingsEventListener.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDisplaySettingsEventListener (IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener) = 0;

        /**
        *@page DOX_IADLXDisplayChangedHandling_RemoveDisplaySettingsEventListener RemoveDisplaySettingsEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the display settings change. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDisplaySettingsEventListener (@ref DOX_IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplaySettingsChangedListener,@ref DOX_IADLXDisplaySettingsChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving display settings change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX
        * If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX
        * After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDisplaySettingsChangedListener_OnDisplaySettingsChanged method of the listener when display settings change.
        * The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDisplaySettings.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDisplaySettingsEventListener (IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener) = 0;
    }; //IADLXDisplayChangedHandling
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayChangedHandling> IADLXDisplayChangedHandlingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayChangedHandling, L"IADLXDisplayChangedHandling")
typedef struct IADLXDisplayChangedHandling IADLXDisplayChangedHandling;

typedef struct IADLXDisplayChangedHandlingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplayChangedHandling* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplayChangedHandling* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplayChangedHandling* pThis, const wchar_t* interfaceId, void** ppInterface );

    // IADLXDisplayChangedHandling interface
    ADLX_RESULT (ADLX_STD_CALL* AddDisplayListEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayListChangedListener* pDisplayListChangedListener );
    ADLX_RESULT (ADLX_STD_CALL* RemoveDisplayListEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayListChangedListener* pDisplayListChangedListener );

    ADLX_RESULT (ADLX_STD_CALL* AddDisplayGamutEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener );
    ADLX_RESULT (ADLX_STD_CALL* RemoveDisplayGamutEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayGamutChangedListener* pDisplayGamutChangedListener );

    ADLX_RESULT (ADLX_STD_CALL* AddDisplayGammaEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener );
    ADLX_RESULT (ADLX_STD_CALL* RemoveDisplayGammaEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplayGammaChangedListener* pDisplayGammaChangedListener );

    ADLX_RESULT (ADLX_STD_CALL* AddDisplay3DLUTEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener );
    ADLX_RESULT (ADLX_STD_CALL* RemoveDisplay3DLUTEventListener)( IADLXDisplayChangedHandling* pThis, IADLXDisplay3DLUTChangedListener* pDisplay3DLUTChangedListener );

    ADLX_RESULT (ADLX_STD_CALL* AddDisplaySettingsEventListener)(IADLXDisplayChangedHandling* pThis, IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener);
    ADLX_RESULT (ADLX_STD_CALL* RemoveDisplaySettingsEventListener)(IADLXDisplayChangedHandling* pThis, IADLXDisplaySettingsChangedListener* pDisplaySettingsChangedListener);

} IADLXDisplayChangedHandlingVtbl;

struct IADLXDisplayChangedHandling { const IADLXDisplayChangedHandlingVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayChangedHandling

//Display Services interface
#pragma region IADLXDisplayServices
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayServices : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDisplayServices")
        /**
        * @page DOX_IADLXDisplayServices_GetNumberOfDisplays GetNumberOfDisplays
        * @ENG_START_DOX
        * @brief Gets the number of displays connected to the AMD GPUs.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetNumberOfDisplays (adlx_uint* numDisplays)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,numDisplays,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the number of displays is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the number of displays is successfully returned, __ADLX_OK__ is returned. <br>
        * If the number of displays is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and errors codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details For more information about the AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support".
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetNumberOfDisplays (adlx_uint* numDisplays) = 0;

        /**
        * @page DOX_IADLXDisplayServices_GetDisplays GetDisplays
        * @ENG_START_DOX
        * @brief  Gets the reference counted list of displays connected to the AMD GPUs.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetDisplays (@ref DOX_IADLXDisplayList** ppDisplay)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,ppDisplay,@ref DOX_IADLXDisplayList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.
        * @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details For more information about the AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support". <br>
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetDisplays (IADLXDisplayList** ppDisplay) = 0;

        /**
        * @page DOX_IADLXDisplayServices_Get3DLUT Get3DLUT
        * @ENG_START_DOX
        * @brief Gets the reference counted 3D LUT interface for a display.
        * @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Get3DLUT (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplay3DLUT** ppDisp3DLUT)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppDisp3DLUT,@ref DOX_IADLXDisplay3DLUT** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisp3DLUT__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        * @ENG_START_DOX
        * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Get3DLUT (IADLXDisplay* pDisplay, IADLXDisplay3DLUT** ppDisp3DLUT) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetGamut GetGamut
        *@ENG_START_DOX @brief Gets the reference counted gamut interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGamut (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayGamut** ppDispGamut)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppDispGamut,@ref DOX_IADLXDisplayGamut** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDispGamut__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        *If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        *If the interface is not returned, an error code is returned.<br>
        *Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        * @ENG_START_DOX
        * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGamut (IADLXDisplay* pDisplay, IADLXDisplayGamut** ppDispGamut) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetGamma GetGamma
        *@ENG_START_DOX @brief  Gets the reference counted gamma interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGamma (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayGamma** ppDispGamma)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppDispGamma,@ref DOX_IADLXDisplayGamma** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDispGamma__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details  The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++ when using a smart pointer for the returned interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetGamma (IADLXDisplay* pDisplay, IADLXDisplayGamma** ppDispGamma) = 0;

        /**
        * @page DOX_IADLXDisplayServices_GetDisplayChangedHandling GetDisplayChangedHandling
        * @ENG_START_DOX
        * @brief Gets the reference counted interface that allows registering and unregistering for notifications when display change.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetDisplayChangedHandling (@ref DOX_IADLXDisplayChangedHandling** ppDisplayChangedHandling)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,ppDisplayChangedHandling,@ref DOX_IADLXDisplayChangedHandling** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplayChangedHandling__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
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
        * In C++ when using a smart pointer for the display changing listener interface there is no need to call @ref DOX_IADLXInterface_Release because the smart pointer calls it internally.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL GetDisplayChangedHandling (IADLXDisplayChangedHandling** ppDisplayChangedHandling) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetFreeSync GetFreeSync
        *@ENG_START_DOX @brief Gets the reference counted AMD FreeSync™ Technology interface. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetFreeSync (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayFreeSync** ppFreeSync)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppFreeSync,@ref DOX_IADLXDisplayFreeSync** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppFreeSync__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetFreeSync (IADLXDisplay* pDisplay, IADLXDisplayFreeSync** ppFreeSync) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetVirtualSuperResolution GetVirtualSuperResolution
        *@ENG_START_DOX @brief Gets the reference counted virtual super resolution interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetVirtualSuperResolution (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayVSR** ppVSR)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out] ,ppVSR,@ref DOX_IADLXDisplayVSR** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppVSR__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetVirtualSuperResolution (IADLXDisplay* pDisplay, IADLXDisplayVSR** ppVSR) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetGPUScaling GetGPUScaling
        *@ENG_START_DOX @brief Gets the reference counted GPU scaling interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetGPUScaling (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayGPUScaling** ppGPUScaling)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay* ,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppGPUScaling,@ref DOX_IADLXDisplayGPUScaling** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppGPUScaling__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetGPUScaling (IADLXDisplay* pDisplay, IADLXDisplayGPUScaling** ppGPUScaling) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetScalingMode GetScalingMode
        *@ENG_START_DOX @brief Gets the reference counted scaling mode interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetScalingMode (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayScalingMode** ppScalingMode)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppScalingMode,@ref DOX_IADLXDisplayScalingMode**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppScalingMode__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetScalingMode (IADLXDisplay* pDisplay, IADLXDisplayScalingMode** ppScalingMode) = 0;

        /**
        *@page DOX_IADLXDisplayServices_GetIntegerScaling GetIntegerScaling
        *@ENG_START_DOX @brief Gets the reference counted integer scaling interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetIntegerScaling (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayIntegerScaling** ppIntegerScaling)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppIntegerScaling,@ref DOX_IADLXDisplayIntegerScaling**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppIntegerScaling__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetIntegerScaling (IADLXDisplay* pDisplay, IADLXDisplayIntegerScaling** ppIntegerScaling) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetColorDepth GetColorDepth
        *@ENG_START_DOX @brief Gets the reference counted color depth interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetColorDepth (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayColorDepth** ppColorDepth)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppColorDepth,@ref DOX_IADLXDisplayColorDepth**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppColorDepth__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetColorDepth (IADLXDisplay* pDisplay, IADLXDisplayColorDepth** ppColorDepth) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetPixelFormat GetPixelFormat
        *@ENG_START_DOX @brief Gets the reference counted pixel format interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetPixelFormat (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayPixelFormat** ppPixelFormat)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppPixelFormat,@ref DOX_IADLXDisplayPixelFormat**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppPixelFormat__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetPixelFormat (IADLXDisplay* pDisplay, IADLXDisplayPixelFormat** ppPixelFormat) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetCustomColor GetCustomColor
        *@ENG_START_DOX @brief Gets the reference counted custom color interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCustomColor (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayCustomColor** ppCustomColor)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppCustomColor,@ref DOX_IADLXDisplayCustomColor**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppCustomColor__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCustomColor (IADLXDisplay* pDisplay, IADLXDisplayCustomColor** ppCustomColor) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetHDCP GetHDCP
        *@ENG_START_DOX @brief Gets the reference counted HDCP interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetHDCP (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayHDCP** ppHDCP)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppHDCP,@ref DOX_IADLXDisplayHDCP**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppHDCP__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetHDCP (IADLXDisplay* pDisplay, IADLXDisplayHDCP** ppHDCP) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetCustomResolution GetCustomResolution
        *@ENG_START_DOX @brief Gets the reference counted custom resolution interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetCustomResolution (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayCustomResolution** ppCustomResolution)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppCustomResolution,@ref DOX_IADLXDisplayCustomResolution**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address  __*ppCustomResolution__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetCustomResolution (IADLXDisplay* pDisplay, IADLXDisplayCustomResolution** ppCustomResolution) = 0;
        /**
        *@page DOX_IADLXDisplayServices_GetVariBright GetVariBright
        *@ENG_START_DOX @brief Gets the reference counted Vari-Bright interface for a display. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetVariBright (@ref DOX_IADLXDisplay* pDisplay, @ref DOX_IADLXDisplayVariBright ** ppVariBright)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in],pDisplay,@ref DOX_IADLXDisplay*,@ENG_START_DOX The pointer to the display interface. @ENG_END_DOX}
        *@paramrow{2.,[out],ppVariBright,@ref DOX_IADLXDisplayVariBright **,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppVariBright__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX
        * If the interface is successfully returned, __ADLX_OK__ is returned. <br>
        * If the interface is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX
        *In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDisplays.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetVariBright (IADLXDisplay* pDisplay, IADLXDisplayVariBright** ppVariBright) = 0;
    };  //IADLXDisplayServices
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDisplayServices> IADLXDisplayServicesPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDisplayServices, L"IADLXDisplayServices")
typedef struct IADLXDisplayServices IADLXDisplayServices;

typedef struct IADLXDisplayFreeSync IADLXDisplayFreeSync;
typedef struct IADLXDisplayVSR IADLXDisplayVSR;
typedef struct IADLXDisplayGPUScaling IADLXDisplayGPUScaling;
typedef struct IADLXDisplayScalingMode IADLXDisplayScalingMode;
typedef struct IADLXDisplayIntegerScaling IADLXDisplayIntegerScaling;
typedef struct IADLXDisplayColorDepth IADLXDisplayColorDepth;
typedef struct IADLXDisplayPixelFormat IADLXDisplayPixelFormat;
typedef struct IADLXDisplayCustomColor IADLXDisplayCustomColor;
typedef struct IADLXDisplayHDCP IADLXDisplayHDCP;
typedef struct IADLXDisplayCustomResolution IADLXDisplayCustomResolution;
typedef struct IADLXDisplayChangedHandling IADLXDisplayChangedHandling;
typedef struct IADLXDisplayVariBright IADLXDisplayVariBright;

typedef struct IADLXDisplayServicesVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL* Acquire)( IADLXDisplayServices* pThis );
    adlx_long (ADLX_STD_CALL* Release)( IADLXDisplayServices* pThis );
    ADLX_RESULT (ADLX_STD_CALL* QueryInterface)( IADLXDisplayServices* pThis, const wchar_t* interfaceId, void** ppInterface );

    //IADLXDisplayServices
    ADLX_RESULT (ADLX_STD_CALL* GetNumberOfDisplays)( IADLXDisplayServices* pThis, adlx_uint* numDisplays );
    ADLX_RESULT (ADLX_STD_CALL* GetDisplays)( IADLXDisplayServices* pThis, IADLXDisplayList** ppDisplays );
    ADLX_RESULT (ADLX_STD_CALL* Get3DLUT)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplay3DLUT** ppDisp3DLUT );
    ADLX_RESULT (ADLX_STD_CALL* GetGamut)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayGamut** ppDispGamut );
    ADLX_RESULT (ADLX_STD_CALL* GetGamma)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayGamma** ppDispGamma );
    ADLX_RESULT (ADLX_STD_CALL* GetDisplayChangedHandling)( IADLXDisplayServices* pThis, IADLXDisplayChangedHandling** ppDisplayChangedHandling );
    ADLX_RESULT (ADLX_STD_CALL* GetFreeSync)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayFreeSync** ppFreeSync );
    ADLX_RESULT (ADLX_STD_CALL* GetVirtualSuperResolution)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayVSR** ppVSR );
    ADLX_RESULT (ADLX_STD_CALL* GetGPUScaling)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayGPUScaling** ppGPUScaling );
    ADLX_RESULT (ADLX_STD_CALL* GetScalingMode)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayScalingMode** ppScalingMode );
    ADLX_RESULT (ADLX_STD_CALL* GetIntegerScaling)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayIntegerScaling** ppIntegerScaling );
    ADLX_RESULT (ADLX_STD_CALL* GetColorDepth)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayColorDepth** ppColorDepth );
    ADLX_RESULT (ADLX_STD_CALL* GetPixelFormat)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayPixelFormat** ppPixelFormat );
    ADLX_RESULT (ADLX_STD_CALL* GetCustomColor)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayCustomColor** ppCustomColor );
    ADLX_RESULT (ADLX_STD_CALL* GetHDCP)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayHDCP** ppHDCP );
    ADLX_RESULT (ADLX_STD_CALL* GetCustomResolution)( IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayCustomResolution** ppCustomResolution );
    ADLX_RESULT (ADLX_STD_CALL* GetVariBright)(IADLXDisplayServices* pThis, IADLXDisplay* pDisplay, IADLXDisplayVariBright** ppVariBright);
} IADLXDisplayServicesVtbl;

struct IADLXDisplayServices { const IADLXDisplayServicesVtbl* pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDisplayServices

#endif//ADLX_IDISPLAYS_H
