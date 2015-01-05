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
    public class ModuleRepository : IModule
    {
        private readonly DataProvider _provider;
        public ModuleRepository(DataProvider provider)
        {
            _provider = provider;
        }

        public ModuleEntry GetByCode(string moduleCode)
        {
            return _provider.Database.FirstOrDefault<ModuleEntry>("WHERE modulecode = @0", moduleCode);
        }

        public ModuleEntry GetById(int moduleId)
        {
            return _provider.Database.FirstOrDefault<ModuleEntry>("WHERE moduleid = @0", moduleId);
        }

        public IEnumerable<ModuleEntry> GetList()
        {
            return _provider.Database.Query<ModuleEntry>("");
        }

        public int Add(ModuleEntry entry)
        {
            return DataCast.Get<int>(_provider.Database.Insert(entry)); 
        }

        public int Update(ModuleEntry entry)
        {
            return _provider.Database.Update(entry);
        }

        public int Delete(int moduleId)
        {
            return _provider.Database.Delete("modules", "moduleid", null, moduleId);
        }
    }
}
