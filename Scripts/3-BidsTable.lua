--получить таблицу заявок
-- если у заявки нет id то trans_id = 0


OrdersTable = "";
AllRows = tonumber(getNumberOf("orders"));
if AllRows > 0 then
	for i=AllRows-1, AllRows-1, 1 do
	BidRow = getItem("orders", i)
	OrdersTable = OrdersTable..tostring("-"..BidRow.trans_id)..";"..BidRow.class_code..";"..BidRow.sec_code..";"..tostring(tonumber(BidRow.flags))..";"
	end;
end;
message(OrdersTable);
--trans_id
--class_code
--sec_code
--flags