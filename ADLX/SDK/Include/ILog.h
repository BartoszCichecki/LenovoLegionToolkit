// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------------
//ILog.h - Interfaces for ADLX log functionality
#ifndef ADLX_LOG_H
#define ADLX_LOG_H
#pragma once

#include "ADLXDefines.h"

#pragma region IADLXLog interface
#if defined (__cplusplus)

namespace adlx
{
    class ADLX_NO_VTABLE IADLXLog
    {
    public:
        /**
         * @page DOX_IADLXLog_WriteLog WriteLog
         * @ENG_START_DOX
         * @brief The __WriteLog__ method is called by ADLX when a new ADLX log message is available.
         * @ENG_END_DOX
         * @syntax
         * @codeStart
         *  @ref ADLX_RESULT    WriteLog (const wchar_t* msg)
         * @codeEnd
         *
         * @params
         * @paramrow{1.,[out], msg, const wchar_t*, @ENG_START_DOX The new log message from the internal code execution of ADLX. @ENG_END_DOX}
         *
         * @retvalues
         * @ENG_START_DOX
         * The method must return __ADLX_OK__.<br>
         * @ENG_END_DOX
         *
         * @detaileddesc
         * @ENG_START_DOX
         * @details Once the application enables the ADLX logs with @ref DOX_IADLXSystem_EnableLog using __APPLICATION__ for the @ref ADLX_LOG_DESTINATION parameter, ADLX will call this method when a new log trace is generated. <br>
		 * The method should return quickly to not block the execution path in ADLX. If the method requires a long processing of the log trace, the application must copy the log message and process it asynchronously.
         * @ENG_END_DOX
         *
         * @requirements
         * @DetailsTable{#include"ISystem.h", @ADLX_First_Ver}
         *
         * @seealso
         * @ref DOX_IADLXSystem_EnableLog
         */
        virtual ADLX_RESULT ADLX_STD_CALL WriteLog (const wchar_t* msg) = 0;
    }; //IADLXLog
}
#else
ADLX_DECLARE_IID (IADLXLog, L"IADLXLog");
typedef struct IADLXLog IADLXLog;

typedef struct IADLXLogVtbl
{
    //IADLXLog
    ADLX_RESULT (ADLX_STD_CALL *WriteLog)(IADLXLog* pThis, const wchar_t* msg);
} IADLXLogVtbl;

struct IADLXLog
{
    const IADLXLogVtbl *pVtbl;
};

#endif
#pragma region IADLXLog interface

#endif  //ADLX_LOG_H
