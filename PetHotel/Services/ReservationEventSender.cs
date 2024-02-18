using PetHotel.IServices;
using PetHotel.Models;

namespace PetHotel.Services
{
    public class ReservationEventSender : IReservationEventSender
    {
        private readonly IRepository<Pet> _petRepo;
        private readonly IRepository<Owner> _ownerRepo;

        public ReservationEventSender(IRepository<Pet> petRepo, IRepository<Owner> ownerRepo)
        {
            _petRepo = petRepo;
            _ownerRepo = ownerRepo;

        }
        public Task SendCanceledEvent(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task SendCheckedInEvent(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task SendCheckedOutEvent(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        public Task SendCreatedEvent(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        private async Task<Pet> GetPetAsync(int petId)
        {
            var pet = await _petRepo.GetAsync(x => x.Id == petId);
            if (pet == null)
            {
                throw new Exception();
            }

            return pet;
        }

        private async Task<Owner> GetOwnerAsync(int OwnerId)
        {
            var owner = await _ownerRepo.GetAsync(x => x.Id == OwnerId);
            if (owner == null)
            {
                throw new Exception();
            }

            return owner;
        }
    }
}
