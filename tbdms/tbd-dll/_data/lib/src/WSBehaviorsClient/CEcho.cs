using System;
using System.Collections.Generic;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;

namespace TBD.Client.Behavior {
  public class CB_Echo : WSClient {
    public CB_Echo(string serverAddress, int port) {

      this.service = service;
      this.ws = new WebSocketSharp.WebSocket("ws://" + serverAddress + ":" + port + "/echo");
      this.isOpen = false;
      
      this.ws.OnOpen += (sender, e) => { OnOpen((WebSocketSharp.WebSocket)sender); };
      this.ws.OnMessage += (sender, e) => { OnMessage((WebSocketSharp.WebSocket)sender, (MessageEventArgs)e); };
      this.ws.OnError += (sender, e) => { base.OnError((WebSocketSharp.WebSocket)sender, (ErrorEventArgs)e); };
      this.ws.Connect();
    }

    protected override void OnMessage(WebSocketSharp.WebSocket sender, MessageEventArgs msg) {
      JObject data = msg.Data.toJObject();

      Console.WriteLine(data["data"]);
      Console.Write("$ ");

      string input = Console.ReadLine();

      var res = new WSMessage(
        WSMessageType.msg,
        new Dictionary<string, object>{
          { "data", input }
      });

      if (input == "exit") {
        Close();
        Environment.Exit(1);
      }

      Send(res);
    }

    protected override void OnOpen(WebSocketSharp.WebSocket sender) {
      this.isOpen = true;
    }
  }
}