tradeStr = getItem("ORDERS", 2)  -- строка 2 из таблицы ORDERS (заявки)
order = tostring(tradeStr.order_num)
	

-- снятие заявки
 t = {
	 ACTION="KILL_ORDER",
	 ORDER_KEY = order,
	 CLASSCODE="TQBR",
	 trans_id = "1"  --можно ставить любым кроме 0 и менее
	 }
res=sendTransaction(t)
message(res,1)
