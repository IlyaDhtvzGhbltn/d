#include <windows.h>

#define LUA_LIB
#define LUA_BUILD_AS_DLL

extern "C" {
#include "Lua\lauxlib.h"
#include "Lua\lua.h"
}

 
// Имя для выделенной памяти для команды
TCHAR Name[] = TEXT("QUIKCommand");
// Создаст, или подключится к уже созданной памяти с таким именем
HANDLE hFileMapQUIKCommand = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 256, Name);


// Имя для выделенной памяти для стакана
TCHAR NameTerminal[] = TEXT("TerminalQuote");
HANDLE hFileMapTerminalQuote = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameTerminal);


// Имя для выделенной памяти ордеров
TCHAR NameOrders[] = TEXT("Orders");
HANDLE hFileMapOrders = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameOrders);

// Имя для свечей
TCHAR NameCandles[] = TEXT("Candles");
HANDLE hFileMapCandles = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 1400, NameCandles);


//=== Стандартная точка входа для DLL ==========================================================================//
BOOL APIENTRY DllMain(HANDLE hModule, DWORD  fdwReason, LPVOID lpReserved)
{
	return TRUE;
}

//=== Реализация функций, вызываемых из LUA ====================================================================//

//Записывает в Lua-стек полученную команду
static int forLua_GetCommand(lua_State *L)// Возвращает заданный текст
{
	//Если указатель на память получен
	if (hFileMapQUIKCommand)
	{
		//Получает доступ к байтам памяти
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapQUIKCommand, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 256));
		//Если доступ к байтам памяти получен
		if (pb != NULL)
		{     
			//Если память чиста
			if (pb[0] == 0)
			{
				//Записывает в Lua-стек пустую строку
				lua_pushstring(L, "");
			}
			else //Если в памяти есть команда
			{
				//Записывает в Lua-стек полученную команду
				lua_pushstring(L, (char*)(pb));
				//Стирает запись, чтобы повторно не выполнить команду
				for (int i = 0; i < 256; i++)pb[i] = '\0';
			}
			//Закрывает представление
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");//Если доступ к байтам памяти не был получен, записывает в Lua-стек пустую строку
	}
	else //Указатель на память не был получен
	{
		//Записывает в Lua-стек пустую строку
		lua_pushstring(L, "");
	}

	return(1);
}

//Проверяет получил-ли робот последний СТАКАН
static int forLua_CheckGotQuote(lua_State *L)
{
	//Если указатель на память получен
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

//Отправляет новые изменения стакана
static int forLua_SendQuote(lua_State *L)
{
	//Если указатель на память получен
	if (hFileMapTerminalQuote)
	{
		//Получает доступ к байтам памяти
		PBYTE pb = (PBYTE)(MapViewOfFile(hFileMapTerminalQuote, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, 1400));

		//Если доступ к байтам памяти получен
		if (pb != NULL)
		{
			//Получает из Lua-стека переданное значение
			const char *Quote = lua_tostring(L, 1);
			int Size = 0;
			//считает количество символов в строке
			for (int i = 0; i < 1400; i++)
			{
				if (Quote[i] == 0)break;
				Size++;
			}
			//записывает стакан в память
			memcpy(pb, Quote, Size);
			//lua_pushstring(L, (char*)pb);//возвращает то, что записалось (если раскомментировать) (может пригодиться при отладке)
			//закрывает представление
			UnmapViewOfFile(pb);
		}
		else lua_pushstring(L, "");
	}
	else lua_pushstring(L, "");

	return(1);
}

// Проверяет получил-ли робот таблицу ОРДЕРОВ
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

//Отправляет таблицу заявок из терминала
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

//отправляет график СВЕЧЕЙ
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

//=== Регистрация реализованных в dll функций, чтобы они стали "видимы" для Lua ================================//
static struct luaL_reg ls_lib[] = {
	{ "GetCommand", forLua_GetCommand }, // из скрипта Lua эту функцию можно будет вызывать так: QluaCSharpConnector.TestFunc()
	{ "CheckGotQuote", forLua_CheckGotQuote},
	{ "SendQuote", forLua_SendQuote},
	{ "CheckOrders", forLua_CheckOrders },
	{ "SendOrders", forLua_SendOrders },
	{ "SendCandles", forLua_SendCandles },
	{ NULL, NULL }
};

//=== Регистрация названия библиотеки, видимого в скрипте Lua ==================================================//
extern "C" LUALIB_API int luaopen_QluaCSharpConnector(lua_State *L) {
	luaL_openlib(L, "QluaCSharpConnector", ls_lib, 0);
	return 0;
}