using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.Entities;
using CDS.APICore.Helpers;
using CDS.APICore.Models.Location;

namespace CDS.APICore.Services
{
    public interface ILocationService
    {
        void AddLocation(AddLocationRequest request, Account account);
    }

    public class LocationService : ILocationService
    {
        private readonly ILocationManager _locationManager;
        private readonly ICustomerManager _customerManager;

        public LocationService(ILocationManager locationManager)
        {
            _locationManager = locationManager;
        }

        public void AddLocation(AddLocationRequest request, Account account)
        {
            var customer = _customerManager.Get(request.CustomerIdentityTag);

            if(customer is null)
            {
                throw new AppException("customer not found");
            }

            _locationManager.Save(new CustomerLocation
            {
                Customer = customer,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            });
        }
    }
}
