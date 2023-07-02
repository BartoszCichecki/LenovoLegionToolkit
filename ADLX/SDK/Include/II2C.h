//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#ifndef ADLX_II2C_H
#define ADLX_II2C_H
#pragma once

#include "ADLXDefines.h"

//-------------------------------------------------------------------------------------------------
//II2C.h - Interfaces for ADLX GPU I2C functionality

//I2C setting interface
#pragma region IADLXI2C
#if defined (__cplusplus)
namespace adlx
{
    class ADLX_NO_VTABLE IADLXI2C : public IADLXInterface
    {
    public:
        ADLX_DECLARE_IID (L"IADLXI2C")

        /**
        *@page DOX_IADLXI2C_Version Version
        *@ENG_START_DOX @brief Retrieves the major and minor software version of I2C interface on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Version (adlx_int* major, adlx_int* minor)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[out],major,adlx_int*,@ENG_START_DOX A pointer to a variable where the I2C major version is returned. @ENG_END_DOX}
        * @paramrow{2.,[out],minor,adlx_int*,@ENG_START_DOX A pointer to a variable where the I2C minor version is returned. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the I2C major version and the I2C minor version are successfully returned, __ADLX_OK__ is returned. <br>
        * If the I2C major version and the I2C minor version are not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *
        *@requirements
        *@DetailsTable{#include "II2C.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Version (adlx_int* major, adlx_int* minor) = 0;

        /**
        *@page DOX_IADLXI2C_IsSupported IsSupported
        *@ENG_START_DOX @brief Checks if the OEM device data can be read and written through the I2C bus on a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    IsSupported (@ref ADLX_I2C_LINE line, adlx_int address, adlx_bool* isSupported)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],line,@ref ADLX_I2C_LINE,@ENG_START_DOX The I2C line. @ENG_END_DOX}
        * @paramrow{2.,[in],address,adlx_int,@ENG_START_DOX The 7-bit I2C slave device address which is shifted one bit to the left. @ENG_END_DOX}
        * @paramrow{2.,[out],isSupported,adlx_bool*,@ENG_START_DOX A pointer to a variable where the status of the I2C bus read and write is returned. The variable is __true__ if the OEM device data can be read and written through the I2C bus. The variable is __false__ if the OEM device data cannot be read and written through the I2C bus. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the status of the I2C bus read and write is successfully returned, __ADLX_OK__ is returned. <br>
        * If the status of the I2C bus read and write is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *
        *
        *@requirements
        *@DetailsTable{#include "II2C.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL IsSupported (ADLX_I2C_LINE line, adlx_int address, adlx_bool* isSupported) = 0;

        /**
        *@page DOX_IADLXI2C_Read Read
        *@ENG_START_DOX @brief Reads the OEM device data through the I2C bus of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Read (@ref ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],line,@ref ADLX_I2C_LINE,@ENG_START_DOX The I2C line. @ENG_END_DOX}
        * @paramrow{2.,[in],speed,adlx_int,@ENG_START_DOX The I2C clock speed (in KHz). @ENG_END_DOX}
        * @paramrow{3.,[in],address,adlx_int,@ENG_START_DOX The 7-bit I2C slave device address which is shifted one bit to the left. @ENG_END_DOX}
        * @paramrow{4.,[in],offset,adlx_int,@ENG_START_DOX The offset of the data from the address. @ENG_END_DOX}
        * @paramrow{5.,[in],dataSize,adlx_int,@ENG_START_DOX The size (in bytes) of the buffer for the OEM device data. @ENG_END_DOX}
        * @paramrow{6.,[in],data,adlx_byte*,@ENG_START_DOX The address to the buffer with the new OEM data. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the OEM device data is successfully returned, __ADLX_OK__ is returned. <br>
        * If the OEM device data is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The application is responsible to allocate a buffer sufficiently large to hold the requested OEM device data. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "II2C.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Read (ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data) = 0;

        /**
        *@page DOX_IADLXI2C_RepeatedStartRead RepeatedStartRead
        *@ENG_START_DOX @brief Repeat start reads the OEM device data through the I2C bus of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    RepeatedStartRead (@ref ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],line,@ref ADLX_I2C_LINE,@ENG_START_DOX The I2C line. @ENG_END_DOX}
        * @paramrow{2.,[in],speed,adlx_int,@ENG_START_DOX The I2C clock speed (in KHz). @ENG_END_DOX}
        * @paramrow{3.,[in],address,adlx_int,@ENG_START_DOX The 7-bit I2C slave device address which is shifted one bit to the left. @ENG_END_DOX}
        * @paramrow{4.,[in],offset,adlx_int,@ENG_START_DOX The offset of the data from the address. @ENG_END_DOX}
        * @paramrow{5.,[in],dataSize,adlx_int,@ENG_START_DOX The size (in bytes) of the buffer for the OEM device data. @ENG_END_DOX}
        * @paramrow{6.,[in],data,adlx_byte*,@ENG_START_DOX The address to the buffer with the new OEM data. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the OEM device data is successfully returned, __ADLX_OK__ is returned. <br>
        * If the OEM device data is not successfully returned, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The application is responsible to allocate a buffer sufficiently large to hold the requested OEM device data. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "II2C.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL RepeatedStartRead (ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data) = 0;

        /**
        *@page DOX_IADLXI2C_Write Write
        *@ENG_START_DOX @brief Writes the OEM device data through the I2C bus of a GPU. @ENG_END_DOX
        *
        *@syntax
        *@codeStart
        * @ref ADLX_RESULT    Write (@ref ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data)
        *@codeEnd
        *
        *@params
        * @paramrow{1.,[in],line,@ref ADLX_I2C_LINE,@ENG_START_DOX The I2C line. @ENG_END_DOX}
        * @paramrow{2.,[in],speed,adlx_int,@ENG_START_DOX The I2C clock speed (in KHz). @ENG_END_DOX}
        * @paramrow{3.,[in],address,adlx_int,@ENG_START_DOX The 7-bit I2C slave device address which is shifted one bit to the left. @ENG_END_DOX}
        * @paramrow{4.,[in],offset,adlx_int,@ENG_START_DOX The offset of the data from the address. @ENG_END_DOX}
        * @paramrow{5.,[in],dataSize,adlx_int,@ENG_START_DOX The size (in bytes) of the buffer for the OEM device data.  @ENG_END_DOX}
        * @paramrow{6.,[in],data,adlx_byte*,@ENG_START_DOX . The address to the buffer with the new OEM data. @ENG_END_DOX}
        *
        *@retvalues
        *@ENG_START_DOX  If the OEM device data is successfully written, __ADLX_OK__ is returned. <br>
        * If the OEM device data is not successfully written, an error code is returned. <br>
        * Refer to @ref ADLX_RESULT for success codes and error codes. @ENG_END_DOX
        *
        *@detaileddesc
        *@ENG_START_DOX @details The application is responsible to allocate a buffer sufficiently large to hold the requested OEM device data. @ENG_END_DOX
        *
        *
        *@requirements
        *@DetailsTable{#include "II2C.h", @ADLX_First_Ver}
        *
        */
        virtual ADLX_RESULT         ADLX_STD_CALL Write (ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data) = 0;

    };  //IADLXI2C
     //----------------------------------------------------------------------------------------------
    typedef IADLXInterfacePtr_T<IADLXI2C> IADLXI2CPtr;
} //namespace adlx
#else //__cplusplus
ADLX_DECLARE_IID (IADLXI2C, L"IADLXI2C")

typedef struct IADLXI2C IADLXI2C;

typedef struct IADLXI2CVtbl
{
    //IADLXInterface
    adlx_long (ADLX_STD_CALL *Acquire)(IADLXI2C* pThis);
    adlx_long (ADLX_STD_CALL *Release)(IADLXI2C* pThis);
    ADLX_RESULT (ADLX_STD_CALL *QueryInterface)(IADLXI2C* pThis, const wchar_t* interfaceId, void** ppInterface);

    //IADLXI2C
    ADLX_RESULT(ADLX_STD_CALL *Version)(IADLXI2C* pThis, adlx_int* major, adlx_int* minor);
    ADLX_RESULT(ADLX_STD_CALL *IsSupported)(IADLXI2C* pThis, ADLX_I2C_LINE line, adlx_int address, adlx_bool* isSupported);
    ADLX_RESULT(ADLX_STD_CALL *Read)(IADLXI2C* pThis, ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data);
    ADLX_RESULT(ADLX_STD_CALL *RepeatedStartRead)(IADLXI2C* pThis, ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data);
    ADLX_RESULT(ADLX_STD_CALL *Write)(IADLXI2C* pThis, ADLX_I2C_LINE line, adlx_int speed, adlx_int address, adlx_int offset, adlx_int dataSize, adlx_byte* data);

}IADLXI2CVtbl;

struct IADLXI2C { const IADLXI2CVtbl *pVtbl; };
#endif //__cplusplus
#pragma endregion IADLXI2C

#endif //ADLX_II2C_H
