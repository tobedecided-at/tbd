using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using TBD.Users;

namespace TBD.IO {
  public static class Io {
    static string DataPath = Application.dataPath + "/config/";
    static string PasswdPath = "server/passwd.tbd";
		static string LocalUserdataPath = "userdata.tbd";
    
    /*public static bool Save(List<User> users) {
      FileStream fs = new fileStream(PasswdPath, FileMode.Open, FileAccess.Write);
      
    }*/
    
    public static async Task<List<string>> LoadAsync(string file) {
			using (FileStream fs = new FileStream(DataPath + file, FileMode.Open, FileAccess.Read)) {
				var list = new List<string>();
				using (var sr = new StreamReader(fs, Encoding.UTF8)) {
					string line;
					while (( line = await sr.ReadLineAsync() ) != null) {
						list.Add(line);
					}
				}
				return list;
			}
    }

		public static async Task<Userdata> GetLocalUserdata() {
			string user = (await LoadAsync(LocalUserdataPath) )[0];

			var data = new Userdata {
				Username = user.Split(':')[0],
				PasswordHash = user.Split(':')[1],
				DisplayName = user.Split(':')[2]
			};

			return data;
		}
    
    public static async Task<bool> UserExists(string username) {
      string[] parts;

      foreach (string entry in await LoadAsync(PasswdPath)) {
        parts = entry.Split(':');
        if (parts != null && parts[0] == username)
          return true;
      }
      return false;
    }

    public static async Task<int> GetPermissionLevel(string username) {
      string[] parts;
      foreach (string entry in await LoadAsync(PasswdPath).ConfigureAwait(false)) {
        parts = entry.Split(':');
        if (parts != null && parts[0] == username)
          return int.Parse(parts[3]);
      }
      return 0;
    }
    
    public static async Task<string> GetPasswordHash(string username) {
      string[] parts;
      foreach (string entry in await LoadAsync(PasswdPath).ConfigureAwait(false)) {
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