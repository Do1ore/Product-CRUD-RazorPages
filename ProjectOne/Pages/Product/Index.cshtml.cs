using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ProjectStore.Pages.Product
{
    [Authorize(Roles = "admin")]

    public class IndexModel : PageModel
    {
        private readonly ProjectStore.Models.AppDbContext _context;

        public IndexModel(ProjectStore.Models.AppDbContext context)
        {
            _context = context;
        }

        public IList<Models.Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products.ToListAsync();
            }
        }
    }
}
