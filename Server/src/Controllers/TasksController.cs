using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.JsonModels;
using UserTask = Server.Models.UserTask;


namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TheOnlyDbContext db;
        private readonly string matricesDir;
        public TasksController(TheOnlyDbContext context, IConfiguration configuration)
        {
            this.db = context;
            matricesDir = configuration["Custom:MatricesDirectory"]!;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTaskMinimalInfo>>> GetTasks()
        {
            var all = await db.UserTasks.Include(t=>t.InitialMatrix).ToListAsync();
            var converted = all.Select(t => new UserTaskMinimalInfo() {
                Id = t.Id,
                Name = t.Name,
                Polynom = t.Polynom,
                Status = t.State,
            });
            return converted.ToList();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetTask(long id)
        {
            var task = await db.UserTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        [HttpGet("{taskId}/result")]
        public async Task<ActionResult<UserTask>> GetTaskResult(long taskId)
        {
            var task = await db.UserTasks
                .Include(t => t.Result)
                .SingleOrDefaultAsync(t => t.Id == taskId);
            if (task == null || task.State != TaskState.ResultReady)
            {
                return NotFound();
            }
            Stream src = System.IO.File.OpenRead(task.Result.FilePath);
            return File(src, "application/octet-stream");
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostTask(NewUserTask request)
        {
            var initialMatrix = await db.Matrices.FindAsync(request.InitialMatrixId);
            if(initialMatrix == null)
            {
                return BadRequest();
            }
            var resultPath = Path.Join(matricesDir, $"{Guid.NewGuid()}.bin");
            var result = Matrix.EmptyWithMetadata(resultPath, initialMatrix.Metadata);
            await db.Matrices.AddAsync(result);
            var task = new UserTask {
                Name = request.Name,
                Polynom = request.Polynom,
                InitialMatrix = initialMatrix,
                InitialMatrixId = initialMatrix.Id,
                UnscheduledColumns = new IntRange{
                    Start = 0,
                    End = initialMatrix.Metadata.Columns,
                },
                ResultMatrixId = result.Id,
                Result = result,
            };
            await db.UserTasks.AddAsync(task);
            await db.SaveChangesAsync();
            db.NotifyTaskAdded();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new { id = task.Id });
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var task = await db.UserTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (task.State == TaskState.ResultReady)
            {
                System.IO.File.Delete(task.InitialMatrix.FilePath);
            }
            db.UserTasks.Remove(task);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
