@echo off
rem ===========================================================================
rem EAAddinSetup.bat �o�[�W����3.5
rem Copyright(C) 2010-2016 Sparx Systems Japan Co., Ltd. All rights Reserved.
rem ===========================================================================
rem �p�����[�^
rem �Ȃ� : �A�h�C���C���X�g�[��
rem /u   : �A�h�C���A���C���X�g�[��
rem ===========================================================================

setlocal

set result=0

rem ���� �p�����[�^���擾 ����
IF not "%1"=="/u" (
 set SetupKind=add
 set /a delCounter=-1
 set isExistCopyCommandForDel=0
 set Message=�o�^
)
IF "%1"=="/u" (
 set SetupKind=del
 set /a delCounter=1
 set isExistCopyCommandForDel=0
 set Message=�폜
)

rem ���� OS���� ����
for /f "tokens=3-4 delims=. " %%i in ('ver') do (
 IF  "%%i"=="XP" ( set os=xp ) ELSE ( set os=notxp )
)

rem ���� EA�C���X�g�[���p�X�擾 ����
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

rem ���� �A�N�Z�X�����m�F ����
copy nul "%EAInstallPath%\addin.tmp" > nul 2>&1
IF not %errorlevel%==0 (goto err2)
del /F "%EAInstallPath%\addin.tmp"

rem ���� ���݂̃p�X�擾 ����
cd /d %~dp0
set CurrentPath=%cd%

rem ���� ini�t�@�C���ǂݍ��� ����

rem �C���X�g�[�����F�擪�s������s
IF %SetupKind%==add (
 for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
) ELSE (
rem �A���C���X�g�[�����F2��ini�t�@�C����ǂݍ��݁A1��ڂ�reg�n�R�}���h�E2��ڂ�copy�n�R�}���h�Ńt�@�C���폜
 IF %delCounter%==1 (
  for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
  set /a delCounter=delCounter+1
  for /f "tokens=1,2,3,4,5,6,7*" %%a in (EAAddin.ini) do (call :START %%a %%b %%c %%d %%e %%f %%g)
))

rem  �I�����̃��b�Z�[�W�\�� 
IF %result%==0 (
 echo -----�A�h�C����%Message%���������܂����B-----) ELSE (
 echo -----�A�h�C����%Message%�Ɏ��s���܂����B-----)
pause
goto end_repeat

rem ���� ini�t�@�C���ݒ���擾 ����
:START
rem ������
set Header=""
set Command=""
set AddinName=""
set FileName=""
set DLLInfo=""
set RegValue=""
set NoCopy=""

rem ini�t�@�C�����Z�b�g
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

echo ���G���[�Fini�t�@�C����1���ځi�R�}���h�j���Ԉ���Ă��܂��Breg�܂���copy���w�肵�Ă��������B
goto end_repeat_fail

rem ���� �R�}���h���R�s�[(copy,xcopy)�̏ꍇ ����
:copyProcess
IF "%AddinName%"=="" (
echo ���G���[�Fini�t�@�C����copy�R�}���h��2���ځi�A�h�C�����j����͂��Ă��������B
goto end_repeat_fail)

IF "%FileName%"=="" (
echo ���G���[�Fini�t�@�C���w%AddinName%�x�Ɋւ���3���ځi�t�@�C�����j����͂��Ă��������B
goto end_repeat_fail)

IF %SetupKind%==del (goto delProcess)

rem �C���X�g�[���̏ꍇ�F�t���t�@�C��/�t�H���_�R�s�[
rem �t�@�C���R�s�[�̏ꍇ
IF %Command%==copy (
 echo:
 echo ---�w%AddinName%�x�̕t���t�@�C��%FileName%���R�s�[���܂��B
 %windir%\system32\xcopy.exe "%FileName%" "%EAInstallPath%" /y /q
 IF not %errorlevel%==0 (
  echo ���G���[�F%FileName%�t�@�C���𐳂����R�s�[�ł��Ȃ������\��������܂��B
  goto end_repeat_fail
))

rem �t�H���_�R�s�[�̏ꍇ
IF %Command%==xcopy (
 echo:
 echo ---�w%AddinName%�x�̕t���t�H���_%FileName%���R�s�[���܂��B
 cd /d "%EAInstallPath%"
 mkdir "%FileName%"
 cd "%FileName%"
 %windir%\system32\xcopy.exe "%CurrentPath%\%FileName%" . /y /q
 cd /d "%CurrentPath%"

 IF not %errorlevel%==0 (
  echo ���G���[�F%FileName%�t�H���_�𐳂����R�s�[�ł��Ȃ������\��������܂��B
  goto end_repeat_fail
))

rem ���΃p�X�̎w�肪����ꍇ
IF not "%DLLInfo%"=="" (
 cd /d "%EAInstallPath%"
 IF not exist "%DLLInfo%" (md "%DLLInfo%")
 MOVE /y "%FileName%" "%DLLInfo%"
 cd /d "%CurrentPath%"
)
IF not %errorlevel%==0 (
  echo ���G���[�F%FileName%�t�@�C���𐳂����ړ��ł��Ȃ������\��������܂��B
  goto end_repeat_fail
)
goto end_repeat

rem �A���C���X�g�[���̏ꍇ�F�t���t�@�C��/�t�H���_�폜
:delProcess
rem �t�@�C���폜�̏ꍇ
IF %Command%==copy (
 IF %delCounter%==2 (
  echo:
  echo ---�w%AddinName%�x�̕t���t�@�C��%FileName%���폜���܂��B
  cd /d "%EAInstallPath%"
  IF not "%DLLInfo%"=="" (cd "%DLLInfo%")
  del /F %FileName%
  cd /d "%CurrentPath%"
  goto end_repeat
))

rem �t�H���_�폜�̏ꍇ
IF %Command%==xcopy (
 IF %delCounter%==2 (
  echo:
  echo ---�w%AddinName%�x�̕t���t�H���_%FileName%���폜���܂��B
  cd /d "%EAInstallPath%"
  rmdir /q /s "%FileName%"
  cd /d "%CurrentPath%"
  goto end_repeat
))
goto end_repeat

rem ���� �R�}���h���o�^(reg)�̏ꍇ ����
:regProcess
rem �A���C���X�g�[������2��ڂ̌Ăяo�����͉������Ȃ�
IF %SetupKind%==del (
 IF %delCounter%==2 (goto end_repeat)
)
IF "%AddinName%"=="" (
 echo ���G���[�Fini�t�@�C����reg�R�}���h��2���ځi�A�h�C�����j�ȍ~����͂��Ă��������B
 goto end_repeat_fail
)
IF "%FileName%"=="" (
 echo ���G���[�Fini�t�@�C���w%AddinName%�x��3���ځiDLL���j�ȍ~����͂��Ă��������B
 goto end_repeat_fail
)
IF "%DLLInfo%"=="" (
 echo ���G���[�Fini�t�@�C���w%AddinName%�x��4���ځiDLL���j�ȍ~����͂��Ă��������B
 goto end_repeat_fail
)
IF "%RegValue%"=="" (
 echo ���G���[�Fini�t�@�C���w%AddinName%�x��5���ځi���W�X�g���l�j����͂��Ă��������B
 goto end_repeat_fail
)

rem �����J�n���̃��b�Z�[�W�\��
echo ------------------------------------------
echo �w%AddinName%�x��%Message%���܂��B
echo ------------------------------------------
rem ���� DLL��EA�C���X�g�[���t�H���_�ɃR�s�[ ����
IF %SetupKind%==add (
IF not "%NoCopy%"=="-" (
 %windir%\system32\xcopy.exe "%FileName%" "%EAInstallPath%" /y /q
 IF not %errorlevel%==0 (
  echo ���G���[�F%FileName%�t�@�C���𐳂����R�s�[�ł��Ȃ������\��������܂��B
  goto end_repeat_fail
)))

rem ���� DLL�o�^�E�o�^���� ����
IF "%DLLInfo%"==".Net" (goto DotNetReg)
IF "%DLLInfo%"==".Net2.0" (goto DotNetReg)
IF "%DLLInfo%"==".Net4.0" (goto DotNetReg)
IF "%DLLInfo%"=="not.Net" (goto notDotNetReg) ELSE (
 echo ���G���[�Fini�t�@�C���w%AddinName%�x��4���ځiDLL���j�̓��e���s���ł��B
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
 echo ���G���[�Fini�t�@�C���Ŏw�肳�ꂽ�o�[�W������.NetFramework���C���X�g�[������Ă��Ȃ��\��������܂��B
 goto end_repeat_fail
)
cd /d "%EAInstallPath%"
IF %SetupKind%==add (
 %DotNetReg%\regasm.exe "%FileName%") ELSE (
 %DotNetReg%\regasm.exe /unregister "%FileName%"
)

IF not %errorlevel%==0 (
 echo ���G���[�F%FileName%�t�@�C���ɑ΂��Đ������A�Z���u���o�^�c�[�������s�ł��Ȃ������\��������܂��B
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
 echo ���G���[�F%FileName%�t�@�C���ɑ΂��Đ������A�Z���u���o�^�c�[�������s�ł��Ȃ������\��������܂��B
 goto end_repeat_fail
)
cd /d "%CurrentPath%"
goto regImport

rem ���� ���W�X�g���o�^�E�폜 ����
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
    IF %SetupKind%==del (echo ���x���F ���łɃ��W�X�g���͍폜����Ă���\��������܂��B) ELSE (goto end_repeat_fail)
)))

IF not "%RegValue%"=="-" (
 IF %SetupKind%==add (
  %RegExeFolder%\reg.exe add HKCU%RegistoryPath% /ve /f /d %RegValue%) ELSE (
   IF %delCounter%==1 (%RegExeFolder%\reg.exe DELETE HKCU%RegistoryPath% /f)
   IF not %errorlevel%==0 (
    IF %SetupKind%==del (echo ���x���F ���łɃ��W�X�g���͍폜����Ă���\��������܂��B) ELSE (goto end_repeat_fail)
)))

rem ���� DLL�t�@�C���폜 ����
IF %SetupKind%==del (
 IF not "%NoCopy%"=="-" (
  cd /d "%EAInstallPath%"
  del %FileName% /f
  IF not %errorlevel%==0 (echo ���x���F ���łɃt�@�C���͍폜����Ă���\��������܂��B)
  cd /d "%CurrentPath%"
))

:end_repeat
exit /b 0

:end_repeat_fail
cd /d "%CurrentPath%"
set result=1
exit /b 0

:err1
echo ���G���[�F EA�C���X�g�[���p�X���s���ł��BEA����x�N�����A���Ă���ēx���s���Ă��������B
set result=1
pause
exit /b %result%

:err2
echo ���G���[�F �A�N�Z�X��������܂���B�u�Ǘ��҂Ƃ��Ď��s�v���Ă��������B
set result=1
pause
exit /b %result%
