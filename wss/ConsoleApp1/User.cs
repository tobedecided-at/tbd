using System;
using System.Collections.Generic;

using TBD.Utility;

namespace TBD.Users {
  public class User {
    public static Object AskUsernamePass() {
      
      string username = GetUsername();
      string password = GetPassword(username);

      var ret = new {
				username,
        password
      };        
      return ret;
    }

    public static string GetUsername() {
      Console.Write("username: ");
      string username = Console.ReadLine();

      if (username.Length > 30) username = username.Substring(0, 30);
      return username;
    }
    
    public static string GetPassword(string username) {
      Console.Write("password for {0}: ", username);
      return Utils.ReadLineHidden();
    }
	}

	public struct Userdata {
		public string Username;
		public string PasswordHash;
		public string DisplayName;
	}
}