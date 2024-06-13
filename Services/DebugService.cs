public class DebugService
{
    public List<(string, string, string)> Messages = new List<(string, string, string)>();

    ProfileService? PService;
    
    public void Initialize(ProfileService pservice)
    {
        PService = pservice;
    }
    
    public void Log(string Message)
    {
        Messages.Insert(0, (Message, "", DateTime.Now.ToString("HH:mm:ss:fff")));
        Console.WriteLine(Message);
    }
    
    public void LogError(string Message)
    {
        Messages.Insert(0, (Message, "error", DateTime.Now.ToString("HH:mm:ss:fff")));
        Console.WriteLine(Message);
    }
    
    public void LogSuccess(string Message)
    {
        Messages.Insert(0, (Message, "success", DateTime.Now.ToString("HH:mm:ss:fff")));
        Console.WriteLine(Message);
    }
    
    public async Task HandleError(string Message, HandleOptions Option)
    {
        LogError(Message);
        switch(Option)
        {
            case HandleOptions.ClearProfile: await PService!.ClearProfile(); break;
        }
    }
}

public enum HandleOptions
{
    ClearProfile
}