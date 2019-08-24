using AcceptanceTests.DomainLayer;
using MovieService.DomainLayer.Managers.Models;
using System.Collections.Generic;
using System.Net;

namespace AcceptanceTests.TestMediators
{
    internal sealed class TestMediatorForAcceptanceTests
    {
        public IEnumerable<Movie> Movies { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public ExceptionInformation ExceptionInformation { get; set; }

        public void Reset()
        {
            Movies = null;
            HttpStatusCode = HttpStatusCode.OK;
            ExceptionInformation = null;
        }
    }
}
