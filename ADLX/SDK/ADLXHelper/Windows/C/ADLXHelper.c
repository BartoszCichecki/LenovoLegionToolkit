//
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------
#include "ADLXHelper.h"

typedef struct ADLXHelper
{
	//Handle to the ADLX dll
	adlx_handle m_hDLLHandle;

	//Full Version of this ADLX instance
	adlx_uint64 m_ADLXFullVersion;

	//Version of this ADLX instance
	const char* m_ADLXVersion;

	//The ADLX system services interface
	IADLXSystem* m_pSystemServices;

	//the ADL mapping interface
	IADLMapping* m_pAdlMapping;

	//ADLX function - query full version
	ADLXQueryFullVersion_Fn m_fullVersionFn;

	//ADLX function - qery version
	ADLXQueryVersion_Fn m_versionFn;

	//ADLX function - initialize with ADL
	ADLXInitializeWithCallerAdl_Fn m_initWithADLFn;

	//ADLX function - initialize with incompatible driver
	ADLXInitialize_Fn m_initFnEx;

	//ADLX function - initialize
	ADLXInitialize_Fn m_initFn;

	//ADLX function - terminate
	ADLXTerminate_Fn m_terminateFn;
}ADLXHelper;

static ADLXHelper g_ADLX = {NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL};

ADLX_RESULT InitializePrivate (adlx_handle  adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree, adlx_bool useIncompatibleDriver);

//-------------------------------------------------------------------------------------------------
//Initialization
ADLX_RESULT ADLXHelper_Initialize ()
{
    return InitializePrivate (NULL, NULL, false);
}

ADLX_RESULT ADLXHelper_InitializeWithIncompatibleDriver()
{
    return InitializePrivate(NULL, NULL, true);
}

ADLX_RESULT ADLXHelper_InitializeWithCallerAdl (adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
{
	if (adlContext == NULL || adlMainMemoryFree == NULL)
	{
		return ADLX_INVALID_ARGS;
	}
    return InitializePrivate (adlContext, adlMainMemoryFree, false);
}

//-------------------------------------------------------------------------------------------------
//Termination
ADLX_RESULT ADLXHelper_Terminate()
{
	ADLX_RESULT res = ADLX_OK;
	if (g_ADLX.m_hDLLHandle != NULL)
	{
		g_ADLX.m_ADLXFullVersion = 0;
		g_ADLX.m_ADLXVersion = NULL;
		g_ADLX.m_pSystemServices = NULL;
		g_ADLX.m_pAdlMapping = NULL;
		if (NULL != g_ADLX.m_terminateFn)
		{
			res = g_ADLX.m_terminateFn();
		}
		g_ADLX.m_fullVersionFn = NULL;
		g_ADLX.m_versionFn = NULL;
		g_ADLX.m_initWithADLFn = NULL;
		g_ADLX.m_initFnEx = NULL;
		g_ADLX.m_initFn = NULL;
		g_ADLX.m_terminateFn = NULL;
		adlx_free_library(g_ADLX.m_hDLLHandle);
		g_ADLX.m_hDLLHandle = NULL;
	}
	return res;
}

//-------------------------------------------------------------------------------------------------
//Get the full version for this ADLX instance
adlx_uint64 ADLXHelper_QueryFullVersion()
{
	return g_ADLX.m_ADLXFullVersion;
}

//-------------------------------------------------------------------------------------------------
//Get the version for this ADLX instance
const char* ADLXHelper_QueryVersion()
{
	return g_ADLX.m_ADLXVersion;
}

//-------------------------------------------------------------------------------------------------
//Get the IADLXSystem interface
IADLXSystem* ADLXHelper_GetSystemServices()
{
	return g_ADLX.m_pSystemServices;
}

//-------------------------------------------------------------------------------------------------
//Get the IADLMapping interface
IADLMapping* ADLXHelper_GetAdlMapping ()
{
    return g_ADLX.m_pAdlMapping;
}

//-------------------------------------------------------------------------------------------------
//Loads ADLX and finds the function pointers to the ADLX functions
ADLX_RESULT LoadADLXDll()
{
	if (g_ADLX.m_hDLLHandle == NULL)
	{
		g_ADLX.m_hDLLHandle = adlx_load_library(ADLX_DLL_NAME);
		if (g_ADLX.m_hDLLHandle)
		{
			g_ADLX.m_fullVersionFn = (ADLXQueryFullVersion_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_QUERY_FULL_VERSION_FUNCTION_NAME);
			g_ADLX.m_versionFn = (ADLXQueryVersion_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_QUERY_VERSION_FUNCTION_NAME);
			g_ADLX.m_initWithADLFn = (ADLXInitializeWithCallerAdl_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME);
			g_ADLX.m_initFnEx = (ADLXInitialize_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME);
			g_ADLX.m_initFn = (ADLXInitialize_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_INIT_FUNCTION_NAME);
			g_ADLX.m_terminateFn = (ADLXTerminate_Fn)adlx_get_proc_address(g_ADLX.m_hDLLHandle, ADLX_TERMINATE_FUNCTION_NAME);
		}
	}
	if (g_ADLX.m_fullVersionFn && g_ADLX.m_versionFn && g_ADLX.m_initWithADLFn && g_ADLX.m_initFnEx && g_ADLX.m_initFn && g_ADLX.m_terminateFn)
	{
		return ADLX_OK;
	}
	return ADLX_FAIL;
}

//-------------------------------------------------------------------------------------------------
//Initializes ADLX based on the  parameters
ADLX_RESULT InitializePrivate (adlx_handle  adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree, adlx_bool useIncompatibleDriver)
{
	ADLX_RESULT res = LoadADLXDll();
	if (ADLX_OK == res)
	{
		g_ADLX.m_fullVersionFn(&g_ADLX.m_ADLXFullVersion);
		g_ADLX.m_versionFn(&g_ADLX.m_ADLXVersion);
		if (adlContext != NULL && adlMainMemoryFree != NULL)
		{
			res = g_ADLX.m_initWithADLFn(ADLX_FULL_VERSION, &g_ADLX.m_pSystemServices, &g_ADLX.m_pAdlMapping, adlContext, adlMainMemoryFree);
		}
		else
		{
			if (useIncompatibleDriver)
			{
				res = g_ADLX.m_initFnEx(ADLX_FULL_VERSION, &g_ADLX.m_pSystemServices);
			}
			else
			{
				res = g_ADLX.m_initFn(ADLX_FULL_VERSION, &g_ADLX.m_pSystemServices);
			}
		}
		return res;
	}

	return ADLX_FAIL;
}
