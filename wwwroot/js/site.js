// Site-wide JavaScript functionality
$(document).ready(function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Update reservation badge count
    function updateReservationBadge() {
        $.get('/Home/GetReservationCount', function(count) {
            $('.reservation-badge').text(count);
        });
    }
    
    // Update badge on page load
    updateReservationBadge();

    // Initialize date range picker for search and details page
    if ($('#dateRange').length > 0) {
        const checkInDate = $('#checkInDate').val();
        const checkOutDate = $('#checkOutDate').val();
        
        $('#dateRange').daterangepicker({
            opens: 'left',
            autoUpdateInput: false,
            minDate: moment(),
            startDate: checkInDate ? moment(checkInDate) : moment().add(1, 'days'),
            endDate: checkOutDate ? moment(checkOutDate) : moment().add(3, 'days'),
            locale: {
                format: 'MM/DD/YYYY',
                cancelLabel: 'Clear'
            }
        });

        // Handle date selection
        $('#dateRange').on('apply.daterangepicker', function(ev, picker) {
            $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
            $('#checkInDate').val(picker.startDate.format('YYYY-MM-DD'));
            $('#checkOutDate').val(picker.endDate.format('YYYY-MM-DD'));
            
            // If on details page, update total cost
            if (typeof calculateTotalCost === 'function') {
                calculateTotalCost(picker.startDate, picker.endDate);
            }
        });

        $('#dateRange').on('cancel.daterangepicker', function(ev, picker) {
            $(this).val('');
            $('#checkInDate').val('');
            $('#checkOutDate').val('');
        });

        // Set initial display if dates exist
        if (checkInDate && checkOutDate) {
            $('#dateRange').val(moment(checkInDate).format('MM/DD/YYYY') + ' - ' + moment(checkOutDate).format('MM/DD/YYYY'));
        }
    }

    // Update reservation count badge
    function updateReservationCount() {
        $.get('/Home/GetReservationCount', function(count) {
            $('.reservation-badge').text(count);
        });
    }

    // Auto-dismiss alerts after 5 seconds
    $('.alert-dismissible').fadeTo(5000, 500).slideUp(500);

    // Handle reservation form submission
    $('#reservationForm').on('submit', function(e) {
        const startDate = $('#startDate').val();
        const endDate = $('#endDate').val();
        
        if (!startDate || !endDate) {
            e.preventDefault();
            alert('Please select check-in and check-out dates');
            return false;
        }
        
        if (moment(startDate).isSameOrAfter(moment(endDate))) {
            e.preventDefault();
            alert('Check-out date must be after check-in date');
            return false;
        }
    });
});