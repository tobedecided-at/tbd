@echo off
cls

set dataPath=%~dp0_data
set libPath=%dataPath%\lib

set cscPath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -lib:%libPath% /nologo /d:DEBUG

rem Make dll
%cscPath% -target:library -r:Newtonsoft.Json.dll -r:websocket-sharp.dll -out:tbd.dll %libPath%\src\*.cs %libPath%\src\WSBehaviorsServer\*.cs %libPath%\src\WSBehaviorsClient\*.cs