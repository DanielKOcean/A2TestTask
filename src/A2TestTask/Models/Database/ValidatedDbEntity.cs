using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2TestTask.Models.Database
{
    public abstract class ValidatedDbEntity
    {
        public virtual bool Validate() => true;
    }
}
