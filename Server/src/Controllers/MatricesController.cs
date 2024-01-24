using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using MatrixFile;
using Matrix = Server.Models.Matrix;


namespace Server.src.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MatricesController : ControllerBase
{
    private readonly TheOnlyDbContext context;
    private readonly string matricesDir;

    public MatricesController(TheOnlyDbContext context, IConfiguration configuration)
    {
        this.context = context;
        matricesDir = configuration["Custom:MatricesDirectory"]!;
    }

    // GET: api/Matrices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Matrix>>> GetMatrix()
    {
        return await context.Matrices.ToListAsync();
    }

    // GET: api/Matrices/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBinary(long id)
    {
        var matrix = await context.Matrices.FindAsync(id);

        if (matrix == null)
        {
            return NotFound();
        }
        Stream src = System.IO.File.OpenRead(matrix.FilePath);
        return File(src, "application/octet-stream", matrix.FilePath);
    }

    // POST: api/Matrices
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Consumes("application/octet-stream")]
    public async Task<ActionResult> PostBinary()
    {
        var path = Path.Join(matricesDir, $"{Guid.NewGuid()}.txt");
        using(Stream from = Request.Body, to = System.IO.File.OpenWrite(path))
        {
            await from.CopyToAsync(to);
        }
        using var file = System.IO.File.OpenRead(path);
        var matrix = new Matrix() { 
            FilePath = path,
            Columns = Metadata.ReadFrom(file).Columns,
        };
        await context.Matrices.AddAsync(matrix);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMatrix), new { id = matrix.Id }, new { id = matrix.Id });
    }

    // DELETE: api/Matrices/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMatrix(long id)
    {
        var matrix = await context.Matrices.FindAsync(id);
        if (matrix == null)
        {
            return NotFound();
        }

        context.Matrices.Remove(matrix);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
