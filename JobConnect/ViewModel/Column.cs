namespace ViewModel
{
    public class Column
    {
        public Column(string name, string title, bool selected = true,
            bool isSorting = true)
        {
            Name = name;
            Title = title;
            Selected = selected;
            IsSorting = isSorting;
        }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }
        public bool IsSorting { get; set; }
        public bool HideInExcel { get; set; }

        public bool IsNumber { get; set; }
    }
}
