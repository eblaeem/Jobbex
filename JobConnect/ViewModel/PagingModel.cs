using System.ComponentModel.DataAnnotations;

namespace ViewModel
{
    public class PagingModel
    {
        private int _pageSize;
        [Display(Name ="اندازه صفحه بندی")]
        public int PageSize
        {
            get
            {
                return _pageSize == 0 ? 10 : _pageSize;
            }

            set { _pageSize = value; }
        }

        private int _pageNumber;
        public int PageNumber
        {
            get
            {
                if (_pageNumber <= 0)
                {
                    return 0;
                }
                return _pageNumber - 1;
            }
            set { _pageNumber = value; }
        }


        public string Sort { get; set; }
    }
}
