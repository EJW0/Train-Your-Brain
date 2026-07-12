using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

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
        private const string ApiBase = "https://abundantminds.org/wp-json/tyb/v1";

        public TYBAuthService(IJSRuntime js, HttpClient http)
        {
            _js = js;
            _http = http;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
        public string? DisplayName => _displayName;
        public event Action? OnStatsChanged;
        public void NotifyStatsChanged() => OnStatsChanged?.Invoke();


        /// Waits for the parent window to post the JWT + display name in.
        /// Call this once during app startup (e.g. in MainLayout
        /// or App.razor), and re-use the SAME instance everywhere via
        /// dependency injection (register as Scoped) so every
        /// component sees the same token without waiting again.
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
        /// (e.g. "game1", "math", "game3").
        /// </summary>
        public async Task<UserStats?> SubmitScoreAsync(string gameId, int score, int timeSeconds){
            if (!IsAuthenticated) return null;
            var response = await _http.PostAsJsonAsync($"{ApiBase}/stats", new{
                game_id = gameId,
                score = score,
                time_seconds = timeSeconds
            });

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserStats>();
        }

        private class AuthPayload{
            public string? Token { get; set; }
            public string? DisplayName { get; set; }
        }

        public static List<string> RequiredGamesForCycleDay(int cycleDay){
        if (cycleDay == 1 || cycleDay == 8)
            return new List<string> { "stroop", "memory" };
        return new List<string> { "math" };
    }
    }

    public class UserStats
    {
        [JsonPropertyName("streak")]
        public StreakInfo Streak { get; set; } = new();
        
        [JsonPropertyName("cycle_day")]
        public int CycleDay { get; set; }

        [JsonPropertyName("daily_progress")]
        public DailyProgress DailyProgress { get; set; } = new();

        [JsonPropertyName("games")]
        public Dictionary<string, GameStats> Games { get; set; } = new();
        
        [JsonPropertyName("completed_dates")]
        public List<string> CompletedDates { get; set; } = new();       
    }

    public class StreakInfo
    {
        [JsonPropertyName("current")]
        public int Current { get; set; }

        [JsonPropertyName("longest")]
        public int Longest { get; set; }

        [JsonPropertyName("last_completed_date")]
        public string? LastCompletedDate { get; set; }
    }

    public class DailyProgress
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("completed_games")]
        public List<string> CompletedGames { get; set; } = new();
    }

    public class GameStats
    {
        [JsonPropertyName("games_played")]
        public int GamesPlayed { get; set; }

        [JsonPropertyName("best_score")]
        public int BestScore { get; set; }

        [JsonPropertyName("total_score")]
        public int TotalScore { get; set; }

        [JsonPropertyName("best_time")]
        public int BestTime { get; set; }

        [JsonPropertyName("history")]
        public List<GameHistoryEntry> History { get; set; } = new();
    }

    public class GameHistoryEntry
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; } = "";

        [JsonPropertyName("time_seconds")]
        public int TimeSeconds { get; set; }
        
        [JsonPropertyName("cycle_day")]
        public int? CycleDay { get; set; }
    }
}
