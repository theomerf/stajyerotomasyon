function openModal(date) {
    const modal = document.getElementById('eventModal');
    const modalTitle = document.getElementById('modalTitle');
    const selectedDateInput = document.getElementById('selectedDate');

    modalTitle.textContent = `${new Date(date).toLocaleDateString('tr-TR')} için Etkinlik`;
    selectedDateInput.value = date;

    document.getElementById('eventForm').reset();
    selectedDateInput.value = date; 

    modal.style.display = 'block';
}

function deleteEvent(event) {
    event.stopPropagation();
    hideTooltip();
}

function updatePrevButtonUrl() {
    const year = parseInt(document.getElementById('modelYear')?.dataset.year);
    const month = parseInt(document.getElementById('modelMonth')?.dataset.month);

    let newYear = year;
    let newMonth = month - 1;

    if (newMonth < 1) {
        newMonth = 12;
        newYear--;
    }

    const url = `/Calendar/Index?year=${newYear}&month=${newMonth}`;
    setTimeout(() => {
        htmx.ajax('GET', url, {
            target: '#calendarContainer',
            headers:
            {
                'Date-Send' : 'true'
            }
        });
    }, 160);
}

function updateNextButtonUrl() {
    const year = parseInt(document.getElementById('modelYear')?.dataset.year);
    const month = parseInt(document.getElementById('modelMonth')?.dataset.month);

    let newYear = year;
    let newMonth = month + 1;

    if (newMonth > 12) {
        newMonth = 1;
        newYear++;
    }

    const url = `/Calendar/Index?year=${newYear}&month=${newMonth}`;
    setTimeout(() => {
        htmx.ajax('GET', url, {
            target: '#calendarContainer',
            headers:
            {
                'Date-Send': 'true'
            }
        });
    }, 160);

}

function closeModal() {
    document.getElementById('eventModal').style.display = 'none';
}

function showTooltip(e, date, events) {
    const tooltip = document.getElementById('tooltip');

    if (events && events.length > 0) {
        let tooltipContent = `<strong>${new Date(date).toLocaleDateString('tr-TR')}</strong><br>`;
        events.forEach(event => {
            tooltipContent += `• ${event.time} - ${event.title}<br>`;
        });

        tooltip.innerHTML = tooltipContent;
        tooltip.style.left = e.pageX + 10 + 'px';
        tooltip.style.top = e.pageY - 10 + 'px';
        tooltip.classList.add('show');
    }
}

window.changeSelects = function () {
    var selectedMonth = document.getElementById('modelMonth').dataset.month;
    var selectedYear = document.getElementById('modelYear').dataset.year;

    document.getElementById('yearSelect').value = selectedYear;
    document.getElementById('monthSelect').value = selectedMonth;
}

function hideTooltip() {
    document.getElementById('tooltip').classList.remove('show');
}

window.addEventListener('click', function (e) {
    const modal = document.getElementById('eventModal');
    if (e.target === modal) {
        closeModal();
    }
});

