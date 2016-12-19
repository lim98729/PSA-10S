del /s *.bak

cd ./AccessoryLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./AjinExTekLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./DefineLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./HalconLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./MeiLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./PSA_Application
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./PSA_SystemLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./SystemLibrary
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

cd ./WindowsApplication
del /S /F /Q bin
del /S /F /Q obj
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd Properties
rename *.cs *.cpp
rename *.resx *.resx.cpp
rename *.dll *.dll.cpp
rename *.csproj *.csproj.cpp
cd ../..

rename *.sln *.sln.cpp
rename *.suo *.suo.cpp
rename *.sdf *.sdf.cpp
rename *.txt *.txt.cpp
rename *.bat *.cpp

pause
