using System;
using System.Text;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

using TBD.WS;
using TBD.CLI;
using TBD.Server.Behavior;

namespace TBD.Server {
  class Tbd {
    static void Main(string[] args) {
      Console.OutputEncoding = Encoding.UTF8;

      Cli.Init();
      
      var wss = new WSServer(710, true);
      wss.GetServer().AddWebSocketService<B_Echo>("/echo");
      wss.GetServer().AddWebSocketService<B_TBDGame>("/tbd_game");
      wss.GetServer().AddWebSocketService<B_Auth>("/auth");
      
      Console.WriteLine("Server listening on 710 and providing:");
      foreach (var path in wss.GetServer().WebSocketServices.Paths) {
        Console.WriteLine(path);
      }

      Console.ReadLine();

      wss.Stop();
    }
  }
}

