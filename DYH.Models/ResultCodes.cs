using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DYH.Models
{
    public enum ResultCodes
    {
        [Description("OK")]
        OK,
        [Description("Failed")]
        Failed,
        [Description("Exception")]
        Exception
    }
}
