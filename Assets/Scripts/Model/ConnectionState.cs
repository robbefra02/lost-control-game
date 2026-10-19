namespace LostControl.Model
{
    /// <summary>The player's remote session. There is at most one at a time.</summary>
    public enum ConnectionState
    {
        None = 0,
        Connecting = 1,
        Connected = 2,
    }
}
