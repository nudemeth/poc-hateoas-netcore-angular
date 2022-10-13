namespace Customer.Api
{
    public record Link
    {
        public string Href { get; init; }

        public Link(string href)
        {
            Href = href;
        }
    }
}
