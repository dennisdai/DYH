using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DYH.IDAL;

namespace DYH.Web.Framework
{
    public class UserModelPrincipal : IPrincipal
    {
        private IUser _user;
        public UserModelPrincipal(IUser user)
        {
            _user = user;
        }

         

        //[ScriptIgnore]    //在序列化的时候忽略该属性
        public IIdentity Identity
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}
