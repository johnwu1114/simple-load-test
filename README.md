# 簡易的壓力測試

用於測試 dotnet core 寫入資料到 mongodb，是否會造成效能瓶頸。

## 執行測試

執行專案目錄的 Script 即可開始測試：  

* `test-h2d.sh`  
  從本機啟動 dotnet core，將資料打進 mongodb 的 docker container。  
  執行完成可在專案目錄 `logs/` 資料夾查看測試結果。  
* `test-d2d.sh`  
  從 dotnet core 的 docker container，將資料打進 mongodb 的 docker container。  
  執行完成可在 conosle output 查看測試結果。  
