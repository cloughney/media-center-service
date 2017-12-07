using Microsoft.AspNetCore.Mvc;

namespace MediaCenterService.Applications.Controllers
{
	[Route("applications"), Produces("application/json")]
    public class ApplicationsController : Controller
    {
		private readonly IApplicationManager applicationManager;

		public ApplicationsController(IApplicationManager applicationManager)
		{
			this.applicationManager = applicationManager;
		}

		[HttpPost("start")]
		public IActionResult Start(StartRequest request)
		{
			if (!this.ModelState.IsValid)
			{
				return this.BadRequest();
			}

			return this.Ok();
		}

		[HttpPost("stop")]
		public IActionResult Stop(StopRequest request)
		{
			if (!this.ModelState.IsValid)
			{
				return this.BadRequest();
			}

			return this.Ok();
		}

		[HttpPost("switch")]
		public IActionResult Switch(SwitchRequest request)
		{
			if (!this.ModelState.IsValid)
			{
				return this.BadRequest();
			}

			var application =  this.applicationManager.Switch("chrome", null);

			return this.Ok();
		}
	}

	public class StartRequest
	{

	}

	public class StopRequest
	{

	}

	public class SwitchRequest
	{

	}
}