@echo off
rem ===========================================================================
rem EAAddinSetup.bat バージョン3.5
rem Copyright(C) 2010-2016 Sparx Systems Japan Co., Ltd. All rights Reserved.
rem ===========================================================================
rem パラメータ
rem なし : アドインインストール
rem /u   : アドインアンインストール
rem ===========================================================================

setlocal

set result=0

rem ■■ パラメータ情報取得 ■■
IF not "%1"=="/u" (
 set SetupKind=add
 set /a delCounter=-1
 set isExistCopyCommandForDel=0
 set Message=登録
)
IF "%1"=="/u" (
 set SetupKind=del
 set /a delCounter=1
 set isExistCopyCommandForDel=0
 set Message=削除
)

rem ■■ OS判定 ■■
for /f "tokens=3-4 delims=. " %%i in ('ver') do (
 IF  "%%i"=="XP" ( set os=xp ) ELSE ( set os=notxp )
)

rem ■■ EAインストールパス取得 ■■
set EAInstallPathRegistory="HKCU\Software\Sparx Systems\EA400\EA" /v "Install Path"
IF %os%==notxp (
 for /f "skip=2 tokens=4,5,6,7,8,9,10*" %%i in ('%windir%\system32\reg.exe query %EAInstallPathRegistory%') do set EAInstallPath=%%i %%j %%k %%l %%m %%n %%o) ELSE (
 for /f "skip=4 tokens=4,5,6,7,8,9,10*" %%i in ('%windir%\system32\reg.exe query %EAInstallPathRegistory%') do set EAInstallPath=%%i %%j %%k %%l %%m %%n %%o)
:LOOP
IF "%EAInstallPath%"=="" (goto err1)
IF not "%EAInstallPath:~-1%"==" " (GOTO NEXT) else set EAInstallPath=%EAInstallPath:~0,-1%
GOTO LOOP
:NEXT
IF not EXIST "%EAInstallPath%" (goto err1)
IF not EXIST "%EAInstallPath%\EA.exe" (goto err1)

rem ■■ アクセス権限確認 ■■
copy nul "%EAInstallPath%\addin.tmp" > nul 2>&1
IF not %errorlevel%==0 (goto err2)
del /F "%EAInstallPath%\addin.tmp"

rem ■■ 現在のパス取得 ■■
cd /d %~dp0
set CurrentPath=%cd%

rem ■■ iniファイル読み込み ■■

rem インストール時：先頭行から実行
IF %SetupKind%==add (
 for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
) ELSE (
rem アンインストール時：2回iniファイルを読み込み、1回目はreg系コマンド・2回目はcopy系コマンドでファイル削除
 IF %delCounter%==1 (
  for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
  set /a delCounter=delCounter+1
  for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
))

rem  終了時のメッセージ表示 
IF %result%==0 (
 echo -----アドインの%Message%が完了しました。-----) ELSE (
 echo -----アドインの%Message%に失敗しました。-----)
pause
goto end_repeat

rem ■■ iniファイル設定情報取得 ■■
:START
rem 初期化
set Header=""
set Command=""
set AddinName=""
set FileName=""
set DLLInfo=""
set RegValue=""
set NoCopy=""

rem iniファイル情報セット
set Header=%1
set Command=%2
set AddinName=%3
set FileName=%4
set DLLInfo=%5
set RegValue=%6
set NoCopy=%7

IF not "%Header%"=="-" (goto end_repeat)
IF %Command%==reg (goto regProcess)
IF %Command%==copy (goto copyProcess)
IF %Command%==xcopy (goto copyProcess)

echo ▲エラー：iniファイルの1項目（コマンド）が間違っています。regまたはcopyを指定してください。
goto end_repeat_fail

rem ■■ コマンドがコピー(copy,xcopy)の場合 ■■
:copyProcess
IF "%AddinName%"=="" (
echo ▲エラー：iniファイルのcopyコマンドの2項目（アドイン名）を入力してください。
goto end_repeat_fail)

IF "%FileName%"=="" (
echo ▲エラー：iniファイル『%AddinName%』に関する3項目（ファイル名）を入力してください。
goto end_repeat_fail)

IF %SetupKind%==del (goto delProcess)

rem インストールの場合：付属ファイル/フォルダコピー
rem ファイルコピーの場合
IF %Command%==copy (
 echo:
 echo ---『%AddinName%』の付属ファイル%FileName%をコピーします。
 %windir%\system32\xcopy.exe "%FileName%" "%EAInstallPath%" /y /q
 IF not %errorlevel%==0 (
  echo ▲エラー：%FileName%ファイルを正しくコピーできなかった可能性があります。
  goto end_repeat_fail
))

rem フォルダコピーの場合
IF %Command%==xcopy (
 echo:
 echo ---『%AddinName%』の付属フォルダ%FileName%をコピーします。
 cd /d "%EAInstallPath%"
 mkdir "%FileName%"
 cd "%FileName%"
 %windir%\system32\xcopy.exe "%CurrentPath%\%FileName%" . /y /q
 cd /d "%CurrentPath%"

 IF not %errorlevel%==0 (
  echo ▲エラー：%FileName%フォルダを正しくコピーできなかった可能性があります。
  goto end_repeat_fail
))

rem 相対パスの指定がある場合
IF not "%DLLInfo%"=="" (
 cd /d "%EAInstallPath%"
 IF not exist "%DLLInfo%" (md "%DLLInfo%")
 MOVE /y "%FileName%" "%DLLInfo%"
 cd /d "%CurrentPath%"
)
IF not %errorlevel%==0 (
  echo ▲エラー：%FileName%ファイルを正しく移動できなかった可能性があります。
  goto end_repeat_fail
)
goto end_repeat

rem アンインストールの場合：付属ファイル/フォルダ削除
:delProcess
rem ファイル削除の場合
IF %Command%==copy (
 IF %delCounter%==2 (
  echo:
  echo ---『%AddinName%』の付属ファイル%FileName%を削除します。
  cd /d "%EAInstallPath%"
  IF not "%DLLInfo%"=="" (cd "%DLLInfo%")
  del /F %FileName%
  cd /d "%CurrentPath%"
  goto end_repeat
))

rem フォルダ削除の場合
IF %Command%==xcopy (
 IF %delCounter%==2 (
  echo:
  echo ---『%AddinName%』の付属フォルダ%FileName%を削除します。
  cd /d "%EAInstallPath%"
  rmdir /q /s "%FileName%"
  cd /d "%CurrentPath%"
  goto end_repeat
))
goto end_repeat

rem ■■ コマンドが登録(reg)の場合 ■■
:regProcess
rem アンインストール時で2回目の呼び出し時は何もしない
IF %SetupKind%==del (
 IF %delCounter%==2 (goto end_repeat)
)
IF "%AddinName%"=="" (
 echo ▲エラー：iniファイルのregコマンドの2項目（アドイン名）以降を入力してください。
 goto end_repeat_fail
)
IF "%FileName%"=="" (
 echo ▲エラー：iniファイル『%AddinName%』の3項目（DLL名）以降を入力してください。
 goto end_repeat_fail
)
IF "%DLLInfo%"=="" (
 echo ▲エラー：iniファイル『%AddinName%』の4項目（DLL情報）以降を入力してください。
 goto end_repeat_fail
)
IF "%RegValue%"=="" (
 echo ▲エラー：iniファイル『%AddinName%』の5項目（レジストリ値）を入力してください。
 goto end_repeat_fail
)

rem 処理開始時のメッセージ表示
echo ------------------------------------------
echo 『%AddinName%』を%Message%します。
echo ------------------------------------------
rem ■■ DLLをEAインストールフォルダにコピー ■■
IF %SetupKind%==add (
IF not "%NoCopy%"=="-" (
 %windir%\system32\xcopy.exe "%FileName%" "%EAInstallPath%" /y /q
 IF not %errorlevel%==0 (
  echo ▲エラー：%FileName%ファイルを正しくコピーできなかった可能性があります。
  goto end_repeat_fail
)))

rem ■■ DLL登録・登録抹消 ■■
IF "%DLLInfo%"==".Net" (goto DotNetReg)
IF "%DLLInfo%"==".Net2.0" (goto DotNetReg)
IF "%DLLInfo%"==".Net4.0" (goto DotNetReg)
IF "%DLLInfo%"=="not.Net" (goto notDotNetReg) ELSE (
 echo ▲エラー：iniファイル『%AddinName%』の4項目（DLL情報）の内容が不正です。
 goto end_repeat_fail
)

:DotNetReg
IF "%DLLInfo%"==".Net4.0" (
 set DotNetReg="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319"
) ELSE (
 set DotNetReg="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727"
)

cd /d %DotNetReg%
IF not %errorlevel%==0 (
 echo ▲エラー：iniファイルで指定されたバージョンの.NetFrameworkがインストールされていない可能性があります。
 goto end_repeat_fail
)
cd /d "%EAInstallPath%"
IF %SetupKind%==add (
 %DotNetReg%\regasm.exe "%FileName%") ELSE (
 %DotNetReg%\regasm.exe /unregister "%FileName%"
)

IF not %errorlevel%==0 (
 echo ▲エラー：%FileName%ファイルに対して正しくアセンブリ登録ツールを実行できなかった可能性があります。
 goto end_repeat_fail
)
cd /d "%CurrentPath%"
goto regImport

cd /d "%CurrentPath%"
goto regImport

:notDotNetReg
cd /d "%EAInstallPath%"
IF %SetupKind%==add (
 %windir%\system32\regsvr32.exe /s "%FileName%") ELSE (
 %windir%\system32\regsvr32.exe /u /s "%FileName%")

IF not %errorlevel%==0 (
 echo ▲エラー：%FileName%ファイルに対して正しくアセンブリ登録ツールを実行できなかった可能性があります。
 goto end_repeat_fail
)
cd /d "%CurrentPath%"
goto regImport

rem ■■ レジストリ登録・削除 ■■
:regImport
set RegistoryPath="\Software\Sparx Systems\EAAddins"\%AddinName%

IF EXIST "%SystemRoot%\syswow64\reg.exe" (
 set RegExeFolder="%SystemRoot%\syswow64"
) ELSE (
 set RegExeFolder="%SystemRoot%\system32"
)

IF not "%RegValue%"=="-" (
 IF %SetupKind%==add (
  %RegExeFolder%\reg.exe add HKLM%RegistoryPath% /ve /f /d %RegValue%) ELSE (
   IF %delCounter%==1 (%RegExeFolder%\reg.exe DELETE HKLM%RegistoryPath% /f)
   IF not %errorlevel%==0 (
    IF %SetupKind%==del (echo ▲警告： すでにレジストリは削除されている可能性があります。) ELSE (goto end_repeat_fail)
)))

IF not "%RegValue%"=="-" (
 IF %SetupKind%==add (
  %RegExeFolder%\reg.exe add HKCU%RegistoryPath% /ve /f /d %RegValue%) ELSE (
   IF %delCounter%==1 (%RegExeFolder%\reg.exe DELETE HKCU%RegistoryPath% /f)
   IF not %errorlevel%==0 (
    IF %SetupKind%==del (echo ▲警告： すでにレジストリは削除されている可能性があります。) ELSE (goto end_repeat_fail)
)))

rem ■■ DLLファイル削除 ■■
IF %SetupKind%==del (
 IF not "%NoCopy%"=="-" (
  cd /d "%EAInstallPath%"
  del %FileName% /f
  IF not %errorlevel%==0 (echo ▲警告： すでにファイルは削除されている可能性があります。)
  cd /d "%CurrentPath%"
))

:end_repeat
exit /b 0

:end_repeat_fail
cd /d "%CurrentPath%"
set result=1
exit /b 0

:err1
echo ▲エラー： EAインストールパスが不明です。EAを一度起動し、閉じてから再度実行してください。
set result=1
pause
exit /b %result%

:err2
echo ▲エラー： アクセス権がありません。「管理者として実行」してください。
set result=1
pause
exit /b %result%
