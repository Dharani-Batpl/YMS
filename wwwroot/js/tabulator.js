// Ensure this file is included after Tabulator and Bootstrap CSS are loaded

// Set global Tabulator default options
Tabulator.defaultOptions.ajaxLoader = true;

Tabulator.defaultOptions.ajaxLoaderLoading = `
  <div class="text-primary d-flex align-items-center justify-content-center" style="height:100%;">
    <div class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></div>
    Loading data...
  </div>`;

Tabulator.defaultOptions.ajaxLoaderError = `
  <div class="text-danger d-flex align-items-center justify-content-center" style="height:100%;">
    <i class="bi bi-exclamation-triangle me-2"></i>
    Loading Error
  </div>`;

// Optionally, set other global defaults here if needed
