using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21H.API.Entities;
using MISA.HUST._21H.API.Entities.DTO;
using MySqlConnector;

namespace MISA.HUST._21H.API.Controllers
{
    //Tên con đường, controller này sẽ được thay thế bởi tiền tố, vd: api/[controller] -> api/Employees
    //[EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
                   
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// API Lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns>Danh sách tất cả nhân viên</returns>
        /// created by : NHLINH (8/9/2022)
        [HttpGet]
        [Route("")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                //Tao ket noi db
                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);
                //Chuan bi cau lenh SQL
                string getAllEmployeesQuery = "SELECT * FROM EMPLOYEE";
                //Thuc hien goi vao DB de chay cau lenh SQL voi tham sodau vao tren
                var employeeList = mySqlConnection.Query<Employee>(getAllEmployeesQuery);

                //Xu li ket qua tra ve tu DB
                if (employeeList != null)
                    return StatusCode(StatusCodes.Status200OK, employeeList);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
                //Try catch de bat exception
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API lấy thông tin nhân viên bằng ID
        /// </summary>
        /// <param name="employeeID">ID nhân viên</param>
        /// <returns>Thông tin nhân viên</returns>
        [HttpGet]
        [Route("{employeeID}")]
        public IActionResult GetEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {

                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string getAllEmployeesQuery = "SELECT * FROM EMPLOYEE WHERE EMPLOYEEID = @EMPLOYEEID;";

                var parameters = new DynamicParameters();
                parameters.Add("@EMPLOYEEID", employeeID);

                var employee = mySqlConnection.Query<Employee>(getAllEmployeesQuery, parameters).FirstOrDefault();

                if (employee != null)
                    return StatusCode(StatusCodes.Status200OK, employee);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// Thêm mới nhân viên 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>ID nhân viên</returns>
        [HttpPost]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string insertEmployeeQuery = "INSERT INTO employee (EmployeeID, EmployeeCode, EmployeeName, DateOfBirth, Gender, IdentityNumber, IdentityIssuedDate, IdentityIssuedPlace, Email, PhoneNumber, PositionID, PositionName, DepartmentID, DepartmentName, TaxCode, Salary, JoiningDate, WorkStatus, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) VALUES ( @EmployeeID, @EmployeeCode, @EmployeeName, @DateOfBirth, @Gender, @IdentityNumber, @IdentityIssuedDate, @IdentityIssuedPlace, @Email, @PhoneNumber, @PositionID, @PositionName, @DepartmentID, @DepartmentName, @TaxCode, @Salary, @JoiningDate, @WorkStatus, @CreatedDate, @CreatedBy, @ModifiedDate, @ModifiedBy );";

                var employeeID = Guid.NewGuid();
                var parameters = new DynamicParameters();
                parameters.Add("@EMPLOYEEID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@TaxCode", employee.TaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoiningDate", employee.JoiningDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedDate", employee.CreateDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                var numberOfAffectedRows = mySqlConnection.Execute(insertEmployeeQuery, parameters);

                if (numberOfAffectedRows > 0) return StatusCode(StatusCodes.Status200OK, employeeID);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");

            }
            catch (MySqlException mySqlException)
            {
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry) return StatusCode(StatusCodes.Status400BadRequest, "e003");
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// Xóa nhân viên theo ID
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns>Trả về ID nhân viên đã xóa</returns>
        [HttpDelete]
        [Route("{employeeID}")]
        public IActionResult DeleteEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {

                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string deleteEmployeeQuery = "DELETE FROM EMPLOYEE WHERE EMPLOYEEID = @EMPLOYEEID;";

                var parameters = new DynamicParameters();
                parameters.Add("@EMPLOYEEID", employeeID);

                var numberOfAffectedRows = mySqlConnection.Execute(deleteEmployeeQuery, parameters);

                if (numberOfAffectedRows > 0)
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="employeeID"></param>
        /// <returns>Trả về ID nhân viên đã cập nhật</returns>
        [HttpPut]
        [Route("{employeeID}")]
        public IActionResult UpdateEmployeeByID([FromBody] Employee employee, [FromRoute] Guid employeeID)
        {
            try
            {

                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string updateEmployeeQuery = "UPDATE EMPLOYEE SET "+
                   "EmployeeCode= @EmployeeCode, "                 +
                   "EmployeeName = @EmployeeName, "                +
                   "DateOfBirth= @DateOfBirth, "                   +
                   "Gender =@Gender, "                             +
                   "IdentityNumber= @IdentityNumber, "             +
                   "IdentityIssuedDate= @IdentityIssuedDate, "     +
                   "IdentityIssuedPlace= @IdentityIssuedPlace, "   +
                   "Email =@Email, "                               +
                   "PhoneNumber= @PhoneNumber, "                   +
                   "PositionID= @PositionID, "                     +
                   "positionName= @positionName, "                 +
                   "DepartmentID =@DepartmentID, "                 +
                   "DepartmentName =@DepartmentName, "             +
                   "TaxCode =@TaxCode, "                           +
                   "Salary = @Salary, "                            +
                   "JoiningDate =@JoiningDate, "                   +
                   "WorkStatus =@WorkStatus, "                     +
                   "CreatedDate =@CreatedDate, "                   +
                   "CreatedBy =@CreatedBy, "                       +
                   "ModifiedDate =@ModifiedDate, "                 +
                   "ModifiedBy =@ModifiedBy "                      +
                   "WHERE EMPLOYEEID = @EMPLOYEEID;";             

                var parameters = new DynamicParameters();
                parameters.Add("@EMPLOYEEID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@TaxCode", employee.TaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoiningDate", employee.JoiningDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedDate", employee.CreateDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);


                var numberOfAffectedRows = mySqlConnection.Execute(updateEmployeeQuery, parameters);

                if (numberOfAffectedRows > 0)
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }

        }
        
       /// <summary>
       /// Lấy mã nhân viên lớn nhất
       /// </summary>
       /// <returns>Trả về mã nhân viên</returns>
        [HttpGet]
        [Route("NewEmployeeCode")]
        public IActionResult GetAutoIncrementEmployeeCode()
        {
            try
            {

                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string getEmployeeCode = "SELECT EMPLOYEECODE FROM EMPLOYEE;";

                var employeeCodeList = mySqlConnection.Query(getEmployeeCode);

                if (employeeCodeList != null)
                { 
                    var maxCodeValue = employeeCodeList.Max(emp => int.Parse(emp.EMPLOYEECODE.Substring(2)));
                    return StatusCode(StatusCodes.Status200OK, $"NV{maxCodeValue+1}");
                }
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }

        }


        /// <summary>
        /// Lấy danh sách nhân viên theo trang
        /// </summary>
        /// <param name="filterWord"></param>
        /// <param name="positionID"></param>
        /// <param name="departmentID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("filter")]
        public IActionResult FilterEmployees(
            [FromQuery] string? filterWord,
            [FromQuery] Guid? positionID,
            [FromQuery] Guid? departmentID,
            [FromQuery] int pageSize=10,
            [FromQuery] int pageNumber=1)
        {
            try
            {
                string connectionString = "Server =localhost; Port =3306; Database =hust.21h.2022.nhlinh; Uid =root; Pwd =hpyDZK46@";
                var mySqlConnection = new MySqlConnection(connectionString);

                string procedureGetPagingQuery = "Proc_employee_GetPaging";


                var parameters = new DynamicParameters();
                parameters.Add("@v_Offset", (pageNumber - 1) * pageSize);
                parameters.Add("@v_Limit", pageSize);
                parameters.Add("@v_Sort", "MODIFIEDDATE DESC");
                string whereClause = "";
                List<string> orConditions = new List<string>();
                List<string> andConditions = new List<string>();

                if (filterWord != null)
                {
                    orConditions.Add($"EMPLOYEEID LIKE '%{filterWord}%'");
                    orConditions.Add($"EMPLOYEENAME LIKE '%{filterWord}%'");
                    orConditions.Add($"PHONENUMBER LIKE '%{filterWord}%'");
                }
                if (orConditions.Count > 0) whereClause += $"({string.Join(" OR ", orConditions)})";

               

                if (positionID != null) andConditions.Add($"POSITIONID LIKE '%{positionID}'");
                if (departmentID != null) andConditions.Add($"DEPARTMENTID LIKE '%{departmentID}'");
                if(orConditions.Count != 0 && andConditions.Count!=0) whereClause += $" AND ";
                if (andConditions.Count > 0) whereClause += $"{string.Join(" AND ", andConditions)}";
                 
                parameters.Add("@v_Where", whereClause);

                var multipleTables = mySqlConnection.QueryMultiple(procedureGetPagingQuery, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (multipleTables != null)
                {
                    var employees = multipleTables.Read<Employee>().ToList();
                    var totalCount = multipleTables.Read<long>().Single();
                    return StatusCode(StatusCodes.Status200OK, new PaingData()
                    {
                        Data = employees,
                        Total = totalCount
                    });
                }
                else return StatusCode(StatusCodes.Status400BadRequest, "e002");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
       
        }
    }
}