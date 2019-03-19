using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TBD.WS {
  public static class WSBehaviorHelper {

    public static string toJsonString(this Object o) {
      return JObject.FromObject(o).ToString();
    }

    public static JObject toJObject(this string input) {
      try {
        return JObject.Parse(input);
      } catch (JsonReaderException) {
        return null;
      }
    }

    public static JObject toJObject(this Object o) {
      return JObject.FromObject(o);
    }

		public static WSData Clean(this WSData wsdata) {
			return new WSData {
				id = wsdata.id,
				username = wsdata.username,
				clearance = wsdata.clearance,
				isAuth = wsdata.isAuth,
				InGame = wsdata.InGame,
				Position = wsdata.Position,
				Rotation = wsdata.Rotation
			};
		}
	}

  public struct WSMessage {
    public WSMessageType type;
    public Dictionary<string, object> data;
    string message;

    public WSMessage(WSMessageType t, Dictionary<string,object> d, string m = null) {
      this.type = t; this.data = d; this.message = m;
    }

    public static implicit operator string(WSMessage wsm) {
      return wsm.message = new { wsm.type, wsm.data }.toJsonString();
    }
  }

	public enum WSMessageType {
		ClientWrite,
		ClientGetID,
		ClientConnected,
		ClientDisconnected,
		ClientLoginS1,
		ClientLoginSuccess,
		ClientLoginFail,
		ClientToken,
		NetworkSyncUpdate,
		NetworkSyncInstantiate,
		NetworkSyncDestroy,
		NetworkSyncClientList,
		NetworkLoadScene,
		NetworkLoadSceneComplete,
		NetworkGetActiveScene,
	}
}