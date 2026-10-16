# Họ và tên: Vi Thái Học
# Mã sinh viên: 23103100054
# Nội dung thực hiện: Chạy các kiểm thử logic và kiểm thử tích hợp Module 2.
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
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.4" />
    <Compile Include="../../AppDbContext.cs" />
    <Compile Include="../../Models/TaiKhoan.cs" />
    <Compile Include="../../Models/LoaiBan.cs" />
    <Compile Include="../../Models/MonAn.cs" />
    <Compile Include="../../Queries/BanAnFormLogic.cs" />
    <Compile Include="../../Queries/BanAnPagination.cs" />
    <Compile Include="../../Controllers/BanAnController.cs" />
    <Compile Include="../../Data/BanAnSeeder.cs" />
    <Compile Include="../../Migrations/*.cs" />
    <Compile Include="../../Models/BanAn.cs" />
    <Compile Include="../../Models/BanAnTrangThai.cs" />
    <Compile Include="../../ViewModels/BanAnFormViewModel.cs" />
    <Compile Include="../../ViewModels/BanAnIndexViewModel.cs" />
    <Compile Include="../../Queries/BanAnQueryExtensions.cs" />
    <Compile Include="../../Filters/Module2AdminAttribute.cs" />
    <Compile Include="../../Filters/Module2SessionAttribute.cs" />
    <Compile Include="../../Helpers/PasswordHelper.cs" />
  </ItemGroup>
</Project>
'@
Set-Content -LiteralPath (Join-Path $checkPath 'Checks.csproj') -Value $projectText -Encoding utf8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Module2Checks.cs.txt') -Destination (Join-Path $checkPath 'Program.cs') -Force
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Module2SqlChecks.cs.txt') -Destination (Join-Path $checkPath 'Module2SqlChecks.cs') -Force
dotnet run --project (Join-Path $checkPath 'Checks.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Module 2 checks failed.' }

