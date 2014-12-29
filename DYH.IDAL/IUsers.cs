using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DYH.Models;

namespace DYH.IDAL
{
    public interface IUser
    {
        List<UserEntry> GetList(string condition, int pageSize, int pageIndex, out int records, params object[] args);
        UserEntry GetUser(int userId);
        UserEntry GetUser(string userName);
        int Add(UserEntry entry);
        int Update(UserEntry entry);
        int Delete(UserEntry entry);
    }
}
