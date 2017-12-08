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

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetActiveWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

		[DllImport("user32.dll")]
		private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		private static readonly int SW_MAXIMIZE = 3;

		[DllImport("kernel32.dll")]
		private static extern uint GetCurrentThreadId();

		private delegate bool WindowEnumCallback(int hwnd, int lparam);

		private readonly IDictionary<String, Application> applicationsByMoniker;

		public ApplicationManager()
		{
			this.applicationsByMoniker = new Dictionary<String, Application>
			{
				{ "chrome", new Application { ProcessName = @"chrome" } },
				{ "browser", new Application { ProcessName = @"chrome" } },
				{ "kodi", new Application { ProcessName = @"kodi" } }
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
			if (!this.applicationsByMoniker.TryGetValue(moniker, out Application application))
			{
				return null;
			}

			var windowHandles = this.GetVisibleWindows();

			uint appThreadId = 0;
			var appWindowHandle = IntPtr.Zero;

			foreach (var hWnd in windowHandles)
			{
				appThreadId = GetWindowThreadProcessId(hWnd, out uint processId);
				var process = Process.GetProcessById((int)processId); // hrm
				
				if (String.Equals(process.ProcessName, application.ProcessName, StringComparison.OrdinalIgnoreCase))
				{
					appWindowHandle = hWnd;
					break;
				}
			}

			if (appThreadId == 0)
			{
				return null;
			}

			var currentThreadId = GetCurrentThreadId();
			var isCrossThread = currentThreadId != appThreadId;

			if (isCrossThread)
			{
				AttachThreadInput(currentThreadId, appThreadId, true);
			}

			ShowWindowAsync(appWindowHandle, SW_MAXIMIZE);
			SetForegroundWindow(appWindowHandle);
			SetActiveWindow(appWindowHandle);

			if (currentThreadId != appThreadId)
			{
				AttachThreadInput(currentThreadId, appThreadId, false);
			}

			return null;
		}

		private IList<IntPtr> GetVisibleWindows()
		{
			var windowHandles = new List<IntPtr>();
			EnumWindows(new WindowEnumCallback((hWnd, lParam) =>
			{
				var hWndPtr = new IntPtr(hWnd);
				var titleLength = GetWindowTextLength(hWndPtr);
				if (titleLength > 0 && IsWindowVisible(hWnd))
				{
					windowHandles.Add(hWndPtr);
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
