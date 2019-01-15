@echo off
cls

set server=main.exe
set dataPath=%~dp0_data
set libPath=%dataPath%\shared_lib
set cscPath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -lib:%libPath% /nologo /d:DEBUG

%cscPath% -r:Newtonsoft.Json.dll -r:websocket-sharp.dll %libPath%\src\*.cs %libPath%\src\WSBehaviorsServer\*.cs %libPath%\src\WSBehaviorsClient\*.cs

rem Run TBDMS.exe
%dataPath%\..\%server%