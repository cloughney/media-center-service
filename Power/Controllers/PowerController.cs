using System.Threading;
using System.Threading.Tasks;
using MediaCenterService.Power.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediaCenterService.Power.Controllers
{
	[Route("power")]
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

			Task.Run(() => {
				Thread.Sleep((request.DelaySeconds ?? 5) * 1000);
				this.powerStateManager.SetState(request.State);
			});

			return this.Ok();
        }
    }
}
