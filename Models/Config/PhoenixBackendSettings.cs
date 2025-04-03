namespace SimpLN.Models.Config;

public class PhoenixBackendSettings
{
	public const string SectionName = "PhoenixBackend";
	public required string ApiUrl { get; set; }
	public required string ApiPassword { get; set; }
	public required string UiDomain { get; set; }
	public required string LnUrlpDomain { get; set; }
}
