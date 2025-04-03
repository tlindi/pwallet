namespace SimpLN.Models.Config;

public class BackendSettingsModel
{
	public int Id { get; set; }
	public string BackendUrl { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
	public string UserId { get; set; }
}
