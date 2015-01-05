using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Models;

namespace DYH.IDAL
{
    public interface IAction
    {
        IEnumerable<ActionEntry> GetList();
        ActionEntry GetById(int id);
        ActionEntry GetByCode(string code);
        int Add(ActionEntry entry);
        int Update(ActionEntry entry);
        int Delete(int id);
        int Delete(string ids);
    }
}
