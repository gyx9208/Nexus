for %%i in (*.proto) do (protogen +langver=3.0 --csharp_out=./../../Assets/Scripts/Logic/Proto %%i)

pause