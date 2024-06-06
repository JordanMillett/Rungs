using System.Diagnostics;
using System.Text.Json;

public class Song
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string URL { get; set; }

    public Song(string title, string artist, string url)
    {
        Title = title;
        Artist = artist;
        URL = url;
    }
}

public class SongService
{
    Dictionary<string, Song> AllSongs;

    DebugService Debug;
    HttpClient Http { get; set; }

    public SongService(HttpClient httpClient, DebugService debug)
    {
        AllSongs = new Dictionary<string, Song>();
        Http = httpClient;
        Debug = debug;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        await LoadSongList();
        
        stopwatch.Stop();
        Debug.Log($"Loaded {AllSongs.Count} songs in {stopwatch.ElapsedMilliseconds}ms");
    }

    async Task LoadSongList()
    {
        string JSON = await Http.GetStringAsync("data/songlist.txt");

        List<Song> LoadedSongs = JsonSerializer.Deserialize<List<Song>>(JSON)!;

        foreach (Song S in LoadedSongs)
        {
            AddSong(S.Title, S.Artist, S.URL);
        }
    }

    private void AddSong(string title, string artist, string url)
    {
        Song S = new Song(title, artist, url);
        AllSongs.Add(title, S);
    }

    public async Task<List<Song>> Search(string searchTerm)
    {
        return await Task.Run(() =>
        {
            var matchedSongs = new List<Song>();

            foreach (var song in AllSongs.Values)
            {
                if (song.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    song.Artist.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    matchedSongs.Add(song);
                }
            }

            return matchedSongs;
        });
    }
}
