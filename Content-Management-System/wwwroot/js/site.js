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