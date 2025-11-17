// ====================
// 📦 Global Utility: Download CSV Template
// ====================
window.downloadTemplate1 = function (headers, fileName = "Template") {
    if (!Array.isArray(headers) || headers.length === 0) {
        console.error("downloadTemplate: headers must be a non-empty array.");
        return;
    }

    // Create CSV content
    const csvHeader = headers.join(",") + "\n";
    const blob = new Blob([csvHeader], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);

    // Create hidden download link
    const link = document.createElement("a");
    link.setAttribute("href", url);
    link.setAttribute("download", `${fileName}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    // Optional cleanup
    URL.revokeObjectURL(url);
};
window.downloadTemplate = function (headers, fileName = "Template") {
    if (!Array.isArray(headers) || headers.length === 0) {
        console.error("downloadTemplate: headers must be a non-empty array.");
        return;
    }

    // Create worksheet with only header row
    const worksheet = XLSX.utils.aoa_to_sheet([headers]);

    // Create workbook
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

    // Trigger download as .xlsx
    XLSX.writeFile(workbook, `${fileName}.xlsx`);
};
