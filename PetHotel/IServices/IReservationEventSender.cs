using PetHotel.Models;

namespace PetHotel.IServices
{
    public interface IReservationEventSender
    {
        Task SendCreatedEvent(Reservation reservation);

        Task SendCheckedInEvent(Reservation reservation);

        Task SendCheckedOutEvent(Reservation reservation);

        Task SendCanceledEvent(Reservation reservation);
    }
}
