using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

using TBD.WS;
using TBD.Client.Behavior;

namespace TBD.Client {
  class Tbd {
    static void Main(string[] args) {
      Console.OutputEncoding = Encoding.UTF8;

      var authSocket = new CBAuth("localhost", 710);
      
      // "Block" until we get a token
      var token = Task.Run( async () =>
        await authSocket.GetToken().ConfigureAwait(false)).Result;

      Console.WriteLine("Got token, connecting to TBD...");

      // Got a token, use it to connect to /tbd_game endpoint
      var gameSocket = new CTBDGame("localhost", 710, token);

      while (true) {};
    }
  }
}