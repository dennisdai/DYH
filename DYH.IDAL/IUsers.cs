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
        List<UserEntry> GetList(PageModel model);
        UserEntry GetUserById(int userId);
        UserEntry GetUserByName(string userName);
        UserEntry GetUserByEmail(string email);

        int Add(UserEntry entry);
        int Update(UserEntry entry);
        int Delete(int id);
        int Delete(string ids);

    }
}
