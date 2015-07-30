@echo off
rem 
rem 全ツールファイルのリリースビルドをZIPに格納する
rem 

if exist work rmdir /S /Q work
mkdir work
if exist release rmdir /S /Q release
mkdir release
setlocal ENABLEDELAYEDEXPANSION
rem まずはファイルをコピーする
echo Copy EXE Files
for /D %%F IN (*) DO (
  set PA=%%F
  set PB=!PA:Test=!
  set FP=%~dp0!PB!\bin\Release\!PB!.exe
  if exist "!FP!" (
    copy "!FP!" "work\!PB!.exe" > NUL
  )
)

rem 次にzipを作成する
cd work
for %%E IN (*) DO (
  echo Creating %%~nE.zip ..
  zip ../release/%%~nE.zip %%E>NUL
)
echo Creating tools.zip ..
zip ../release/tools.zip *.exe>NUL
cd ..
echo Create Completed
rmdir /S /Q work