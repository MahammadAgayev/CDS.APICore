
using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Models.Catering;

namespace CDS.APICore.Services
{
    public interface ICateringService
    {
        void AddCategory(AddCategoryRequest request);
        void AddCatering(AddCateringRequest request);
    }

    public class CateringService : ICateringService
    {
        private readonly ICateringManager _cateringManager;

        public CateringService(ICateringManager cateringManager)
        {
            _cateringManager = cateringManager;
        }

        public void AddCategory(AddCategoryRequest request)
        {
            _cateringManager.CreateCategory(new Entities.CateringCategory
            {
                Name = request.Name,
                Comment = request.Comment,
                DisplayName = request.DisplayName,
            });
        }

        public void AddCatering(AddCateringRequest request)
        {
            _cateringManager.Create(new Entities.Catering
            {
                Name = request.Name,
                AddressText = request.AddressText,
                Comment = request.Comment,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CateringCategory = new Entities.CateringCategory { Id = request.CategoryId }
            });
        }
    }
}
