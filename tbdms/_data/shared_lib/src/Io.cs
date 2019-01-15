using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace TBD.IO {
  public static class Io {
    static string DataPath = "_data\\";
    static string PasswdPath = "passwd.tbd";
    static string ShadowPath = "shadow.tbd";
    
    /*public static bool Save(List<User> users) {
      FileStream fs = new fileStream(PasswdPath, FileMode.Open, FileAccess.Write);
      
    }*/
    
    public static List<string> Load(string file) {
      FileStream fs = new FileStream(DataPath+file, FileMode.Open, FileAccess.Read);
      
      var list = new List<string>();
      using (var sr = new StreamReader(fs, Encoding.UTF8)) {
        string line;
        while ((line = sr.ReadLine()) != null) {
          list.Add(line);
        }
      }
      return list;
    }
    
    public static bool UserExists(string username) {
      string[] parts;
      foreach (string entry in Load(PasswdPath)) {
        parts = entry.Split(':');
        if (parts != null && parts[0] == username)
          return true;
      }
      return false;
    }

    public static int GetClearance(string username) {
      string[] parts;
      foreach (string entry in Load(PasswdPath)) {
        parts = entry.Split(':');
        if (parts != null && parts[0] == username)
          return int.Parse(parts[5]);
      }
      return 0;
    }
    
    public static string GetHash(string username) {
      string[] parts;
      foreach (string entry in Load(ShadowPath)) {
        parts = entry.Split(':');
        if (parts != null && parts[0] == username) {
          string hash = parts[1];
            return hash;
        }
      }
      return "false";
    }
  }
}