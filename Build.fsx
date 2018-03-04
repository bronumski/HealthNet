#r @"packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.AssemblyInfoHelper
open Fake.Git

let srcDir = "./src"
let packagesDir = "./packages"
let toolsDir = "./tools"
let testDir = srcDir + "/tests/"
let slnPath = srcDir + "/HealthNet.sln"

let versionMajorMinor = "1.2"

let commitHash = Information.getCurrentSHA1("")

let versionMacroBuild = 
    match buildServer with
    | AppVeyor -> ("0." + appVeyorBuildVersion.Replace("1.1.", ""))
    | _ -> "0"

let buildVersion = versionMajorMinor + "." + versionMacroBuild

Target "Clean" (fun _ ->
    CleanDirs []
    DeleteDir "bin"
    CreateDir "bin"
)

Target "Version" (fun _ ->
    CreateCSharpAssemblyInfo (srcDir + "/VersionInfo.cs")
        [Attribute.Version buildVersion
         Attribute.FileVersion buildVersion
         Attribute.Metadata("githash", commitHash)]

    match buildServer with
    | _ -> ()
)

Target "RestorePackages" (fun _ -> 
     slnPath
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = packagesDir
             ToolPath = (toolsDir + "/Nuget/nuget.exe")
             Retries = 4 }))

Target "Build" (fun _ ->
    !! slnPath
        |> MSBuildReleaseExt "" [("Configuration", "Release")] "Build"
        |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    let testResultFile = testDir + "TestResults.xml"
    !! (testDir + "**/bin/Release/*Tests.dll")
        |> NUnit (fun p ->
            {p with
                ToolPath = "./packages/NUnit.Runners/tools"
                ToolName = "nunit-console-x86.exe"
                DisableShadowCopy = true
                OutputFile = testResultFile })
)

Target "CreatePackage" (fun _ ->
    // Copy all the package files into a package folder
    for nuspec in !! (srcDir + "/**/*.nuspec") do

        let projFileName = nuspec.Replace(".nuspec", ".csproj")
        
        NuGetPack (fun p -> 
            {p with
                OutputPath = "bin"
                WorkingDir = "bin"
                Version = buildVersion
                IncludeReferencedProjects = true
                Properties = [ ("configuration", "release") ]
                }) 
                projFileName
)

Target "Default" (fun _ ->
    trace "Build Complete"
)

"Clean"
 ==> "Version"
 ==> "RestorePackages"
 ==> "Build"
 ==> "Test"
 ==> "CreatePackage"
 ==> "Default"

RunTargetOrDefault "Default"