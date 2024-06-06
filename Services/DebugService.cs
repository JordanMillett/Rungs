public class DebugService
{
    public List<string> Messages = new List<string>();
    
    public DebugService()
    {
        
    }
    
    public void Log(string Message)
    {
        Messages.Insert(0, Message);
        Console.WriteLine(Message);
    }
}
