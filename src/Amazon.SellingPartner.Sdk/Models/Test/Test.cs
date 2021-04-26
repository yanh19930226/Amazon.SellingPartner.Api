using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Test
{
    public class TestRequest : BaseRequest<TestRequestParameter>
    {
        public TestRequest(TestRequestParameter data) : base(data)
        {

        }
    }

    public class TestRequestParameter
    {
        public string Action { get; set; }

        public string Version { get; set; }

    }
}
