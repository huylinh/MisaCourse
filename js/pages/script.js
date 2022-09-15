$(document).ready(function () {
  initEvents();
  loadDataCombobox();
  loadData();
});

/**
 * Biến cục bộ
 */
var employee = {}; // sử dụng để lưu dữ liêu thay đổi từ form
var employeeID = null; // sử dụng để tìm thông tin nhân viên cho sự kiện double click
var format; // button lưu giờ sử dụng cho cả thêm mới và cả sửa nên phải dùng chung.
// Biến sử dụng cho phân trang
var pageSize = 10;
var pageNumber = 1;
var positionID = "";
var departmentID = "";
var filterWord = "";
var totalPages;
var paginationSize = 8; //Tức là có 8 số để chuyển trang
/**
 * Tạo các sự kiện
 * Author: Nguyễn Huy Linh
 */
function initEvents() {
  //Hiện form khi thêm nhân viên mới
  $("#btnAdd").click(function () {
    //Hiển thị form
    $("#employeeForm").show();
    //Khởi tạo lại employee rỗng và form rỗng
    employee = {};
    $(".dialog__body input,.dialog__body select").val("");
    $("#employeeCode").focus();
    $.ajax({
      url: "http://localhost:6268/api/Employees/NewEmployeeCode",
      method: "GET",
      success: function (newEmployeeCode) {
        $("#employeeCode").val(newEmployeeCode);
      },
    });
    format = "add";
  });

  //Bấm close form
  $(".dialog__btn--close").click(function () {
    $(this).parents(".dialog").hide();
  });

  //Bấm cancle form
  $(".dialog__btn--cancle").click(function () {
    $(this).parents(".dialog").hide();
  });

  // click thi highlight dòng trong bảng
  $(document).on("click", ".tbEmployeelst tbody tr", function () {
    // Xóa tất cả các trạng thái được chọn của các dòng dữ liệu khác:
    $(this).siblings().removeClass("row-selected");
    // In đậm dòng được chọn:
    this.classList.add("row-selected");
    employeeID = $(this).data("id");
  });

  //Bấm lưu là lưu cả thông tin người dùng lẫn thông tin chỉnh sửa
  $("#btnSave").click(function () {
    if (format == "add") {
      if (validateData()) {
        if (!employee["salary"]) employee["salary"] = 0;
        employee["TaxCode"] = "";
        employee["CreatedBy"] = "";
        employee["ModifiedBy"] = "";
        employee["PositionName"] = "";
        employee["DepartmentName"] = "";
        employee["ModifiedDate"] = new Date();
        console.log(JSON.stringify(employee));
        $.ajax({
          type: "POST",
          url: "http://localhost:6268/api/Employees",
          data: JSON.stringify(employee),
          dataType: "json",
          contentType: "application/json",
          success: function (response) {
            alert("Thêm mới dữ liệu thành công!");
            loadData();
            $("#employeeForm").hide();
          },
        });
      }
    } else if (format == "edit") {
      //Kiem tra lai du lieu moi roi update
      //http://localhost:6268/api/Employees/3fa85f64-5717-4562-b3fc-2c963f66afa6

      employee = $(".row-selected").data("entity");

      if (validateData()) {
        employee["ModifiedDate"] = new Date();
        $.ajax({
          type: "PUT",
          url: "http://localhost:6268/api/Employees/" + employeeID,
          data: JSON.stringify(employee),
          dataType: "json",
          contentType: "application/json",
          success: function (response) {
            alert("Sửa thông tin thành công");
            loadData();
            $("#employeeForm").hide();
          },
        });
      }
    }
  });

  //double click sẽ hiện dữ liệu lấy từ database về lền form
  $(document).on("dblclick", ".tbEmployeelst tbody tr", function () {
    $("#employeeForm").show();
    $("#employeeCode").focus();
    fillForm();
    format = "edit";
  });

  //Bấm nút xóa hiện dialog cảnh báo người dùng có muốn xóa bản ghi hay không và xóa
  $("#btnDelete").click(function () {
    if (employeeID) {
      $("#deleteMsg").show();
    } else alert("Bạn chưa chọn dòng cần xóa");
  });

  //Xac nhan xoa ban ghi
  $(".msg__btn--delete").click(function () {
    $.ajax({
      type: "DELETE",
      url: "http://localhost:6268/api/Employees/" + employeeID,
      success: function (response) {
        $("#deleteMsg").hide();
        alert("Xóa bản ghi thành công");
        // Load lại dữ liệu:
        loadData();
      },
    });
  });

  //Nhan ban
  $("#btnDuplicate").click(function () {
    if (employeeID) {
      $("#employeeForm").show();
      fillForm();
      format = "add";
    } else alert("Chưa chọn bản ghi để nhân bản!");
  });

  //Lựa chọn bao nhiêu bản ghi trên một trang
  $(".table__paging--right select").change(function () {
    pageSize = $(this).val();
    loadData();
  });

  //Tìm kiếm thông tin theo combobox
  $(".page__toolbar--left #departmentID").change(function () {
    departmentID = $(this).val();
    loadData();
  });
  $(".page__toolbar--left #positionID").change(function () {
    positionID = $(this).val();
    loadData();
  });
  $(".page__toolbar--left input").keyup(function () {
    filterWord = $(this).val();
    loadData();
  });

  //Load lại trang ban đầu
  $("#btnRefresh").click(function () {
    employee = {}; // sử dụng để lưu dữ liêu thay đổi từ form
    employeeID = ""; // sử dụng để tìm thông tin nhân viên cho sự kiện double click
    format = ""; // button lưu giờ sử dụng cho cả thêm mới và cả sửa nên phải dùng chung.
    pageSize = 10;
    positionID = "";
    departmentID = "";
    filterWord = "";
    $(".row-selected").removeClass("row-selected");
    loadData();
    $(".page__toolbar--left #departmentID").val("");
    $(".page__toolbar--left #positionID").val("");
    $(".page__toolbar--left input").val("");
  });

  //Phân trang
  $(".paging__number").click(function () {
    pageNumber = $(this).text();
    $(this).siblings().removeClass("paging__number--selected");
    $(this).addClass("paging__number--selected");
    loadData();
  });
}
// Vẽ lên form data của nhân viên đang được chọn
function fillForm() {
  $.ajax({
    type: "GET",
    url: "http://localhost:6268/api/Employees/" + employeeID,
    success: function (employee) {
      inputElements = $("#employeeForm input,#employeeForm select");
      for (const input of inputElements) {
        //luu lai du lieu moi nhat cua nhan vien tu database
        $(".row-selected").data("id", employee.employeeID);
        employeeID = employee.employeeID;
        $(".row-selected").data("entity", employee);
        //vẽ thông tin lên form
        propVal = input["id"];
        if (input["type"] == "date") {
          console.log(yymmddFormat(employee[propVal]));
          input.value = yymmddFormat(employee[propVal]);
        } else if (employee[propVal]) input.value = employee[propVal];
      }
    },
  });
}

//Kiểm tra dữ liệu liệu đầu vào, cập nhật biến cục bộ employee
function validateData() {
  let countError = 0;
  const inputElements = $("#employeeForm input,#employeeForm select");
  for (input of inputElements) {
    const propVal = input["id"]; // id duoc dat theo giong voi propertiy value nen su dung chung duoc
    const isOK = checkValue(input, propVal);
    if (isOK) {
      if (propVal == "salary") {
        employee[propVal] = parseFloat(input.value);
      }

      employee[propVal] = input.value;
    } else {
      countError++;
    }
  }
  if (countError == 0) return true;
  else return false;
}

//format lại ngày tháng năm để cho lên form
function yymmddFormat(date) {
  if (date) {
    date = new Date(date);

    // Lấy ra ngày:
    dateValue = date.getDate();
    dateValue = dateValue < 10 ? `0${dateValue}` : dateValue;

    // lấy ra tháng:
    let month = date.getMonth() + 1;
    month = month < 10 ? `0${month}` : month;

    // lấy ra năm:
    let year = date.getFullYear();
    if (year == 1) return "";
    return `${year}-${month}-${dateValue}`;
  }
}

//Validate từng dữ liệu
function checkValue(input, propValue) {
  // console.log(propValue);
  let isDuplicateCode = false;
  switch (propValue) {
    case "employeeCode":
      $.ajax({
        url: "http://localhost:6268/api/Employees",
        method: "GET",
        async: false,
        success: function (employees) {
          for (const emp of employees) {
            if (emp.employeeCode == input.value) {
              $(input).addClass("input--error");
              isDuplicateCode = true;
              //neu la edit thi chi phep chinh sua code nhung ma khong duoc trung voi code nguoi khac
              if (format == "edit") isDuplicateCode = false;
            }
          }
        },
        error: function (employees) {
          console.log(employees);
        },
      });

      if (!input.value || isDuplicateCode) {
        $(input).addClass("input--error");
        if (isDuplicateCode) $(input).attr("title", "Mã nhân viên bị trùng");
        else $(input).attr("title", "Thông tin này không được phép để trống");
        return false;
      } else {
        $(input).removeClass("input--error");
        $(input).removeAttr("title");
        return true;
      }
    case "employeeName":
    case "identityNumber":
    case "phoneNumber":
      if (input.value) {
        $(input).removeClass("input--error");
        $(input).removeAttr("title");
        return true;
      } else {
        $(input).addClass("input--error");
        $(input).attr("title", "Thông tin này không được phép để trống");
        return false;
      }
    case "email":
      if (!input.value) {
        $(input).addClass("input--error");
        $(input).attr("title", "Thông tin này không được phép để trống");
        return false;
      } else if (!checkEmailFormat(input.value)) {
        $(input).addClass("input--error");
        $(input).attr("title", "Email không đúng định dạng");
        return false;
      } else {
        $(input).removeClass("input--error");
        $(input).removeAttr("title");
        return true;
      }
    //email phai dung dinh dang
    case "dateOfBirth":
    case "joiningDate":
    case "identityIssuedDate":
      const date = new Date(input.value);
      if (date.getTime() < new Date()) {
        $(input).removeClass("input--error");
        $(input).removeAttr("title");
        return true;
      } else {
        $(input).addClass("input--error");
        $(input).attr("title", "Ngày không hợp lệ");
        return false;
      }
    default:
      return true;
  }
}

/**
 * Load dữ liệu
 * Author: nhlinh
 *
 * */
function loadData() {
  //Lay du lieu
  $.ajax({
    url: `http://localhost:6268/api/Employees/filter?filterWord=${filterWord}&positionID=${positionID}&departmentID=${departmentID}&pageSize=${pageSize}&pageNumber=${pageNumber}`,
    method: "GET",
    async: false,
    success: function (employees) {
      $(".tbEmployeelst tbody").empty();
      //Lay ra tat ca cac cot roi lay gia tri thuoc tinh can dua vao trong bang
      const thList = $(".tbEmployeelst thead tr th");
      let sort = pageSize * (pageNumber - 1) + 1;
      console.log(employees);
      for (const emp of employees.data) {
        // duyệt từng cột trong tiêu đề:
        let trElement = $("<tr></tr>");
        for (const th of thList) {
          // Lấy ra propValue tương ứng với các cột:
          const propValue = $(th).attr("propValue");
          const format = $(th).attr("format");
          // Lấy giá trị tương ứng với tên của propValue trong đối tượng:
          let value = null;
          if (propValue == "Sort") value = sort;
          else value = emp[propValue];

          switch (format) {
            case "date":
              value = formatDate(value);
              break;
            case "money":
              value = formatMoney(value);
              break;
            case "workStatus":
              switch (value) {
                case 0:
                  value = "Nghỉ việc";
                  break;
                case 1:
                  value = "Đang làm việc";
                  break;
                case 2:
                  value = "Nghỉ phép";
                  break;
                case 3:
                  value = "Thử việc";
                  break;
                case 4:
                  value = "Thực tập sinh";
                  break;
              }
              break;
            case "gender":
              switch (value) {
                case 0:
                  value = "Khác";
                  break;
                case 1:
                  value = "Nam";
                  break;
                case 2:
                  value = "Nữ";
                  break;
              }
              break;
            default:
              break;
          }
          // Tạo thHTML:
          let thHTML = $(`<td>${value || ""}</td>`);
          // Đẩy vào trHMTL:
          trElement.append(thHTML);
        }
        sort++;
        $(trElement).data("id", emp.employeeID);
        $(trElement).data("entity", emp);
        //Cap nhat du lieu len tren bang
        $(".tbEmployeelst tbody").append(trElement);

        //Hiển thị số số nhân viên trên màn hình
        if (employees.total < pageSize) {
          $("#numberOfEmployees").text(
            `${pageSize * (pageNumber - 1) + 1}-${employees.total}/${
              employees.total
            }`
          );
        } else {
          $("#numberOfEmployees").text(
            `${pageSize * (pageNumber - 1) + 1}-${pageSize * pageNumber}/${
              employees.total
            }`
          );
        }
        // Tính số trang
        totalPages = Math.ceil(employees.total / pageSize);
        drawPagination();
      }
    },
    error: function (employees) {
      console.log(employees);
    },
  });
}

/**
 * Vẽ lại pagination môi khi load dữ liệu lại từ đầu
 * Author: nhlinh
 */

function drawPagination() {}

/**
 * Load dữ liệu từ database lên combobox, form
 * Author : nhlinh
 */
function loadDataCombobox() {
  //Đặt 2 chỗ id giống nhau ?? Vi phạm không nhỉ?
  const searchDepartmentCombobox = $(".page__toolbar--left #departmentID");
  const searchPositionCombobox = $(".page__toolbar--left #positionID");
  const formDepartmentCombobox = $(".dialog__body #departmentID");
  const formPositionCombobox = $(".dialog__body #positionID");
  searchDepartmentCombobox.empty();
  searchPositionCombobox.empty();
  formDepartmentCombobox.empty();
  formPositionCombobox.empty();
  $.ajax({
    url: "http://localhost:6268/api/Departments",
    method: "GET",
    success: function (departments) {
      const optionElement0 = $(`<option value="">Chọn phòng ban</option>`);
      searchDepartmentCombobox.append(optionElement0);
      for (const department of departments) {
        const optionElement1 = $(
          `<option value="${department["departmentID"]}">${department["departmentName"]}</option>`
        );
        const optionElement2 = $(
          `<option value="${department["departmentID"]}">${department["departmentName"]}</option>`
        );
        searchDepartmentCombobox.append(optionElement1);
        formDepartmentCombobox.append(optionElement2);
      }
    },
  });

  $.ajax({
    url: "http://localhost:6268/api/Positions",
    method: "GET",
    success: function (positions) {
      const optionElement0 = $(`<option value="">Chọn vị trí</option>`);
      searchPositionCombobox.append(optionElement0);
      for (const position of positions) {
        const optionElement1 = $(
          `<option value="${position["positionID"]}">${position["positionName"]}</option>`
        );
        const optionElement2 = $(
          `<option value="${position["positionID"]}">${position["positionName"]}</option>`
        );
        searchPositionCombobox.append(optionElement1);
        formPositionCombobox.append(optionElement2);
      }
    },
  });
}

/**
 * format Date
 * Author : nhlinh
 */
function formatDate(date) {
  if (date) {
    date = new Date(date);

    // Lấy ra ngày:
    dateValue = date.getDate();
    dateValue = dateValue < 10 ? `0${dateValue}` : dateValue;

    // lấy ra tháng:
    let month = date.getMonth() + 1;
    month = month < 10 ? `0${month}` : month;

    // lấy ra năm:
    let year = date.getFullYear();
    if (year == 1) return "";
    return `${dateValue}/${month}/${year}`;
  }
}
/**
 * format money
 * Author : nhlinh
 */
function formatMoney(number) {
  if (!number) return "";
  return number.toLocaleString("it-IT", { style: "currency", currency: "VND" });
}

//format Email
function checkEmailFormat(email) {
  const re =
    /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
  return email.match(re);
}
