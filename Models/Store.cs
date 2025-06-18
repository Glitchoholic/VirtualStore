using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VirtualStore.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsMain { get; set; }
        public bool IsInvoiceDirect { get; set; }
        public List<Space> Spaces { get; set; }

    }
}
