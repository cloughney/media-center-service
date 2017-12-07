using System.Threading;
using System.Threading.Tasks;
using MediaCenterService.Power.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediaCenterService.Power.Controllers
{
	[Route("power"), Produces("application/json")]
    public class PowerController : Controller
    {
		private readonly IPowerStateManager powerStateManager;

		public PowerController(IPowerStateManager powerStateManager)
		{
			this.powerStateManager = powerStateManager;
		}

        [HttpPost]
        public IActionResult Post([FromBody] PowerRequest request)
        {
			if (!this.ModelState.IsValid)
			{
				return this.BadRequest();
			}

			var delayMilliseconds = (request.DelaySeconds ?? 5) * 1000;
			delayMilliseconds = delayMilliseconds > 0 ? delayMilliseconds : 0;

			Task.Run(() => {
				Thread.Sleep(delayMilliseconds);
				this.powerStateManager.SetState(request.State);
			});

			return this.Ok();
        }
    }
}
