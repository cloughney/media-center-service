using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaCenterService.Applications
{
	public interface IApplicationManager
	{
		Application Start(string moniker, object options);

		Application Stop(string moniker, object options);

		Application Switch(string moniker, object options);
	}

	public class ApplicationManager : IApplicationManager
    {
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumWindows(WindowEnumCallback lpEnumFunc, int lParam);

		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(int h);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		
		[DllImport("user32.dll")]
		private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		private delegate bool WindowEnumCallback(int hwnd, int lparam);

		private readonly IDictionary<String, Application> applicationsByMoniker;

		public ApplicationManager()
		{
			this.applicationsByMoniker = new Dictionary<String, Application>
			{
				{ "chrome", new Application { ProcessName = @"chrome" } }
			};
		}

		public Application Start(string moniker, object options)
		{
			throw new NotImplementedException();
		}

		public Application Stop(string moniker, object options)
		{
			throw new NotImplementedException();
		}

		public Application Switch(string moniker, object options)
		{
			Application application;
			if (!this.applicationsByMoniker.TryGetValue(moniker, out application))
			{
				return null;
			}

			var windowHandles = this.GetVisibleWindows();
			var appProcess = Process.GetProcesses()
				.Where(x => windowHandles.Contains(x.Handle))
				.SingleOrDefault(x => String.Equals(application.ProcessName, x.ProcessName, StringComparison.OrdinalIgnoreCase));

			var currentThreadId = GetCurrentThreadId();
			var otherThreadId = GetWindowThreadProcessId(appProcess.Handle, new IntPtr(appProcess.Id));
			var isCrossThread = currentThreadId != otherThreadId;

			if (isCrossThread)
			{
				AttachThreadInput(currentThreadId, otherThreadId, true);
			}
			
			SetActiveWindow(appProcess.Handle);

			if (currentThreadId != otherThreadId)
			{
				AttachThreadInput(currentThreadId, otherThreadId, false);
			}

			return null;
		}

		private IList<IntPtr> GetVisibleWindows()
		{
			var windowHandles = new List<IntPtr>();
			EnumWindows(new WindowEnumCallback((hWnd, lParam) =>
			{
				if (IsWindowVisible(hWnd))
				{
					windowHandles.Add(new IntPtr(hWnd));
				}

				return true;
			}), 0);

			return windowHandles;
		}
	}

	public class Application
	{
		public String ProcessName { get; set; }
	}
}
