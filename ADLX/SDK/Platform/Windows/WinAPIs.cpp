// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------
//This abstracts Win32 APIs in ADLX ones so we insulate from platform
#include "../../Include/ADLXDefines.h"
#include <cstdlib>

#if defined(_WIN32) // Microsoft compiler
    #include <Windows.h>
#else
#error define your copiler
#endif
static volatile uint64_t v = 0;

//----------------------------------------------------------------------------------------
// threading
//----------------------------------------------------------------------------------------
adlx_long ADLX_CDECL_CALL adlx_atomic_inc (adlx_long* X)
{
#if defined(_WIN32) // Microsoft compiler
    return InterlockedIncrement ((long*)X);
#endif
}

//----------------------------------------------------------------------------------------
adlx_long ADLX_CDECL_CALL adlx_atomic_dec (adlx_long* X)
{
#if defined(_WIN32) // Microsoft compiler
    return InterlockedDecrement((long*)X);
#endif
}

//----------------------------------------------------------------------------------------
adlx_handle ADLX_CDECL_CALL adlx_load_library (const TCHAR* filename)
{
#if defined(METRO_APP)
    return LoadPackagedLibrary (filename, 0);
#elif defined(_WIN32) // Microsoft compiler
    return ::LoadLibraryEx (filename, nullptr, LOAD_LIBRARY_SEARCH_USER_DIRS |
        LOAD_LIBRARY_SEARCH_APPLICATION_DIR |
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS |
        LOAD_LIBRARY_SEARCH_SYSTEM32);
#endif
}

//----------------------------------------------------------------------------------------
int ADLX_CDECL_CALL adlx_free_library (adlx_handle module)
{
#if defined(_WIN32) // Microsoft compiler
	return ::FreeLibrary((HMODULE)module) == TRUE;
#endif
}

//----------------------------------------------------------------------------------------
void* ADLX_CDECL_CALL adlx_get_proc_address (adlx_handle module, const char* procName)
{
#if defined(_WIN32) // Microsoft compiler
	return (void*)::GetProcAddress((HMODULE)module, procName);
#endif
}


//#endif //_WIN32