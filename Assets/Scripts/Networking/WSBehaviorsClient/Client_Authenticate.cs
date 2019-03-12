using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using WebSocketSharp;
using WebSocketSharp.Server;

using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.IO;
using TBD.Users;
using TBD.Crypto;
using TBD.Networking;

using UnityEngine;

namespace TBD.Client.WSBehaviorsClient {
	public class Client_Authenticate : WSClient {

		Userdata userdata;
		WSClient client;

		Task<Userdata> GetUserdata;

		public TaskCompletionSource<string> AuthToken;
		public List<string> Errors = new List<string>();

		public Client_Authenticate(string _ip, int _port) {
			Data = new WSData();
			Handler = new MessageHandler();
			AuthToken = new TaskCompletionSource<string>();

			Ws = new WebSocket("ws://" + _ip + ":" + _port + "/authenticate");
			IsOpen = false;

			Ws.OnOpen += (sender, e) => { OnOpen((WebSocket)sender); };
			Ws.OnMessage += (sender, e) => { OnMessage((WebSocket)sender, e); };
			Ws.OnError += (sender, e) => { base.OnError((WebSocket)sender, e); };
			Ws.OnClose += (sender, e) => { base.OnClose(e); };
			Ws.Connect();
		}

		protected override void OnOpen(WebSocket sender) {
			GetUserdata = Io.GetLocalUserdata();

			Handler.Register(WSMessageType.ClientWrite, TBDNetworking.instance.OnClientWrite);
			Handler.Register(WSMessageType.ClientLoginS1, Login);
			Handler.Register(WSMessageType.ClientToken, OnRcvToken);

		}

		protected override void OnMessage(WebSocket sender, MessageEventArgs e) {

			JObject data = e.Data.toJObject();

			WSMessageType type = (WSMessageType)(int)data["type"];
			string argv = data["data"].toJsonString();

			UnityThread.executeInFixedUpdate(() => {
				Handler.Handle(type, argv);
			});
			
		}

		string OnRcvToken(int c, string v) {
			JObject u = v.toJObject();

			AuthToken.TrySetResult((string)u["token"] ?? "");
			return "SUCCESS";
		}

		string Login(int c, string v) {
			userdata = GetUserdata.Result;

			var msg = new WSMessage(WSMessageType.ClientLoginS1,
				new Dictionary<string, object> {
					{ "username", userdata.Username },
					{ "password", userdata.PasswordHash },
				});

			Ws.Send(msg);
			return "SUCCESS";
		}
	}
}
