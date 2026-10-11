$ErrorActionPreference = 'Stop'
$project = Join-Path (Split-Path $PSScriptRoot) 'QuanLyNhaHang.csproj'
$checkDir = Join-Path ([IO.Path]::GetTempPath()) ('AccountSeederCheck-' + [guid]::NewGuid())
New-Item -ItemType Directory -Path $checkDir | Out-Null
$escapedProject = [System.Security.SecurityElement]::Escape($project)
@"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup>
  <ItemGroup><ProjectReference Include="$escapedProject" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $checkDir 'Check.csproj')
@'
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Utils;

// No provider: invalid input must fail before any database access.
using var db = new AppDbContext(new DbContextOptions<AppDbContext>());
async Task Reject<T>(string username, string password, string name, string email, VaiTro role)
    where T : Exception
{
    try { await AccountSeeder.CreateAsync(db, username, password, name, email, role); }
    catch (T) { return; }
    throw new Exception("Expected " + typeof(T).Name);
}
await Reject<ArgumentException>(" ", "password", "Tester", "test@example.com", VaiTro.Admin);
await Reject<ArgumentException>("tester", " ", "Tester", "test@example.com", VaiTro.User);
await Reject<ArgumentException>("tester", "password", " ", "test@example.com", VaiTro.User);
await Reject<ArgumentException>("tester", "password", "Tester", " ", VaiTro.User);
await Reject<ArgumentOutOfRangeException>("tester", "password", "Tester", "test@example.com", (VaiTro)99);
await Reject<ValidationException>("tester", "password", "Tester", "invalid-email", VaiTro.Admin);
await Reject<ValidationException>(new string('x', 51), "password", "Tester", "test@example.com", VaiTro.User);
await Reject<ValidationException>("tester", new string('x', 51), "Tester", "test@example.com", VaiTro.Admin);
Console.WriteLine("PASS: seeder rejects invalid input before database access.");
'@ | Set-Content (Join-Path $checkDir 'Program.cs')
dotnet run --project (Join-Path $checkDir 'Check.csproj') --property:UseAppHost=false
if ($LASTEXITCODE -ne 0) { throw 'AccountSeeder check failed.' }
