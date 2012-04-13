using System.Collections.Generic;

namespace FluentNHibernateMVC3.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public IList<Store> Stores { get; set; }

        public HomeIndexViewModel()
        {
            Stores = new List<Store>();
        }
    }
}