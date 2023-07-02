// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_IDESKTOPS_H
#define ADLX_IDESKTOPS_H
#pragma once

#include "ADLXStructures.h"
#include "ICollections.h"

//-------------------------------------------------------------------------------------------------
//IDesktops.h - Interfaces for ADLX Desktops functionality, including AMD Eyefinity

//Desktop interface: Describe a generic desktop
#pragma region IADLXDesktop
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplayList;
    class ADLX_NO_VTABLE IADLXDesktop : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDesktop")
        /**
        *@page DOX_IADLXDesktop_Orientation Orientation
        *@ENG_START_DOX @brief Gets the orientation of a desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Orientation (@ref ADLX_ORIENTATION* orientation)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,orientation,@ref ADLX_ORIENTATION* ,@ENG_START_DOX The pointer to a variable where the orientation is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the orientation is successfully returned, __ADLX_OK__ is returned.<br>
        * If the orientation is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The desktop orientation indicates the rotation angle of the desktop.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Orientation (ADLX_ORIENTATION* orientation) = 0;
        /**
        *@page DOX_IADLXDesktop_Size Size
        *@ENG_START_DOX @brief Gets the size of a desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Size (adlx_int* width, adlx_int* height)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,width,adlx_int* ,@ENG_START_DOX The pointer to a variable where the width is returned. @ENG_END_DOX}
        *@paramrow{2.,[out] ,height,adlx_int* ,@ENG_START_DOX The pointer to a variable where the height is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the width and height are successfully returned, __ADLX_OK__ is returned.<br>
        * If the width and height are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The desktop size represents the pixel resolution of the desktop.
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Size (adlx_int* width, adlx_int* height) = 0;
        /**
        *@page DOX_IADLXDesktop_TopLeft TopLeft
        *@ENG_START_DOX @brief Get the top left position of a desktop in Windows screen coordinates. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    TopLeft (@ref ADLX_Point* locationTopLeft)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,locationTopLeft,@ref ADLX_Point* ,@ENG_START_DOX The pointer to a variable where the top left position is returned. @ENG_END_DOX}
         *
        *@retvalues
        *@ENG_START_DOX  If the top left position is successfully returned, __ADLX_OK__ is returned.<br>
        * If the top left position is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The desktop top left position is measured in screen coordinates.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL TopLeft (ADLX_Point* locationTopLeft) = 0;
        /**
        *@page DOX_IADLXDesktop_Type Type
        *@ENG_START_DOX @brief Get the type of a desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Type (@ref ADLX_DESKTOP_TYPE* desktopType)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,desktopType,@ref ADLX_DESKTOP_TYPE* ,@ENG_START_DOX The pointer to a variable where the desktop type is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the desktop type is successfully returned, __ADLX_OK__ is returned.<br>
        * If the desktop type is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The desktop type indicates if the desktop is single, duplicate or AMD Eyefinity.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Type (ADLX_DESKTOP_TYPE* desktopType) = 0;
        /**
        *@page DOX_IADLXDesktop_GetNumberOfDisplays GetNumberOfDisplays
        *@ENG_START_DOX @brief Gets the number of displays that show pixels from a desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetNumberOfDisplays (adlx_uint* numDisplays)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,numDisplays,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the number of displays is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the number of displays is successfully returned, __ADLX_OK__ is returned.<br>
        * If the number of displays is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
        * The number of displays that show pixels from a desktop depends on the desktop @ref ADLX_DESKTOP_TYPE "Type".<br>
        * A single desktop is associated with one display.<br>
        * A duplicate desktop is associated with two or more displays.<br>
        * An AMD Eyefinity desktop is associated with two or more displays.<br> @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetNumberOfDisplays (adlx_uint* numDisplays) = 0;
        /**
        *@page DOX_IADLXDesktop_GetDisplays GetDisplays
        *@ENG_START_DOX @brief Gets the reference counted list of displays that show pixels from a desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GetDisplays (@ref DOX_IADLXDisplayList** ppDisplays)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppDisplays,@ref DOX_IADLXDisplayList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplays__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the list of displays is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of displays is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplays (IADLXDisplayList** ppDisplays) = 0;
    };  //IADLXDesktop
    //----------------------------------------------------------------------------------------------    
    typedef IADLXInterfacePtr_T<IADLXDesktop> IADLXDesktopPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDesktop, L"IADLXDesktop")
typedef struct IADLXDesktop IADLXDesktop;
typedef struct IADLXDisplayList IADLXDisplayList;
typedef struct IADLXDesktopVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDesktop* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDesktop* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDesktop* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDesktop
    ADLX_RESULT (ADLX_STD_CALL *Orientation) (IADLXDesktop* pThis, ADLX_ORIENTATION* orientation);
    ADLX_RESULT (ADLX_STD_CALL *Size) (IADLXDesktop* pThis, adlx_int* width, adlx_int* height);
    ADLX_RESULT (ADLX_STD_CALL *TopLeft) (IADLXDesktop* pThis, ADLX_Point* locationTopLeft);
    ADLX_RESULT (ADLX_STD_CALL *Type) (IADLXDesktop* pThis, ADLX_DESKTOP_TYPE* desktopType);
    ADLX_RESULT (ADLX_STD_CALL *GetNumberOfDisplays) (IADLXDesktop* pThis, adlx_uint* numDisplays);
    ADLX_RESULT (ADLX_STD_CALL *GetDisplays) (IADLXDesktop* pThis, IADLXDisplayList** ppDisplays);
}IADLXDesktopVtbl;
struct IADLXDesktop { const IADLXDesktopVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDesktop


//AMD Eyefinity Desktop interface: describe and AMD Eyefinity desktop
#pragma region IADLXEyefinityDesktop
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDisplay;
    class ADLX_NO_VTABLE IADLXEyefinityDesktop : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXEyefinityDesktop")
        /**
        *@page DOX_IADLXEyefinityDesktop_GridSize GridSize
        *@ENG_START_DOX @brief Gets the number of rows and columns that describes the display composition of an AMD Eyefinity desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    GridSize (adlx_uint* rows, adlx_uint* cols)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,rows,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the number of rows is returned. @ENG_END_DOX}
        *@paramrow{2.,[out] ,cols,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the number of cols is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the number of rows and cols are successfully returned, __ADLX_OK__ is returned.<br>
        * If the number of rows and cols are not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The grid location of an AMD Eyefinity desktop is identified by a row and a column from zero to the respective value returned from this method minus one. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GridSize (adlx_uint* rows, adlx_uint* cols) = 0;
        /**
         *@page DOX_IADLXEyefinityDesktop_GetDisplay GetDisplay
         *@ENG_START_DOX @brief Gets the reference counted display interface for the display that shows the portion of an AMD Eyefinity desktop at the requested grid location. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT    GetDisplay (adlx_uint row, adlx_uint col, @ref DOX_IADLXDisplay** ppDisplay)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[in] ,row,adlx_uint ,@ENG_START_DOX The row of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{2.,[in] ,col,adlx_uint ,@ENG_START_DOX The column of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{3.,[out] ,ppDisplay,@ref DOX_IADLXDisplay** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDisplay__ to __nullptr__. @ENG_END_DOX}
         *
         *@retvalues
         *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
         * If the interface is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
         *
         *@detaileddesc
         *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
         *
         *@addinfo
         *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetDisplay (adlx_uint row, adlx_uint col, IADLXDisplay** ppDisplay) = 0;
        /**
         *@page DOX_IADLXEyefinityDesktop_DisplayOrientation DisplayOrientation
         *@ENG_START_DOX @brief Gets the orientation of the display area on an AMD Eyefinity desktop at a given grid location. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT    DisplayOrientation (adlx_uint row, adlx_uint col, @ref ADLX_ORIENTATION* displayOrientation)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[in] ,row,adlx_uint ,@ENG_START_DOX The row of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{2.,[in] ,col,adlx_uint ,@ENG_START_DOX The column of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{3.,[out] ,displayOrientation,@ref ADLX_ORIENTATION* ,@ENG_START_DOX The pointer to a variable where the display orientation is returned. @ENG_END_DOX}
         *
         *@retvalues
         *@ENG_START_DOX  If the display orientation is successfully returned, __ADLX_OK__ is returned.<br>
         * If the display orientation is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
         *
         *@detaileddesc
         *@ENG_START_DOX @details The display orientation indicates the rotation angle of the display area on an AMD Eyefinity desktop. @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL DisplayOrientation (adlx_uint row, adlx_uint col, ADLX_ORIENTATION* displayOrientation) = 0;
        /**
         *@page DOX_IADLXEyefinityDesktop_DisplaySize DisplaySize
         *@ENG_START_DOX @brief Gets the size of the display area on an AMD Eyefinity desktop at a given grid location. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT    DisplaySize (adlx_uint row, adlx_uint col, adlx_int* displayWidth, adlx_int* displayHeight)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[in] ,row,adlx_uint ,@ENG_START_DOX The row of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{2.,[in] ,col,adlx_uint ,@ENG_START_DOX The column of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{3.,[out] ,displayWidth,adlx_int* ,@ENG_START_DOX The pointer to a variable where the display width is returned. @ENG_END_DOX}
         *@paramrow{4.,[out] ,displayHeight,adlx_int* ,@ENG_START_DOX The pointer to a variable where the display height is returned. @ENG_END_DOX}
         *
         *@retvalues
         *@ENG_START_DOX  If the display size is successfully returned, __ADLX_OK__ is returned.<br>
         * If the display size is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
         *
         *@detaileddesc
         *@ENG_START_DOX @details The display size represents the pixel resolution of the dispay area in an AMD Eyefinity desktop.
		 * @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL DisplaySize (adlx_uint row, adlx_uint col, adlx_int* displayWidth, adlx_int* displayHeight) = 0;
        /**
         *@page DOX_IADLXEyefinityDesktop_DisplayTopLeft DisplayTopLeft
         *@ENG_START_DOX @brief Gets the top left position of the display area on an AMD Eyefinity desktop at a given grid location. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT    DisplayTopLeft (adlx_uint row, adlx_uint col, @ref ADLX_Point* displayLocationTopLeft)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[in] ,row,adlx_uint ,@ENG_START_DOX The row of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{2.,[in] ,col,adlx_uint ,@ENG_START_DOX The column of the AMD Eyefinity grid location for the requested display. @ENG_END_DOX}
         *@paramrow{3.,[out] ,displayLocationTopLeft,@ref ADLX_Point* ,@ENG_START_DOX The pointer to a variable where the top left position is returned. @ENG_END_DOX}
         *
         *@retvalues
         *@ENG_START_DOX  If the top left position is successfully returned, __ADLX_OK__ is returned.<br>
         * If the top left position is not successfully returned, an error code is returned.<br>
         * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
         *
         *@detaileddesc
         *@ENG_START_DOX @details The top left position is relative to the desktop's top left position.
         * @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL DisplayTopLeft (adlx_uint row, adlx_uint col, ADLX_Point* displayLocationTopLeft) = 0;
    };  //IADLXEyefinityDesktop
    //----------------------------------------------------------------------------------------------    
    typedef IADLXInterfacePtr_T<IADLXEyefinityDesktop> IADLXEyefinityDesktopPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXEyefinityDesktop, L"IADLXEyefinityDesktop")
typedef struct IADLXEyefinityDesktop IADLXEyefinityDesktop;
typedef struct IADLXDisplay IADLXDisplay;
typedef struct IADLXDisplayList IADLXDisplayList;
typedef struct IADLXEyefinityDesktopVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXEyefinityDesktop* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXEyefinityDesktop* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXEyefinityDesktop* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXEyefinityDesktop
    ADLX_RESULT (ADLX_STD_CALL *GridSize) (IADLXEyefinityDesktop* pThis, adlx_uint* rows, adlx_uint* cols);
    ADLX_RESULT (ADLX_STD_CALL *GetDisplay) (IADLXEyefinityDesktop* pThis, adlx_uint row, adlx_uint col, IADLXDisplay** ppDisplay);
    ADLX_RESULT (ADLX_STD_CALL *DisplayOrientation) (IADLXEyefinityDesktop* pThis, adlx_uint row, adlx_uint col, ADLX_ORIENTATION* displayOrientation);
    ADLX_RESULT (ADLX_STD_CALL *DisplaySize) (IADLXEyefinityDesktop* pThis, adlx_uint row, adlx_uint col, adlx_int* displayWidth, adlx_int* displayHeight);
    ADLX_RESULT (ADLX_STD_CALL *DisplayTopLeft) (IADLXEyefinityDesktop* pThis, adlx_uint row, adlx_uint col, ADLX_Point* displayLocationTopLeft);
}IADLXEyefinityDesktopVtbl;
struct IADLXEyefinityDesktop { const IADLXEyefinityDesktopVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXEyefinityDesktop


//Desktops list interface: List for IADLXDesktop items
#pragma region IADLXDesktopList
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDesktopList : public IADLXList
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDesktopList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (IADLXDesktop::IID ())
        /**
        * @page DOX_IADLXDesktopList_At At
        * @ENG_START_DOX
        * @brief Returns the reference counted interface at the requested location.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXDesktop** ppItem)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[in] ,location,const adlx_uint ,@ENG_START_DOX The location of the requested interface. @ENG_END_DOX}
        * @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXDesktop** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned then the method sets the dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}
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
        * @DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXDesktop** ppItem) = 0;
        /**
        *@page DOX_IADLXDesktopList_Add_Back Add_Back
        *@ENG_START_DOX @brief Adds an interface to the end of a list. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXDesktop* pItem)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pItem,@ref DOX_IADLXDesktop* ,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is added successfully to the end of the list, __ADLX_OK__ is returned.<br>
        * If the interface is not added to the end of the list, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXDesktop* pItem) = 0;
    };  //IADLXDisplayList
    //----------------------------------------------------------------------------------------------    
    typedef IADLXInterfacePtr_T<IADLXDesktopList> IADLXDesktopListPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDesktopList, L"IADLXDesktopList")
ADLX_DECLARE_ITEM_IID (IADLXDesktop, IID_IADLXDesktop ())

typedef struct IADLXDesktopList IADLXDesktopList;
typedef struct IADLXDesktopListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDesktopList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDesktopList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDesktopList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXDesktopList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXDesktopList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXDesktopList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXDesktopList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXDesktopList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXDesktopList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXDesktopList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXDesktopList* pThis, IADLXInterface* pItem);

    //IADLXDesktopList
    ADLX_RESULT (ADLX_STD_CALL *At_DesktopList)(IADLXDesktopList* pThis, const adlx_uint location, IADLXDesktop** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back_DesktopList)(IADLXDesktopList* pThis, IADLXDesktop* pItem);

}IADLXDesktopListVtbl;

struct IADLXDesktopList { const IADLXDesktopListVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDesktopList

//Desktop List changed listener interface. To be implemented in application and passed in IADLXDesktopChangedHandling::AddDesktopListEventListener()
#pragma region IADLXDesktopListChangedListener
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDesktopListChangedListener
    {
    public:
        /**
        *@page DOX_IADLXDesktopListChangedListener_OnDesktopListChanged OnDesktopListChanged
        *@ENG_START_DOX @brief The __OnDesktopsListChanged__ is called by ADLX when the desktop list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * adlx_bool    OnDesktopListChanged (@ref DOX_IADLXDesktopList* pNewDesktop)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,pNewDesktop,@ref DOX_IADLXDesktopList* ,@ENG_START_DOX The pointer to the new desktop list. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the application requires ADLX to continue notifying the next listener, __true__ must be returned.<br>
        * If the application requires ADLX to stop notifying the next listener, __false__ must be returned.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  Once the application registers to the notifications with @ref DOX_IADLXDesktopChangedHandling_AddDesktopListEventListener, ADLX will call this method until the application unregisters from the notifications with @ref DOX_IADLXDesktopChangedHandling_RemoveDesktopListEventListener.<br>
        * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the event notification, the application must hold onto a reference to the new desktop list with @ref DOX_IADLXInterface_Acquire and make it available on an asynchronous thread and return immediately. When the asynchronous thread is done processing it must discard the new desktop list with @ref DOX_IADLXInterface_Release.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual adlx_bool ADLX_STD_CALL OnDesktopListChanged (IADLXDesktopList* pNewDesktop) = 0;
    }; //IADLXDesktopListChangedListener
} //namespace adlx
#else //__cplusplus
typedef struct IADLXDesktopListChangedListener IADLXDesktopListChangedListener;

typedef struct IADLXDesktopListChangedListenerVtbl
{
    // IADLXDesktopListChangedListener interface
    adlx_bool (ADLX_STD_CALL *OnDesktopListChanged)(IADLXDesktopListChangedListener* pThis, IADLXDesktopList* pNewDesktop);

} IADLXDesktopListChangedListenerVtbl;

struct IADLXDesktopListChangedListener { const IADLXDesktopListChangedListenerVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDesktopListChangedListener


#pragma region IADLXDesktopChangedHandling
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDesktopChangedHandling : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDesktopChangedHandling")
        /**
        *@page DOX_IADLXDesktopChangedHandling_AddDesktopListEventListener AddDesktopListEventListener
        *@ENG_START_DOX @brief Registers an event listener for notifications when the desktop list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    AddDesktopListEventListener (@ref DOX_IADLXDesktopListChangedListener* pDesktopListChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDesktopListChangedListener,@ref DOX_IADLXDesktopListChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to register for receiving the desktop list change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully registered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully registered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully registered, ADLX will call @ref DOX_IADLXDesktopListChangedListener_OnDesktopListChanged of the listener when the desktop list changes.<br>
        * The event listener instance must exist until the application unregisters the event listener with @ref DOX_IADLXDesktopChangedHandling_RemoveDesktopListEventListener.<br> @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL AddDesktopListEventListener (IADLXDesktopListChangedListener* pDesktopListChangedListener) = 0;
        /**
        *@page DOX_IADLXDesktopChangedHandling_RemoveDesktopListEventListener RemoveDesktopListEventListener
        *@ENG_START_DOX @brief Unregisters an event listener from notifications when the desktop list changes. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RemoveDesktopListEventListener (@ref DOX_IADLXDesktopListChangedListener* pDesktopListChangedListener)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDesktopListChangedListener,@ref DOX_IADLXDesktopListChangedListener* ,@ENG_START_DOX The pointer to the event listener interface to unregister from receiving the desktop list change notifications. @ENG_END_DOX}
        *
        *
        *@retvalues
        *@ENG_START_DOX  If the event listener is successfully unregistered, __ADLX_OK__ is returned.<br>
        * If the event listener is not successfully unregistered, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX  After the event listener is successfully unregistered, ADLX will no longer call @ref DOX_IADLXDesktopListChangedListener_OnDesktopListChanged method of the listener when the desktop list changes. The application can discard the event listener instance. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL RemoveDesktopListEventListener (IADLXDesktopListChangedListener* pDesktopListChangedListener) = 0;

    }; //IADLXDesktopChangedHandling
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXDesktopChangedHandling> IADLXDesktopChangedHandlingPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDesktopChangedHandling, L"IADLXDesktopChangedHandling")
typedef struct IADLXDesktopChangedHandling IADLXDesktopChangedHandling;

typedef struct IADLXDesktopChangedHandlingVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDesktopChangedHandling* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDesktopChangedHandling* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDesktopChangedHandling* pThis, const wchar_t* interfaceId, void** ppInterface);

    // IADLXDesktopChangedHandling interface
    ADLX_RESULT (ADLX_STD_CALL *AddDesktopListEventListener)(IADLXDesktopChangedHandling* pThis, IADLXDesktopListChangedListener* pDesktopListChangedListener);
    ADLX_RESULT (ADLX_STD_CALL *RemoveDesktopListEventListener)(IADLXDesktopChangedHandling* pThis, IADLXDesktopListChangedListener* pDesktopListChangedListener);

} IADLXDesktopChangedHandlingVtbl;

struct IADLXDesktopChangedHandling { const IADLXDesktopChangedHandlingVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDesktopChangedHandling

//Simple AMD Eyefinity interface: allows creation and destruction of AMD Eyefinity desktops
#pragma region IADLXSimpleEyefinity
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXSimpleEyefinity : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXSimpleEyefinity")
        /**
        *@page DOX_IADLXSimpleEyefinity_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if an AMD AMD Eyefinity desktop can be created with all the enabled displays. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (adlx_bool* supported)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,supported,adlx_bool* ,@ENG_START_DOX The pointer to a variable where the state of AMD Eyefinity desktop creation is returned. The variable is __true__ if AMD Eyefinity desktop creation is supported. The variable is __false__ if AMD Eyefinity desktop creation is not supported. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the state is successfully returned, __ADLX_OK__ is returned.<br>
        * If the state is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
        * AMD AMD Eyefinity desktops can be created with ADLX using all the enabled displays connected to the AMD GPUs, except LCD panel displays. For more information about AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support". Use @ref DOX_IADLXDisplay_DisplayType to check if a display is an LCD panel.<br>
        * All the desktops must be single desktops. Use @ref DOX_IADLXDesktop_Type to check if a desktop is a single desktop.<br>
        * All the enabled displays must be connected to the same GPU.<br>
        * The AMD AMD Eyefinity desktop configuration must be supported by the AMD driver. Driver support varies depending on the GPU.<br>
        * @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL IsSupported (adlx_bool* supported) = 0;
        /**
        *@page DOX_IADLXSimpleEyefinity_Create Create
        *@ENG_START_DOX @brief Creates an AMD AMD Eyefinity desktop with all the enabled displays. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Create (@ref DOX_IADLXEyefinityDesktop** ppEyefinityDesktop)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[out] ,ppEyefinityDesktop,@ref DOX_IADLXEyefinityDesktop** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppEyefinityDesktop__ to __nullptr__. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details
		* Use @ref DOX_IADLXSimpleEyefinity_IsSupported to check if an AMD AMD Eyefinity desktop can be created.<br>
		* Creating an AMD Eyefinity desktop can take a couple of seconds to complete. The method will block the execution thread until the operation is finished.<br>
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. Discarding the interface does not destroy the AMD Eyefinity desktop.<br> @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Create (IADLXEyefinityDesktop** ppEyefinityDesktop) = 0;
        /**
        *@page DOX_IADLXSimpleEyefinity_DestroyAll DestroyAll
        *@ENG_START_DOX @brief Destroys all the AMD Eyefinity desktops. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    DestroyAll ()
        *@codeEnd
        *
        *@retvalues
        *@ENG_START_DOX  If all AMD Eyefinity desktops are successfully destroyed, __ADLX_OK__ is returned.<br>
        * If all AMD Eyefinity desktops are not successfully destroyed, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Destroying all AMD Eyefinity desktops can take a couple of seconds to complete. The method will block the execution thread until the operation is finished. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL DestroyAll () = 0;
        /**
        *@page DOX_IADLXSimpleEyefinity_Destroy Destroy
        *@ENG_START_DOX @brief Destroys a specified AMD Eyefinity desktop. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Destroy (@ref DOX_IADLXEyefinityDesktop* pDesktop)
        *@codeEnd
        *
        *@params
        *@paramrow{1.,[in] ,pDesktop,@ref DOX_IADLXEyefinityDesktop* ,@ENG_START_DOX The pointer to the eyefinity desktop to be destroyed. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the AMD Eyefinity desktop is successfully destroyed, __ADLX_OK__ is returned.<br>
        * If the AMD Eyefinity desktop is not successfully destroyed, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details Destroying an AMD Eyefinity desktop can take a couple of seconds to complete. The method will block the execution thread until the operation is finished. @ENG_END_DOX
        *
        *@requirements
        *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL Destroy (IADLXEyefinityDesktop* pDesktop) = 0;
    };  //IADLXSimpleEyefinity
    //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXSimpleEyefinity> IADLXSimpleEyefinityPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXSimpleEyefinity, L"IADLXSimpleEyefinity")
typedef struct IADLXSimpleEyefinity IADLXSimpleEyefinity;
typedef struct IADLXSimpleEyefinityVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXSimpleEyefinity* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXSimpleEyefinity* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXSimpleEyefinity* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXSimpleEyefinity
    ADLX_RESULT (ADLX_STD_CALL *IsSupported) (IADLXSimpleEyefinity* pThis, adlx_bool* supported);
    ADLX_RESULT (ADLX_STD_CALL *Create) (IADLXSimpleEyefinity* pThis, IADLXEyefinityDesktop** ppEyefinityDesktop);
    ADLX_RESULT (ADLX_STD_CALL *DestroyAll) (IADLXSimpleEyefinity* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Destroy) (IADLXSimpleEyefinity* pThis, IADLXEyefinityDesktop* pDesktop);
}IADLXSimpleEyefinityVtbl;
struct IADLXSimpleEyefinity { const IADLXSimpleEyefinityVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXSimpleEyefinity

//Desktops services interface: top level interface for accessing desktop functionality:
//describe desktops (including AMD Eyefinity), register for desktop changed events and access AMD Eyefinity functionality
#pragma region IADLXDesktopServices 
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXDesktopServices : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXDesktopServices")
        /**
        * @page DOX_IADLXDesktopServices_GetNumberOfDesktops GetNumberOfDesktops
        * @ENG_START_DOX
        * @brief Gets the number of desktops instantiated on the AMD GPUs.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetNumberOfDesktops (adlx_uint* numDesktops)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,numDesktops,adlx_uint* ,@ENG_START_DOX The pointer to a variable where the number of desktops is returned. @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the number of desktops is successfully returned, __ADLX_OK__ is returned.<br>
        * If the number of desktops is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
		* @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details For more information about the AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support".
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetNumberOfDesktops (adlx_uint* numDesktops) = 0;
        /**
        * @page DOX_IADLXDesktopServices_GetDesktops GetDesktops
        * @ENG_START_DOX
        * @brief Gets the reference counted list of desktops instantiated on the AMD GPUs.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT    GetDesktops (@ref DOX_IADLXDesktopList** ppDesktops)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,ppDesktops,@ref DOX_IADLXDesktopList** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDesktops__ to __nullptr__.  @ENG_END_DOX}
        *
        * @retvalues
        * @ENG_START_DOX
        * If the list of desktops is successfully returned, __ADLX_OK__ is returned.<br>
        * If the list of desktops is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br>
		* @ENG_END_DOX
        *
        * @detaileddesc
        * @ENG_START_DOX
        * @details For more information about the AMD GPUs, refer to @ref @adlx_gpu_support "ADLX GPU Support".<br>
        * The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed.<br>
        * @ENG_END_DOX
        *
        * @addinfo
        * @ENG_START_DOX
        * In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
        * @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDesktops (IADLXDesktopList** ppDesktops) = 0;
        /**
        * @page DOX_IADLXDesktopServices_GetDesktopChangedHandling GetDesktopChangedHandling
        * @ENG_START_DOX
        * @brief Gets the reference counted interface that allows registering and unregistering for notifications when desktop settings change.
        * @ENG_END_DOX
        *
        * @syntax
        * @codeStart
        *  @ref ADLX_RESULT   GetDesktopChangedHandling (@ref DOX_IADLXDesktopChangedHandling** ppDesktopChangedHandling)
        * @codeEnd
        *
        * @params
        * @paramrow{1.,[out] ,ppDesktopChangedHandling,@ref DOX_IADLXDesktopChangedHandling** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppDeskChangedHandling__ to __nullptr__.  @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
        *
        * @requirements
        * @DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT ADLX_STD_CALL GetDesktopChangedHandling (IADLXDesktopChangedHandling** ppDesktopChangedHandling) = 0;
        /**
         *@page DOX_IADLXDesktopServices_GetSimpleEyefinity GetSimpleEyefinity
         *@ENG_START_DOX @brief Gets the reference counted interface to create and destroy AMD Eyefinity desktops. @ENG_END_DOX
         *
         *@syntax
         *@codeStart
         * @ref ADLX_RESULT   GetSimpleEyefinity (@ref DOX_IADLXSimpleEyefinity** ppSimpleEyefinity)
         *@codeEnd
         *
         *@params
         *@paramrow{1.,[out] ,ppSimpleEyefinity,@ref DOX_IADLXSimpleEyefinity** ,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets the dereferenced address __*ppSimpleEyefinity__ to __nullptr__. @ENG_END_DOX}
         *
         *@retvalues
        *@ENG_START_DOX  If the interface is successfully returned, __ADLX_OK__ is returned.<br>
        * If the interface is not successfully returned, an error code is returned.<br>
        * Refer to @ref ADLX_RESULT for success codes and error codes.<br> @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The returned interface must be discarded with @ref DOX_IADLXInterface_Release when it is no longer needed. @ENG_END_DOX
        *
        *@addinfo
        *@ENG_START_DOX  In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation. @ENG_END_DOX
         *
         *@requirements
         *@DetailsTable{#include "IDesktops.h", @ADLX_First_Ver}
         *
         */
        virtual ADLX_RESULT ADLX_STD_CALL GetSimpleEyefinity (IADLXSimpleEyefinity** ppSimpleEyefinity) = 0;
    };  //IADLXDesktopServices
    //----------------------------------------------------------------------------------------------    
    typedef IADLXInterfacePtr_T<IADLXDesktopServices> IADLXDesktopServicesPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXDesktopServices, L"IADLXDesktopServices")
typedef struct IADLXDesktopServices IADLXDesktopServices;
typedef struct IADLXDesktopServicesVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXDesktopServices* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXDesktopServices* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXDesktopServices* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXDesktopServices
    ADLX_RESULT (ADLX_STD_CALL *GetNumberOfDesktops) (IADLXDesktopServices* pThis, adlx_uint* pNumDesktops);
    ADLX_RESULT (ADLX_STD_CALL *GetDesktops) (IADLXDesktopServices* pThis, IADLXDesktopList** ppDesktops);
    ADLX_RESULT (ADLX_STD_CALL *GetDesktopChangedHandling) (IADLXDesktopServices* pThis, IADLXDesktopChangedHandling** ppDesktopChangedHandling);
    ADLX_RESULT (ADLX_STD_CALL *GetSimpleEyefinity) (IADLXDesktopServices* pThis, IADLXSimpleEyefinity** ppSimpleEyefinity);
}IADLXDesktopServicesVtbl;
struct IADLXDesktopServices { const IADLXDesktopServicesVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXDesktopServices 

#endif //ADLX_IDESKTOPS_H
