# Họ và tên: Vi Thái Học
# Mã sinh viên: 23103100054
# Nội dung thực hiện: Kiểm tra validation, truy vấn và phân quyền Module 2.
$ErrorActionPreference = 'Stop'
$repoPath = Split-Path $PSScriptRoot -Parent
$checkPath = Join-Path $repoPath 'obj/Module2Checks'
New-Item -ItemType Directory -Path $checkPath -Force | Out-Null
$projectText = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <Using Include="Microsoft.AspNetCore.Http" />
    <Compile Include="../../Models/BanAn.cs" />
    <Compile Include="../../Models/BanAnTrangThai.cs" />
    <Compile Include="../../ViewModels/BanAnFormViewModel.cs" />
    <Compile Include="../../ViewModels/BanAnIndexViewModel.cs" />
    <Compile Include="../../Queries/BanAnQueryExtensions.cs" />
    <Compile Include="../../Filters/Module2AdminAttribute.cs" />
  </ItemGroup>
</Project>
'@
Set-Content -LiteralPath (Join-Path $checkPath 'Checks.csproj') -Value $projectText -Encoding utf8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Module2Checks.cs.txt') -Destination (Join-Path $checkPath 'Program.cs') -Force
dotnet run --project (Join-Path $checkPath 'Checks.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Module 2 checks failed.' }

