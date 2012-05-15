// This is the main DLL file.

#include "stdafx.h"
#include "wfspyhook.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace wfspy;
using namespace System::Reflection;

wchar_t __gc* GetAssemblyPath()
{
	return PtrToStringChars(Assembly::GetExecutingAssembly()->Location);
}

HMODULE GetThisModuleHandle()
{
	wchar_t __pin* wszModule = GetAssemblyPath();
	return GetModuleHandle(wszModule);
}

wchar_t __gc* GetFileMappingName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String* fileMappingName = String::Format(S"wfavhook_{0}_{1}_filemap", __box(processId), __box(threadId)); 
	return PtrToStringChars(fileMappingName);
}

wchar_t __gc* GetEventName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String* eventName = String::Format(S"wfavhook_{0}_{1}_event", __box(processId), __box(threadId)); 
	return PtrToStringChars(eventName);
}

template<class T>
void _Win32X(T t)
{
	if (t == NULL)
		Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
}

struct WFAVCommunicationData
{
	BOOL m_bErrorFlag;
	HANDLE m_hFileMap; //Stored to help in cleanup
	HANDLE m_hEvent; //To be set when assembly is injected
	DWORD m_dwAssemblyPathSize;
	DWORD m_dwTypeNameSize;
	DWORD m_dwDataSize; //Length of additional data
	BYTE ab[1];
	
	//Assembly name starts at offset 0
	LPWSTR _GetAssemblyName()
	{
		return reinterpret_cast<LPWSTR>(&ab[0]);
	}
	
	//Immediately after the assembly name
	LPWSTR _GetTypeName()
	{
		return reinterpret_cast<LPWSTR>(&ab[m_dwAssemblyPathSize]);
	}
	
	//Immediately after the type name
	BYTE* _GetData()
	{
		return (&ab[m_dwAssemblyPathSize + m_dwTypeNameSize]);
	}
	
	//managed variants of the above methods
	String* GetAssemblyName()
	{
		return _GetAssemblyName();
	}
	
	String* GetTypeName()
	{
		return _GetTypeName();
	}
	
	Byte GetData() []
	{
		Byte data[] = new Byte[m_dwDataSize];
		Marshal::Copy(IntPtr(_GetData()), data, 0, m_dwDataSize);

		return data;
	}
	
	void Cleanup()
	{
		if (m_hEvent)
			CloseHandle(m_hEvent);

		if (m_hFileMap)
			CloseHandle(m_hFileMap);
	}
	
	static WFAVCommunicationData* Construct(int processID, int threadID, String* assemblyPath, String* typeName, Byte additionalData[])
	{
		DWORD dwAssemblyPathSize = (assemblyPath->Length + 1)*sizeof(WCHAR);
		DWORD dwTypeNameSize = (typeName->Length + 1)*sizeof(WCHAR);
		DWORD dwDataSize = additionalData->Length;
	
		DWORD cbTotalLen = 
				dwAssemblyPathSize + dwTypeNameSize + dwDataSize + //Length of data
				sizeof(DWORD)*3 + //Length of the data length holders
				sizeof(WFAVCommunicationData); //Size of structure itself
		
		WFAVCommunicationData* pwfcd = NULL;

		try
		{
			wchar_t __pin * wszFileMappingName = GetFileMappingName(processID, threadID);
			HANDLE hFileMapping = CreateFileMapping(NULL, NULL, PAGE_READWRITE, 0, cbTotalLen, wszFileMappingName);
			
			int err = GetLastError();

			//Could not be created
			_Win32X(hFileMapping);

			if (err == ERROR_ALREADY_EXISTS)
			{
				CloseHandle(hFileMapping);
				throw new InvalidOperationException(); //Only one process can inject at a time
			}

			pwfcd = (WFAVCommunicationData*)MapViewOfFile(hFileMapping, FILE_MAP_ALL_ACCESS, 0, 0, 0);
			
			_Win32X(pwfcd);
			
			pwfcd->m_bErrorFlag = FALSE;
			pwfcd->m_hFileMap = hFileMapping;
			pwfcd->m_hEvent = NULL;
			pwfcd->m_dwAssemblyPathSize = dwAssemblyPathSize;
			pwfcd->m_dwDataSize = dwDataSize;
			pwfcd->m_dwTypeNameSize = dwTypeNameSize;
			
			wchar_t __pin * wszEventName = GetEventName(processID, threadID);
			pwfcd->m_hEvent = CreateEvent(NULL, FALSE, FALSE, wszEventName);
			
			if (pwfcd->m_hEvent == NULL)
				Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));

			HANDLE hTargetProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processID);

			_Win32X(hTargetProcess);

			//Now copy the strings
			wchar_t __pin * wszAssemblyPath = PtrToStringChars(assemblyPath);
			wchar_t __pin * wszTypeName = PtrToStringChars(typeName);
			
			lstrcpy(pwfcd->_GetAssemblyName(), wszAssemblyPath);
			lstrcpy(pwfcd->_GetTypeName(), wszTypeName);
			Marshal::Copy(additionalData, 0, IntPtr(pwfcd->_GetData()), pwfcd->m_dwDataSize);
		
			CloseHandle(hTargetProcess);
		}
		catch(Exception* e)
		{
			if (pwfcd)
			{
				pwfcd->Cleanup();
			}

			throw e;
		}

		return pwfcd;
	}
};

//The hook proc
extern "C" DWORD CALLBACK LocalHookProc(int code, DWORD wParam, LONG lParam)
{
	if (code < 0)
		return CallNextHookEx(NULL, code, wParam, lParam);

	DWORD dwRet;
	
	HANDLE hMap = NULL;
	HANDLE hEvent = NULL;
	BOOL bSuccess = false;
	WFAVCommunicationData* pwfcd = NULL;

	try
	{
		wchar_t __pin* wszFileMappingName = GetFileMappingName();
		hMap = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, wszFileMappingName);
		
		_Win32X(hMap);
		
		wchar_t __pin* wszEventName = GetEventName();
		hEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, wszEventName);
		
		_Win32X(hEvent);
		
		pwfcd = (WFAVCommunicationData*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		
		if (!pwfcd)
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
		
		if (hEvent)
		{
			SetEvent(hEvent);
		}

		Assembly* assem = Assembly::LoadFrom(pwfcd->GetAssemblyName());
		String* typeName = pwfcd->GetTypeName();
		Type* type = assem->GetType(typeName, true, true);
		IHookInstall* hookinstall = __try_cast<IHookInstall*>(Activator::CreateInstance(type));
 		hookinstall->OnInstallHook(pwfcd->GetData());

		bSuccess = true;
	}
	catch(Exception* e)
	{
		System::Diagnostics::Trace::WriteLine(e->ToString());
		
		if (pwfcd)
			pwfcd->m_bErrorFlag = TRUE;

		if (hEvent)
		{
			SetEvent(hEvent);
		}
	}
	
	if (hMap)
		CloseHandle(hMap);

	if (hEvent)	
		CloseHandle(hEvent);

	dwRet = CallNextHookEx(NULL, code, wParam, lParam);

	return dwRet;
}

void HookHelper::InstallIdleHandler(int processID, int threadID, String* assemblyLocation, String* typeName, Byte additionalData[])
{
	WFAVCommunicationData* pwfcd = WFAVCommunicationData::Construct(processID, threadID, assemblyLocation, typeName, additionalData);

	__try
	{
		HMODULE hMod = GetThisModuleHandle();
		HOOKPROC proc = (HOOKPROC)GetProcAddress(hMod, "LocalHookProc");
		//HHOOK hhook = SetWindowsHookEx(WH_FOREGROUNDIDLE, proc, hMod, threadID);
		HHOOK hhook = SetWindowsHookEx(WH_GETMESSAGE, proc, hMod, threadID);

		if (hhook)
		{
			//Post a simple message
			PostThreadMessage(threadID, WM_NULL, 0, 0);

			bool bError = (WaitForSingleObject(pwfcd->m_hEvent, 60*1000) != WAIT_OBJECT_0);
			
			UnhookWindowsHookEx(hhook);

			if (bError || pwfcd->m_bErrorFlag)
				throw new ApplicationException(S"Error waiting for hook to be setup");
		}
		else
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
	}
	__finally
	{
		pwfcd->Cleanup();
	}
}

