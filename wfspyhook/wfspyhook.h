// wfspyhook.h

#pragma once

namespace wfspy
{
	public __gc __interface IHookInstall
	{
	public:
		void OnInstallHook(System::Byte data[]) = 0;
	};
	
	public __gc class HookHelper
	{
	private:

	public:
		//The type should be derived from IIdleHook
		static void InstallIdleHandler(int processID, int threadID, System::String* assemblyLocation, System::String* typeName, System::Byte additionalData[]);
	};
}
