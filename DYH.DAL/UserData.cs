using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DYH.Core;
using DYH.Data;
using DYH.IDAL;
using DYH.Models;

namespace DYH.DAL
{
    public class UserData : IUser
    {
        private readonly DbConn _conn;
        public UserData(DbConn conn)
        {
            _conn = conn;
        }

        public UserEntry GetUser(int userId)
        {
            return _conn.Database.FirstOrDefault<UserEntry>("WHERE userid = @0", userId);
        }

        public UserEntry GetUser(string userName)
        {
            return _conn.Database.FirstOrDefault<UserEntry>("WHERE username = @0", userName);
        }

        public int Add(UserEntry entry)
        {
            return DataCast.Get<int>(_conn.Database.Insert(entry));
        }

        public int Update(UserEntry entry)
        {
            return _conn.Database.Update(entry);
        }

        public int Delete(UserEntry entry)
        {
            return _conn.Database.Delete(entry);
        }

        public IEnumerable<UserEntry> GetList()
        {
            return _conn.Database.Query<UserEntry>("WHERE 1=1");
        }
    }
}
