namespace Vestigio.Models.DTOs
{
    public class FilterResult
    {
        public IEnumerable<object> Data { get; set; }
        public string ViewName { get; set; }

        public FilterResult(IEnumerable<object> data, string viewName)
        {
            Data = data;
            ViewName = viewName;
        }
    }
}
