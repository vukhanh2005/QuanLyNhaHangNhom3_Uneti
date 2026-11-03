$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot
$checkDir = Join-Path ([IO.Path]::GetTempPath()) ('LoaiBanCheck-' + [guid]::NewGuid())
New-Item -ItemType Directory -Path $checkDir | Out-Null
$project = [System.Security.SecurityElement]::Escape((Join-Path $root 'QuanLyNhaHang.csproj'))
@"
<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup><ProjectReference Include="$project" /></ItemGroup>
</Project>
"@ | Set-Content (Join-Path $checkDir 'Check.csproj')
@'
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLyNhaHang.Controllers.Module01;
using QuanLyNhaHang.DTO;
using QuanLyNhaHang.Repository;
using QuanLyNhaHang.Service.Implement;

void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
var config = new ConfigurationBuilder().SetBasePath(args[0]).AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true).AddEnvironmentVariables().Build();
using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(config.GetConnectionString("DefaultConnection")).Options);
await using var transaction = await db.Database.BeginTransactionAsync();
try
{
    var service = new LoaiBanService(new LoaiBanRepository(db));
    var controller = new LoaiBanController(service);
    var http = new DefaultHttpContext { Session = new TestSession() };
    var actionContext = new ActionContext(http, new RouteData(), new ControllerActionDescriptor());
    controller.ControllerContext = new ControllerContext(actionContext);
    ActionExecutingContext Guard()
    {
        var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(),
            new Dictionary<string, object?>(), controller);
        controller.OnActionExecuting(context);
        return context;
    }
    Check(Guard().Result is RedirectToActionResult { ActionName: "Login" }, "Anonymous must log in");
    http.Session.SetInt32("MaTaiKhoan", 1);
    http.Session.SetString("VaiTro", "User");
    Check(Guard().Result is StatusCodeResult { StatusCode: 403 }, "User must be forbidden");
    http.Session.SetString("VaiTro", "Admin");
    Check(Guard().Result == null, "Admin must pass");
    Check(Attribute.IsDefined(typeof(LoaiBanController), typeof(AutoValidateAntiforgeryTokenAttribute)), "Missing CSRF filter");
    Check(await controller.CapNhatTrangThai(1, null) is BadRequestResult, "Missing status must be rejected");
    controller.ModelState.AddModelError("TenLoaiBan", "Invalid");
    Check(await controller.Them(new LoaiBanRequest()) is ViewResult { ViewName: "Form" }, "Invalid form must redisplay");
    controller.ModelState.Clear();

    var name = "check-" + Guid.NewGuid().ToString("N");
    var request = new LoaiBanRequest { TenLoaiBan = "  " + name + "  ", MoTa = null, TrangThai = true };
    Check(!(await service.Them(new LoaiBanRequest { TenLoaiBan = " " })).IsSuccess, "Blank name accepted");
    Check(!(await service.Them(new LoaiBanRequest { TenLoaiBan = new string('x', 101) })).IsSuccess, "Long name accepted");
    Check(!(await service.Them(new LoaiBanRequest { TenLoaiBan = name, MoTa = new string('x', 501) })).IsSuccess, "Long description accepted");
    var created = await service.Them(request);
    Check(created.IsSuccess && created.MaLoaiBan > 0 && created.TenLoaiBan == name, "Create/trim failed");
    Check(!(await service.Them(request)).IsSuccess, "Duplicate accepted");
    var other = await service.Them(new LoaiBanRequest { TenLoaiBan = name + "-other" });
    Check(!(await service.Sua(other.MaLoaiBan, request)).IsSuccess, "Duplicate edit accepted");
    request.MoTa = "Updated";
    var updated = await service.Sua(created.MaLoaiBan, request);
    Check(updated.IsSuccess && updated.MaLoaiBan == created.MaLoaiBan && updated.MoTa == "Updated", "Update changed key or failed");
    Check((await service.CapNhatTrangThai(created.MaLoaiBan, false)).IsSuccess, "Status update failed");
    db.ChangeTracker.Clear();
    Check(!(await service.TimKiem(created.MaLoaiBan)).TrangThai, "Status not persisted");
    Check((await service.GetAllLoaiBan()).Any(x => x.MaLoaiBan == created.MaLoaiBan), "List missing item");
    Check(await controller.ChiTiet(created.MaLoaiBan) is ViewResult, "Details failed");
    Check(await controller.Xoa(created.MaLoaiBan) is ViewResult, "Delete confirmation failed");
    Check((await service.TimKiem(created.MaLoaiBan)).IsSuccess, "GET deleted data");
    Check((await service.Xoa(created.MaLoaiBan)).IsSuccess, "Delete failed");
    Check(await controller.ChiTiet(created.MaLoaiBan) is NotFoundResult, "Missing item must return 404");
    Console.WriteLine("PASS: CRUD, validation, duplicates, key preservation, status, authorization, CSRF declaration and missing IDs.");
}
finally
{
    await transaction.RollbackAsync();
    Console.WriteLine("Test transaction rolled back; no test records retained.");
}

sealed class TestSession : ISession
{
    private readonly Dictionary<string, byte[]> data = new();
    public bool IsAvailable => true;
    public string Id => "test";
    public IEnumerable<string> Keys => data.Keys;
    public void Clear() => data.Clear();
    public void Remove(string key) => data.Remove(key);
    public void Set(string key, byte[] value) => data[key] = value;
    public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out byte[]? value) => data.TryGetValue(key, out value);
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
'@ | Set-Content (Join-Path $checkDir 'Program.cs')
dotnet run --project (Join-Path $checkDir 'Check.csproj') --property:BaseOutputPath="$checkDir/out/" --property:UseAppHost=false -- $root
if ($LASTEXITCODE -ne 0) { throw 'LoaiBan checks failed.' }
