namespace ViewModel
{
    public class LabelValue
    {
        public LabelValue()
        {

        }
        public LabelValue(int? value, string label = "")
        {
            Value = value;
            Label = label;
            if (string.IsNullOrEmpty(label))
            {
                Label = value.ToString();
            }
        }

        public int? Value { get; set; }
        public string Label { get; set; }

        public string Data { get; set; }

        public bool Selected { get; set; }
        private string _id;
        public int? Id
        {
            get => Value;
        }
    }
}
