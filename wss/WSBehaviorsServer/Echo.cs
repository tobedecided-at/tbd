using WebSocketSharp;
using WebSocketSharp.Server;
using System;

using TBD.WS;

namespace TBD.Server.Behavior {
  public class B_Echo : WebSocketBehavior {
    protected override void OnMessage(MessageEventArgs e) {
      // e.Data == JObject as string
      Console.WriteLine(e.Data); // e.Data = string with valid json
      Send(e.Data);
    }

    protected override void OnOpen() {
      string msg = new { type = "MSG", data = "Welcome to the TBDMS echo service!" }.toJsonString();
      Send(msg);
    }
  }
}