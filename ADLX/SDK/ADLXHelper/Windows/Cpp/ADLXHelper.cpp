// 
// Copyright (c) 2021 - 2022 Advanced Micro Devices, Inc. All rights reserved.
//
//-------------------------------------------------------------------------------------------------

#include "ADLXHelper.h"

ADLXHelper g_ADLX;

//-------------------------------------------------------------------------------------------------
//Constructor
ADLXHelper::ADLXHelper()
{
}

//-------------------------------------------------------------------------------------------------
//Destructor
ADLXHelper::~ADLXHelper()
{
	Terminate();
}

//-------------------------------------------------------------------------------------------------
//Initialization
ADLX_RESULT ADLXHelper::Initialize ()
{
	return InitializePrivate(nullptr, nullptr);
}

ADLX_RESULT ADLXHelper::InitializeWithIncompatibleDriver()
{
    return InitializePrivate(nullptr, nullptr, true);
}

ADLX_RESULT ADLXHelper::InitializeWithCallerAdl (adlx_handle adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree)
{
	if (adlContext == nullptr || adlMainMemoryFree == nullptr)
	{
		return ADLX_INVALID_ARGS;
	}
	return InitializePrivate(adlContext, adlMainMemoryFree);
}

//-------------------------------------------------------------------------------------------------
//Termination
ADLX_RESULT ADLXHelper::Terminate ()
{
	ADLX_RESULT res = ADLX_OK;
	if (m_hDLLHandle != nullptr)
	{
		m_ADLXFullVersion = 0;
		m_ADLXVersion = nullptr;
		m_pSystemServices = nullptr;
		m_pAdlMapping = nullptr;
		if (nullptr != m_terminateFn)
		{
			res = m_terminateFn();
		}
		m_fullVersionFn = nullptr;
		m_versionFn = nullptr;
		m_initWithADLFn = nullptr;
		m_initFnEx = nullptr;
		m_initFn = nullptr;
		m_terminateFn = nullptr;
		adlx_free_library(m_hDLLHandle);
		m_hDLLHandle = nullptr;
	}
    return res;
}

//-------------------------------------------------------------------------------------------------
//Gets the full version of ADLX
adlx_uint64 ADLXHelper::QueryFullVersion ()
{
    return m_ADLXFullVersion;
}

//-------------------------------------------------------------------------------------------------
//Gets the version of ADLX
const char* ADLXHelper::QueryVersion ()
{
    return m_ADLXVersion;
}

//-------------------------------------------------------------------------------------------------
//Gets the ADLX system interface
adlx::IADLXSystem* ADLXHelper::GetSystemServices ()
{
    return m_pSystemServices;
}

//-------------------------------------------------------------------------------------------------
//Gets the ADL Mapping interface
adlx::IADLMapping* ADLXHelper::GetAdlMapping ()
{
    return m_pAdlMapping;
}

//-------------------------------------------------------------------------------------------------
//Loads ADLX and finds the function pointers to the ADLX functions
ADLX_RESULT ADLXHelper::LoadADLXDll()
{
	if (m_hDLLHandle == nullptr)
	{
		m_hDLLHandle = adlx_load_library(ADLX_DLL_NAME);
		if (m_hDLLHandle)
		{
			m_fullVersionFn = (ADLXQueryFullVersion_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_QUERY_FULL_VERSION_FUNCTION_NAME);
			m_versionFn = (ADLXQueryVersion_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_QUERY_VERSION_FUNCTION_NAME);
			m_initWithADLFn = (ADLXInitializeWithCallerAdl_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_INIT_WITH_CALLER_ADL_FUNCTION_NAME);
			m_initFnEx = (ADLXInitialize_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_INIT_WITH_INCOMPATIBLE_DRIVER_FUNCTION_NAME);
			m_initFn = (ADLXInitialize_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_INIT_FUNCTION_NAME);
			m_terminateFn = (ADLXTerminate_Fn)adlx_get_proc_address(m_hDLLHandle, ADLX_TERMINATE_FUNCTION_NAME);
		}
	}

	if (m_fullVersionFn && m_versionFn && m_initWithADLFn && m_initFnEx && m_initFn && m_terminateFn)
	{
		return ADLX_OK;
	}
	
	return ADLX_FAIL;
}

//-------------------------------------------------------------------------------------------------
//Initializes ADLX based on the  parameters
ADLX_RESULT ADLXHelper::InitializePrivate(adlx_handle  adlContext, ADLX_ADL_Main_Memory_Free adlMainMemoryFree, adlx_bool useIncompatibleDriver)
{
	ADLX_RESULT res = LoadADLXDll();
	if (ADLX_OK == res)
	{
		m_fullVersionFn(&m_ADLXFullVersion);
		m_versionFn(&m_ADLXVersion);
		if (adlContext != nullptr && adlMainMemoryFree != nullptr)
		{
			res = m_initWithADLFn(ADLX_FULL_VERSION, &m_pSystemServices, &m_pAdlMapping, adlContext, adlMainMemoryFree);
		}
		else
		{
			if (useIncompatibleDriver)
			{
				res = m_initFnEx(ADLX_FULL_VERSION, &m_pSystemServices);
			}
			else
			{
				res = m_initFn(ADLX_FULL_VERSION, &m_pSystemServices);
			}
		}
		return res;
	}

	return ADLX_FAIL;
}