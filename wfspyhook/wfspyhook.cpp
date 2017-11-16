// This is the main DLL file.

#include "stdafx.h"
#include "wfspyhook.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace wfspy;
using namespace System::Reflection;

const interior_ptr<const wchar_t> GetAssemblyPath()
{
	return PtrToStringChars(Assembly::GetExecutingAssembly()->Location);
}

HMODULE GetThisModuleHandle()
{
	pin_ptr<const wchar_t> wszModule = GetAssemblyPath();
	return GetModuleHandle(wszModule);
}

interior_ptr<const wchar_t> GetFileMappingName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String^ fileMappingName = String::Format("wfavhook_{0}_{1}_filemap", processId, threadId);
	return PtrToStringChars(fileMappingName);
}

interior_ptr<const wchar_t> GetEventName(int processId = GetCurrentProcessId(), int threadId = GetCurrentThreadId())
{
	String^ eventName = String::Format("wfavhook_{0}_{1}_event", processId, threadId);
	return PtrToStringChars(eventName);
}

void DumpString(String^ name, String^ text)
{
	String^ message = String::Format("DumpString {0} {1:x04}", name, text->Length);
	for (size_t i = 0; i < text->Length; i++)
	{
		if (i % 8 == 0) message = message + ": ";
		else if (i % 4 == 0) message = message + ". ";
		message = message + String::Format("{0:x02} ", (int)text[i]);
	}
	System::Diagnostics::Debug::WriteLine(message);
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
	String^ GetAssemblyName()
	{
		return GetString(_GetAssemblyName(), m_dwAssemblyPathSize);
	}
	
	String^ GetTypeName()
	{
		return GetString(_GetTypeName(), m_dwTypeNameSize);
	}
	
	array<System::Byte>^ GetData()
	{
		array<System::Byte>^ data = gcnew array<System::Byte>(m_dwDataSize);
		Marshal::Copy(IntPtr(_GetData()), data, 0, m_dwDataSize);
		return data;
	}

	String^ GetString(LPWSTR rawData, DWORD dwDataSize)
	{
		int length = dwDataSize / sizeof(WCHAR) - 1;
		return gcnew String(rawData, 0, length);
	}

	void Cleanup()
	{
		if (m_hEvent)
			CloseHandle(m_hEvent);

		if (m_hFileMap)
			CloseHandle(m_hFileMap);
	}
	
	static WFAVCommunicationData* Construct(int processID, int threadID, String^ assemblyPath, String^ typeName, array<System::Byte>^ additionalData)
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
			pin_ptr<const wchar_t> wszFileMappingName = GetFileMappingName(processID, threadID);
			HANDLE hFileMapping = CreateFileMapping(NULL, NULL, PAGE_READWRITE, 0, cbTotalLen, wszFileMappingName);
			
			int err = GetLastError();

			//Could not be created
			_Win32X(hFileMapping);

			if (err == ERROR_ALREADY_EXISTS)
			{
				CloseHandle(hFileMapping);
				throw gcnew InvalidOperationException(); //Only one process can inject at a time
			}

			pwfcd = (WFAVCommunicationData*)MapViewOfFile(hFileMapping, FILE_MAP_ALL_ACCESS, 0, 0, 0);
			
			_Win32X(pwfcd);
			
			pwfcd->m_bErrorFlag = FALSE;
			pwfcd->m_hFileMap = hFileMapping;
			pwfcd->m_hEvent = NULL;
			pwfcd->m_dwAssemblyPathSize = dwAssemblyPathSize;
			pwfcd->m_dwDataSize = dwDataSize;
			pwfcd->m_dwTypeNameSize = dwTypeNameSize;
			
			pin_ptr<const wchar_t> wszEventName = GetEventName(processID, threadID);
			pwfcd->m_hEvent = CreateEvent(NULL, FALSE, FALSE, wszEventName);
			
			if (pwfcd->m_hEvent == NULL)
				Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));

			HANDLE hTargetProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processID);

			_Win32X(hTargetProcess);

			//Now copy the strings
			pin_ptr<const wchar_t> wszAssemblyPath = PtrToStringChars(assemblyPath);
			pin_ptr<const wchar_t> wszTypeName = PtrToStringChars(typeName);
			
			lstrcpy(pwfcd->_GetAssemblyName(), wszAssemblyPath);
			lstrcpy(pwfcd->_GetTypeName(), wszTypeName);
			Marshal::Copy(additionalData, 0, IntPtr(pwfcd->_GetData()), pwfcd->m_dwDataSize);
		
			CloseHandle(hTargetProcess);
		}
		catch(Exception^ e)
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
	{
		//System::Diagnostics::Debug::WriteLine(String::Format("LocalHookProc chain: code={0:X04}, wParam={1:X08}, lParam={2:X08}", code, wParam, lParam));
		return CallNextHookEx(NULL, code, wParam, lParam);
	}
	//System::Diagnostics::Debug::WriteLine("LocalHookProc enter");

	DWORD dwRet;
	
	HANDLE hMap = NULL;
	HANDLE hEvent = NULL;
	BOOL bSuccess = false;
	WFAVCommunicationData* pwfcd = NULL;

	try
	{
		pin_ptr<const wchar_t> wszFileMappingName = GetFileMappingName();
		hMap = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, wszFileMappingName);
		
		_Win32X(hMap);
		
		pin_ptr<const wchar_t> wszEventName = GetEventName();
		hEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, wszEventName);
		
		_Win32X(hEvent);
		
		pwfcd = (WFAVCommunicationData*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		
		if (!pwfcd)
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
		
		if (hEvent)
		{
			SetEvent(hEvent);
		}

		String^ assemblyName = pwfcd->GetAssemblyName();
		System::Diagnostics::Debug::WriteLine(String::Format("LocalHookProc AssemblyName: '{0}'", assemblyName));
		Assembly^ assem = Assembly::LoadFrom(assemblyName);
		String^ typeName = pwfcd->GetTypeName();
		Type^ type = assem->GetType(typeName, true, true);
		IHookInstall^ hookinstall = safe_cast<IHookInstall^>(Activator::CreateInstance(type));
 		hookinstall->OnInstallHook(pwfcd->GetData());

		bSuccess = true;
	}
	catch(Exception^ e)
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

void HookHelper::InstallIdleHandler(int processID, int threadID, String^ assemblyLocation, String^ typeName, array<System::Byte>^ additionalData)
{
	System::Diagnostics::Debug::WriteLine("InstallIdleHandler enter");
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
			{
				throw gcnew ApplicationException(String::Format("Error waiting for hook to be setup, Error={0:X08}, ErrorFlag={1:X08}", bError, pwfcd->m_bErrorFlag));
			}
		}
		else
		{
			Marshal::ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
		}
	}
	__finally
	{
		pwfcd->Cleanup();
	}
}

