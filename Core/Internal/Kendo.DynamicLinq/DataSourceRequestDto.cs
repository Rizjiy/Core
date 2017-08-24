namespace Core.Internal.Kendo.DynamicLinq
{
    public class DataSourceRequestDto<TFilter> : DataSourceRequest
    {
        /// <summary>
        /// Custom filter
        /// </summary>
        public TFilter FilterDto { get; set; }
    }
}
