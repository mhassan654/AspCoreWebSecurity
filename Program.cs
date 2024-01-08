using ASPCoreWebSecurity.Authorization;
using Microsoft.AspNetCore.Authorization;
using static ASPCoreWebSecurity.Authorization.HRManagerProbationRequirement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.ExpireTimeSpan=TimeSpan.FromSeconds(30);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("HRManagerOnly", 
        policy => policy.RequireClaim("Department", "HRManager")
            .RequireClaim("Manager")
            .Requirements.Add(new HRManagerProbationRequirement(300))
        );
}); 

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7288/");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();