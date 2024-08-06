#include "pch.h"
#include <windows.h>
#include <mutex>
#include <combaseapi.h>
#include <Shlwapi.h>
#include <Psapi.h>

//INSTALL DETOURS FROM NUGET! (or build from source yourself)
#include <detours/detours.h>
#include <iostream>

#include "undoc.h"

typedef HRESULT(CALLBACK* SetShellWindow)(HWND hwnd);

long PcRef = 0;
IUnknown* ThreadObject = NULL;

LRESULT ProgmanWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		auto user32 = LoadLibrary(TEXT("user32.dll"));
		SetShellWindow SetShellWindowFunc = (SetShellWindow)GetProcAddress(user32, "SetShellWindow");
		if (SetShellWindowFunc == NULL)
		{
			printf("immersive shell starter: failed to get pointer to SetShellWindow()\n");
			system("pause");
			return GetLastError();
		}

		if (FAILED(SetShellWindowFunc(hwnd)))
		{
			printf("SetShellWindow failed\n");
			return 0;
		}
		SendMessageW(hwnd, WM_CHANGEUISTATE, 3u, 0);
		SendMessageW(hwnd, WM_UPDATEUISTATE, 0x10001u, 0);

		SetProp(hwnd, TEXT("NonRudeHWND"), (HANDLE)HANDLE_FLAG_INHERIT);
		SetProp(hwnd, TEXT("AllowConsentToStealFocus"), (HANDLE)HANDLE_FLAG_INHERIT);


		if (SUCCEEDED(SHCreateThreadRef(&PcRef, &ThreadObject)))
		{
			SHSetThreadRef(ThreadObject);
		}
		else
		{
			printf("Failed to create thread reference");
		}
	}
	else if (msg == WM_DESTROY)
	{
		if (GetShellWindow() == hwnd)
		{
			RemovePropW(hwnd, L"AllowConsentToStealFocus");
			RemovePropW(hwnd, L"NonRudeHWND");

			auto user32 = LoadLibrary(TEXT("user32.dll"));
			SetShellWindow SetShellWindowFunc = (SetShellWindow)GetProcAddress(user32, "SetShellWindow");
			if (SetShellWindowFunc == NULL)
			{
				printf("immersive shell starter: failed to get pointer to SetShellWindow()\n");
				system("pause");
				return GetLastError();
			}
			SetShellWindowFunc(NULL);
		}
	}
	else if (msg == WM_SIZE)
	{
		ShowWindow(hwnd, 5);
	}
	else if (msg == WM_CLOSE)
	{
		return -1;
	}
	else if (msg == WM_PAINT)
	{
		PAINTSTRUCT Paint;
		RECT Rect;
		HDC dc = BeginPaint(hwnd, &Paint);
		GetClientRect(hwnd, &Rect);
		FillRect(dc, &Rect, CreateSolidBrush(RGB(0, 36, 0)));
		EndPaint(hwnd, &Paint);
		return 0;
	}
	else if (msg == WM_TIMER)
	{
		printf("timer!\n");
	}
	return DefWindowProc(hwnd, msg, w, l);
}



HINSTANCE g_hInstance = NULL;

HWND CreateProgman()
{
	printf("Create program manager\n");

	WNDCLASSEX progmanclass = {};
	progmanclass.cbClsExtra = 0;
	progmanclass.hIcon = 0;
	progmanclass.lpszMenuName = 0;
	progmanclass.hIconSm = 0;
	progmanclass.cbSize = 80;
	progmanclass.style = 8;
	progmanclass.lpfnWndProc = (WNDPROC)ProgmanWndProc;
	progmanclass.cbWndExtra = 8;
	progmanclass.hInstance = g_hInstance;
	progmanclass.hCursor = LoadCursor(NULL, IDC_ARROW);
	progmanclass.hbrBackground = (HBRUSH)2;
	progmanclass.lpszClassName = TEXT("Progman");

	if (!RegisterClassExW(&progmanclass))
	{
		printf("failed to register progman class %d", GetLastError());
		return NULL;
	}

	return CreateWindowExW(128, L"Progman", TEXT("Program Manager"), 0x82000000, 0, 0, 0, 0, 0, 0, progmanclass.hInstance, 0);
}

int MainHook(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR     lpCmdLine,
	int       nShowCmd
)
{
	printf("Winmain hooked!\n");
	g_hInstance = hInstance;

	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return -1;
	}
	printf("Starting the immersive shell provider/controller\n");




	// find progman window and set it as the shell window
	HWND progman = CreateProgman();
	if (!progman)
	{
		printf("failed to create progman window\n");
		system("pause");
		return GetLastError();
	}

	IImmersiveShellBuilder* ImmersiveShellBuilder = NULL;

	GUID guid;
	if (FAILED(CLSIDFromString(L"{c71c41f1-ddad-42dc-a8fc-f5bfc61df957}", &guid)))
	{
		printf("failed to read guid1\n");
		return -1;
	}
	GUID guid2;
	if (FAILED(CLSIDFromString(L"{1c56b3e4-e6ea-4ced-8a74-73b72c6bd435}", &guid2)))
	{
		printf("failed to read guid2\n");
		return -1;
	}
	HRESULT hr = CoCreateInstance(
		guid,
		0,
		1u,
		guid2,
		(LPVOID*)&ImmersiveShellBuilder);

	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell builder: 0x%x\n", hr);
		system("pause");
		return hr;
	}
	IImmersiveShellController* controller = NULL;
	hr = ImmersiveShellBuilder->CreateImmersiveShellController(&controller); //CImmersiveShellController
	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell controller: 0x%x\n", hr);
		system("pause");
		return hr;
	}



	hr = controller->Start();


	if (FAILED(hr))
	{
		printf("immersive shell start failed with 0x%x\n", hr);
	}
	else
	{
		printf("Immersive shell created. Starting message loop\n");

		if (!CreateEventW(0, TRUE, 0, L"Local\\ShellStartupEvent"))
		{
			printf("Failed to create start event: %d\n", GetLastError());

		}

		HANDLE StartEvent = OpenEvent(2, FALSE, TEXT("Local\\ShellStartupEvent"));
		if (!StartEvent)
		{
			printf("Failed to open start event: %d\n", GetLastError());
		}
		else
		{
			if (!SetEvent(StartEvent))
			{
				printf("Failed to set start event: %d\n", GetLastError());
			}
		}
	}

	return -1;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		DisableThreadLibraryCalls(hModule);
		AllocConsole();

		freopen("CONIN$", "r", stdin);
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);

		printf("Hooked into runtimebroker");

		//LaunchCustomShellHost
		MODULEINFO info = {};
		if (!GetModuleInformation(GetCurrentProcess(), NULL, &info, sizeof(MODULEINFO)))
		{
			printf("GetModuleInformation() failure: %d", GetLastError());
		}
		else
		{
			printf("entry point of runtimebroker is at %x, hooking it now\n", info.EntryPoint);

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&info.EntryPoint, (PVOID)MainHook);
			DetourTransactionCommit();
		}
	}
	break;
	case DLL_PROCESS_DETACH:
	{
		//TODO
	}
	break;
	}
	return TRUE;
}

void HamCloseActivity()
{
	printf("STUB FUNCTION: HamCloseActivity\n");
}

void HamPopulateActivityProperties()
{
	printf("STUB FUNCTION: HamPopulateActivityProperties\n");
}