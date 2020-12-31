set path=%~d0
cd %path%
cd /d %~dp0

RegAsm.exe DuSwToglTF.dll /codebase
pause
