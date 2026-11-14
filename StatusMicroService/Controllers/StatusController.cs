// ==============================================================================
// 3. THE CONTROLLER
// The [ApiController] attribute tells ASP.NET that this class handles API requests.
// The [Route("api/[controller]")] attribute sets the base URL for this whole class.
// Because the class is named "StatusController", the route becomes "/api/status"
// ==============================================================================
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    // A mock in-memory database to demonstrate functionality without needing SQL.
    // We make it static so the data persists while the application is running.
    private static readonly List<SystemStatus> _mockDatabase = new()
    {
        new SystemStatus { Id = 1, ServiceName = "Web Dashboard", Status = "Online" },
        new SystemStatus { Id = 2, ServiceName = "Payment Gateway", Status = "Degraded" },
        new SystemStatus { Id = 3, ServiceName = "Email Server", Status = "Online" }
    };

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
        return Ok(_mockDatabase);
    }

    /// <summary>
    /// ROUTE: GET /api/status/{id}
    /// The "{id}" in the attribute means this route expects a variable at the end of the URL.
    /// Example: A request to /api/status/2 will execute this method and pass '2' into the id parameter.
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetStatusById(int id)
    {
        // Search our mock database for a service matching the requested ID
        var serviceStatus = _mockDatabase.FirstOrDefault(s => s.Id == id);

        // Defensive Programming: What if they ask for an ID that doesn't exist?
        if (serviceStatus == null)
        {
            // Return an HTTP 404 (Not Found) with a helpful error message
            return NotFound(new { Message = $"No service found with ID {id}." });
        }

        // If found, return it as JSON with an HTTP 200 code
        return Ok(serviceStatus);
    }

    /// <summary>
    /// ROUTE: POST /api/status
    /// The [HttpPost] attribute dictates this method is for CREATING or UPDATING data.
    /// The [FromBody] attribute tells ASP.NET to look inside the incoming HTTP Request body, 
    /// read the JSON, and convert it into our SystemStatus C# object.
    /// </summary>
    [HttpPost]
    public IActionResult ReportIncident([FromBody] SystemStatus incomingData)
    {
        // 1. Validation: Ensure they provided a status
        if (string.IsNullOrWhiteSpace(incomingData.Status))
        {
            // Return an HTTP 400 (Bad Request) if the data is invalid
            return BadRequest(new { Message = "You must provide a status state (e.g., Offline)." });
        }

        // 2. Logic: Check if the service already exists
        var existingService = _mockDatabase.FirstOrDefault(s => s.Id == incomingData.Id);

        if (existingService != null)
        {
            // If it exists, update it
            existingService.Status = incomingData.Status;
            return Ok(new { Message = "Service status updated.", Data = existingService });
        }
        else
        {
            // If it doesn't exist, generate a new ID and add it to our mock database
            incomingData.Id = _mockDatabase.Max(s => s.Id) + 1;
            _mockDatabase.Add(incomingData);

            // CreatedAtAction returns an HTTP 201 (Created) and tells the client
            // how to find the newly created resource via the GetStatusById method.
            return CreatedAtAction(nameof(GetStatusById), new { id = incomingData.Id }, incomingData);
        }
    }
}