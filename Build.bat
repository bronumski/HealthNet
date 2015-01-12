@echo off
cls
"tools\nuget\nuget.exe" "Install" "packages.config" "-OutputDirectory" "packages" "-ExcludeVersion"
"packages\FAKE\tools\Fake.exe" build.fsx