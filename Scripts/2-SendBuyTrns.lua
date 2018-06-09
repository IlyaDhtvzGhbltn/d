--покупка
t = {

    ["CLASSCODE"]="TQBR",
    ["SECCODE"]="SBER",
    ["ACTION"]="NEW_ORDER",
    ["ACCOUNT"]="L01-00000F00",
    ["OPERATION"]="B",
    ["QUANTITY"]="1",
    ["PRICE"]="220",
    ["TRANS_ID"]="165"		-- можно ставить любой

    }
	
m = {
["ACCOUNT"]="L01-00000F00",
["ACTION"] = "NEW_ORDER", 
["CLASSCODE"] = "TQBR", 
["SECCODE"] = "SBER", 
["OPERATION"] = "S", 
["TYPE"] = "M", 
["PRICE"] = "0", 
["QUANTITY"]="29",  --нужно
["TRANS_ID"] = "165"
}	

res=sendTransaction(m)
message(res,1)
