using System.Runtime.InteropServices;
using MediaCenterService.Power.Models;

namespace MediaCenterService.Power
{
	public class PowerStateManager : IPowerStateManager
    {
		[DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);
		
		public void SetState(PowerState state)
		{
			//SetSuspendState(state == PowerState.Hibernation, true, true);
		}
	}

	public interface IPowerStateManager
	{
		void SetState(PowerState state);
	}
}
