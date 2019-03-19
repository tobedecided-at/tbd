using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using TBD.Networking;


namespace TBD.WS {

	public struct WSData {
		public string id;
		public string username;
		public int clearance;
		public string jwt;
		public bool gotToken;
		public bool isAuth;
		public byte[] sessionKey;

		public string ActiveSceneName;
		public bool InGame;

		public SVector3 Position;
		public SQuaternion Rotation;
	}
}