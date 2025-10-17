// Add to existing Scripts section
document.addEventListener('DOMContentLoaded', function() {
    // Make table rows expandable on mobile
    if (window.innerWidth < 768) {
        const contentCells = document.querySelectorAll('.content-col .cell-content');
        contentCells.forEach(cell => {
            if (cell.scrollHeight > 150) {
                cell.addEventListener('click', function() {
                    this.style.maxHeight = 
                        this.style.maxHeight === 'none' ? '150px' : 'none';
                });
            }
        });
    }

    // Improve filter dropdowns on mobile
    const filterSelects = document.querySelectorAll('.filter-item select');
    filterSelects.forEach(select => {
        select.addEventListener('change', function() {
            // Add visual feedback
            this.classList.add('changed');
            setTimeout(() => this.classList.remove('changed'), 300);
        });
    });
});