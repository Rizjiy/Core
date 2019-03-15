using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Ioc
{
    /// <summary>
    /// Аттрибут регистрации зависимости
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class RegisterAttribute : Attribute
    {
        /// <summary>
        /// Сервис (Интерфейс) для регистрации класса 
        /// </summary>
        public Type ServiceType { get; set; }

        public readonly static Lifetime DefaultLifetime = Lifetime.PerScope;

        /// <summary>
        /// Время жизни объекта
        /// </summary>
        public Lifetime Lifetime { get; set; } = DefaultLifetime;

        public RegisterAttribute()
        {

        }

        public RegisterAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public RegisterAttribute(Type serviceType, Lifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
    }
}
