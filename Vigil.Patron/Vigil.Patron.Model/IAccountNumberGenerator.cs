using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vigil.Patron.Model
{
    public interface IAccountNumberGenerator
    {
        string GetAccountNumber();
    }
}
