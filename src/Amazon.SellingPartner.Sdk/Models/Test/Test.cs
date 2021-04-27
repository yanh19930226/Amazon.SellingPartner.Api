using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Test
{
    public class TestRequest : BaseRequest<TestRequestParameter,BaseResponse<TestResponse>>
    {
        public TestRequest(TestRequestParameter data,string token) : base(data,token)
        {

        }
    }

    public class TestRequestParameter
    {
        public string Action { get; set; }

        public string Version { get; set; }

    }
    public class TestResponse
    {
    }
}
