namespace ViewModel.AccessCode
{
    public class AccessCodeResponse : IDataTable
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Url { get; set; }
        public int TotalRowCount { get; set; }

        public bool IsActive { get; set; }
        public List<Column> GetColumns()
        {
            return new List<Column>()
            {
                 new Column(nameof(Name),"عنوان "),
                 new Column(nameof(Number),"شناسه"),
                 new Column(nameof(IsActive),"وضعیت"),
            };
        }
    }
}
