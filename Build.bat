@echo off
cls
"tools\nuget\nuget.exe" "Install" "packages.config" "-OutputDirectory" "packages" "-ExcludeVersion"
"tools\nuget\nuget.exe" "Restore" "src\HealthNet.sln"
"packages\FAKE\tools\Fake.exe" build.fsx