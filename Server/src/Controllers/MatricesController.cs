using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using MatrixFile;
using Matrix = Server.Models.Matrix;
using Server.JsonModels;


namespace Server.Controllers;

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
    public async Task<ActionResult<IEnumerable<Matrix>>> GetMatricesList()
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
    [HttpPost]
    [Consumes("application/octet-stream")]
    public async Task<ActionResult> PostBinary()
    {
        var path = Path.Join(matricesDir, $"{Guid.NewGuid()}.bin");
        using(Stream from = Request.Body, to = System.IO.File.OpenWrite(path))
        {
            await from.CopyToAsync(to);
        }
        using var file = System.IO.File.OpenRead(path);
        var matrixFile = new MatrixFile.Matrix(path);
        var matrix = new Matrix() { 
            FilePath = matrixFile.FilePath,
            Metadata = matrixFile.Metadata,
            Name = "Unspecified",
        };
        await context.Matrices.AddAsync(matrix);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMatricesList), new { id = matrix.Id }, new { id = matrix.Id });
    }

    // PATCH api/Matrices/{id}
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchMatrix(MatrixPatch patch)
    {
        var matrix = await context.Matrices.FindAsync(patch.Id);
        if (matrix == null)
        {
            return NotFound();
        }

        if (patch.Name == null)
        {
            return Ok();
        }

        matrix.Name = patch.Name;
        await context.SaveChangesAsync();
        return Ok();
    }

    // DELETE: api/Matrices/{id}
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
