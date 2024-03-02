namespace PetHotel.IServices
{
    public interface IHashService
    {
        string GetHashedValue(string password, out string saltEncoded);

        bool VerifyPassword(string password, string hashedPassword, string saltEncoded);
    }
}
