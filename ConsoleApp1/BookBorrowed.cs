namespace ConsoleApp1
{
    public sealed class BookBorrowed
    {
        public BookBorrowed(string bookName, string userName)
        {
            BookName = bookName;
            UserName = userName;
        }

        public string BookName { get; }
        
        public string UserName { get; }

        public override string ToString()
        {
            return $"{nameof(BookName)}: {BookName}, {nameof(UserName)}: {UserName}";
        }
    }
}