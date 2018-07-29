#include <windows.h>

#define LUA_LIB
#define LUA_BUILD_AS_DLL

extern "C" {
#include "Lua\lauxlib.h"
#include "Lua\lua.h"
}

 
// ��� ��� ���������� ������ ��� �������
TCHAR Name[] = TEXT("QUIKCommand");
// �������, ��� ����������� � ��� ��������� ������ � ����� ������
HANDLE hFileMapQUIKCommand = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 256, Name);


// ��� ��� ���������� ������ ��� �������
TCHAR NameTerminal[] = TEXT("TerminalQuote");
HANDLE hFileMapTerminalQuote = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameTerminal);


// ��� ��� ���������� ������ �������
TCHAR NameOrders[] = TEXT("Orders");
HANDLE hFileMapOrders = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameOrders);

// ��� ��� ������
TCHAR NameCandles[] = TEXT("Candles");
HANDLE hFileMapCandles = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameCandles);


//=== ����������� ����� ����� ��� DLL ==========================================================================//
BOOL APIENTRY DllMain(HANDLE hModule, DWORD  fdwReason, LPVOID lpReserved)
{
	return TRUE;
}

//=== ���������� �������, ���������� �� LUA ====================================================================//

//���������� � Lua-���� ���������� �������
static int forLua_GetCommand(lua_State *L)// ���������� �������� �����
{
	//���� ��������� �� ������ �������
	if (hFileMapQUIKCommand)
	{
		//�������� ������ � ������ ������
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapQUIKCommand, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 256));
		//���� ������ � ������ ������ �������
		if (pb != NULL)
		{     
			//���� ������ �����
			if (pb[0] == 0)
			{
				//���������� � Lua-���� ������ ������
				lua_pushstring(L, "");
			}
			else //���� � ������ ���� �������
			{
				//���������� � Lua-���� ���������� �������
				lua_pushstring(L, (char*)(pb));
				//������� ������, ����� �������� �� ��������� �������
				for (int i = 0; i < 256; i++)pb[i] = '\0';
			}
			//��������� �������������
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");//���� ������ � ������ ������ �� ��� �������, ���������� � Lua-���� ������ ������
	}
	else //��������� �� ������ �� ��� �������
	{
		//���������� � Lua-���� ������ ������
		lua_pushstring(L, "");
	}

	return(1);
}

//��������� �������-�� ����� ��������� ������
static int forLua_CheckGotQuote(lua_State *L)
{
	//���� ��������� �� ������ �������
	if (hFileMapTerminalQuote)
	{
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapTerminalQuote, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));
		if (pb != NULL)
		{
			if (pb[0] == 0)
			{
				lua_pushboolean(L, true);
			}
			else
			{
				lua_pushboolean(L, false);
			}
			UnmapViewOfFile(pb);
		}

		else lua_pushboolean(L, false);
	}
	else
	{
		lua_pushboolean(L, false);
	}
	return(1);
}

//���������� ����� ��������� �������
static int forLua_SendQuote(lua_State *L)
{
	//���� ��������� �� ������ �������
	if (hFileMapTerminalQuote)
	{
		//�������� ������ � ������ ������
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapTerminalQuote, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));

		//���� ������ � ������ ������ �������
		if (pb != NULL)
		{
			//�������� �� Lua-����� ���������� ��������
			const char *Quote = lua_tostring(L, 1);
			int Size = 0;
			//������� ���������� �������� � ������
			for (int i = 0; i < 1400; i++)
			{
				if (Quote[i] == 0)break;
				Size++;
			}
			//���������� ������ � ������
			memcpy(pb, Quote, Size);
			//lua_pushstring(L, (char*)pb);//���������� ��, ��� ���������� (���� �����������������) (����� ����������� ��� �������)
			//��������� �������������
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");
	}
	else lua_pushstring(L, "");

	return(1);
}

// ��������� �������-�� ����� ������� �������
static int forLua_CheckOrders(lua_State *L) 
{
	if (hFileMapOrders) 
	{
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapOrders, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));
		if (pb!=NULL)
		{
			if (pb[0] == 0)
			{
				lua_pushboolean(L, true);
			}
			else
			{
				lua_pushboolean(L, false);
			}
			UnmapViewOfFile(pb);
		}
		else lua_pushboolean(L, false);
	}
	else lua_pushboolean(L, false);
	return(1);
}

//���������� ������� ������ �� ���������
static int forLua_SendOrders(lua_State *L) 
{
	if (hFileMapOrders) 
	{
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapOrders, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));
		if (pb!=NULL)
		{
			const char *Order = lua_tostring(L, 1);
			int Size = 0;
			for (int i=0; i < 1400; i++) 
			{
				if (Order[i] == 0)break;
				Size++;
			}
			memcpy(pb, Order, Size);
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");
	}
	else lua_pushstring(L, "");

	return(1);
}

//���������� ������ ������
static int forLua_SendCandles(lua_State *L) 
{
	if (hFileMapCandles)
	{
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapCandles, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));
		if (pb != NULL)
		{
			const char *Candles = lua_tostring(L, 1);
			int Size = 0;
			for (int i = 0; i < 1400; i++)
			{
				if (Candles[i] == 0)break;
				Size++;
			}
			memcpy(pb, Candles, Size);
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");
	}
	else lua_pushstring(L, "");

	return(1);
}

//=== ����������� ������������� � dll �������, ����� ��� ����� "������" ��� Lua ================================//
static struct luaL_reg ls_lib[] = {
	{ "GetCommand", forLua_GetCommand }, // �� ������� Lua ��� ������� ����� ����� �������� ���: QluaCSharpConnector.TestFunc()
	{ "CheckGotQuote", forLua_CheckGotQuote},
	{ "SendQuote", forLua_SendQuote},
	{ "CheckOrders", forLua_CheckOrders },
	{ "SendOrders", forLua_SendOrders },
	{ "SendCandles", forLua_SendCandles },
	{ NULL, NULL }
};

//=== ����������� �������� ����������, �������� � ������� Lua ==================================================//
extern "C" LUALIB_API int luaopen_QluaCSharpConnector(lua_State *L) {
	luaL_openlib(L, "QluaCSharpConnector", ls_lib, 0);
	return 0;
}