using AutoMapper;
using AutoMapper.Configuration;
using Core.Internal.ExceptionHandling;
using Core.Internal.LinqToDB;
using Core.LinqToDB.Interfaces;
using Core.Log;
using Core.Services;
using Core.Utils;
using LightInject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Core.Internal.Dependency
{
    public class DependencyInitializer
    {
        private ICollection<Assembly> _assemblies { get; set; }

        public DependencyInitializer()
        {
            _assemblies = new List<Assembly> { GetType().Assembly };
        }

        /// <summary>
        /// Добавляет сборку в инициализацию
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public DependencyInitializer ForAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        /// <summary>
        /// Устанавливает режим для авто-тестов
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public DependencyInitializer TestMode(bool testMode)
        {
            ServiceBase.TestMode = testMode;
            return this;
        }

        /// <summary>
        /// Инициализация сервисной инфраструктуры: DataConnection и Injections
        /// </summary>
        /// <param name="doConfig"> Словарь соединений к БД, где key: корневое namespace сервисов,
        /// а value:  </param>
        public void Init(Action<IDictionary<string, string>, IServiceContainer> doConfig)
        {
            // Инициализация IoC
            var container = new ServiceContainer
            {
                ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider() // Все зависимости в Scope-режиме
            };

            // Регистрирую текущий контейнер сам в себе
            container.Register<IServiceContainer>(factory => container, new PerContainerLifetime());

            //Регистрируем DataConnectionFactory
            container.Register<IDataConnectionFactory, DataConnectionFactory>(new PerScopeLifetime());

            // Все остальные зависимости в Scope-режиме 
            //container.SetDefaultLifetime<PerScopeLifetime>(); //Убираем и везде проставляем вручную

            // Регистрирую все возможные IDependency в домене
            new DependencyRegister().RegisterAssemblyDependencies(container, _assemblies);

            // Применяю частные настройки соединений к БД и режим работы IoC
            doConfig(ServiceBase.NamespaceConnectionDict, container);

            // В тестовом режиме GlobalExceptionHandler не требуется
            if (!ServiceBase.TestMode)
            {
                // Перехватчик ошибок для Api-контроллеров
                GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler),
                    container.GetInstance<GlobalExceptionHandler>());
            }
        }

        /// <summary>
        /// Регистратор зависимостей
        /// </summary>
        public class DependencyRegister
        {
            private readonly ConcurrentStack<Type> _listOfNativeDependencies = new ConcurrentStack<Type>();
            private readonly ConcurrentStack<MethodInfo> _listOfCustomDependencies = new ConcurrentStack<MethodInfo>();
            private readonly ConcurrentStack<Profile> _listOfMapperProfiles = new ConcurrentStack<Profile>();

            private void InternalProcessAssembly(Assembly ass)
            {
                try
                {
                    ass.GetLoadableTypes()
                        .AsParallel()
                        .Where(t =>
                            typeof(IDependency).IsAssignableFrom(t)
                        || typeof(FluentValidation.IValidator).IsAssignableFrom(t)
                        )
                        .ForEach(t =>
                        {
                            try
                            {
                                if (typeof(Profile).IsAssignableFrom(t))
                                {
                                    //Это профиль автомаппера
                                    var prof = (Profile)Activator.CreateInstance(t);
                                    _listOfMapperProfiles.Push(prof);
                                }
                                else
                                {
                                    var mi = t.GetMethod(nameof(DependencyRegister), BindingFlags.Public | BindingFlags.Static);
                                    if (mi != null && mi.GetParameters().Length == 1)
                                        _listOfCustomDependencies.Push(mi);
                                    else
                                        _listOfNativeDependencies.Push(t);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Не удалось загрузить один из типов
                                Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{ex}");
                            }
                        });
                }
                catch (Exception exception)
                {
                    // Не удалось загрузить одну из связанных сборок
                    Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{exception}");
                }
            }
            private void InternalRegisterDependencies(IServiceContainer container)
            {
                // Регистрирую общие зависимости 
                _listOfNativeDependencies.ForEach(t =>
                {
                    try
                    {
                        container.Register(t, new PerScopeLifetime());
                    }
                    catch (Exception ex)
                    {
                        // Не удалось зарегистрировать один из типов
                        Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{ex}");
                    }
                });

                // Регистрирую частные зависимости 
                _listOfCustomDependencies
                    .ForEach(mi =>
                    {
                        try
                        {
                            mi.Invoke(null, new object[] { container });
                        }
                        catch (Exception exception)
                        {
                            // Не удалось исполнить частную регистрацию типа
                            Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{exception}");
                        }
                    });

                // Регистрирую Automapper
                try
                {
                    var mapper = CreateMapper(_listOfMapperProfiles);
                    container.RegisterInstance(mapper);
                }
                catch (Exception ex)
                {
                    // Не удалось зарегистрировать Automapper
                    Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{ex}");
                }

                //Регистрирую логгер
                try
                {
                    container.Register(typeof(Logger<>), new PerContainerLifetime());
                }
                catch (Exception ex)
                {
                    // Не удалось зарегистрировать логгер
                    Debug.WriteLine($"{typeof(ServiceBase).Namespace}\r\n{ex}");
                }

            }

            private IMapper CreateMapper(IEnumerable<Profile> profiles)
            {
                var cfg = new MapperConfigurationExpression();

                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }

                return new MapperConfiguration(cfg).CreateMapper();
            }

            private void Init()
            {
                _listOfNativeDependencies.Clear();
                _listOfCustomDependencies.Clear();
                _listOfMapperProfiles.Clear();
            }

            public void RegisterAssemblyDependencies(IServiceContainer container, ICollection<Assembly> assemblies)
            {
                Init();

                // Получаю всех имплементоров IDependency переданных сборок
                var ass = assemblies.Distinct();
                Parallel.ForEach(ass, InternalProcessAssembly);

                // Регистрирую зависимости 
                InternalRegisterDependencies(container);
            }


        }

    }
}
