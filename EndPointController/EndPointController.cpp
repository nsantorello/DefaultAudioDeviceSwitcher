// EndPointController.cpp : Defines the entry point for the console application.
//
#include <stdio.h>
#include <wchar.h>
#include <tchar.h>
#include "windows.h"
#include "Mmdeviceapi.h"
#include "PolicyConfig.h"
#include "Propidl.h"
#include "Functiondiscoverykeys_devpkey.h"

extern "C" HRESULT SetDefaultAudioPlaybackDevice(LPCWSTR devID)
{	
	IPolicyConfigVista *pPolicyConfig;
	ERole reserved = eConsole;

    HRESULT hr = CoCreateInstance(__uuidof(CPolicyConfigVistaClient), 
		NULL, CLSCTX_ALL, __uuidof(IPolicyConfigVista), (LPVOID *)&pPolicyConfig);
	if (SUCCEEDED(hr))
	{
		hr = pPolicyConfig->SetDefaultEndpoint(devID, reserved);
		pPolicyConfig->Release();
	}
	return hr;
}

extern "C" __declspec(dllexport) void __stdcall GetDevices(LPWSTR* deviceNames)
{
	HRESULT hr = CoInitialize(NULL);
	if (SUCCEEDED(hr))
	{
		IMMDeviceEnumerator *pEnum = NULL;
		// Create a multimedia device enumerator.
		hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL,
			CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnum);
		if (SUCCEEDED(hr))
		{
			IMMDeviceCollection *pDevices;
			// Enumerate the output devices.
			hr = pEnum->EnumAudioEndpoints(eRender, DEVICE_STATE_ACTIVE, &pDevices);
			if (SUCCEEDED(hr))
			{
				UINT count;
				pDevices->GetCount(&count);
				if (SUCCEEDED(hr))
				{
					for (int i = 0; i < count; i++)
					{
						IMMDevice *pDevice;
						hr = pDevices->Item(i, &pDevice);
						if (SUCCEEDED(hr))
						{
							LPWSTR wstrID = NULL;
							hr = pDevice->GetId(&wstrID);
							if (SUCCEEDED(hr))
							{
								IPropertyStore *pStore;
								hr = pDevice->OpenPropertyStore(STGM_READ, &pStore);
								if (SUCCEEDED(hr))
								{
									PROPVARIANT friendlyName;
									PropVariantInit(&friendlyName);
									hr = pStore->GetValue(PKEY_Device_FriendlyName, &friendlyName);
									if (SUCCEEDED(hr))
									{
										deviceNames[i] = friendlyName.pwszVal;
									}
									pStore->Release();
								}
							}
							pDevice->Release();
						}
					}
				}
				pDevices->Release();
			}
			pEnum->Release();
		}
	}
}