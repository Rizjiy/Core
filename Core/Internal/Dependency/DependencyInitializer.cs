using AutoMapper;
using AutoMapper.Configuration;
using Core.Internal.ExceptionHandling;
using Core.Internal.LinqToDB;
using Core.Services;
using Core.Log;
using Core.Utils;
using LightInject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Core.Caching.Interface;
using Core.Interfaces;
using LinqToDB.Common;
using Core.Ioc;
using FluentValidation;

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
            LegacyServiceBase.TestMode = testMode;
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

            // Регистрирую все возможные IDependency в домене
            new DependencyRegister().RegisterAssemblyDependencies(container, _assemblies);

            // Применяю частные настройки соединений к БД и режим работы IoC
            doConfig(LegacyServiceBase.NamespaceConnectionDict, container);

            // В тестовом режиме GlobalExceptionHandler не требуется
            if (!LegacyServiceBase.TestMode)
            {
                // Перехватчик ошибок для Api-контроллеров
                GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler),
                    container.GetInstance<GlobalExceptionHandler>());
            }

            //без этой настройки sql для query с хинтами генерится некорректно (пропадают целые таблицы) а то и вовсе падает с экспешном.. 
            //по совету https://github.com/linq2db/linq2db/issues/949
            Configuration.Linq.OptimizeJoins = false;
        }

        /// <summary>
        /// Регистратор зависимостей
        /// </summary>
        public class DependencyRegister
        {
            private readonly ConcurrentStack<Type> _listOfNativeDependencies = new ConcurrentStack<Type>();
            private readonly ConcurrentStack<MethodInfo> _listOfCustomDependencies = new ConcurrentStack<MethodInfo>();
            private readonly ConcurrentStack<Type> _listOfMapperProfiles = new ConcurrentStack<Type>();
            private readonly ConcurrentStack<Type> _listOfCaches = new ConcurrentStack<Type>();

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
                                    _listOfMapperProfiles.Push(t);
                                }
                                else if (typeof(ICache).IsAssignableFrom(t))
                                {
                                    //Это кэш
                                    _listOfCaches.Push(t);
                                }
                                else
                                {
                                    var mi = t.GetMethod(nameof(DependencyRegister), BindingFlags.Public | BindingFlags.Static);
                                    if (mi != null && mi.GetParameters().Length == 1)
                                        _listOfCustomDependencies.Push(mi);
                                    else
                                    {
                                        _listOfNativeDependencies.Push(t);
                                        
                                    }


                                }
                            }
                            catch (Exception ex)
                            {
                                // Не удалось загрузить один из типов
                                Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{ex}");
                            }
                        });
                }
                catch (Exception exception)
                {
                    // Не удалось загрузить одну из связанных сборок
                    Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{exception}");
                }
            }
            private void InternalRegisterDependencies(IServiceContainer container)
            {
                // Регистрирую общие зависимости 
                _listOfNativeDependencies.ForEach(t =>
                {
                    try
                    {
                        //смотрим по атрибуту, с каким временем жизни надо регистрировать тип
                        var registerAttr = t.GetCustomAttribute<RegisterAttribute>(false);
                        var lifetime = registerAttr?.Lifetime ?? RegisterAttribute.DefaultLifetime;

                        //получаем интерфейс для регистрации
                        Type serviceType = registerAttr?.ServiceType;

                        //Регистрируем от интерфейса
                        if(serviceType!=null)
                        {
                            container.Register(serviceType, t, _lifetimeDict[lifetime]());
                        }
                        else
                        {
                            // Регестрируем сервисы как реализацию интерфейсов от которых они унаследованы
                            t.GetInterfaces()
                                .Where(i => !(i == typeof(IDependency) || i == typeof(IDisposable) || i == typeof(IValidator)))
                                .ForEach(i =>
                                    {
                                        // Проверка по равенству наименований интерфейса и класса наслединка
                                        // Пример - Интерфейс: ISomeLogic, Имя класса: SomeLogic
                                        if(i.Name.StartsWith("I") && i.Name.Substring(1) == t.Name)
                                            container.Register(i, t, _lifetimeDict[lifetime]());
                                    }
                                );
                        }

                        //регистрирую как самого себя в любом случае
                        container.Register(t, _lifetimeDict[lifetime]());
                    }
                    catch (Exception ex)
                    {
                        // Не удалось зарегистрировать один из типов
                        Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{ex}");
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
                            Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{exception}");
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
                    Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{ex}");
                }

                //Регистрирую логгер
                try
                {
                    container.Register(typeof(Logger<>));
                }
                catch (Exception ex)
                {
                    // Не удалось зарегистрировать логгер
                    Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{ex}");
                }

                //регистрирую кэши
                _listOfCaches.ForEach(t =>
                {
                    try
                    {
                        //каждый кэш - синглтон. За счет этого механизм автообновления (реализуемый в провайдере) всегда может получить доступ к кэшу. 
                        //ps. todo: с этим можно поспорить, так что тут есть что еще обсудить..
                        container.Register(t, new PerContainerLifetime());
                    }
                    catch (Exception ex)
                    {
                        // Не удалось зарегистрировать один из типов
                        Debug.WriteLine($"{typeof(LegacyServiceBase).Namespace}\r\n{ex}");
                    }
                });
            }

            private IMapper CreateMapper(IEnumerable<Type> profiles)
            {
                var cfg = new MapperConfigurationExpression();
                cfg.AddProfiles(profiles);

                return new MapperConfiguration(cfg).CreateMapper();
            }

            private void Init()
            {
                _listOfNativeDependencies.Clear();
                _listOfCustomDependencies.Clear();
                _listOfMapperProfiles.Clear();
                _listOfCaches.Clear();
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

        private static readonly Dictionary<Lifetime, Func<ILifetime>> _lifetimeDict = new Dictionary<Lifetime, Func<ILifetime>>
        {
            [Lifetime.Transient] = () => null,
            [Lifetime.PerScope] = () => new PerScopeLifetime(),
            [Lifetime.PerContainer] = () => new PerContainerLifetime(),
        };
        



    }
}
