using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TBD.Networking {

	public class SVector3 {
		public float x;
		public float y;
		public float z;

		public SVector3(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	public class SQuaternion {
		public float x;
		public float y;
		public float z;
		public float w;

		public SQuaternion(float x, float y, float z, float w) {
			this.x = x; this.y = y; this.z = z; this.w = w;
		}
	}
}
