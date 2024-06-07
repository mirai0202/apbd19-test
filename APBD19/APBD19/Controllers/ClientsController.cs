using APBD19.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD19.Controllers;

[Route("api/clients")]
[ApiController]
public class ClientsController(MasterContext context) : ControllerBase
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var client = await context.Clients
            .Include(client => client.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == id);
        
        if (client == null)
        {
            return NotFound("Client not found");
        }

        if (client.ClientTrips.Count > 0)
        {
            return BadRequest("Client has trips assigned to him");
        }

        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        
        return Ok("Client was deleted");
    }
}