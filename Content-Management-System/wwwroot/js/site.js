/* showForm() is a function that shows modals.
   hideForm() is a function that hides modals.
*/
function showForm(modalID) {
    var modal = new bootstrap.Modal(document.getElementById(modalID));
    modal.show();
}

function hideForm(modalID) {
    var modal = new bootstrap.Modal(document.getElementById(modalID));
    modal.hide();
}

// Redirect to any page
function redirectToPage(link) {
    window.location.href = link;
}

/**
 * Generic form submit handler
 * @param {string} submitBtnID - Button ID that triggers submit
 * @param {string} endpoint - API/controller endpoint (e.g. "/Announcements/AddAnnouncement")
 * @param {function} getPayload - Callback that returns the JSON payload to send
 * @param {function} onSuccess - Callback to run on success
 */
function initFormSubmit(submitBtnID, endpoint, getPayload, onSuccess) {
    const submitBtn = document.getElementById(submitBtnID);
    if (!submitBtn) return;

    submitBtn.addEventListener("click", function (e) {
        e.preventDefault();

        const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenInput ? tokenInput.value : "";

        const payload = getPayload();

        fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify(payload)
        })
        .then(response => {
            if (!response.ok) throw new Error(`Server returned ${response.status}`);
            return response.json();
        })
        .then(result => {
            if (result.success) {
                Swal.fire({
                    icon: "success",
                    title: "Success",
                    text: result.message,
                    confirmButtonText: "OK",
                    confirmButtonColor: "#198754"
                }).then(() => {
                    if (onSuccess) onSuccess(result);
                });
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: result.message || "Something went wrong.",
                    confirmButtonText: "OK",
                    confirmButtonColor: "#dc3545"
                });
            }
        })
        .catch(err => {
            console.error("Error submitting form:", err);
            Swal.fire({
                icon: "error",
                title: "Error",
                text: "An unexpected error occurred.",
                confirmButtonText: "OK",
                confirmButtonColor: "#dc3545"
            });
        });
    });
}

/**
 * Enable sorting for any table with Bootstrap icons
 * @param {string} tableSelector - CSS selector for the table (e.g. "table" or "#myTable")
 * @param {number[]} excludeCols - (optional) array of column indexes (0-based) to exclude
 */
function enableTableSorting(tableSelector, excludeCols = []) {
    const table = document.querySelector(tableSelector);
    if (!table) return;

    const headers = table.querySelectorAll("thead th");

    headers.forEach((header, index) => {
        // Skip excluded columns
        if (excludeCols.includes(index)) {
            return;
        }

        // Wrap header text in a span so we can add icons after it
        if (!header.querySelector(".sort-text")) {
            const text = header.innerHTML.trim();
            header.innerHTML = `<span class="sort-text">${text}</span> <span class="sort-icon"></span>`;
        }

        header.style.cursor = "pointer";
        header.addEventListener("click", () => {
            const tbody = table.querySelector("tbody");
            const rows = Array.from(tbody.querySelectorAll("tr"));
            const isAscending = header.classList.contains("asc");

            // Reset all headers
            headers.forEach(h => {
                h.classList.remove("asc", "desc");
                const icon = h.querySelector(".sort-icon");
                if (icon) icon.innerHTML = "";
            });

            // Toggle sorting direction
            header.classList.toggle("asc", !isAscending);
            header.classList.toggle("desc", isAscending);

            // Add arrow icon
            const icon = header.querySelector(".sort-icon");
            if (icon) {
                icon.innerHTML = header.classList.contains("asc")
                    ? '<i class="bi bi-caret-up-fill"></i>'
                    : '<i class="bi bi-caret-down-fill"></i>';
            }

            // Sort rows
            rows.sort((a, b) => {
                const aText = a.cells[index].innerText.trim();
                const bText = b.cells[index].innerText.trim();

                // Try numeric comparison first
                const aNum = parseFloat(aText.replace(/[^0-9.-]+/g, ""));
                const bNum = parseFloat(bText.replace(/[^0-9.-]+/g, ""));
                if (!isNaN(aNum) && !isNaN(bNum)) {
                    return isAscending ? bNum - aNum : aNum - bNum;
                }

                // Fallback to string comparison
                return isAscending
                    ? bText.localeCompare(aText)
                    : aText.localeCompare(bText);
            });

            // Re-append sorted rows
            rows.forEach(row => tbody.appendChild(row));
        });
    });
}

// Disable button and show loading
function showLoading(buttonClass, textClass, spinnerClass) {
    var button = document.querySelector(buttonClass);
    var text = document.querySelector(textClass);
    var spinner = document.querySelector(spinnerClass);

    button.disabled = true;
    text.style.display = "none";
    spinner.style.display = "inline-block";
}


