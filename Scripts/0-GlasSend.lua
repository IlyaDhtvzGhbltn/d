require("QluaCSharpConnector")

IsStop = false;         -- Флаг остановки скрипта
 
Stack = {};             -- Массив для стека
Stack.idx_for_add = 1;  -- Индекс для добавления следующей, или первой записи
Stack.idx_for_get = 1;  -- Индекс для изъятия следующей, или первой записи
Stack.count = 0;        -- Количество находящихся в стеке записей
Stack.max = 1000;       -- Максимально возможное количество записей в стеке (при переполнении старые записи будут замещаться новыми) 
 
CLASS_CODE   = "QJSIM";   -- Класс бумаги
SEC_CODE     = "SBER";	   -- Код бумаги
 
function main()
   local FirstQuote = true;
   local Quote = "";
   -- ОСНОВНОЙ ЦИКЛ
   while not IsStop do	
 		local CommandStr = tostring(QluaCSharpConnector.GetCommand());
			if CommandStr ~= "" then
				Header = tostring(string.sub(CommandStr,1,1));
				
				if Header == 'w' then
				sleep(1);
				
				elseif Header == 'k' then
					KillTrans(CommandStr)
				else
					local Tr = CommandStr
					result = sendTransaction(Tr);
				end
			end;
		
      if QluaCSharpConnector.CheckGotQuote() or FirstQuote then
         -- берет стакан из стека
         Quote = GetFromStack();
         -- если стек не пустой
         if Quote ~= nil then 
            if FirstQuote then FirstQuote = false; end;
            -- отправляет стакан роботу
            QluaCSharpConnector.SendQuote(Quote);
         end;
      end
	  if QluaCSharpConnector.CheckOrders() or FirstQuote then
		Orders = GetOrders();
			if Orders~=nil then
			QluaCSharpConnector.SendOrders(Orders);
			end;
	  end;
 
      sleep(1);
   end;
end
 
function AddToStack(NewEntry)
   -- Добавляет запись в стек
   Stack[Stack.idx_for_add] = NewEntry;
   -- Корректирует счетчик находящихся в стеке записей
   if Stack.count < Stack.max then Stack.count = Stack.count + 1; end;
   -- Увеличивает индекс для добавления следующей записи
   Stack.idx_for_add = Stack.idx_for_add + 1;
   -- Если индекс больше максимально допустимого, то следующая запись будет добавляться в начало стека
   if Stack.idx_for_add > Stack.max then Stack.idx_for_add = 1; end;
   -- Если изъятие записей отстало от записи (новая запись переписала старую), то увеличивает индекс для изъятия следующей записи
   if Stack.idx_for_add - Stack.idx_for_get == 1 and Stack.count > 1 -- смещение внутри стека
      then Stack.idx_for_get = Stack.idx_for_get + 1;
   -- Добавил в конец, когда индекс для изъятия тоже был в конце и количество не равно 0
   elseif Stack.idx_for_get - Stack.idx_for_add == Stack.max - 1 and Stack.count > 1
      then Stack.idx_for_get = 1; 
   end;
end;

function GetFromStack()
   local OldInxForGet = Stack.idx_for_get;
   if Stack.count == 0 then return nil; end;
   -- Уменьшает количество записей на 1
   Stack.count = Stack.count - 1;
   -- Корректирует, если это была единственная запись
   if Stack.count == -1 then
      Stack.count = 0;
      Stack.idx_for_get = Stack.idx_for_add; -- Выравнивает индексы
   else -- Если еще есть записи 
      -- Сдвигает индекс изъятия на 1 вправо
      Stack.idx_for_get = Stack.idx_for_get + 1;
      -- Корректирует, если достигнут конец
      if Stack.idx_for_get > Stack.max then Stack.idx_for_get = 1; end;
   end;
   return Stack[OldInxForGet];
end;
--- Функция вызывается терминалом QUIK при получении изменения стакана котировок
function OnQuote(class, sec )
   if class == CLASS_CODE and sec == SEC_CODE then
      -- Получает стакан по нужному инструменту
      ql2 = getQuoteLevel2(class, sec);
      -- Представляет снимок СТАКАНА в виде СТРОКИ
      QuoteStr = "";
	  
      for i = tonumber(ql2.bid_count), 1, -1 do
         if ql2.bid[i].quantity ~= nil then
            QuoteStr = QuoteStr..tostring(tonumber(ql2.bid[i].quantity))..";"..tostring(tonumber(ql2.bid[i].price))..";";
         else
            QuoteStr = QuoteStr.."0;"..tostring(tonumber(ql2.bid[i].price))..";";
         end;
      end;
	  
      for i = 1, tonumber(ql2.offer_count), 1 do
         if ql2.offer[i].quantity ~= nil then
            if i < tonumber(ql2.offer_count) then 
               QuoteStr = QuoteStr..tostring(tonumber(ql2.offer[i].quantity))..";"..tostring(tonumber(ql2.offer[i].price))..";";
            else 
               QuoteStr = QuoteStr..tostring(tonumber(ql2.offer[i].quantity))..";"..tostring(tonumber(ql2.offer[i].price));
            end;
         else 
            if i < tonumber(ql2.offer_count) then
               QuoteStr = QuoteStr.."0;"..tostring(tonumber(ql2.offer[i].price))..";";
            else
               QuoteStr = QuoteStr.."0;"..tostring(tonumber(ql2.offer[i].price));
            end;
         end;
      end;
	  
      -- Добавляет СТАКАН-строку в стек
      AddToStack(QuoteStr);
	  
   end;
end;
 
function OnStop(s)
   IsStop = true;
end


function SendTransaction(transStr)
message('send trans');
t = transStr;
res=sendTransaction(t)
message(res,1);
end

function KillTrans(trans_id)

local killingId = string.sub(trans_id, 3);
	count = tonumber(getNumberOf("orders"));
	if  count > 0 then
	
		for i=0, count - 1, 1 do
			OrderRow = getItem("orders", i)
			trId =  tostring(OrderRow.trans_id);
				if trId == killingId then
					 t = {
					 ACTION="KILL_ORDER",
					 ORDER_KEY = tostring(OrderRow.order_num),
					 CLASSCODE="QJSIM",
					 trans_id = tostring(killingId)
					 }
					res=sendTransaction(t)
					message(res,1);
					message('kill trans');
				end;
		end;
	end;
end;

function GetOrders()
local OrdersTable = "";
AllRows = tonumber(getNumberOf("orders"));
	if AllRows > 0 then
		for i=AllRows-1, AllRows-1, 1 do
			BidRow = getItem("orders", i)
			OrdersTable = OrdersTable..tostring("-;"..BidRow.trans_id)..";"..BidRow.class_code..";"..BidRow.sec_code..";"..tostring(tonumber(BidRow.flags))..";"
		end;
	return OrdersTable;
	end;
end;


























