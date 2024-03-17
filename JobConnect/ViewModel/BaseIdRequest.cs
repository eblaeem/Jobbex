namespace ViewModel
{
    public class BaseIdRequest<T>
    {
        public T Id { get; set; }
        public List<T> Items { get; set; }
    }
}
