using System.Collections.Generic;
using System;

namespace globals {
	public class os {
		public static double tick {
			get {
				DateTime epoch = new DateTime(1970, 1, 1);
				DateTime now = DateTime.Now;
				return (now - epoch).TotalMilliseconds;
			}
		}
	}
}