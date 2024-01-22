using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrpcServer.Models;
using UserTask = GrpcServer.Models.UserTask;

namespace GrpcServer.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly UserTaskContext context;

        public TasksController(UserTaskContext context)
        {
            this.context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks()
        {
            return await context.Tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTask>> GetTask(long id)
        {
            var task = await context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserTask>> PostTask(UserTask task)
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var task = await context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            if (task.State == UserTaskState.ResultReady)
            {
                System.IO.File.Delete(task.inputMatrix.FilePath);
            }
            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
