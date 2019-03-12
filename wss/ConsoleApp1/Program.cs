using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBD.Networking;

namespace TBD {
	class Program {
		static void Main(string[] args) {
			NetworkingServer.Init();
			NetworkingServer.Start();

			Console.ReadKey();
		}
	}
}
