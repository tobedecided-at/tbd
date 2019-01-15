using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.Users;
using TBD.Crypto;
using TBD.CLI;

namespace TBD.Client.Behavior {
  public class CBAuth : WSClient {
    public CBAuth(string serverAddress, int port) {

      this.service = service;
      this.data = new WSData();

      this.ws = new WebSocket("ws://" + serverAddress + ":" + port + "/auth");
      this.isOpen = false;
      
      this.ws.OnOpen += (sender, e) => { base.OnOpen((WebSocket)sender); };
      this.ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, (MessageEventArgs)e); };
      this.ws.OnError += (sender, e) => { base.OnError((WebSocket)sender, (ErrorEventArgs)e); };
      this.ws.OnClose += (sender, e) => { base.OnClose((CloseEventArgs)e); };
      this.ws.Connect();
    }

    protected override void OnMessage(WebSocketSharp.WebSocket sender, MessageEventArgs msg) {
      JObject data = msg.Data.toJObject();

      switch ((string)data["data"]["data"]) {
        case "LOGIN_INIT":
          LoginS1();
        break;
        case "LOGIN_S2":
          LoginS2();
        break;
        case "LOGIN_FAIL":
          Console.WriteLine("Too many attempts, blocking for 1 min");
          LoginS2();
        break;
        case "LOGIN_FAIL_USER":
          Console.WriteLine("User does not exist!");
          LoginS1();
        break;
        case "LOGIN_FAIL_PASS":
          Console.WriteLine("Incorrect password");
          LoginS2();
        break;
        case "LOGIN_SUCCESS":
          LoginSuccess();
        break;
        case "RQ_PW":
          GetPW();
        break;
        case "AUTH_TOKEN":
          HandleToken(data["data"]);
        break;
      }
    }

    void HandleToken(JToken data) {

      this.data.jwt = (string)data["token"];
      JObject tokenObject = JWToken.DecodeToken(this.data.jwt);
      
      switch ((int)tokenObject["state"]) {
        case (int)JWToken.JWTErrorCode.Expired:
          // TODO: Request new token, shouldn't happen tho
          // HACK
          this.Close();
        break;
        case (int)JWToken.JWTErrorCode.Tampered:
          this.Close();
        break;
        case (int)JWToken.JWTErrorCode.Valid:
          this.data.gotToken = true;
        break;
      }
    } 

    void GetPW() {
      var res = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "CLIENT_PW"},
          {"password", User.GetPassword(this.data.username)}
      });
      Send(res);
    }

    void LoginS1() {
      WSMessage res = new WSMessage();
      res.type = WSMessageType.cmd;
      #if DEBUG
        res.data = new Dictionary<string, object>{
          {"data", "LOGIN_S1"},
          {"username", "gabe"},
          {"password", "asdf"}};
      #else
        res.data = new Dictionary<string, object>{
          {"data", "LOGIN_S1"},
          {"username", "gabe"},
          {"password", ""}};
      #endif

      this.data.username = (string)res.data["username"];

      Send(res);
    }

    void LoginS2() {
      WSMessage res = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "LOGIN_S2"},
          {"username", this.data.username},
          {"password", User.GetPassword(this.data.username)}
      });

      Send(res);
    }

    void LoginSuccess() {
      // Local representation, NOT SAFE TO USE
      // USE GetAuthStatus() instead
      // TODO: Not implemented
      data.isAuth = true;
    }

    void RequestTokenFromServer() {

      var res = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "REQ_AUTH_TOKEN"},
          {"newToken", false}}
      );

      Send(res);
    }

    public async Task<string> GetToken() {
      if (!this.data.gotToken)
        RequestTokenFromServer();

      while (!this.data.gotToken) {
        await Task.Delay(10);
      }

      return this.data.jwt;
    }
  }
}