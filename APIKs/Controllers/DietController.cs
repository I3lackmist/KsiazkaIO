using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIKs.JSONModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace APIKs.Controllers {
    public class DietController {
        public DietController() {}

        [HttpPut]
        public async Task<ActionResult<Diet>> GetDiet([FromHeader] userName) {
            return 
        }

    }
    
}