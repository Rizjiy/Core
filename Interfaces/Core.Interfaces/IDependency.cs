namespace Core.Interfaces
{
    /// <summary>
    /// Интерфейс для регистрации зависимостей. Регистрирует всех имплементеров в контейнере.
    /// Если в типе присутствует публичный статический метод DependencyRegister(container), 
    /// - для регистрации используется имено он.
    /// </summary>
	public interface IDependency { }
}
