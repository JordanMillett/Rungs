using System.Diagnostics;

public class Term
{
    public string RU { get; set; }

    public Term(string ru)
    {
        RU = ru;
    }
}

public class ProfileService
{
    public event Action OnProfileSave = () => { };
    
    Blazored.LocalStorage.ILocalStorageService Local;
    DebugService Debug;

    //-----     SAVED DATA     -----
    public string ProfileName = "User";
    public Dictionary<string, Term> Terms = new Dictionary<string, Term>();
    public List<string> RecentPages = new List<string>();

    public ProfileService(Blazored.LocalStorage.ILocalStorageService localStorage, DebugService debug)
    {
        Local = localStorage;
        Debug = debug;
    }

    public async Task InitializeAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        await LoadProfile();

        stopwatch.Stop();
        Debug.Log($"Loaded {Terms.Count} terms in {stopwatch.ElapsedMilliseconds}ms");
        Debug.Log($"Profile: {ProfileName} loaded");
    }
    
    public async Task UpdateRecent(string Link)
    {
        for(int i = 0; i < RecentPages.Count; i++)
        {
            if (RecentPages[i] == Link)
                RecentPages.RemoveAt(i);
        }

        RecentPages.Insert(0, Link);

        int recentSize = 4;

        if (RecentPages.Count > recentSize)
        {
            RecentPages.RemoveAt(recentSize);
        }

        await SaveProfile();
    }
    
    public async Task AddTerm(string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value))
        {
            Value = Value.ToLower();
            
            if (!Terms.ContainsKey(Value))
            {
                Terms.Add(Value, new Term(Value));
                Debug.Log("Added Term: " + Value);
                await SaveProfile();
            }
        }
    }
    
    public async Task RemoveTerm(string Value)
    {
        Terms.Remove(Value.ToLower());
        Debug.Log("Removed Term: " + Value);
        await SaveProfile();
    }
    
    public bool HasTerm(string Value)
    {
        return Terms.ContainsKey(Value.ToLower());
    }
    
    public async Task ToggleTerm(string Value)
    {        
        if(HasTerm(Value))
        {
            await RemoveTerm(Value);
        }else
        {
            await AddTerm(Value);
        }
    }

    public async Task LoadProfile()
    {
        if(await Local.ContainKeyAsync("name"))
        {
            ProfileName = await Local.GetItemAsync<string>("name") ?? ProfileName;
        }
        
        if(await Local.ContainKeyAsync("terms"))
        {
            Terms = await Local.GetItemAsync<Dictionary<string, Term>>("terms") ?? Terms;
        }
        
        if(await Local.ContainKeyAsync("recent"))
        {
            RecentPages = await Local.GetItemAsync<List<string>>("recent") ?? RecentPages;
        }
    }
    
    public async Task SaveProfile()
    {
        await Local.SetItemAsync<string>("name", ProfileName);
        
        await Local.SetItemAsync<Dictionary<string, Term>>("terms", Terms);
        
        await Local.SetItemAsync<List<string>>("recent", RecentPages);
        
        OnProfileSave?.Invoke();
    }
}
