using LapAdvisor.Bl;
using LapAdvisor.Model;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LapAdvisor.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController] // means make this controller web api
    public class ItemsController : ControllerBase // without view just api and This controller is responsible only for data exchange, not UI rendering
    {
        // dependency injection 
        // We used dependency injection to decouple the API layer from the business logic
        IItems oItem;
        public ItemsController(IItems iitem) 
        {
            oItem = iitem;
        }
        // GET: api/<ItemsController>
        // To bring all the elements
        [HttpGet]
        public ApiResponse Get()
        {
            ApiResponse oApiResponse = new ApiResponse();
            // from the bussiness logic in interface IItems and put it in apiresponse
            oApiResponse.Data = oItem.GetAll();
            oApiResponse.Errors = null;
            oApiResponse.StatusCode = "200";
            return oApiResponse;
        }

        // GET api/<ItemsController>/5
        // used in item details page
        [HttpGet("{id}")]
        public ApiResponse Get(int id)
        {
            ApiResponse oApiResponse = new ApiResponse();
            oApiResponse.Data = oItem.GetById(id);
            oApiResponse.Errors = null;
            oApiResponse.StatusCode = "200";
            return oApiResponse;
        }
        // used for filters
        [HttpGet("GetByCategoryId/{categoryId}")]
        public ApiResponse GetByCategoryId(int categoryId)
        {
            ApiResponse oApiResponse = new ApiResponse();
            oApiResponse.Data = oItem.GetAllItemsData(categoryId);
            oApiResponse.Errors = null;
            oApiResponse.StatusCode = "200";
            return oApiResponse;
        }
        // POST api/<ItemsController>
        [HttpPost]
        // the data comes in json format and transforms into a TbItem object
        public ApiResponse Post([FromBody] TbItem item)
        {
            // Error handling is centralized at the API response level
            try
            {
                oItem.Save(item);
                ApiResponse oApiResponse = new ApiResponse();
                oApiResponse.Data = "done";
                oApiResponse.Errors = null;
                oApiResponse.StatusCode = "200";
                return oApiResponse;
            }
            catch (Exception ex)
            {
                ApiResponse oApiResponse = new ApiResponse();
                oApiResponse.Data = null;
                oApiResponse.Errors = ex.Message;
                oApiResponse.StatusCode = "502";
                return oApiResponse;
            }
        }

        // its an soft delete from cuuren state not a delete from the database this is for security
        [HttpPost]
        [Route("Delete")]
        public void Delete([FromBody] int id)
        {
            oItem.Delete(id);
        }
    }
}
