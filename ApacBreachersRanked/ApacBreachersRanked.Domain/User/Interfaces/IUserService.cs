namespace ApacBreachersRanked.Domain.User.Interfaces
{
    public interface IUserService
    {
        public Task<IUser> GetUserAsync(IUserId userId);
    }
}
