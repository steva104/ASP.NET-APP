
var dataTable;


$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
        dataTable = $('#tblProduct').DataTable({
            "ajax": { url:'/admin/product/getall'},
            "columns": [
            { data: 'title' ,"width":"25%"},
            { data: 'artist' ,"width": "15%" },
            { data: 'year', "width": "10%", "className": "text-center" },
            { data: 'upc', "width": "20%" },
            { data: 'listPrice', "width": "10%", "className": "text-center" },
            { data: 'genre.name', "width": "10%" },
            {
                    data: 'id',
                    "render" : function (data) {
                        return `<div class="w-75 btn-group role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-outline-primary btn-sm mx-2">  <i class="bi bi-pencil-square"></i>Edit</a>
                         <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-outline-danger btn-sm mx-2"> <i class="bi bi-trash"></i>Delete</a>
                        </div >`
                    },
                    "width": "20%"
            }
        ]
    });
}


function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                
                success: function (data) {
                    if (data.success === false) {
                        toastr.error(data.message); 
                    } else if (data.success === true) {
                        dataTable.ajax.reload();
                        toastr.success(data.message); 
                    } else {
                        toastr.error("Unexpected server response.");
                    }
                },              
            })
        }
        
        
    });
}

