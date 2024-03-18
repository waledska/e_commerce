using e_commerce.e_commerceData;
using e_commerce.e_commerceData.Models;
using e_commerce.IdentityData;
using e_commerce.Services;
using e_commerce.vModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace e_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class addressController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly businessContext _businessContext;
        //private readonly identityContext _identityContext;
        public addressController(IAuthService authService, businessContext businessContext/*, identityContext identityContext*/)
        {
            _authService = authService;
            _businessContext = businessContext;
            //_identityContext = identityContext;
        }
        [HttpPost("addAddressToUser")]
        public async Task<IActionResult> addAddressToUser([FromBody]UserAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = _authService.getUserId();
            if (userId == null)
            {
                return BadRequest("some thing went wrong for the user ID");
            }

            var newAddress = new Address
            {
                UnitNumber = model.unitNumber,
                City = model.city,
                CountryId = model.countryId,
                PostalCode = model.postalCode,
                Region = model.region,
                StreetNumber = model.streetNumber,
            };
            // check if this address is already available
            var isAddressAvailable = _businessContext.Addresses
            .Any(a => a.UnitNumber == newAddress.UnitNumber &&
                        a.City == newAddress.City &&
                        a.CountryId == newAddress.CountryId &&
                        a.PostalCode == newAddress.PostalCode &&
                        a.Region == newAddress.Region &&
                        a.StreetNumber == newAddress.StreetNumber);

            // adding address to table address
            if (!isAddressAvailable)
            {
                await _businessContext.Addresses.AddAsync(newAddress);
                int recordsAffected = await _businessContext.SaveChangesAsync();

                if (!(recordsAffected > 0))
                {
                    return BadRequest("address didn't add. some thing went wrong");
                }
            }
            // get the new address ID
            var newAddressAdded = _businessContext.Addresses
            .Where(a => a.UnitNumber == newAddress.UnitNumber &&
                        a.City == newAddress.City &&
                        a.CountryId == newAddress.CountryId &&
                        a.PostalCode == newAddress.PostalCode &&
                        a.Region == newAddress.Region &&
                        a.StreetNumber == newAddress.StreetNumber).ToList();
            var addressId = newAddressAdded[0].Id;
            // know if this user has another default address 
            var isDefault = !_businessContext.UserAddresses.Any(x => x.UserId == userId);
            // check to don't repeat the data in table userAddreses
            var notNew = !_businessContext.UserAddresses.Any(t => t.AddressId == addressId && t.UserId == userId);
            // adding to table userAddreses
            if (notNew)
            {
                await _businessContext.UserAddresses.AddAsync(new UserAddress
                {
                    AddressId = addressId,
                    IsDefault = isDefault,
                    UserId = userId
                });
                // save in the db
                int recordsAffected2 = await _businessContext.SaveChangesAsync();

                if (!(recordsAffected2 > 0))
                {
                    return BadRequest("address didn't add to user. some thing went wrong");
                }
                return Ok("address added to user successfully");
            }
            return BadRequest("this user already have this address");
        }
        [HttpGet("deleteAddressFromUser/addressId")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = _authService.getUserId();
            if (userId == null)
                return BadRequest("Something went wrong. User ID is invalid.");

            var isAvailableAddress = _businessContext.Addresses.Any(x => x.Id == addressId);
            if (!isAvailableAddress)
                return BadRequest("Invalid address ID.");

            var userAddress = _businessContext.UserAddresses
                .FirstOrDefault(a => a.AddressId == addressId && a.UserId == userId);

            if (userAddress == null)
                return BadRequest("User doesn't have this address.");

            var userAddresses = _businessContext.UserAddresses
                .Where(a => a.UserId == userId)
                .ToList();

            if (userAddress.IsDefault == true && userAddresses.Count > 1)
                return BadRequest("Please make another address default first, then delete.");
             

            // Remove this address from this user
            _businessContext.UserAddresses.Remove(userAddress);

            await _businessContext.SaveChangesAsync();

            return Ok(new { message = "Address deleted successfully.", deletedAddress = userAddress });
        }
        [HttpPost("updateAddress")]
        public async Task<IActionResult> updateAddress([FromBody]updateAddressModel model) 
        {
            // check the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // userId
            var userId = _authService?.getUserId();
            if (userId == null)
                return BadRequest("invalid user ID");

            // check if this user has this address 
            var userAddress = _businessContext.UserAddresses.FirstOrDefault(x => x.AddressId == model.addressId && x.UserId == userId);
            if (userAddress == null)
                return BadRequest("invalid address ID");

            // check if this is there are more than one user using the same address
            // then while updating create new address for the user 
            var usersUseThisAddress = _businessContext.UserAddresses.Where(x => x.AddressId == model.addressId).ToList();
            if (usersUseThisAddress.Count > 1)
            {
                // then remove the old address from this User
                await DeleteAddress(model.addressId);
                // call add address to user API here please and how can i make that
                await addAddressToUser(model.newAddressData);
                return Ok("address updated successfully");
            }

            // update this address's data
            Address updatedAddress = new Address {
                Id = model.addressId,
                City = model.newAddressData.city,
                CountryId = model.newAddressData.countryId,
                PostalCode = model.newAddressData.postalCode,
                Region = model.newAddressData.region,
                StreetNumber = model.newAddressData.streetNumber,
                UnitNumber = model.newAddressData.unitNumber
            };
            _businessContext.Addresses.Update(updatedAddress);
            var changesrows = await _businessContext.SaveChangesAsync();
            if(!(changesrows > 0))
                return BadRequest("Some thing went wrong while updating address");

            return Ok("address updated successfully");
        }

        [HttpGet("makeMainAddress/addressId")]
        public async Task<IActionResult> makeMainAddress(int addressId)
        {
            var userId = _authService.getUserId();
            var address = _businessContext.UserAddresses.FirstOrDefault(x => x.AddressId == addressId && x.UserId == userId);
            if (address == null)
                return BadRequest("invalid address ID");

            // make the this address main address 
            var userAddreses = _businessContext.UserAddresses.Where(a => a.UserId == userId).ToList();
            // update the list 
            foreach (var userAddress in userAddreses)
            {
                if(userAddress.AddressId == addressId) {
                    userAddress.IsDefault = true;
                    address = userAddress;
                }
                else
                    userAddress.IsDefault = false;
            }

            await _businessContext.SaveChangesAsync();
            
            return Ok(new { message = "address updated successfully", updatedAddres = address });
        }
        [HttpGet("getCountries")]
        public async Task<IActionResult> getCountries()
        {
            List<simpleCountry> result = new List<simpleCountry>();
            
            var countries = _businessContext.Countries.ToList();
            foreach (var country in countries)
            {
                result.Add(new simpleCountry { id = country.Id, name = country.CountyName });
            }
            return Ok(result);
        }
        [HttpGet("getUserAddresses/userId")]
        public async Task<IActionResult> getUserAddresses(string userId)
        {
            var thisUserId = userId;
            if (userId == null || !_authService.IsThereUserWithId(userId))
                return BadRequest("invalid user ID");

            
            var result = _businessContext.UserAddresses
                .Where(ua => ua.UserId == thisUserId)
                .Join(_businessContext.Addresses,
                    userAddress => userAddress.AddressId,
                    address => address.Id,
                    (userAddress, address) => new { userAddress, address })
                .Join(_businessContext.Countries,
                    joined => joined.address.CountryId,
                    country => country.Id,
                    (joined, country) => new { userId = joined.userAddress.UserId, Address = joined.address, Country = country, isDefault = joined.userAddress.IsDefault })
                .GroupBy(joined => joined.userId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Addresses = group.Select(a => new
                    {
                        Id = a.Address.Id,
                        isDefault = a.isDefault,
                        UnitNumber = a.Address.UnitNumber,
                        StreetNumber = a.Address.StreetNumber,
                        City = a.Address.City,
                        Region = a.Address.Region,
                        PostalCode = a.Address.PostalCode,
                        Country = new
                        {
                            CountryName = a.Country.CountyName,
                            CountryId = a.Country.Id
                        }
                    }).ToList()
                }).ToList();
            if (!result.Any()) // This checks if the list is empty
            {
                return Ok(new { Message = "User doesn't have any addresses" });
            }
            return Ok(result);
        }
        [HttpGet("getDefaultAddress/userId")]
        public async Task<IActionResult> getDefaultAddress(string userId)
        {
            var thisUserId = userId;
            if (userId == null || !_authService.IsThereUserWithId(userId))
                return BadRequest("invalid user ID");


            var result = _businessContext.UserAddresses
                .Where(ua => ua.UserId == thisUserId)
                .Join(_businessContext.Addresses,
                    userAddress => userAddress.AddressId,
                    address => address.Id,
                    (userAddress, address) => new { userAddress, address })
                .Where(t => t.userAddress.IsDefault == true)
                .Join(_businessContext.Countries,
                    joined => joined.address.CountryId,
                    country => country.Id,
                    (joined, country) => new { userId = joined.userAddress.UserId, Address = joined.address, Country = country, isdefault = joined.userAddress.IsDefault })
                .GroupBy(joined => joined.userId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Addresses = group.Select(a => new
                    {
                        Id = a.Address.Id,
                        isDefault = a.isdefault,
                        UnitNumber = a.Address.UnitNumber,
                        StreetNumber = a.Address.StreetNumber,
                        City = a.Address.City,
                        Region = a.Address.Region,
                        PostalCode = a.Address.PostalCode,
                        Country = new
                        {
                            CountryName = a.Country.CountyName,
                            CountryId = a.Country.Id
                        }
                    }).ToList()
                }).ToList();
            if (!result.Any()) // This checks if the list is empty
            {
                return Ok(new { Message = "User doesn't have any addresses" });
            }
            return Ok(result);

        }
    }
}
