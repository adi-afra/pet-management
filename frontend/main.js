// Select all filter buttons
const filterButtons = document.querySelectorAll('.filter-btn');

// Add click event to each button
filterButtons.forEach(button => {
    button.addEventListener('click', () => {

        // Remove active class from all buttons
        filterButtons.forEach(btn => {
            btn.classList.remove('active');
        });

        // Add active class to the clicked button
        button.classList.add('active');
    });
});
