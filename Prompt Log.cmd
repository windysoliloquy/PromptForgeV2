@echo off
setlocal
set "PROMPTFORGE_ENABLE_UI_EVENT_LOG=1"
set "PROMPTFORGE_UI_EVENT_LOG_SCOPE=hoverdeck"
start "" "%~dp0AppOutput\PromptForge\PromptForge.App.exe"
