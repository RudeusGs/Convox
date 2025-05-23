using Microsoft.AspNetCore.Mvc;

namespace server.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class BaseController : ControllerBase
        {
            private const int HTTP_SUCCESS = 200;

            protected ObjectResult Response<T>(T value, int statusCode)
            {
                return StatusCode(statusCode, new ResponseModel(value));
            }

            protected ObjectResult Response<T>(T value)
            {
                return StatusCode(HTTP_SUCCESS, new ResponseModel(value));
            }

            protected class ResponseModel
            {
                public ResponseModel(object result)
                {
                    Result = result;
                }

                public object Result { get; set; }
            }
        }
}
