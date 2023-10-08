using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ProjectStore.Pages.Product
{
    public class DetailsModel : PageModel
    {
        private readonly ProjectStore.Models.AppDbContext _context;

        public DetailsModel(ProjectStore.Models.AppDbContext context)
        {
            _context = context;
        }

      public Models.Product Product { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else 
            {
                Product = product;
            }
            return Page();
        }
    }
}
