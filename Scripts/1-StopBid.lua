tradeStr = getItem("ORDERS", 2)  -- ������ 2 �� ������� ORDERS (������)
order = tostring(tradeStr.order_num)
	

-- ������ ������
 t = {
	 ACTION="KILL_ORDER",
	 ORDER_KEY = order,
	 CLASSCODE="TQBR",
	 trans_id = "1"  --����� ������� ����� ����� 0 � �����
	 }
res=sendTransaction(t)
message(res,1)
