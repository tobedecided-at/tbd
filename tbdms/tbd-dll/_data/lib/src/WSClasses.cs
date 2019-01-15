using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace TBD.WS {
  public struct WSData {
    public int id;
    public string username;
    public int clearance;
    public string jwt;
    public bool gotToken;
    public bool isAuth;
    public RSACryptoServiceProvider keypair;
    public RSACryptoServiceProvider serverPublicKey;
    public byte[] sessionKey;
  }
}