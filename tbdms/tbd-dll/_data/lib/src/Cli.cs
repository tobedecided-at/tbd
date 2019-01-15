using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using WebSocketSharp;
using WebSocketSharp.Server;

using TBD.WS;
using TBD.Globals;
using TBD.Crypto;
using TBD.Server.Behavior;

namespace TBD.CLI {
  public static class Cli {

    public static void Init() {

      Command.Register("echo", (B_TBDGame b, string[] args) => {
        WebSocket client = b.Context.WebSocket;

        string res = "";

        var sb = new StringBuilder();
        for (int i = 1; i < args.Length; i++) {
          if (i == args.Length - 1)
            sb.Append(args[i]);
          else
            sb.Append(args[i] + " ");
        }
        
        res = sb.ToString();

        return res;
      });

      Command.Register("close", (B_TBDGame b, string[] args) => {
        WebSocket client = b.Context.WebSocket;
        
        client.WriteLine("Closing connection");
        client.Close();
        return "";
      });

      Command.Register("renew", (B_TBDGame b, string[] args) => {
        WebSocket client = b.Context.WebSocket;

        string usage = "Usage: renew [what]";
        string res = usage;

        string username = b.GetClientdata().username;

        if (args.Length < 2) return res;

        switch (args[1]) {
          case "token":
            client.WriteLine("Generating a new Token...");
            return JWToken.CreateToken(username);
          default: break;
        }

        return res;
      });
    }
    
    // Gets user input
    public static string GetInput(bool auth = false) {
      if (auth) { Console.Write(Global.PROMPT_AUTH); }
      else { Console.Write(Global.PROMPT); }

      return Console.ReadLine();
    }

    // Called on the server to run command
    // Can be reused for client usage (maybe useful, idk)
    public static string Handle(B_TBDGame client, string input) {
      return Command.Run(ref client, input);
    }

    // Called on the client to send the command to the server
    public static void WSPrompt(WSClient ws) {
      string input = GetInput();
        
      var msg = new WSMessage(
        WSMessageType.cmd,
        new Dictionary<string, object> {
          {"data", "CLI_REQ"},
          {"cmd", input}
      });
      ws.Send(msg);
    }
  }

  public class Command {
    static Dictionary<string, Func<B_TBDGame, string[], string>> tbd_list = new Dictionary<string, Func<B_TBDGame, string[], string>>();

    public static bool Register(string slug, Func<B_TBDGame, string[], string> action) {
      if (tbd_list.ContainsKey(slug)) {
        return false;
      }

      tbd_list.Add(slug, action);

      return true;
    }

    public static string Run(ref B_TBDGame client, string input) {
      var inputParts = input.Split(' ');
      if (inputParts[0] == string.Empty) return "";
      if (tbd_list.ContainsKey(inputParts[0])) {
        return tbd_list[inputParts[0]](client, inputParts);
      }
      return "The Command \"" + inputParts[0] + "\" was not found";
    }
  }
}