namespace LibOpenNFS
{
    /// <summary>
    /// The list of supported games.
    /// </summary>
    public enum Game : byte
    {
        Underground2 = 1 << 0,
        MW = 1 << 1,
        Carbon = 1 << 2,
        ProStreet = 1 << 3,
        Undercover = 1 << 4,
        World = 1 << 5,
        
        Unknown = 255
    }
    
    /// <summary>
    /// Handles game loading.
    /// </summary>
    public class GameManager
    {
        
    }
}