@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

set "songDirectory=wwwroot\data\songs"
set "outputFile=wwwroot\data\songlist.txt"

REM Remove existing songlist.txt file
if exist "%outputFile%" del "%outputFile%"

REM Iterate through files in song directory and write their names to songlist.txt
for %%f in ("%songDirectory%\*.*") do (
    set "fileName=%%~nxf"
    echo !fileName! >> "%outputFile%"
)

echo Song list generated successfully.