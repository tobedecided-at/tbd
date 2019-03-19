using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.Crypto;

using UnityEngine;

namespace TBD.Server.Behavior {

  public class Server_TBDGame : WebSocketBehavior {

    static Dictionary<string, WebSocketBehavior> clientList = new Dictionary<string, WebSocketBehavior>();
    static int countOpenSocket = 0;

    WebSocket client;
    WSData client_data;
    RSACryptoServiceProvider server_keypair;

    public WSData GetClientdata() {
      return client_data;
    }

    public static int GetOpenSocketCount() {
      return countOpenSocket;
    }

    // Initialize settings
    protected override void OnOpen() {
      client = Context.WebSocket;

      client_data = new WSData();
      client_data.id = countOpenSocket;

      clientList.Add(ID, this);
      countOpenSocket++;
    }


    protected override void OnClose(CloseEventArgs e) {
      clientList.Remove(this.ID);
      countOpenSocket--;
    }

		protected override void OnMessage(MessageEventArgs e) {
      
      // TODO: handle() which sanitizes msg and checks if all fields needed are there
      // authentication

      JObject data = e.Data.toJObject();
      if (data == null) {
        client.Close();
        return;
      }

      if (data["type"] == null || data["data"] == null || !e.isValid()) {
        OnInvalidMessage("INVALID!");
        return;
      }

      switch ((string)data["data"]["data"]) {
        case "AUTH_TOKEN":
          string token = (string)data["data"]["token"];
          // If this is a new connection
          if (client_data.jwt == null) {
						bool valid = JWToken.ValidateToken(token);
            if (valid) {
              client_data.jwt = token;

              JObject decodedToken = JWToken.DecodeToken(token);
              client_data.username = (string)decodedToken["sub"];
              
            } else client.Close();
          }
        break;
        /*case "CLI_REQ":
          // It works, why not
          string response = Cli.Handle(this, (string)data["data"]["cmd"]);

          var msg = new WSMessage();
          msg.type = WSMessageType.msg;
          msg.data = new { data = response };
          Send(msg);

          var msg2 = new WSMessage();
          msg2.type = WSMessageType.cmd;
          msg2.data = new { data = "CLI_READY" };
          
          Send(msg2);
        break; */
      }
    }

    void OnInvalidMessage(string reason, bool shouldClose = true) {
      Debug.Log("MSG_INVALID " + reason);
      if (shouldClose) client.Close();
    }
  }
}