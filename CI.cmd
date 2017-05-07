@echo Off
cls
set target=%1
if "%target%"=="" (
	set target=CI
)
set config=%2
if "%config%" == "" (
   set config=Release
)
msbuild Solution/.build\Build.proj /t:%target% /p:Configuration="%config%" /fl /flp:LogFile=msbuild.log;Verbosity=normal /nr:false