using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TBD.WS {
	public class WSServer {

		WebSocketServer Wss;
		MessageHandler Handler;

		public WSServer(int port, bool start = false) {
			Wss = new WebSocketServer(port);
			Handler = new MessageHandler();

			if (start)
				this.Wss.Start();
		}

		public WebSocketServer GetServer() {
			return Wss;
		}

		public void Start() {
			Wss.Start();
		}

		public void Stop() {
			if (Wss != null)
				Wss.Stop();
		}
	}
}
