using System;
using System.Text;
using System.IO;

namespace TBD.Utility {
  public static class Utils {
    public static String ReadLineHidden() {
      string output = "";
      
      while (true) {
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
          output += key.KeyChar;
        } else {
          if (key.Key == ConsoleKey.Backspace) {
            if (output.Length > 0)
              output = output.Substring(0, (output.Length - 1));
          } else {
            break;
          }
        }
      }
      Console.WriteLine();
      return output;
    }
    
    public static String Multiply(this string source, int multiplier) {
      StringBuilder sb = new StringBuilder(multiplier * source.Length);
      for (int i = 0; i < multiplier; i++) {
        sb.Append(source);
      }
      return sb.ToString();
    }
  }
}