using System;
using System.ComponentModel.DataAnnotations;

namespace MediaCenterService.Power.Models
{
	public class PowerRequest
	{
		/// <summary>
		/// The requested power state.
		/// </summary>
		[Required]
		public PowerState State { get; set; }

		/// <summary>
		/// Gets or sets the delay before performing the power state change.
		/// </summary>
		public Int32? DelaySeconds { get; set; }
	}
}
