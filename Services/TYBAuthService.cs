using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TYB_AMI.Services
{
    /// <summary>
    /// Holds the JWT received from the parent WordPress window and
    /// configures HttpClient calls to the WordPress REST API.
    /// </summary>
    public class TYBAuthService
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient _http;
        private string? _token;
        private string? _displayName;

        // Set this to your real WordPress site's REST base.
        private const string ApiBase = "https://abundantminds.org/train-your-brain/wp-json/tyb/v1";

        public TYBAuthService(IJSRuntime js, HttpClient http)
        {
            _js = js;
            _http = http;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
        public string? DisplayName => _displayName;

        /// <summary>
        /// Waits for the parent window to post the JWT + display name
        /// in. Call this once during app startup (e.g. in MainLayout
        /// or App.razor), and re-use the SAME instance everywhere via
        /// dependency injection (register as Scoped) so every
        /// component sees the same token without waiting again.
        /// </summary>
        public async Task InitializeAsync()
        {
            var auth = await _js.InvokeAsync<AuthPayload?>("tybAuth.waitForAuth");

            _token = auth?.Token;
            _displayName = auth?.DisplayName;

            if (!string.IsNullOrEmpty(_token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<UserStats?> GetStatsAsync()
        {
            if (!IsAuthenticated) return null;

            var response = await _http.GetAsync($"{ApiBase}/stats");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserStats>();
        }

        /// <summary>
        /// Call this once a single game finishes. gameId must match
        /// one of the IDs the WordPress plugin expects exactly
        /// (e.g. "game1", "game2", "game3").
        /// </summary>
        public async Task<UserStats?> SubmitScoreAsync(string gameId, int score)
        {
            if (!IsAuthenticated) return null;

            var response = await _http.PostAsJsonAsync($"{ApiBase}/stats", new
            {
                game_id = gameId,
                score = score
            });

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserStats>();
        }

        private class AuthPayload
        {
            public string? Token { get; set; }
            public string? DisplayName { get; set; }
        }
    }

    public class UserStats
    {
        public StreakInfo Streak { get; set; } = new();
        public DailyProgress DailyProgress { get; set; } = new();
        public Dictionary<string, GameStats> Games { get; set; } = new();
    }

    public class StreakInfo
    {
        public int Current { get; set; }
        public int Longest { get; set; }
        public string? LastCompletedDate { get; set; }
    }

    public class DailyProgress
    {
        public string? Date { get; set; }
        public List<string> CompletedGames { get; set; } = new();
    }

    public class GameStats
    {
        public int GamesPlayed { get; set; }
        public int BestScore { get; set; }
        public int TotalScore { get; set; }
        public List<GameHistoryEntry> History { get; set; } = new();
    }

    public class GameHistoryEntry
    {
        public int Score { get; set; }
        public string Date { get; set; } = "";
    }
}
