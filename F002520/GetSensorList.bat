@echo off
set SensorList=
del sensor_list
adb wait-for-device
adb shell sns_hal_batch -l 2>sensor_list
set /p SensorList=<sensor_list
echo sensor_list: %sensor_list%