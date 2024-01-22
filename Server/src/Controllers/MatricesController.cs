using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrpcServer.Models;
using NuGet.Packaging;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GrpcServer.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatricesController : ControllerBase
    {
        private readonly MatrixContext context;

        public MatricesController(MatrixContext context)
        {
            this.context = context;
        }

        // GET: api/Matrices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Matrix>>> GetMatrix()
        {
            return await context.Matrix.ToListAsync();
        }

        // GET: api/Matrices/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBinary(long id)
        {
            var matrix = await context.Matrix.FindAsync(id);

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
        public async Task<ActionResult<Matrix>> PostBinary()
        {
            var fileName = string.Format(@"{0}.txt", Guid.NewGuid());
            var path = "uploads/" + fileName;
            var matrix = new Matrix() { FilePath = path };
            using(Stream from = Request.Body, to = System.IO.File.OpenWrite(path))
            {
                await from.CopyToAsync(to);
            }
            context.Matrix.Add(matrix);
            await context.SaveChangesAsync();
            return CreatedAtAction("GetMatrix", new { id = matrix.Id }, matrix);
        }

        // DELETE: api/Matrices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatrix(long id)
        {
            var matrix = await context.Matrix.FindAsync(id);
            if (matrix == null)
            {
                return NotFound();
            }

            context.Matrix.Remove(matrix);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
