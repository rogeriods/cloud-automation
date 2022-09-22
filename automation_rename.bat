REM Copy your backup files to dumps folder
xcopy /s/z/y <source> dumps\<destination>

REM Compact the dumps folder 
zip -r dumps.zip dumps

dotnet build
dotnet run