using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TBD.WS {
  public class WSClient {
    protected WebSocket Ws;
    protected WSData Data;
		protected MessageHandler Handler;

    public bool IsOpen {get; protected set;}
    public string Service {get; protected set;}

    public WSClient() {}

    protected virtual void Init(string serverAddress, int port, string service) {
			IsOpen = false;

      Ws = new WebSocket("ws://" + serverAddress + ":" + port + "/" + service);
			Handler = new MessageHandler();

      Ws.OnOpen += (sender, e) => { OnOpen((WebSocket)sender); };
      Ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, e); };
      Ws.OnError += (sender, e) => { OnError((WebSocket)sender, e); };
      Ws.OnClose += (sender, e) => { OnClose(e); };
      Ws.Connect();
    }

    public WSClient(string serverAddress, int port, string service) {
      Init(serverAddress, port, service);
    }

    public WebSocket GetSocket() {
      return this.Ws;
    }

    #region virtuals

    public virtual void SendAsync(string msg, Action<bool> cb = null) {
      if (Ws.IsAlive && Ws != null) Ws.SendAsync(msg, cb);
    }

    public virtual void Close() {
      if (Ws != null) Ws.Close();
    }

    protected virtual void OnOpen(WebSocket sender) {
      IsOpen = true;
    }
    protected virtual void OnMessage(WebSocket sender, MessageEventArgs msg) {}

    protected virtual void OnClose(CloseEventArgs reason) {
      IsOpen = false;
    }

    protected virtual void OnError(WebSocket sender, ErrorEventArgs ex) {
      IsOpen = false;
    }

    #endregion
  }
}