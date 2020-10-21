using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CognitiveSearch.UI.DAL;
using CognitiveSearch.UI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CognitiveSearch.UI.Controllers
{
    [Route("api/graph")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private readonly GremlinConnector connection;

        public GraphController(FeedbackContext feedbackContext)
        {
            connection = new GremlinConnector();
        }

        [Route("query")]
        [HttpPost]
        public JsonResult Query([FromForm] string[] plantCode, [FromForm] string[] equipmentModel)
        {
            string opu = connection.getPlantCodeDetails(plantCode);
            string opuLink = connection.getPlantCodeUnitRelationship(plantCode);
            string equipmentLink = connection.getUnitEquipmentModelRelationship(plantCode, equipmentModel);
            string equipment = connection.getEquipmentModelDetails(plantCode, equipmentModel);
            var output = D3Converter.StaticQuery(opu, opuLink, equipmentLink, equipment);
            return new JsonResult(output.Values.ToArray());
        }
    }
}
