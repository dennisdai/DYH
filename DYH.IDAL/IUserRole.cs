using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Models;

namespace DYH.IDAL
{
    public interface IUserRole
    {
        IEnumerable<UserRoleEntry> GetList();
        int Add(UserRoleEntry entry);
        int Add(IEnumerable<UserRoleEntry> list);
        int Update(UserRoleEntry entry);
        int Update(IEnumerable<UserRoleEntry> list);
    }
}
