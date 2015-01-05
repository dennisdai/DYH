using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DYH.Core;
using DYH.Data;
using DYH.IDAL;
using DYH.Models;

namespace DYH.DAL
{
    public class ActionRepository : IAction
    {
        private readonly DataProvider _provider;
        public ActionRepository(DataProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<ActionEntry> GetList()
        {
            return _provider.Database.Query<ActionEntry>("");
        }

        public ActionEntry GetById(int id)
        {
            return _provider.Database.FirstOrDefault<ActionEntry>("WHERE actionid = @0", id);
        }

        public ActionEntry GetByCode(string code)
        {
            return _provider.Database.FirstOrDefault<ActionEntry>("WHERE actioncode = @0", code);
        }

        public int Add(ActionEntry entry)
        {
            return DataCast.Get<int>(_provider.Database.Insert(entry));
        }

        public int Update(ActionEntry entry)
        {
            return _provider.Database.Update(entry);
        }

        public int Delete(int id)
        {
            return _provider.Database.Delete("actions", "actionid", null, id);
        }

        public int Delete(string ids)
        {
            int iVal = _provider.Database.Delete<ActionEntry>(Sql.Builder.WhereIn("actionid", ids.Split(',')));
            return iVal;
        }
    }
}
