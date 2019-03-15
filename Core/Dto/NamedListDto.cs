
namespace Core.Dto
{

    /// <summary>
    /// Списочное дто с именем. В основном используется для справочников
    /// </summary>
    public class NamedListDto : BaseListDto
    {
        public string Name { get; set; }
    }
}
