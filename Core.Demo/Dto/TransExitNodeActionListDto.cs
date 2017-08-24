using System;
using Core.Dto;

namespace Core.Demo.Dto
{

    /// <summary>
    ///  Dto-класс.Для отпражения списка точек выхода ДОР в сеть.
    /// </summary>
    public class TransExitNodeActionListDto : EntityDto
    {
        /// <summary>
        /// Имя хоста.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Тип CMAK профиля vpn подключения (шаблон использованный для генерации профиля) 
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Событие (init,connected, error, disconnected)
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Ip - адрес точки выхода.
        /// </summary>
        public string ExternalIP { get; set; }


        /// <summary>
        /// Время начала подключения.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Локальные Ip адреса интерфейсов.
        /// </summary>
        public string NetworkIps { get; set; }


        /// <summary>
        /// Mac-адрес основного интерфейса (с именем "LAN")
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// Идентификатор сессии(ГУИД).
        /// </summary>
        public string SessionId { get; set; }
    }
}
