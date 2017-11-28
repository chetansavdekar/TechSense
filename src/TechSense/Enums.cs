using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechSense
{
    public enum Priority
    {
        High = 1,
        Medium,
        Low
    }

    public enum Status
    {
        Assess = 1,
        Trial,
        Adopt,
        Depreciate,
        Continue
    }

    public enum Visibility
    {
        False = 0,
        True
    }
}
