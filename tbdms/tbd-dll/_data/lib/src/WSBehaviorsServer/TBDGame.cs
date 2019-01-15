using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.CLI;
using TBD.Crypto;

namespace TBD.Server.Behavior {

  public class B_TBDGame : WebSocketBehavior {

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
      client_data.keypair = RsaCrypto.Generate(0, false);

      clientList.Add(this.ID, this);
      countOpenSocket++;

      GenerateAndSendKeypair(4096);
    }

    void GenerateAndSendKeypair(int bytes) {
      // Generate server key (RSA)
      server_keypair = RsaCrypto.Generate(bytes);

      string x_serverPublicKey = server_keypair.ToXmlString(false);
      
      // Send public key
      var msg = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object> {
          {"data", "HS1"},
          {"pubkey", x_serverPublicKey},
          {"bytes", bytes}
      });

      Send(msg);
    }

    protected override void OnClose(CloseEventArgs e) {
      clientList.Remove(this.ID);
      countOpenSocket--;
    }

    protected override void OnMessage(MessageEventArgs e) {
      
      // TODO: handle() which sanitizes msg and checks if all fields needed are there
      // authentication

      #if SHOW
        Console.WriteLine(e.Data);
      #endif

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
              // TODO: Integrate into Unity project
              client_data.jwt = token;

              JObject decodedToken = JWToken.DecodeToken(token);
              client_data.username = ((string)decodedToken["sub"]);
              
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
        case "HS1":
          // Import client public key (RSA)
          try {
            client_data.keypair.FromXmlString((string)data["data"]["pubkey"]);
          } catch (Exception) {
            client.Close();
            return;
          }

          byte[] sessionKey = new byte[256];
          new RNGCryptoServiceProvider().GetBytes(sessionKey);

          var e_sessionkey = client_data.keypair.Encrypt(sessionKey, true);          

          {
            var res = new WSMessage(
              WSMessageType.encrypted,
              new Dictionary<string, object>{
                {"data", "HS2"},
                {"__sessionKey", e_sessionkey}
            });
            Send(res);
          }

        break;
      }
    }

    void OnInvalidMessage(string reason, bool shouldClose = true) {
      Console.ForegroundColor = ConsoleColor.Red;

      Console.WriteLine("MSG_INVALID " + reason);
      
      Console.ForegroundColor = ConsoleColor.Gray;
      if (shouldClose) client.Close();
    }
  }
}