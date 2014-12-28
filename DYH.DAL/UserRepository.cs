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
    public class UserRepository : IUser
    {
        private readonly DataProvider _provider;
        public UserRepository(DataProvider provider)
        {
            _provider = provider;
        }

        public UserEntry GetUser(int userId)
        {
            return _provider.Database.FirstOrDefault<UserEntry>("WHERE userid = @0", userId);
        }

        public UserEntry GetUser(string userName)
        {
            return _provider.Database.FirstOrDefault<UserEntry>("WHERE username = @0", userName);
        }

        public int Add(UserEntry entry)
        {
            return DataCast.Get<int>(_provider.Database.Insert(entry));
        }

        public int Update(UserEntry entry)
        {
            return _provider.Database.Update(entry);
        }

        public int Delete(UserEntry entry)
        {
            return _provider.Database.Delete(entry);
        }

        public IEnumerable<UserEntry> GetList()
        {
            return _provider.Database.Query<UserEntry>("WHERE 1=1");
        }
    }

    
}
