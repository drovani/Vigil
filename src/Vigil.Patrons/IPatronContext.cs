using Microsoft.EntityFrameworkCore;
using System;

namespace Vigil.Patrons
{
    public interface IPatronContext : IDisposable
    {
        DbSet<Patron> Patrons { get; set; }
        int SaveChanges();
    }
}