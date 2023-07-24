namespace ApacBreachersRanked.Domain.User.Interfaces
{
    public interface IUser
    {
        public IUserId UserId { get; }
        public string Name { get; }
        public bool Equals(IUser rhs)
        {
            return UserId.Equals(rhs.UserId);
        }
    }
}
