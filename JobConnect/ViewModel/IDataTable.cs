namespace ViewModel
{
    public interface IDataTable
    {
        public int TotalRowCount { get; set; }
        List<Column> GetColumns();
    }
}
