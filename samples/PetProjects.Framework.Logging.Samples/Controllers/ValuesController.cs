namespace PetProjects.Framework.Logging.Samples.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ILogger<ValuesController> logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            this.logger = logger;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            this.logger.LogCritical("testcritical {asd}", DateTime.UtcNow);
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            this.logger.LogError("testerror {asd} {ex}", DateTime.UtcNow, new Exception("asdadsdasdas"));
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            this.logger.LogInformation("testinfo {asd} {ex}", DateTime.UtcNow, new Exception("213123"));
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            this.logger.LogDebug("testdebug {asd} {ex}", DateTime.UtcNow, new Exception("213123"));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            this.logger.LogTrace("testtrace{asd} {ex}", DateTime.UtcNow, new Exception("213123"));
        }
    }
}
