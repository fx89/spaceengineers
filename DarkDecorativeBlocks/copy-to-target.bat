@echo off

SET TARGET_PATH=.\_target
SET TARGET_DATA_PATH=_target\Data
SET TARGET_MODELS_PATH=_target\Models
SET TARGET_TEXTURES_PATH=_target\Textures
SET TARGET_ICONS_PATH=_target\Textures\Icons

echo Removing target
rmdir %TARGET_PATH% /s /q

echo Creating target
mkdir %TARGET_DATA_PATH%
mkdir %TARGET_MODELS_PATH%
mkdir %TARGET_TEXTURES_PATH%
mkdir %TARGET_ICONS_PATH%

echo Copying data
copy Data\*.sbc %TARGET_DATA_PATH% >NUL

echo Copying models
copy Models\*.mwm %TARGET_MODELS_PATH% >NUL
REM del %TARGET_MODELS_PATH%\*LOD1.mwm
REM del %TARGET_MODELS_PATH%\*LOD2.mwm
REM del %TARGET_MODELS_PATH%\*LOD3.mwm

echo Copying textures
copy Textures\*.dds %TARGET_TEXTURES_PATH% >NUL

echo Copying icons
copy Textures\Icons\*.dds %TARGET_ICONS_PATH% >NUL

echo Copying mod info
copy ModInfo\readme.txt %TARGET_PATH% >NUL
copy ModInfo\thumb.png %TARGET_PATH% >NUL

pause