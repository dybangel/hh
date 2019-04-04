@echo off
start iexplore "http://localhost:4055/Task/Task.aspx?action=auto&tokey=9E797DF579468058"
ping -n 5 127.1>nul
taskkill /f /im IEXPLORE.exe