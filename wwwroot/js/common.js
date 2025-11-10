// ===============================
// 🔹 Common Utility Functions
// ===============================

// 1️⃣ Initialize Bootstrap tooltips
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(el => new bootstrap.Tooltip(el));
}

// 2️⃣ Restrict input to only 0 or 1
function restrictToBinaryInput(elementId) {
    const input = document.getElementById(elementId);
    if (!input) return;
    input.addEventListener('input', function () {
        if (this.value !== '' && this.value !== '0' && this.value !== '1') {
            this.value = this.value.slice(0, -1);
        }
    });
}


//add  in common.js
function updateRecordCount(dataTable, data, fieldName) {
    let recordCount = data.length;

    // If there is only one row, check the specified field of the first row
    if (data.length === 1) {
        let firstRowData = dataTable.getData()[0]; // Get the first row from the dataTable
        // Check if the row exists and then get the value for the specified field
        let firstRowFieldValue = firstRowData ? firstRowData[fieldName] : null;

        // If the field value is null, 0, or empty, set the record count to 0
        if (firstRowFieldValue === null || firstRowFieldValue === 0 || firstRowFieldValue === "") {
            recordCount = 0;
        }
    }

    // Update the record count display
    document.getElementById("recordCount").innerText = "Total Records: " + recordCount;
}


// 3️⃣ Common SweetAlert wrapper
function showAppAlert({ type = 'alert', icon = 'info', title = '', text = '', confirmButtonText = 'OK', timer = null }) {
    const baseOptions = {
        icon,
        title,
        text,
        confirmButtonText,
        allowOutsideClick: false
    };

    if (timer) baseOptions.timer = timer;

    if (type === 'confirm') {
        return Swal.fire({
            ...baseOptions,
            showCancelButton: true,
            confirmButtonText: confirmButtonText || 'Yes',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#1f2a57',
            cancelButtonColor: '#6c757d'
        });
    } else {
        return Swal.fire(baseOptions);
    }
}

// 4️⃣ Reusable toast alert (for success/error/info)
// ==========================
// 🔹 Toast Alert Helper
// ==========================
function showToast({ icon = 'success', text = '', timer = 10000, position = 'top-end' }) {
    Swal.fire({
        toast: true,
        icon,
        title: text,
        position,
        showConfirmButton: false,
        timer,
        timerProgressBar: true,
        background: '#fff',
        customClass: {
            popup: 'swal2-toast-fixed'
        },
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });
}


// ==========================
// 🔹 Generate Tabulator columns dynamically
// ==========================
function generateTabulatorColumns(data) {
    if (!data || data.length === 0) return [];

    const fields = Object.keys(data[0]);

    const formatTitle = (str) => {
        let result = str.replace(/[_\-]/g, ' ');
        result = result.replace(/([a-z])([A-Z])/g, '$1 $2');
        return result.replace(/\w\S*/g, (txt) => txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase());
    };

    return fields.map(field => ({ title: formatTitle(field), field }));
}

// 5️⃣ Common Tabulator initializer
//function initializeTabulator(selector, data, columns, rowSelectionCallback = null) {
//    return new Tabulator(selector, {
//        data,
//        layout: "fitColumns",
//        columns,
//        pagination: "local",
//        paginationSize: 10,
//        paginationSizeSelector: [10, 20, 50],
//        paginationButtonCount: 5,
//        theme: "bootstrap5",
//        selectable: 1,
//        rowSelectionChanged: (data, rows) => {
//            if (typeof rowSelectionCallback === "function") rowSelectionCallback(data);
//        }
//    });
//}


function initializeTabulator(selector, data, columns, rowSelectionCallback = null) {
    //const adjustedColumns = columns.map(column => {
    //    console.log(data[0][column.field]);
    //    // Check if the column field is a date field in the data
    //    if (data.length > 0 && typeof data[0][column.field] === "string") {
    //        const datePattern = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}([+-]\d{2}:\d{2}|Z)?$/; // Matches ISO 8601 with timezone
    //        if (datePattern.test(data[0][column.field])) {
    //            //const formatted = Intl.DateTimeFormat('en-GB', { dateStyle: 'short', timeStyle: 'short' }).format(data[0][column.field])
    //            return {
    //                ...column,
    //                formatter: "datetime",
    //                formatterParams: {
    //                    inputFormat: "iso",
    //                    outputFormat: "dd/MM/yyyy HH:mm:ss",
    //                    invalidPlaceholder: "(invalid)"
    //                }
    //            };
    //        }
    //    }
    //    return column; // Return original column if no date formatting is needed
    //});

    return new Tabulator(selector, {
        data,
        layout: "fitDataFill",       // table width grows with columns - gps old: fitData
        columns,
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 50],
        paginationButtonCount: 5,
        theme: "bootstrap5",
        selectable: 1,
        rowSelectionChanged: (data, rows) => {
            if (typeof rowSelectionCallback === "function") rowSelectionCallback(data);
        },
        responsiveLayout: false, // disables column hiding, allows horizontal scroll -- gps old: false
        movableColumns: true,    // optional, allows dragging columns
        autoResize: true,       // prevents auto fit to container -- gps old: false
        locale: true,
        langs: {
            "default": {
                "pagination": {
                    "first": "<<",
                    "first_title": "First Page",
                    "last": ">>",
                    "last_title": "Last Page",
                    "prev": "<",
                    "prev_title": "Prev Page",
                    "next": ">",
                    "next_title": "Next Page",
                }
            }
        }
    });
}


function initializeTabulatorFitColumns(selector, data, columns, rowSelectionCallback = null) {

    return new Tabulator(selector, {

        data: [],

        layout: "fitColumns",

        columns,

        pagination: "local",

        paginationSize: 3,

        pagination: "local",

        paginationSizeSelector: [3, 5],

        paginationButtonCount: 5,

        theme: "bootstrap5",

        selectable: 1,
        rowSelectionChanged: (data, rows) => {
            if (typeof rowSelectionCallback === "function") rowSelectionCallback(data);
        },
        responsiveLayout: false, // disables column hiding, allows horizontal scroll -- gps old: false
        movableColumns: true,    // optional, allows dragging columns
        autoResize: true,       // prevents auto fit to container -- gps old: false
        locale: true,
        langs: {
            "default": {
                "pagination": {
                    "first": "<<",
                    "first_title": "First Page",
                    "last": ">>",
                    "last_title": "Last Page",
                    "prev": "<",
                    "prev_title": "Prev Page",
                    "next": ">",
                    "next_title": "Next Page",
                }
            }
        }
    });

}


function downloadCsvTemplate(table, filename) {
    const columns = table.getColumns(); // Get all columns

    // Only include visible columns and get the title (alias) for CSV headers
    const headers = columns
        .filter(col => col.isVisible())  // Only visible columns
        .map(col => col.getDefinition().title || col.getField()); // Use title (alias) or field

    const csvContent = headers.join(",") + "\n"; // Only header row

    // Create a Blob and download the CSV
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = `${filename}.csv`;
    link.style.display = "none";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(link.href);
}

/**
* Download CSV from a Tabulator table
* @param {Tabulator} tableInstance - Instance of Tabulator table
* @param {string} fileName - Name of the CSV file to download
*/
function downloadCsv(tableInstance, fileName = "data.csv") {
    if (!tableInstance) {
        console.error("Table instance is required for download");
        return;
    }

    tableInstance.download("csv", fileName);
}

// 7️⃣ Excel upload handler
async function uploadExcelFile(apiUrl, inputId, table) {
    const fileInput = document.getElementById(inputId);
    if (!fileInput || fileInput.files.length === 0) {
        return showAppAlert({ icon: 'warning', title: 'No File', text: 'Please select a file to upload.' });
    }

    const file = fileInput.files[0];
    const formData = new FormData();
    formData.append("file", file);

    try {
        const response = await fetch(apiUrl, { method: "POST", body: formData });

        if (response.ok) {
            const contentDisposition = response.headers.get("content-disposition");
            if (contentDisposition && contentDisposition.includes("attachment")) {
                const blob = await response.blob();
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = "Invalid_Rows.xlsx";
                a.click();
                window.URL.revokeObjectURL(url);
                return showAppAlert({ icon: 'warning', title: 'Invalid Rows', text: 'Download the invalid file and correct it.' });
            } else {
                const result = await response.json();
                showAppAlert({ icon: 'success', title: 'Success', text: result.message || "Upload successful!" });
                if (table) table.replaceData();
            }
        } else {
            showAppAlert({ icon: 'error', title: 'Upload Failed', text: 'Server error. Please try again.' });
        }
    } catch (err) {
        showAppAlert({ icon: 'error', title: 'Error', text: err.message });
    } finally {
        fileInput.value = "";
    }
}

// ===============================
// 🔹 Common Save Record Helper
// ===============================
async function saveRecord(fetchUrl, formData, currentMode, addEditModal, formEl) {
    try {
        const response = await fetch(fetchUrl, {
            method: currentMode === "Add" ? "POST" : "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData)
        });

        const result = await response.json();
        console.log(result);

        // ✅ Show icon and text only
        showToast({
            icon: result.title.toLowerCase(), // e.g., 'success', 'error', 'info'
            text: result.message
        });

        if (response.ok) {
            if (formEl) formEl.reset();
            if (addEditModal) addEditModal.hide();
        }

        return response.ok;
    } catch (error) {
        showToast({
            icon: "error",
            text: error.message || "Action failed"
        });
        return false;
    }
}

// ===============================
// 🔹 Common Delete Record Helper
// ===============================
async function deleteRecord(fetchUrl, payload, method = "PUT") {
    try {
        const response = await fetch(fetchUrl, {
            method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        // Try to read JSON; fall back to a sane default
        let result = {};
        try { result = await response.json(); } catch { }

        // ✅ Show icon and text only
        showToast({
            icon: result.title.toLowerCase(), // e.g., 'success', 'error', 'info'
            text: result.message
        });

        return response.ok;
    } catch (error) {
        showToast({
            icon: "error",
            text: error.message || "Action failed."
        });
        return false;
    }
}


