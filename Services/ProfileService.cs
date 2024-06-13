using System.Diagnostics;

public class ProfileService
{
    public event Action OnProfileSave = () => { };
    
    Blazored.LocalStorage.ILocalStorageService Local;
    DebugService Debug;
    FileService FService;

    //-----     SAVED DATA     -----
    public string ProfileName = "User";
    public List<string> Terms = new List<string>();
    public List<string> RecentPages = new List<string>();

    public ProfileService(Blazored.LocalStorage.ILocalStorageService localStorage, DebugService debug, FileService fservice)
    {
        Local = localStorage;
        Debug = debug;
        FService = fservice;
    }

    void InitializeProfile()
    {
        ProfileName = "User";
        Terms = new List<string>();
        RecentPages = new List<string>();
    }

    public async Task InitializeAsync()
    {
        InitializeProfile();
        
        Stopwatch stopwatch = Stopwatch.StartNew();

        await LoadProfile();
        stopwatch.Stop();
        
        Debug.LogSuccess($"Loaded {Terms.Count} saved terms in {stopwatch.ElapsedMilliseconds}ms");
        await SaveProfile();
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
            
            if (!Terms.Contains(Value))
            {
                Terms.Add(Value);
                Debug.Log("Added Term: " + Value);
                await SaveProfile();
            }
        }
    }
    
    public async Task RemoveTerm(string Value)
    {
        if (HasTerm(Value))
        {
            Terms.Remove(Value.ToLower());
            Debug.Log("Removed Term: " + Value);
            await SaveProfile();
        }
    }
    
    public bool HasTerm(string Value)
    {
        return Terms.Contains(Value.ToLower());
    }
    
    public async Task ToggleTerm(string Value)
    {
        if (!FService.AllTerms.ContainsKey(Value.ToLower()))
            return;
        
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
            try
            {
                ProfileName = await Local.GetItemAsync<string>("name") ?? ProfileName;
            }catch
            {
                await Local.RemoveItemAsync("name");
                Debug.LogError($"Failed to load profile name, clearing...");
            }
        }
        
        if(await Local.ContainKeyAsync("terms"))
        {
            try
            {
                Terms = await Local.GetItemAsync<List<string>>("terms") ?? Terms;
            }catch
            {
                await Local.RemoveItemAsync("terms");
                Debug.LogError($"Failed to load saved terms, clearing...");
            }
            
            for (int i = 0; i < Terms.Count; i++)
            {
                if(!FService.AllTerms.ContainsKey(Terms[i]))
                {
                    Debug.LogError($"Term definition for {Terms[i]} not found, deleting...");
                    Terms.Remove(Terms[i]);
                }
            }
        }
        
        if(await Local.ContainKeyAsync("recent"))
        {
            try
            {
                RecentPages = await Local.GetItemAsync<List<string>>("recent") ?? RecentPages;
            }catch
            {
                await Local.RemoveItemAsync("recent");
                Debug.LogError($"Failed to load recent pages, clearing...");
            }
            
            for (int i = 0; i < RecentPages.Count; i++)
            {
                if(!FService.AllFiles.ContainsKey(RecentPages[i]))
                {
                    Debug.LogError($"Recent page at URL {RecentPages[i]} not found, deleting...");
                    RecentPages.Remove(RecentPages[i]);
                }
            }
        }
    }
    
    public async Task SaveProfile()
    {
        await Local.SetItemAsync<string>("name", ProfileName);
        
        await Local.SetItemAsync<List<string>>("terms", Terms);
        
        await Local.SetItemAsync<List<string>>("recent", RecentPages);
        
        OnProfileSave?.Invoke();
    }
    
    public async Task ClearProfile()
    {
        await Local.ClearAsync();
        Debug.Log("Cleared profile");
        InitializeProfile();
    }
}

public struct SavedTerm
{
    public string RU { get; set; }
    public string EN { get; set; }

    public SavedTerm(string ru, string en)
    {
        RU = ru;
        EN = en;
    }
}
