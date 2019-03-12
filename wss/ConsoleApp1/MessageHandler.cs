using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBD.WS {
	public class MessageHandler {
		Dictionary<WSMessageType, Func<int, string, string>> Messages = new Dictionary<WSMessageType, Func<int, string, string>>();

		public bool Register(WSMessageType type, Func<int, string, string> method) {
			if (method == null) return false;

			Func<int, string, string> foundMethod;

			if (Messages.TryGetValue(type, out foundMethod)) {
				// Already defined a handler for the message, abort
				return false;
			}

			Messages.Add(type, method);
			return true;
		}

		public bool Unregister(WSMessageType type) {
			foreach (var entry in Messages) {
				if (entry.Key == type) {
					Messages.Remove(entry.Key);
					return true;
				}
			}
			// No handler for message defined
			return false;
		}

		public string Handle(WSMessageType type, string argv) {
			Func<int, string, string> foundMethod;
			if (Messages.TryGetValue(type, out foundMethod)) {
				return foundMethod(argv.Length, argv);
			}
			return "ERROR_NO_HANDLER_FOUND: " + type;
		}
	}
}
