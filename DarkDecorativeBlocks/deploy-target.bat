@echo off

SET TARGET_PATH=.\_target
SET DEPLOY_PATH=C:\Users\catal\AppData\Roaming\SpaceEngineers\Mods\Desolate Timelines Decorative Block Set

echo Removing old mod files
del "%DEPLOY_PATH%\*.*" /s /q
FOR /D %%p IN ("%DEPLOY_PATH%\*.*") DO rmdir "%%p" /s /q

echo Copying new mod files
xcopy %TARGET_PATH%\*.* "%DEPLOY_PATH%" /s /e

pause