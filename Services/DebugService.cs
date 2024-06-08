public class DebugService
{
    public List<(string, string)> Messages = new List<(string, string)>();
    
    public DebugService()
    {
        
    }
    
    public void Log(string Message)
    {
        Messages.Insert(0, (Message, ""));
        Console.WriteLine(Message);
    }
    
    public void LogError(string Message)
    {
        Messages.Insert(0, (Message, "error"));
        Console.WriteLine(Message);
    }
    
    public void LogSuccess(string Message)
    {
        Messages.Insert(0, (Message, "success"));
        Console.WriteLine(Message);
    }
}
