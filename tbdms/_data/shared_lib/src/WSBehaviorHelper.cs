using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TBD.WS {
  public static class WSBehaviorHelper {

    // Writes a line on the client screen
    // Implemented for commands, may be misused ;)
    public static void WriteLine(this WebSocket ws, string toWrite) {
      var msg = new WSMessage();
      msg.type = WSMessageType.msg;
      msg.data = new Dictionary<string, object> { {"data", toWrite} };
      if (ws.IsAlive) ws.Send(msg);
    }

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

    public static bool isValid(this MessageEventArgs msg) {
      JObject temp = msg.Data.toJObject();
      if ((temp["type"] == null) ||
          (temp["data"] == null) ||
          (temp["data"]["data"] == null)) {
            return false;
      }

      if (Regex.Match((string)temp["data"]["data"], "LOGIN").Success) {
        return !(temp["data"]["username"] == null || temp["data"]["password"] == null );
      }

      return true;
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
      return wsm.message = new { type = wsm.type, data = wsm.data}.toJsonString();
    }
  }

  public enum WSMessageType {
    msg,
    cmd,
    data,
    encrypted    
  }
}