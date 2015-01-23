﻿using System;
using System.Collections.Generic;
using System.Linq;
using DYH.Core;
using DYH.Data;
using DYH.IDAL;
using DYH.Models;

namespace DYH.DAL
{
    public class RoleRightRepository : IRoleRight
    {
        private readonly DataProvider _provider;
        public RoleRightRepository(DataProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<RoleRightEntry> GetList(int roleId)
        {
            return _provider.Database.Query<RoleRightEntry>("WHERE roleid = @0", roleId);
        }

        public RoleRightEntry GetById(int roleRightId)
        {
            return _provider.Database.FirstOrDefault<RoleRightEntry>("WHERE rolerightid = @0", roleRightId);
        }

        public int Add(RoleRightEntry entry)
        {
            return DataCast.Get<int>(_provider.Database.Insert(entry));
        }

        public int Add(IEnumerable<RoleRightEntry> list)
        {
            var db = _provider.Database;
            int i = 0;
            using (var tran = db.GetTransaction())
            {
                foreach (var item in list)
                {
                    db.Insert(item);
                    i++;
                }

                tran.Complete();
            }

            if (i == list.Count() - 1)
                return 1;

            return 0;
        }

        public int Update(RoleRightEntry entry)
        {
            return _provider.Database.Update(entry);
        }

        public int Update(IEnumerable<RoleRightEntry> list)
        {
            var db = _provider.Database;
            int i = 0;
            using (var tran = db.GetTransaction())
            {
                foreach (var item in list)
                {
                    db.Update(item);
                    i++;
                }

                tran.Complete();
            }

            if (i == list.Count() - 1)
                return 1;

            return 0;
        }
    }
}
