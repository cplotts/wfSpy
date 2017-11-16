// wfspyhook.h

#pragma once

namespace wfspy
{
	public interface class IHookInstall
	{
	public:
		void OnInstallHook(array<System::Byte>^ data) = 0;
	};
	
	public ref class HookHelper
	{
	private:

	public:
		//The type should be derived from IIdleHook
		static void InstallIdleHandler(int processID, int threadID, System::String^ assemblyLocation, System::String^ typeName, array<System::Byte>^ additionalData);
	};
}
