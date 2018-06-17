require("QluaCSharpConnector")

IsStop = false;         -- ���� ��������� �������
 
Stack = {};             -- ������ ��� �����
Stack.idx_for_add = 1;  -- ������ ��� ���������� ���������, ��� ������ ������
Stack.idx_for_get = 1;  -- ������ ��� ������� ���������, ��� ������ ������
Stack.count = 0;        -- ���������� ����������� � ����� �������
Stack.max = 1000;       -- ����������� ��������� ���������� ������� � ����� (��� ������������ ������ ������ ����� ���������� ������) 
 
CLASS_CODE   = "QJSIM";   -- ����� ������
SEC_CODE     = "SBER";	   -- ��� ������
 
function main()
   local FirstQuote = true;
   local Quote = "";
   -- �������� ����
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
         -- ����� ������ �� �����
         Quote = GetFromStack();
         -- ���� ���� �� ������
         if Quote ~= nil then 
            if FirstQuote then FirstQuote = false; end;
            -- ���������� ������ ������
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
   -- ��������� ������ � ����
   Stack[Stack.idx_for_add] = NewEntry;
   -- ������������ ������� ����������� � ����� �������
   if Stack.count < Stack.max then Stack.count = Stack.count + 1; end;
   -- ����������� ������ ��� ���������� ��������� ������
   Stack.idx_for_add = Stack.idx_for_add + 1;
   -- ���� ������ ������ ����������� �����������, �� ��������� ������ ����� ����������� � ������ �����
   if Stack.idx_for_add > Stack.max then Stack.idx_for_add = 1; end;
   -- ���� ������� ������� ������� �� ������ (����� ������ ���������� ������), �� ����������� ������ ��� ������� ��������� ������
   if Stack.idx_for_add - Stack.idx_for_get == 1 and Stack.count > 1 -- �������� ������ �����
      then Stack.idx_for_get = Stack.idx_for_get + 1;
   -- ������� � �����, ����� ������ ��� ������� ���� ��� � ����� � ���������� �� ����� 0
   elseif Stack.idx_for_get - Stack.idx_for_add == Stack.max - 1 and Stack.count > 1
      then Stack.idx_for_get = 1; 
   end;
end;

function GetFromStack()
   local OldInxForGet = Stack.idx_for_get;
   if Stack.count == 0 then return nil; end;
   -- ��������� ���������� ������� �� 1
   Stack.count = Stack.count - 1;
   -- ������������, ���� ��� ���� ������������ ������
   if Stack.count == -1 then
      Stack.count = 0;
      Stack.idx_for_get = Stack.idx_for_add; -- ����������� �������
   else -- ���� ��� ���� ������ 
      -- �������� ������ ������� �� 1 ������
      Stack.idx_for_get = Stack.idx_for_get + 1;
      -- ������������, ���� ��������� �����
      if Stack.idx_for_get > Stack.max then Stack.idx_for_get = 1; end;
   end;
   return Stack[OldInxForGet];
end;
--- ������� ���������� ���������� QUIK ��� ��������� ��������� ������� ���������
function OnQuote(class, sec )
   if class == CLASS_CODE and sec == SEC_CODE then
      -- �������� ������ �� ������� �����������
      ql2 = getQuoteLevel2(class, sec);
      -- ������������ ������ ������� � ���� ������
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
	  
      -- ��������� ������-������ � ����
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


























