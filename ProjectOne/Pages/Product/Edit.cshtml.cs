using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ProjectStore.Pages.Product
{
    public class EditModel : PageModel
    {
        private readonly Models.AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(Models.AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            public Models.Product Product { get; set; } = default!;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            Input = new InputModel { Product = product };
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Product.ItemImage?.Length > 0)
            {
                if (Input.Product.ImagePath != null && System.IO.File.Exists(Input.Product.ImagePath))
                {
                    System.IO.File.Delete(Input.Product.ImagePath);
                }

                var fileName = ("ProductImage" + DateTime.Now.ToShortDateString() + Input!.Product.ItemImage!.FileName)
                    .Trim();
                var relativePath = $@"\img\products\{fileName}";
                var fullPath = _webHostEnvironment.WebRootPath + relativePath;
                await using (FileStream sw = new FileStream(fullPath, FileMode.Create))
                {
                    await Input.Product.ItemImage.CopyToAsync(sw);
                }
                Input.Product.ImagePath = relativePath;
            }

            _context.Attach(Input.Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Input.Product.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}