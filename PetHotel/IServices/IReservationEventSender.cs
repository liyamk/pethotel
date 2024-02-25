using PetHotel.Models;

namespace PetHotel.IServices
{
    public interface IReservationEventSender
    {
        Task SendCreatedEventAsync(Reservation reservation);

        Task SendCheckedInEventAsync(Reservation reservation);

        Task SendCheckedOutEventAsync(Reservation reservation);

        Task SendCanceledEventAsync(Reservation reservation);
    }
}
