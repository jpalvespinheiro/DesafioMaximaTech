
namespace Adm
{
    sealed class HttpClientFactory : IHttpClientFactory
    {
        readonly Uri baseApiUri;

        public HttpClientFactory(Uri baseApiUri)
        {
            this.baseApiUri = baseApiUri;
        }

        public HttpClient CreateClient(string name)
        {
            if (name is "API")
            {
                return new()
                {
                    BaseAddress = baseApiUri
                };
            }

            return new();
        }
    }
}
