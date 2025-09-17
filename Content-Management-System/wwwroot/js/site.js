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