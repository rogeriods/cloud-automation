REM Copy your backup files to dumps folder
xcopy /s/z/y <source> dumps\<destination>

REM Compact the dumps folder 
zip -r dumps.zip dumps

REM Delete folders and files
rmdir /s/q  <source> dumps\<destination>

del /s/q <source> dumps\<destination>

REM Build and running the application
dotnet build
dotnet run

REM Delete dumps.zip file
del /s/q dumps.zip