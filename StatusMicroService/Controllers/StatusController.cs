// ==============================================================================
// 3. THE CONTROLLER
// The [ApiController] attribute tells ASP.NET that this class handles API requests.
// The [Route("api/[controller]")] attribute sets the base URL for this whole class.
// Because the class is named "StatusController", the route becomes "/api/status"
// ==============================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{

    private const string ConnectionString = "Data Source=Netheret_Example_DB.db";

    //// A mock in-memory database to demonstrate functionality without needing SQL.
    //// We make it static so the data persists while the application is running.
    //private static readonly List<SystemStatus> _mockDatabase = new()
    //{
    //    new SystemStatus { Id = 1, ServiceName = "Web Dashboard", Status = "Online" },
    //    new SystemStatus { Id = 2, ServiceName = "Payment Gateway", Status = "Degraded" },
    //    new SystemStatus { Id = 3, ServiceName = "Email Server", Status = "Online" }
    //};

    /// <summary>
    /// ROUTE: GET /api/status
    /// The [HttpGet] attribute tells ASP.NET to run this method when a GET request
    /// is made to the base route (/api/status).
    /// </summary>
    [HttpGet]
    public IActionResult GetAllStatuses()
    {
        // Ok() automatically converts our C# List into a JSON array and returns 
        // an HTTP 200 (Success) status code.
        var statuses = new List<SystemStatus>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ServiceName, Status FROM Statuses";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            statuses.Add(new SystemStatus
            {
                Id = reader.GetInt32(0),
                ServiceName = reader.GetString(1),
                Status = reader.GetString(2)
            });
        }

        return Ok(statuses);
    }

    /// <summary>
    /// ROUTE: GET /api/status/{id}
    /// The "{id}" in the attribute means this route expects a variable at the end of the URL.
    /// Example: A request to /api/status/2 will execute this method and pass '2' into the id parameter.
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetStatusById(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ServiceName, Status FROM Statuses WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id); // Parameterized to prevent SQL Injection!

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var serviceStatus = new SystemStatus
            {
                Id = reader.GetInt32(0),
                ServiceName = reader.GetString(1),
                Status = reader.GetString(2)
            };
            return Ok(serviceStatus);
        }

        return NotFound(new { Message = $"No service found with ID {id}." });
    }

    /// <summary>
    /// ROUTE: POST /api/status
    /// </summary>
    [HttpPost]
    public IActionResult ReportIncident([FromBody] SystemStatus incomingData)
    {
        // 1. Validation
        if (string.IsNullOrWhiteSpace(incomingData.Status) || string.IsNullOrWhiteSpace(incomingData.ServiceName))
        {
            return BadRequest(new { Message = "You must provide a ServiceName and Status." });
        }

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // 2. Check if it exists by checking if the ID was provided and valid
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = "SELECT Id FROM Statuses WHERE Id = @id";
        checkCmd.Parameters.AddWithValue("@id", incomingData.Id);

        bool exists = checkCmd.ExecuteScalar() != null;
        var cmd = connection.CreateCommand();

        if (exists)
        {
            // UPDATE EXISTING RECORD
            cmd.CommandText = "UPDATE Statuses SET Status = @status WHERE Id = @id";
            cmd.Parameters.AddWithValue("@status", incomingData.Status);
            cmd.Parameters.AddWithValue("@id", incomingData.Id);
            cmd.ExecuteNonQuery();

            return Ok(new { Message = "Service status updated.", Data = incomingData });
        }
        else
        {
            // CREATE NEW RECORD
            cmd.CommandText = "INSERT INTO Statuses (ServiceName, Status) VALUES (@name, @status); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@name", incomingData.ServiceName);
            cmd.Parameters.AddWithValue("@status", incomingData.Status);

            // Get the newly auto-generated ID from SQLite
            long newId = (long)cmd.ExecuteScalar();
            incomingData.Id = (int)newId;

            return CreatedAtAction(nameof(GetStatusById), new { id = incomingData.Id }, incomingData);
        }
    }
}