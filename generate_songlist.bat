@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

set "songDirectory=wwwroot\data\songs"
set "outputFile=wwwroot\data\songlinks.txt"
set "songNamesFile=wwwroot\data\songnames.txt"

REM Remove existing files
if exist "%outputFile%" del "%outputFile%"
if exist "%songNamesFile%" del "%songNamesFile%"

REM Iterate through files in song directory, write their names to songlinks.txt
REM and first lines to songnames.txt
for %%f in ("%songDirectory%\*.*") do (
    set "fileName=%%~nxf"
    echo !fileName! >> "%outputFile%"
    set /p firstLine=<"%%f"
    echo !firstLine! >> "%songNamesFile%"
)

echo Song links and names generated successfully.