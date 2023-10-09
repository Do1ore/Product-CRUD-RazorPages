using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectStore.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Display(Name = "Название товара")]
        public string? Name { get; set; }
        [Display(Name = "Стоимость")]

        public double? Price { get; set; }
        [Display(Name = "Описание")]

        public string? Description { get; set; }
        [Display(Name = "Категория")]

        public string? Category { get; set; }

        [Display(Name = "Изображение")]
        [NotMapped]
        public IFormFile? ItemImage { get; set; }
        [Display(Name="Путь к файлу")]
        public string? ImagePath { get; set; }
    }
}
