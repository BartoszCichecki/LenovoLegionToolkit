//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_ICOLLECTIONS_H
#define ADLX_ICOLLECTIONS_H
#pragma once

#include "ADLXDefines.h"

//-------------------------------------------------------------------------------------------------
//ICollections.h - Interfaces for ADLX collections

// These interfaces are used when ADLX returns or receives many-of collections

//IADLXList allows iterating forward in a collection of objects, similar with an stl vector
#pragma region IADLXList interface
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXList : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXList")
        //Lists must declare the type of items it holds - what was passed as ADLX_DECLARE_IID() in that interface
        ADLX_DECLARE_ITEM_IID (L"IADLXInterface")

        /**
        @page DOX_IADLXList_Size Size
        @ENG_START_DOX
        @brief Returns the number of interfaces in a list.
        @ENG_END_DOX

        @syntax
        @codeStart
        adlx_uint    Size ()
        @codeEnd

        @params

        N/A

        @retvalues
        @ENG_START_DOX
        Returns the number of interfaces in the list.
        @ENG_END_DOX

        @detaileddesc
        @ENG_START_DOX
        @details Returns the number of interfaces in the list.<br>

        Returns zero if the list is empty.

        @ENG_END_DOX

        @requirements
        @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

        */
        virtual adlx_uint           ADLX_STD_CALL Size () = 0;
        /**
          @page DOX_IADLXList_Empty Empty
          @ENG_START_DOX
          @brief Checks if the list is empty.
          @ENG_END_DOX

          @syntax
          @codeStart
           adlx_bool    Empty ()
          @codeEnd

          @params

          N/A

          @retvalues
          @ENG_START_DOX
          If the list is empty, __true__ is returned.<br>
          If the list is not empty, __false__ is returned.<br>
          @ENG_END_DOX

          @detaileddesc
          @ENG_START_DOX
          @details If the list has no interfaces, then @ref DOX_IADLXList_Size will return zero, and __Empty__ will return __true__.
          @ENG_END_DOX

          @requirements
          @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

         */
        virtual adlx_bool           ADLX_STD_CALL Empty () = 0;

        /**
          @page DOX_IADLXList_Begin Begin
          @ENG_START_DOX
          @brief Returns the location of the first interface from a list.
          @ENG_END_DOX

          @syntax
          @codeStart
           adlx_uint    Begin ()
          @codeEnd

          @params

          N/A

          @retvalues
          @ENG_START_DOX
          If the list is not empty it returns the location of the first interface in the list.<br>
          If the list is empty the returned location is same as the location returned by @ref DOX_IADLXList_End.<br>
          @ENG_END_DOX

          @detaileddesc
          @ENG_START_DOX
          @details The returned location is valid if it is less than the location returned by @ref DOX_IADLXList_End.
          The __Begin__ is used in combination with @ref DOX_IADLXList_End for parsing the list sequentially from the beginning to the end.<br>
          If the list is empty, __Begin__ and @ref DOX_IADLXList_End return the same location.
          @ENG_END_DOX
          @image  html list.png height=150
          @snippetCode
          @snippet ADLXSnippet.h Iterate ADLXList

          @requirements
          @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

          */
        virtual adlx_uint           ADLX_STD_CALL Begin () = 0;
        /**
         @page DOX_IADLXList_End End
         @ENG_START_DOX
         @brief Returns the location succeeding the last interface from a list.
         @ENG_END_DOX

         @syntax
         @codeStart
          adlx_uint    End ()
         @codeEnd

         @params

         N/A

         @retvalues
         @ENG_START_DOX
         Returns the location succeeding the last interface from a list.
         @ENG_END_DOX

         @detaileddesc
         @ENG_START_DOX
         @details The returned location is a logical representation of the end of the list. It does not hold any valid interface and should not be dereferenced.<br>
         __End__ is used in combination with @ref DOX_IADLXList_Begin for parsing the list sequentially from the beginning to the end.<br>
         If the list is empty, then @ref DOX_IADLXList_Begin and __End__ return the same location.

         @ENG_END_DOX
         @image  html list.png height=150
         @snippetCode
         @snippet ADLXSnippet.h Iterate ADLXList

         @requirements
         @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

         */
        virtual adlx_uint           ADLX_STD_CALL End () = 0;

        /**
        @page DOX_IADLXList_At At
        @ENG_START_DOX
        @brief Returns the reference counted interface from the requested location.
        @ENG_END_DOX

        @syntax
        @codeStart
         @ref ADLX_RESULT    At (const adlx_uint location, @ref DOX_IADLXInterface** ppItem)
        @codeEnd

        @params
        @paramrow{1.,[in] ,location,const adlx_uint,@ENG_START_DOX The location of the returned interface. @ENG_END_DOX}
        @paramrow{2.,[out] ,ppItem,@ref DOX_IADLXInterface**,@ENG_START_DOX The address of a pointer to the returned interface. If the interface is not successfully returned\, the method sets dereferenced address __*ppItem__ to __nullptr__. @ENG_END_DOX}

        @retvalues
        @ENG_START_DOX
        If the location is within the list bounds, __ADLX_OK__ is returned.<br>
        If the location is not within the list bounds, an error code is returned.<br>
        Refer to @ref ADLX_RESULT for success codes and error codes.

        @ENG_END_DOX

        @detaileddesc
        @ENG_START_DOX
        @details @ref DOX_IADLXList is a collection of interfaces which runs incrementally from beginning to the end.<br> The returned interface must be discarded with @ref DOX_IADLXInterface_Release when no longer needed.
        @ENG_END_DOX
        @addinfo
        @ENG_START_DOX
        In C++, when using ADLX interfaces as smart pointers, there is no need to call @ref DOX_IADLXInterface_Release because smart pointers call it in their internal implementation.
        @ENG_END_DOX

        @requirements
        @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

        */
        virtual ADLX_RESULT         ADLX_STD_CALL At (const adlx_uint location, IADLXInterface** ppItem) = 0;

        /**
         @page DOX_IADLXList_Clear Clear
         @ENG_START_DOX
         @brief Removes all the interfaces from a list.
         @ENG_END_DOX

         @syntax
         @codeStart
          @ref ADLX_RESULT    Clear ()
         @codeEnd

         @params

         N/A

         @retvalues
         @ENG_START_DOX
         If all the interfaces from the list are successfully removed, __ADLX_OK__ is returned. <br>
         If interfaces from the list are not successfully removed, an error code is returned. <br>
         Refer to @ref ADLX_RESULT for success codes and error codes.

         @ENG_END_DOX

         @detaileddesc
         @ENG_START_DOX
         @details After this call returns successfully, @ref DOX_IADLXList_Size returns zero.
         @ENG_END_DOX

         @requirements
         @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

         */
        virtual ADLX_RESULT         ADLX_STD_CALL Clear () = 0;
        /**
        @page DOX_IADLXList_Remove_Back Remove_Back
        @ENG_START_DOX
        @brief Removes an interface from the end of a list.
        @ENG_END_DOX

        @syntax
        @codeStart
         @ref ADLX_RESULT    Remove_Back ()
        @codeEnd

        @params

        N/A

        @retvalues
        @ENG_START_DOX
        If the interface from the end of the list is successfully removed, __ADLX_OK__ is returned. <br>
        If the interface from the end of the list is not successfully removed, an error code is returned. <br>
        Refer to @ref ADLX_RESULT for success codes and error codes.

        @ENG_END_DOX

        @detaileddesc
        @ENG_START_DOX
        @details Removes an interface from the end of the list. The list must not be empty.
        @ENG_END_DOX

        @requirements
        @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

        */
        virtual ADLX_RESULT         ADLX_STD_CALL Remove_Back () = 0;
        /**
        @page DOX_IADLXList_Add_Back Add_Back
        @ENG_START_DOX
        @brief Adds an interface to the end of a list.
        @ENG_END_DOX

        @syntax
        @codeStart
         @ref ADLX_RESULT    Add_Back (@ref DOX_IADLXInterface* pItem)
        @codeEnd

        @params
        @paramrow{1.,[in] ,pItem,@ref DOX_IADLXInterface*,@ENG_START_DOX The pointer to the interface to be added to the list. @ENG_END_DOX}

        @retvalues
        @ENG_START_DOX
        If the interface is added successfully to the end of the list, __ADLX_OK__ is returned. <br>
        If the interface is not added to the end of the list, an error code is returned. <br>
        Refer to @ref ADLX_RESULT for success codes and error codes.

        @ENG_END_DOX


        @requirements
        @DetailsTable{#include"ICollections.h", @ADLX_First_Ver}

        */
        virtual ADLX_RESULT         ADLX_STD_CALL Add_Back (IADLXInterface* pItem) = 0;
    };  //IADLXList
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXList> IADLXListPtr;
}   // namespace adlx
#else
ADLX_DECLARE_IID (IADLXList, L"IADLXList")
ADLX_DECLARE_ITEM_IID (IADLXInterface, L"IADLXInterface")
typedef struct IADLXList IADLXList;
typedef struct IADLXListVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXList* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXList* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXList
    adlx_uint (ADLX_STD_CALL *Size)(IADLXList* pThis);
    adlx_bool (ADLX_STD_CALL *Empty)(IADLXList* pThis);
    adlx_uint (ADLX_STD_CALL *Begin)(IADLXList* pThis);
    adlx_uint (ADLX_STD_CALL *End)(IADLXList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *At)(IADLXList* pThis, const adlx_uint location, IADLXInterface** ppItem);
    ADLX_RESULT (ADLX_STD_CALL *Clear)(IADLXList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Remove_Back)(IADLXList* pThis);
    ADLX_RESULT (ADLX_STD_CALL *Add_Back)(IADLXList* pThis, IADLXInterface* pItem);
}IADLXListVtbl;

struct IADLXList
{
    const IADLXListVtbl *pVtbl;
};

#endif
#pragma endregion IADLXList interface

#endif//ADLX_ICOLLECTIONS_H
