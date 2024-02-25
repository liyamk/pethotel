using PetHotel.IServices;
using PetHotel.Models;
using System.Text;
using System.Text.Json;

namespace PetHotel.Services
{
    public class ReservationEventSender : IReservationEventSender
    {
        private readonly IRepository<Pet> _petRepo;
        private readonly IRepository<Owner> _ownerRepo;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public ReservationEventSender(IRepository<Pet> petRepo, IRepository<Owner> ownerRepo, IHttpClientFactory factory, IConfiguration config)
        {
            _petRepo = petRepo;
            _ownerRepo = ownerRepo;
            _clientFactory = factory;
            _config = config; 
        }
        public async Task SendCanceledEventAsync(Reservation reservation)
        {
            var pet = await GetPetAsync(reservation.PetId);
            var owner = await GetOwnerAsync(pet.OwnerId);
            var message = $"Hi {owner.FirstName}, your reservation for {pet.Name} has been cancelled per your request.";
            await SendEventAsync(owner, NotificationType.Sms, message);
        }

        public async Task SendCheckedInEventAsync(Reservation reservation)
        {
            var pet = await GetPetAsync(reservation.PetId);
            var owner = await GetOwnerAsync(pet.OwnerId);
            var message = $"{pet.Name} is checked in and ready to enjoy their stay.";
            await SendEventAsync(owner, NotificationType.Sms, message);
        }

        public async Task SendCheckedOutEventAsync(Reservation reservation)
        {
            var pet = await GetPetAsync(reservation.PetId);
            var owner = await GetOwnerAsync(pet.OwnerId);
            var message = $"{pet.Name} is fully checked out. We hope to see you again soon, {owner.FirstName}!";
            await SendEventAsync(owner, NotificationType.Sms, message);
        }

        public async Task SendCreatedEventAsync(Reservation reservation)
        {
            var pet = await GetPetAsync(reservation.PetId);
            var owner = await GetOwnerAsync(pet.OwnerId);
            var message = $"Hi {owner.FirstName}, your reservation for {pet.Name} has been created.";
            await SendEventAsync(owner, NotificationType.Sms, message);
        }

        private async Task<Pet> GetPetAsync(int petId)
        {
            return await _petRepo.GetAsync(x => x.Id == petId) ?? throw new Exception();
        }
        private async Task<Owner> GetOwnerAsync(int ownerId)
        {
            return await _ownerRepo.GetAsync(x => x.Id == ownerId) ?? throw new Exception();
        }

        private async Task SendEventAsync(Owner owner, NotificationType type, string message)
        {
            var formattedEvent = FormatEvent(owner, type, message);
            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("ce-specversion", "1.0");
            client.DefaultRequestHeaders.Add("ce-type", "reservation.event");
            client.DefaultRequestHeaders.Add("ce-source", "petshotel.api.reservations");
            client.DefaultRequestHeaders.Add("ce-id", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("cd-time", DateTime.UtcNow.ToString());
            var content = new StringContent(JsonSerializer.Serialize(formattedEvent), Encoding.UTF8, "application/json");
            var eventDestination = _config["Event:Destination"];

            // TODO: validate response
            await client.PostAsync(eventDestination, content);
        }

        private static NotificationCloudEvent FormatEvent(Owner owner, NotificationType type, string message)
        {
            var notifyEvent = new NotificationEvent()
            {
                Message = message,
                Type = type,
            };

            if (type == NotificationType.Sms)
            {
                notifyEvent.PhoneNumber = owner.PhoneNumber;
            }
            else
            {
                notifyEvent.EmailAddresss = owner.EmailAddress;
            }

            
            return new NotificationCloudEvent()
            {
                Data = notifyEvent
            };
        }
    }
}
