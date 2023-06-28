using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {

        public Task<int> SaceChangesAsync();
        public void Dispose();
    }
}
