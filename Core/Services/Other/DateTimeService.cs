using Core.Interfaces;
using System;
using Core.Ioc;

namespace Core.Services.Other
{
    /// <summary>
    /// Сервис для замены в тестах свойств Today и Now.
    /// </summary>
    [Register(Lifetime = Lifetime.PerContainer)]
    public class DateTimeService : IDependency
    {
        public virtual DateTime Now => DateTime.Now;
        
        public virtual DateTime Today => DateTime.Today;
    }
}
