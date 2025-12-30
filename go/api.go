package main

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"
)

type unityAPIResponse struct {
	Results []struct {
		ShortRevision string `json:"shortRevision"`
	} `json:"results"`
}

// fetchChangeset fetches the changeset for a Unity version from the Unity API
func fetchChangeset(version string) (string, error) {
	url := fmt.Sprintf("https://services.api.unity.com/unity/editor/release/v1/releases?version=%s", version)

	client := &http.Client{
		Timeout: 10 * time.Second,
	}

	resp, err := client.Get(url)
	if err != nil {
		return "", fmt.Errorf("failed to fetch from Unity API: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return "", fmt.Errorf("Unity API returned status %d", resp.StatusCode)
	}

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return "", fmt.Errorf("failed to read API response: %w", err)
	}

	var apiResp unityAPIResponse
	if err := json.Unmarshal(body, &apiResp); err != nil {
		return "", fmt.Errorf("failed to parse API response: %w", err)
	}

	if len(apiResp.Results) == 0 {
		return "", fmt.Errorf("no results found for Unity version %s", version)
	}

	changeset := apiResp.Results[0].ShortRevision
	if changeset == "" {
		return "", fmt.Errorf("changeset not found in API response")
	}

	return changeset, nil
}
