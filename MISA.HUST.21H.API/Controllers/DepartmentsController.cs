using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21H.API.Entities;
using MySqlConnector;

namespace MISA.HUST._21H.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// API Lấy danh sách tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách thông tin chi tiết phòng ban</returns>
        /// created by : NHLINH (8/9/2022)
        [HttpGet]
        [Route("")]
        public IActionResult GetAllPositions()
        {
            try
            {
                //Tao ket noi db
                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);
                //Chuan bi cau lenh SQL
                string getAllDepartmentsQuery = "SELECT * FROM Department";
                //Thuc hien goi vao DB de chay cau lenh SQL voi tham sodau vao tren
                var departmentList = mySqlConnection.Query<Department>(getAllDepartmentsQuery);

                //Xu li ket qua tra ve tu DB
                if (departmentList != null)
                    return StatusCode(StatusCodes.Status200OK, departmentList);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
                //Try catch de bat exception
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }
    }
}
