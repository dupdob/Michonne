@echo Off
cls
set config=%1
if "%config%" == "" (
   set config=Release
)
msbuild Solution/.build\Build.proj /p:Configuration="%config%" /t:RunAll /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Diagnostic /nr:false