window.initApplicationsScripts = function () {
    window.contentContainer = document.querySelector('.applications-container');
    window.debounceTimeout = null;

    const urlParams = new URLSearchParams(window.location.search);
    window.pageSize = urlParams.get('PageSize');

    const currentSort = urlParams.get('SortBy');
    const currentDepartment = urlParams.get('DepartmentId') || '';
    const currentSearchTerm = urlParams.get('SearchTerm') || '';
    const currentStatus = urlParams.get('Status') || '';
    const currentStartDate = urlParams.get('StartDate') || '';
    const currentEndDate = urlParams.get('EndDate') || '';

    if (currentSort) {
        document.querySelectorAll(`[data-sort="${currentSort}"]`).forEach(btn => {
            btn.classList.add('active');
        });
        updateClearButtons();
    }

    const newDepartmentFilter = document.getElementById('departmentSelect');
    const newSearchInput = document.querySelector('.search-box input[type="text"]');
    const newStatusButton = [...document.querySelectorAll('.status-box')]
        .find(btn => btn.value === currentStatus);
    const newStartDateInput = document.querySelector('.dateFilterStart');
    const newEndDateInput = document.querySelector('.dateFilterEnd');

    if (newDepartmentFilter) newDepartmentFilter.value = currentDepartment;
    if (newSearchInput) newSearchInput.value = currentSearchTerm;
    if (currentStatus)
    {
        const allButtons = document.querySelectorAll('status-box');
        allButtons.forEach(btn => btn.classList.remove('active'));
        newStatusButton.classList.add("active");
    }
    if (currentStartDate) showDateFilterBtn(); newStartDateInput.value = currentStartDate;
    if (currentEndDate) showDateFilterBtn(); newEndDateInput.value = currentEndDate;
}

if (typeof (contentContainer) === 'undefined') {
    window.contentContainer = document.querySelector('.applications-container');
    window.debounceTimeout = null;

    const urlParams = new URLSearchParams(window.location.search);
    window.pageSize = urlParams.get('PageSize');
}

// View değiştirme işlevi
window.viewToggle = function (e) {
    const btn = e.target.closest('.toggle-btn');
    if (!btn) return;

    document.querySelectorAll('.toggle-btn').forEach(b => b.classList.remove('active'));
    btn.classList.add('active');

    var viewType = btn.getAttribute('data-viewType');

    const days = 7;
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    document.cookie = "View=" + viewType + "; expires=" + date.toUTCString() + "; path=/";

    document.querySelectorAll('.view-content').forEach(content => content.classList.remove('active'));
    document.querySelector('.controls-bottom-row').classList.remove('d-none');
    document.querySelector('.controls-bottom-row').classList.toggle('d-none', viewType !== 'Grid');

    const view = btn.dataset.view;
    document.querySelector(`.${view}-view`).classList.add('active');
};

window.changePageSize = function (e) {
    window.scrollTo({ top: 0, behavior: 'smooth' });
    const selectedPageSize = e.target.value;
    if (!selectedPageSize) return;

    const days = 7;
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    document.cookie = "PageSize=" + selectedPageSize + "; expires=" + date.toUTCString() + "; path=/";
}

// Filtreleme işlevi
window.filterApplications = function () {
    $('#loadingOverlay').show();

    const filterSelect = document.getElementById('departmentSelect');
    const searchInput = window.contentContainer.querySelector('.search-box input[type="text"]');
    const sortSelect = window.contentContainer.querySelector('.sort-btn.active');
    const statusSelect = window.contentContainer.querySelector('.status-box.active');
    const startDateInput = window.contentContainer.querySelector('.dateFilterStart');
    const endDateInput = window.contentContainer.querySelector('.dateFilterEnd');

    const departmentId = filterSelect?.value || '';
    const searchTerm = searchInput?.value.trim() || '';
    const status = statusSelect ? statusSelect.value : '';
    const startDate = startDateInput.value || '';
    const endDate = endDateInput.value || '';
    const selectedSort = sortSelect ? sortSelect.dataset.sort : '';

    const params = new URLSearchParams();
    if (pageSize) params.append('PageSize', pageSize);
    if (departmentId) params.append('DepartmentId', departmentId);
    if (searchTerm) params.append('SearchTerm', searchTerm);
    if (startDate) params.append('StartDate', startDate);
    if (endDate) params.append('EndDate', endDate);
    if (status) params.append('Status', status);
    if (selectedSort) params.append('SortBy', selectedSort);

    const urlParamsString = params.toString();
    const url = urlParamsString ? `/Applications/Index?${urlParamsString}` : `/Applications/Index`;

    setTimeout(() => {
        htmx.ajax('GET', url, {
            target: '#content',
            headers: {
                'Filter-Send': 'true'
            }
        });
        history.replaceState({}, '', url);
    }, 160);

}

// Durum filtreleme işlevi

window.statusFilterForApplications = function (event) {
    const clickedButton = event.currentTarget;
    const allButtons = clickedButton.parentElement.querySelectorAll('.status-box');

    const isAlreadyActive = clickedButton.classList.contains('active');

    allButtons.forEach(btn => btn.classList.remove('active'));

    if (!isAlreadyActive) {
        clickedButton.classList.add('active');
    }

    filterApplications();
}

// Tarihi filtrele butonunun gözükme işlevi

window.showDateFilterBtn = function () {
    const btn = document.getElementById('dateFilterBtn')
    const btnClear = document.getElementById('clearDateFilterBtn')
    btn.classList.remove('d-none');
    btn.classList.add('d-block');
    btnClear.classList.remove('d-none');
    btnClear.classList.add('d-block');
}

// Tarihi temizle butonunun işlevi

window.clearDate = function () {
    const startDateInput = window.contentContainer.querySelector('.dateFilterStart').value = '';
    const endDateInput = window.contentContainer.querySelector('.dateFilterEnd').value = '';

    filterApplications();
}


// Arama işlevi
window.searchFilterForApplications = function () {
    if (window.debounceTimeout) clearTimeout(debounceTimeout);
    window.debounceTimeout = setTimeout(filterApplications, 500);
}

// Sıralama butonları işlevi
window.sortingButton = function (e) {
    if (e.target.closest('.sort-btn')) {
        $('#loadingOverlay').show();
        const contentContainer = document.querySelector('.applications-container');
        const filterSelect = contentContainer.querySelector('.filter-select');
        const searchInput = contentContainer.querySelector('.search-box input[type="text"]');
        const sortSelect = window.contentContainer.querySelector('.sort-btn.active');
        const statusSelect = window.contentContainer.querySelector('.status-box.active');
        const startDateInput = window.contentContainer.querySelector('.dateFilterStart');
        const endDateInput = window.contentContainer.querySelector('.dateFilterEnd');

        const departmentId = filterSelect?.value || '';
        const searchTerm = searchInput?.value.trim() || '';
        const status = statusSelect ? statusSelect.value : '';
        const startDate = startDateInput.value || '';
        const endDate = endDateInput.value || '';
        const selectedSort = sortSelect ? sortSelect.dataset.sort : '';

        const btn = e.target.closest('.sort-btn');
        const sortBy = btn.dataset.sort;

        document.querySelectorAll('.sort-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        updateClearButtons();

        const urlParams = new URLSearchParams(window.location.search);
        if (departmentId) urlParams.set('DepartmentId', departmentId);
        if (searchTerm) urlParams.set('SearchTerm', searchTerm);
        if (startDate) urlParams.set('StartDate', startDate);
        if (endDate) urlParams.set('EndDate', endDate);
        if (status) urlParams.set('Status', status);
        if (selectedSort) urlParams.set('SortBy', selectedSort);
        urlParams.set('SortBy', sortBy);
        urlParams.delete('PageNumber');

        const urlParamsString = urlParams.toString();
        const url = urlParamsString ? `/Applications/Index?${urlParamsString}` : `/Applications/Index`;

        setTimeout(() => {
            htmx.ajax('GET', url, {
                target: '#content',
                headers: {
                    'Filter-Send': 'true'
                }
            });
            history.replaceState({}, '', url);
        }, 160);
    }
}

// Sıralama silme butonları işlevi
window.clearSortButtonForApplications = function (e) {
    if (e.target.closest('.clear-cell-sort-btn')) {
        $('#loadingOverlay').show();
        e.preventDefault();

        const contentContainer = document.querySelector('.applications-container');
        const filterSelect = contentContainer.querySelector('.filter-select');
        const searchInput = contentContainer.querySelector('.search-box input[type="text"]');
        const statusSelect = window.contentContainer.querySelector('.status-box.active');
        const sortSelect = window.contentContainer.querySelector('.sort-btn.active');
        const startDateInput = window.contentContainer.querySelector('.dateFilterStart');
        const endDateInput = window.contentContainer.querySelector('.dateFilterEnd');

        const departmentId = filterSelect?.value || '';
        const searchTerm = searchInput?.value.trim() || '';
        const status = statusSelect ? statusSelect.value : '';
        const startDate = startDateInput.value || '';
        const endDate = endDateInput.value || '';
        const selectedSort = sortSelect ? sortSelect.dataset.sort : '';

        const btn = e.target.closest('.clear-cell-sort-btn');
        const category = btn.dataset.category;

        document.querySelectorAll(`[data-sort*="${category}"]`).forEach(sortBtn => {
            sortBtn.classList.remove('active');
        });

        btn.classList.remove('show');

        const urlParams = new URLSearchParams(window.location.search);
        if (departmentId) urlParams.set('DepartmentId', departmentId);
        if (searchTerm) urlParams.set('SearchTerm', searchTerm);
        if (startDate) urlParams.set('StartDate', startDate);
        if (endDate) urlParams.set('EndDate', endDate);
        if (status) urlParams.set('Status', status);
        if (selectedSort) urlParams.set('SortBy', selectedSort);
        urlParams.delete('SortBy');
        urlParams.delete('PageNumber');

        const urlParamsString = urlParams.toString();
        const url = urlParamsString ? `/Applications/Index?${urlParamsString}` : `/Applications/Index`;

        setTimeout(() => {
            htmx.ajax('GET', url, {
                target: '#content',
                headers: {
                    'Filter-Send': 'true'
                }
            });
            history.replaceState({}, '', url);
        }, 160);
    }
}

// Sıralama silme butonlarını günceller
function updateClearButtons() {
    document.querySelectorAll('.clear-cell-sort-btn').forEach(btn => {
        btn.classList.remove('show');
    });

    document.querySelectorAll('.sort-btn.active').forEach(activeBtn => {
        const sortValue = activeBtn.dataset.sort;
        let category = '';

        if (sortValue.includes('NAME')) category = 'NAME';
        else if (sortValue.includes('DEPARTMENT')) category = 'DEPARTMENT';
        else if (sortValue.includes('DATE')) category = 'DATE';
        else if (sortValue.includes('ID')) category = 'ID';
        else if (sortValue === 'APPROVED' || sortValue === 'DENIED') category = 'STATUS';

        if (category) {
            document.querySelectorAll(`[data-category="${category}"]`).forEach(clearBtn => {
                clearBtn.classList.add('show');
            });
        }
    });
}

// Normal yenileme durumunda URL'den mevcut parametreleri kontrol et ve ilgili kısmı düzenle
document.addEventListener('DOMContentLoaded', function loadFromUrl() {
    const urlParams = new URLSearchParams(window.location.search);
    const currentSort = urlParams.get('SortBy');
    const departmentId = urlParams.get('DepartmentId') || '';
    const searchTerm = urlParams.get('SearchTerm') || '';
    const currentStatus = urlParams.get('Status') || '';
    const currentStartDate = urlParams.get('StartDate') || '';
    const currentEndDate = urlParams.get('EndDate') || '';

    const newFilterSelect = document.querySelector('.filter-select');
    const newSearchInput = document.querySelector('.search-box input[type="text"]');
    const newStatusButton = [...document.querySelectorAll('.status-box')]
        .find(btn => btn.value === currentStatus);
    const newStartDateInput = document.querySelector('.dateFilterStart');
    const newEndDateInput = document.querySelector('.dateFilterEnd');
    if (newFilterSelect) newFilterSelect.value = departmentId;
    if (newSearchInput) newSearchInput.value = searchTerm;
    if (currentStatus) {
        const allButtons = document.querySelectorAll('status-box');
        allButtons.forEach(btn => btn.classList.remove('active'));
        newStatusButton.classList.add("active");
    }
    if (currentStartDate) showDateFilterBtn(); newStartDateInput.value = currentStartDate;
    if (currentEndDate) showDateFilterBtn(); newEndDateInput.value = currentEndDate;

    if (currentSort) {
        document.querySelectorAll(`[data-sort="${currentSort}"]`).forEach(btn => {
            btn.classList.add('active');
        });
        updateClearButtons();
    }

}); 

window.updateSortButtons = function () {
    const urlParams = new URLSearchParams(window.location.search);
    const currentSort = urlParams.get('SortBy');

    if (currentSort) {
        document.querySelectorAll(`[data-sort="${currentSort}"]`).forEach(btn => {
            btn.classList.add('active');
        });
        updateClearButtons();
    }
}
