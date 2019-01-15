using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TBD.WS {
  public class WSClient {
    protected WebSocket ws;
    protected WSData data;
    public bool isOpen {get; protected set;}
    public string service {get; protected set;}

    public WSClient() {}

    protected virtual void Init(string serverAddress, int port, string service) {
      this.isOpen = false;

      this.ws = new WebSocket("ws://" + serverAddress + ":" + port + "/" + service);
      
      this.ws.OnOpen += (sender, e) => { OnOpen((WebSocket)sender); };
      this.ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, (MessageEventArgs)e); };
      this.ws.OnError += (sender, e) => { OnError((WebSocket)sender, (ErrorEventArgs)e); };
      this.ws.OnClose += (sender, e) => { OnClose((CloseEventArgs)e); };
      this.ws.Connect();
    }

    public WSClient(string serverAddress, int port, string service) {
      Init(serverAddress, port, service);
    }

    public WebSocket GetSocket() {
      return this.ws;
    }

    #region virtuals

    public virtual void Send(string msg, Action<bool> cb = null) {
      if (ws.IsAlive && ws != null) ws.SendAsync(msg, cb);
    }

    public virtual void Close() {
      if (ws != null) ws.Close();
    }

    protected virtual void OnOpen(WebSocket sender) {
      isOpen = true;
    }
    protected virtual void OnMessage(WebSocket sender, MessageEventArgs msg) {}

    protected virtual void OnClose(CloseEventArgs reason) {
      isOpen = false;
    }

    protected virtual void OnError(WebSocket sender, ErrorEventArgs ex) {
      isOpen = false;
    }

    #endregion
  }

  public class WSServer {

    WebSocketServer wss;

    public WSServer(int port, bool start = false) {
      this.wss = new WebSocketServer(port);

      if (start)
        this.wss.Start();
    }

    public WebSocketServer GetServer() {
      return this.wss;
    }

    public void Start() {
      this.wss.Start();
    }

    public void Stop() {
      if (wss != null)
        this.wss.Stop();
    }
  }
}