using System.Text.Json.Nodes;

// https://services.docs.unity.com/release/v1/
static class UnityReleaseApi
{
	public static async Task<string> FetchChangesetAsync(string version)
	{
		using var client = new HttpClient();
		string url = $"https://services.api.unity.com/unity/editor/release/v1/releases?version={version}";

		var response = await client.GetAsync(url);
		response.EnsureSuccessStatusCode();

		string content = await response.Content.ReadAsStringAsync();
		var apiResponse = JsonNode.Parse(content);

		var results = apiResponse?["results"]?.AsArray();

		if (results == null || results.Count == 0)
			throw new Exception($"No results found for Unity version '{version}'.");
		if (results.Count > 1)
			throw new Exception($"More than one result found for Unity version '{version}'.");

		string? changeset = results[0]?["shortRevision"]?.GetValue<string>();

		if (string.IsNullOrEmpty(changeset))
			throw new Exception("Changeset not found in API response.");

		return changeset;
	}
}