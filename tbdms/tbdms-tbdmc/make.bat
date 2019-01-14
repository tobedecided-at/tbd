@echo off
cls

set server=tbdms.exe
set client=tbdmc.exe

set cscPath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -lib:%libPath% /nologo /d:DEBUG

rem Killall
taskkill /IM tbdmc.exe /T /F > nul

rem Make and copy dll
cd ..\tbd-dll\
call make
cd ..\tbdms-tbdmc\

set dataPath=%~dp0_data
set libPath=%dataPath%\shared_lib
set cscPath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -lib:%libPath% /nologo /d:DEBUG

copy ..\tbd-dll\tbd.dll %libPath%\

rem Make
%cscPath% -appconfig:%client%.config -target:exe -r:Newtonsoft.Json.dll -r:websocket-sharp.dll -r:tbd.dll -out:%dataPath%\..\%client% %dataPath%\tbdmc\main.cs
%cscPath% -appconfig:%server%.config -target:exe -r:Newtonsoft.Json.dll -r:websocket-sharp.dll -r:tbd.dll -out:%dataPath%\..\%server% %dataPath%\tbdms\main.cs

rem Run TBDMS.exe
%dataPath%\..\%server%