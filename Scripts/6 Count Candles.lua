CLASS_CODE   = "QJSIM";   -- Класс бумаги
SEC_CODE     = "SBER";	   -- Код бумаги

ds,Error = CreateDataSource(CLASS_CODE, SEC_CODE, INTERVAL_M1)  
while (Error == "" or Error == nil) and ds:Size() == 0 do sleep(1) end  
if Error ~= "" and Error ~= nil then 
message("Connected Error : "..Error);
end  
message(tostring(ds:Size()));

