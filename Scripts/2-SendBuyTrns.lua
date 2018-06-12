--покупка
t = {

    ["CLASSCODE"]="QJSIM", --TQBR
    ["SECCODE"]="SBER",
    ["ACTION"]="NEW_ORDER",
    ["ACCOUNT"]="NL0011100043",  --SPBFUT00433
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

res=sendTransaction(t)
message(res,1)
