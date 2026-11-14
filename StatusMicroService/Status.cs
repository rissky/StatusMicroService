// ==============================================================================
// 2. DATA MODEL
// This represents the shape of the data we are sending and receiving.
// In a real app, this would be mapped to a database table via Entity Framework.
// ==============================================================================
public class SystemStatus
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string Status { get; set; } // e.g., "Online", "Degraded", "Offline"
}