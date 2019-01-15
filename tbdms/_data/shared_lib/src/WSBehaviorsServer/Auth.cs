using System;
using System.Collections.Generic;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.IO;
using TBD.Crypto;

namespace TBD.Server.Behavior {
  public class B_Auth : WebSocketBehavior {
    WebSocket client;
    WSData client_data;

    int tries = 0;
    
    protected override void OnOpen() {
      client = Context.WebSocket;
      client_data = new WSData();

      WSMessage res = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "LOGIN_INIT"}
      });

      Send(res);
    }

    protected override void OnMessage(MessageEventArgs e) {

      #if SHOW
        Console.WriteLine(e.Data);
      #endif

      if (!e.isValid()) {
        Console.WriteLine("MSG_INVALID");
        return;
      }

      JObject data = e.Data.toJObject();

      if (tries >= 5) {
        var msg = new WSMessage(
          WSMessageType.cmd,
          new Dictionary<string, object>{
            {"data", "LOGIN_FAIL"}
        });
        Send(msg);
        client.Close();
        return;
      }

      switch ((string)data["data"]["data"]) {
        case "LOGIN_S1":
          LoginS1(data);
        break;
        case "LOGIN_S2":
          LoginS2(data);
        break;
        case "REQ_AUTH_TOKEN":
          // TODO: Request user password again, for security reasons
          // OR check old token for validity, verify client
          if (client_data.isAuth) SendToken((bool)data["data"]["newToken"]);
        break;
      }
    }

    void SendToken(bool newToken = false) {
      string token = (newToken || client_data.jwt == "" || client_data.jwt == null) ? GenerateToken() : client_data.jwt;

      var msg = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          { "data", "AUTH_TOKEN" },
          { "token", token }
      });

      Send(msg);
    }

    string GenerateToken() {
      client_data.jwt = JWToken.CreateToken(client_data.username);
      return client_data.jwt;
    }
    
    void LoginS1(JObject data) {
      JToken userData = data["data"];
      string username = (string)userData["username"];
      string password = (string)userData["password"];

      client_data.username = username;

      // If username exists
      if (Io.UserExists(username)) {
        if (password != "") {
          tries++;
          LoginS2(new { data = new { password = password }}.toJsonString().toJObject());
          return;
        }

        var msg = new WSMessage(
          WSMessageType.cmd,
          new Dictionary<string, object>{
            {"data", "LOGIN_S2"}
        });
        Send(msg);

      } else { // User does not exist
        var msg = new WSMessage(
          WSMessageType.cmd,
          new Dictionary<string, object>{
            {"data", "LOGIN_FAIL_USER"}
        });

        Send(msg);
      }
    }

    void LoginS2(JObject data) {
      JToken userData = data["data"];

      // If the password is correct
      if (SHA512Hash.Verify((string)userData["password"], Io.GetHash(client_data.username))) {
        // Let the user know that he is authenticated
        LoginSuccess();
      } else {
        tries++;
        var msg = new WSMessage(
          WSMessageType.cmd,
          new Dictionary<string, object>{
            {"data", "LOGIN_FAIL_PASS"}
        });

        Send(msg);
      }
    }

    void LoginSuccess() {
      tries = 0;

      var msg = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "LOGIN_SUCCESS"}
      });
        
      SendToken();
      Send(msg);
    }

    void RequestPassword() {
      var msg = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object>{
          {"data", "RQ_PW"}
      });
      
      Send(msg);
    }
  }
}