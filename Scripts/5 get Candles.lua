function main() 
while (true) do  
ds,Error = CreateDataSource("QJSIM", "SBER", INTERVAL_M5)  
while (Error == "" or Error == nil) and ds:Size() == 0 do sleep(1) end  
if Error ~= "" and Error ~= nil then message("Connected Error : "..Error) end  
if ds ~= nil then  
local massive = "["
for i=ds:Size()-5, ds:Size(), 1 do  
local item = tostring("{".."\"open\":"..ds:O(i)..",".."\"close\":"..ds:C(i)..",".."\"high\":"..ds:H(i)..",".."\"low\":"..ds:L(i)..",".."\"volume\":"..ds:V(i)..",".."\"index\":"..(i).."}"..",") 
massive = massive..item;
end  
massive = massive.."]";  
message(massive);  
end;  
sleep(1000);  
end  
end 

