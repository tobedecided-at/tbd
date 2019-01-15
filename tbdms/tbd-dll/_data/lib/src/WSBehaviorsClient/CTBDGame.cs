using System;
using System.Security;

using System.Collections.Generic;
using System.Security.Cryptography;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.Users;
using TBD.CLI;
using TBD.Crypto;

namespace TBD.Client.Behavior {
  public class CTBDGame : WSClient {
    public CTBDGame(string serverAddress, int port, string token) {

      this.service = service;
      this.data = new WSData();
      
      this.ws = new WebSocket("ws://" + serverAddress + ":" + port + "/tbd_game");
      this.isOpen = false;
      
      this.ws.OnOpen += (sender, e) => { OnOpen((WebSocket)sender); };
      this.ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, (MessageEventArgs)e); };
      this.ws.OnError += (sender, e) => { OnError((WebSocket)sender, (ErrorEventArgs)e); };
      this.ws.OnClose += (sender, e) => { OnClose((CloseEventArgs)e); };

      this.data.jwt = token;
      this.data.serverPublicKey = RsaCrypto.Generate(0, false);

      // Connect last, else there may be a race condition
      this.ws.Connect();
    }

    protected override void OnMessage(WebSocket sender, MessageEventArgs msg) {
      // TODO:
      // Whole function has to be cleaned up

      JObject data = msg.Data.toJObject();
      #if SHOW
        Console.WriteLine(msg.Data);
      #endif
      
      if ((int)data["type"] == (int)WSMessageType.msg) {
        Console.WriteLine((string)data["data"]["data"]);
        return;
      }
      
      switch ((string)data["data"]["data"]) {
        case "RQ_PW":
          User.GetPassword(this.data.username);
        break;
        case "CLI_READY":
          Cli.WSPrompt(this);
        break;
        case "HS1":
        {
          int bytes = (int)data["data"]["bytes"];

          // Import servers public key (RSA)
          string x_serverPublicKey = (string)data["data"]["pubkey"];
          try {
            this.data.serverPublicKey.FromXmlString(x_serverPublicKey);
          } catch (Exception) {
            ws.Close();
            return;
          }

          // Generate client key (RSA)
          this.data.keypair = RsaCrypto.Generate(bytes);


          // Client public key
          string x_clientPublicKey = this.data.keypair.ToXmlString(false);

          var res = new WSMessage(
            WSMessageType.cmd,
            new Dictionary<string, object> {
              {"data", "HS1"},
              {"pubkey", x_clientPublicKey}
            }
          );

          Send(res);
        }
        break;
        case "HS2":
        {
          int type = (int)data["type"];

          if (type != (int)WSMessageType.encrypted) throw new FormatException();

          byte[] e_sessionKey = (byte[])data["data"]["__sessionKey"];
          this.data.sessionKey = this.data.keypair.Decrypt(e_sessionKey, true);

          {
            var res = new WSMessage(
              WSMessageType.cmd,
              new Dictionary<string, object> {
                { "data", "HS2" },
                { "__sessionKey", e_sessionKey }
              }
            );
            Send(res);
          }
        }
        break;
      }
    }

    protected override void OnOpen(WebSocket sender) {
      this.isOpen = true;
      
      if (JWToken.ValidateToken(this.data.jwt)) {
        var msg = new WSMessage();
        msg.type = WSMessageType.data;
        msg.data = new Dictionary<string, object> {
          {"data", "AUTH_TOKEN"},
          {"token", data.jwt}
        };

        Send(msg);
      } else Close();
    }
  }
}