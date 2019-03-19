using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.IO;
using TBD.Crypto;
using System.Security.Cryptography;

namespace TBD.Server.Behavior {
  public class Server_Authenticate : WebSocketBehavior {
    WebSocket client;
    WSData client_data;
		MessageHandler Handler;

    int tries = 0;

		protected override void OnOpen() {
			client = Context.WebSocket;
			client_data = new WSData();
			Handler = new MessageHandler();

			Handler.Register(WSMessageType.ClientLoginS1, LoginS1);

			WSMessage res = new WSMessage(
				WSMessageType.ClientLoginS1,
				new Dictionary<string, object>{});

      Send(res);
    }

    protected override void OnMessage(MessageEventArgs e) {
			JObject data = e.Data.toJObject();

			WSMessageType type = (WSMessageType)(int)data["type"];
			string argv = data["data"].toJsonString();

			Handler.Handle(type, argv);

		}
	
    void SendToken(bool newToken = false) {
      string token = (newToken || client_data.jwt == "" || client_data.jwt == null) ? GenerateToken() : client_data.jwt;

      var msg = new WSMessage(
        WSMessageType.ClientToken,
        new Dictionary<string, object>{
          { "token", token }
      });

      Send(msg);
    }

    string GenerateToken() {
      client_data.jwt = JWToken.CreateToken(client_data.username);
      return client_data.jwt;
    }
    
    string LoginS1(int l, string data) {
			JObject u = data.toJObject();
      string username = (string)u["username"];
      string password = (string)u["password"];

      client_data.username = username;

			// If username exists

			bool error = false;

			if (!Io.UserExists(username).Result) error = true;
			if (password == "") error = true;

			// Skip authentication for now
			/*if (SHA512Hash.Verify(
				password,
				Io.GetPasswordHash(client_data.username).Result
			))*/

			// Let the user know that he is authenticated

			if (error) {
				// User does not exist
				var msg = new WSMessage(
					WSMessageType.ClientLoginFail,
					new Dictionary<string, object>{
					{"reason", "ERROR_USER_NOT_FOUND"}
				});

				Send(msg);
				return "ERROR_USER_NOT_FOUND";

			}
			LoginSuccess();


			return "SUCCESS";
    }

    void LoginSuccess() {
      tries = 0;

      var msg = new WSMessage(
        WSMessageType.ClientLoginSuccess,
        new Dictionary<string, object>{});
        
      SendToken();
      Send(msg);
    }
  }
}