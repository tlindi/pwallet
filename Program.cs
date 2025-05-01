using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using SimpLN.Components.Account;
using SimpLN.Data;
using SimpLN.Frontend;
using SimpLN.Models.Config;
using SimpLN.Repositories;
using SimpLN.Services.BitcoinPrice;
using SimpLN.Services.InvoiceServices;
using SimpLN.Services.PhoenixServices;
using SimpLN.Services.QrService;
using SimpLN.Services.TranscationHistoryServices;
using SimpLN.Services.UserServices;
using SimpLN.Services.UtilityServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.WebHost.UseUrls("http://0.0.0.0:4949");

builder.Services.AddCors(options =>
{
	options.AddPolicy("LNURL", policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});

builder.Services.AddControllers();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<AppDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddHttpClient();

builder.Services.AddTransient<IPrerenderService, PrerenderService>();

builder.Services.AddHttpClient("PhoenixBackend", client =>
{
	var apiUrl = builder.Configuration["PhoenixBackend:ApiUrl"];
	if (string.IsNullOrEmpty(apiUrl))
		throw new InvalidOperationException("PhoenixBackend:ApiUrl is not configured");

	client.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient("LnUrlpBackend", client =>
{
	var apiUrl = builder.Configuration["PhoenixBackend:LnUrlpDomain"];
	if (string.IsNullOrEmpty(apiUrl))
		throw new InvalidOperationException("PhoenixBackend:LnUrlpDomain is not configured");

	client.BaseAddress = new Uri(apiUrl);
});

builder.Services.AddHttpClient("PublicFacing", client =>
{
	var uiDomain = builder.Configuration["PhoenixBackend:UiDomain"];
	if (string.IsNullOrEmpty(uiDomain))
		throw new InvalidOperationException("PhoenixBackend:UiDomain is not configured");

	client.BaseAddress = new Uri(uiDomain);
});

builder.Services.Configure<PhoenixBackendSettings>(
	builder.Configuration.GetSection(PhoenixBackendSettings.SectionName)
);

builder.Services.AddTransient<IWalletService>(sp =>
	new WalletService(
		sp.GetRequiredService<IHttpClientFactory>().CreateClient("PhoenixBackend"),
		sp.GetRequiredService<IOptions<PhoenixBackendSettings>>()
	)
);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<LoadingService>();
builder.Services.AddSingleton<StateContainerService>();
builder.Services.AddScoped<QrCodeService>();

builder.Services.AddTransient<IInvoiceDecodeService, InvoiceDecodeService>();
builder.Services.AddScoped<IInvoiceTypeService, InvoiceTypeService>();
builder.Services.AddScoped<WebSocketService>();
builder.Services.AddScoped<IPayService, PayService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddTransient<ConfigRepository>();
builder.Services.AddTransient<IConfigService, ConfigService>();
builder.Services.AddTransient<CloudflareDnsService>();
builder.Services.AddSingleton<IBitcoinPriceService, BitcoinPriceService>();
builder.Services.AddTransient<OutgoingPaymentsRepository>();
builder.Services.AddTransient<OutgoingPaymentsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	await dbContext.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();
app.UseStaticFiles(new StaticFileOptions 
{
	ServeUnknownFileTypes = false,
	OnPrepareResponse = ctx =>
	{
		if (ctx.Context.Request.Path.StartsWithSegments("/.well-known"))
		{
			ctx.Context.Response.StatusCode = 404;
			ctx.Context.Response.ContentLength = 0;
			ctx.Context.Response.Body = Stream.Null;
		}
	}
});
app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.UseCors("LNURL");
//	app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//	ForwardedHeaders = ForwardedHeaders.XForwardedFor |
//					   ForwardedHeaders.XForwardedProto |
//					   ForwardedHeaders.XForwardedHost
//});
// Alternative Representation of Forwarders
var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
};
forwardOptions.KnownNetworks.Clear();
forwardOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardOptions);

app.Use(async (context, next) =>
{
	if (context.Request.Headers.TryGetValue("X-Forwarded-Host", out var host))
	{
		context.Request.Host = new HostString(host);
	}
	await next();
});


app.Run();