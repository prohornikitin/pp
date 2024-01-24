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
        private readonly TheOnlyDbContext context;
        public TasksController(TheOnlyDbContext context)
        {
            this.context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTaskMinimalInfo>>> GetTasks()
        {
            var db = await context.UserTasks.Include(t=>t.InitialMatrix).ToListAsync();
            var converted = db.Select(t => new UserTaskMinimalInfo() {
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
            var task = await context.UserTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return task;
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostTask(NewUserTask request)
        {
            var initialMatrix = await context.Matrices.FindAsync(request.InitialMatrixId);
            if(initialMatrix == null)
            {
                return BadRequest();
            }
            var task = new UserTask {
                Name = request.Name,
                Polynom = request.Polynom,
                InitialMatrix = initialMatrix,
                InitialMatrixId = initialMatrix.Id,
                UnscheduledColumns = new IntRange{
                    Start = 0,
                    End = initialMatrix.Columns,
                },
            };
            await context.UserTasks.AddAsync(task);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new { id = task.Id });
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var task = await context.UserTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (task.State == TaskState.ResultReady)
            {
                System.IO.File.Delete(task.InitialMatrix.FilePath);
            }
            context.UserTasks.Remove(task);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
