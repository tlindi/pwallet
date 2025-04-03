using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using SimpLN.Services.PhoenixServices;
using SimpLN.Models;
using Newtonsoft.Json.Linq;
                                                                                                                   using SimpLN.Services.UserServices;

namespace SimpLN.Controllers;

[ApiController]
[Route("api/lnurl")]
public class LNURLController : ControllerBase
{
	private readonly IWalletService _walletService;

	public LNURLController(IWalletService walletService)
	{
		_walletService = walletService;
	}

	[HttpGet("pay")]
	public IActionResult GetLNURLPayRequest()
	{
		var callbackUrl = $"https://{Request.Host}/api/lnurl/pay/callback";
        
		var metadata = new JArray
		{
			new JArray { "text/plain", "Send some sats!" }
		};

		return Ok(new
		{
			callback = callbackUrl,
			maxSendable = 100_000_000,
			minSendable = 1_000,
			metadata = metadata.ToString(Formatting.None),
			tag = "payRequest"
		});
	}

	[HttpGet("pay/callback")]
	public async Task<IActionResult> GenerateInvoice(
		[FromQuery] long amount,
		[FromQuery] string? comment,
		[FromQuery] string? description)
	{

		try
		{
			var metadata = new JArray
			{
				new JArray { "text/plain", "Send some sats!" }
			};

			var metadataString = metadata.ToString(Formatting.None);
			var descriptionHash = SHA256.HashData(Encoding.UTF8.GetBytes(metadataString));

			var request = new CreateInvoiceRequest
			{
				AmountSat = amount / 1000,
				//DescriptionHash = BitConverter.ToString(descriptionHash)
				//	.Replace("-", "").ToLowerInvariant(),
				Description = !string.IsNullOrEmpty(comment) ? comment : !string.IsNullOrEmpty(description) ? description : "LNURL - Tip!",
				ExpirySeconds = 3600
			};

			var invoice = await _walletService.CreateLnUrlInvoiceAsync(request);

			if (invoice?.Serialized == null)
			{
				throw new Exception("Invoice generation failed: Empty response from wallet service");
			}

			return Ok(new { pr = invoice.Serialized, routes = Array.Empty<string>() });
		}
		catch (Exception ex)
		{
			return StatusCode(500, new
			{
				status = "ERROR",
				reason = ex.Message.Replace("Object reference not set", "Invoice generation failed")
			});
		}
	}
}

[ApiController]
[Route(".well-known/lnurlp")]
public class WellKnownController : ControllerBase
{
	private readonly IWebHostEnvironment _env;
	private readonly IFileProvider _fileProvider;

	public WellKnownController(IWebHostEnvironment env)
	{
		_env = env;
		_fileProvider = new PhysicalFileProvider(
			System.IO.Path.Combine(_env.WebRootPath, ".well-known", "lnurlp")
		) {
			UseActivePolling = true,
			UsePollingFileWatcher = true
		};
	}

	[HttpGet("{username}")]
	public IActionResult GetLightningAddress(string username)
	{
		var fullPath = System.IO.Path.Combine(
			_env.WebRootPath, 
			".well-known", 
			"lnurlp", 
			username
		);

		Response.Headers["Cache-Control"] = "no-store, no-cache";
		Response.Headers["Expires"] = "-1";

		if (!System.IO.File.Exists(fullPath))
		{
			return NotFound();
		}

		try
		{
			// Read entire file into memory first
			var fileBytes = System.IO.File.ReadAllBytes(fullPath);
			return File(fileBytes, "application/json");
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { status = "ERROR", reason = ex.Message });
		}
	}
}
