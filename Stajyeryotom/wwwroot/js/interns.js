window.initInternsScripts = function () {
    window.contentContainer = document.querySelector('.stajyer-container');
    window.debounceTimeout = null;

    const urlParams = new URLSearchParams(window.location.search);
    window.pageSize = urlParams.get('PageSize');

    const currentSort = urlParams.get('SortBy');
    const currentDepartment = urlParams.get('DepartmentId') || '';
    const currentSearchTerm = urlParams.get('SearchTerm') || '';

    if (currentSort) {
        document.querySelectorAll(`[data-sort="${currentSort}"]`).forEach(btn => {
            btn.classList.add('active');
        });
        updateClearButtons();
    }

    const newFilterSelect = document.querySelector('.filter-select');
    const newSearchInput = document.querySelector('.search-box input[type="text"]');
    if (newFilterSelect) newFilterSelect.value = currentDepartment;
    if (newSearchInput) newSearchInput.value = currentSearchTerm;
}

if (typeof (contentContainer) === 'undefined') {
    window.contentContainer = document.querySelector('.stajyer-container');
    window.debounceTimeout = null;

    const urlParams = new URLSearchParams(window.location.search);
    window.pageSize = urlParams.get('PageSize');
}


// Progress circle işlevi
function initProgressCircles() {
    document.querySelectorAll('.progress-circle').forEach(circle => {
        const progress = circle.dataset.progress;
        const progressRing = circle.querySelector('.progress-ring-fill');
        const radius = progressRing.r.baseVal.value;
        const circumference = radius * 2 * Math.PI;

        progressRing.style.strokeDasharray = `${circumference} ${circumference}`;
        progressRing.style.strokeDashoffset = circumference;

        setTimeout(() => {
            const offset = circumference - (progress / 100) * circumference;
            progressRing.style.strokeDashoffset = offset;
        }, 200);
    });
}

// View değiştirme işlevi
window.viewToggle = function(e) {
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
window.filterInterns = function() {
    $('#loadingOverlay').show();
    const filterSelect = window.contentContainer.querySelector('.filter-select');
    const searchInput = window.contentContainer.querySelector('.search-box input[type="text"]');
    const sortSelect = window.contentContainer.querySelector('.sort-btn.active');

    const departmentId = filterSelect?.value || '';
    const searchTerm = searchInput?.value.trim() || '';
    const selectedSort = sortSelect ? sortSelect.dataset.sort : '';

    const params = new URLSearchParams();
    if (pageSize) params.append('PageSize', pageSize);
    if (departmentId) params.append('DepartmentId', departmentId);
    if (searchTerm) params.append('SearchTerm', searchTerm);
    if (selectedSort) params.append('SortBy', selectedSort);

    const urlParamsString = params.toString();
    const url = urlParamsString ? `/Interns/Index?${urlParamsString}` : `/Interns/Index`;

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

// Arama işlevi
window.searchFilter = function () {
    if (window.debounceTimeout) clearTimeout(debounceTimeout);
    window.debounceTimeout = setTimeout(filterInterns, 500);
}

// Sıralama butonları işlevi
window.sortingButton = function(e) {
    if (e.target.closest('.sort-btn')) {
        $('#loadingOverlay').show();
        const contentContainer = document.querySelector('.stajyer-container');
        const filterSelect = contentContainer.querySelector('.filter-select');
        const searchInput = contentContainer.querySelector('.search-box input[type="text"]');

        const departmentId = filterSelect?.value || '';
        const searchTerm = searchInput?.value.trim() || '';

        const btn = e.target.closest('.sort-btn');
        const sortBy = btn.dataset.sort;

        document.querySelectorAll('.sort-btn').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        updateClearButtons();

        const urlParams = new URLSearchParams(window.location.search);
        if (departmentId) urlParams.set('DepartmentId', departmentId);
        if (searchTerm) urlParams.set('SearchTerm', searchTerm);
        urlParams.set('SortBy', sortBy);
        urlParams.delete('PageNumber');

        const urlParamsString = urlParams.toString();
        const url = urlParamsString ? `/Interns/Index?${urlParamsString}` : `/Interns/Index`;

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
window.clearSortButton = function(e) {
    if (e.target.closest('.clear-cell-sort-btn')) {
        $('#loadingOverlay').show();
        e.preventDefault();

        const contentContainer = document.querySelector('.stajyer-container');
        const filterSelect = contentContainer.querySelector('.filter-select');
        const searchInput = contentContainer.querySelector('.search-box input[type="text"]');

        const departmentId = filterSelect?.value || '';
        const searchTerm = searchInput?.value.trim() || '';

        const btn = e.target.closest('.clear-cell-sort-btn');
        const category = btn.dataset.category;

        document.querySelectorAll(`[data-sort*="${category}"]`).forEach(sortBtn => {
            sortBtn.classList.remove('active');
        });

        btn.classList.remove('show');

        const urlParams = new URLSearchParams(window.location.search);
        if (departmentId) urlParams.set('DepartmentId', departmentId);
        if (searchTerm) urlParams.set('SearchTerm', searchTerm);
        urlParams.delete('SortBy');
        urlParams.delete('PageNumber');

        const urlParamsString = urlParams.toString();
        const url = urlParamsString ? `/Interns/Index?${urlParamsString}` : `/Interns/Index`;

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
        else if (sortValue.includes('PROGRESS')) category = 'PROGRESS';
        else if (sortValue === 'FINISHED' || sortValue === 'NOTSTARTED') category = 'STATUS';

        if (category) {
            document.querySelectorAll(`[data-category="${category}"]`).forEach(clearBtn => {
                clearBtn.classList.add('show');
            });
        }
    });
}

// Normal yenileme durumunda URL'den mevcut parametreleri kontrol et ve ilgili kısmı düzenle
document.addEventListener('DOMContentLoaded', function loadFromUrl() {
    initProgressCircles();
    const urlParams = new URLSearchParams(window.location.search);
    const currentSort = urlParams.get('SortBy');
    const departmentId = urlParams.get('DepartmentId') || '';
    const searchTerm = urlParams.get('SearchTerm') || '';

    const newFilterSelect = document.querySelector('.filter-select');
    const newSearchInput = document.querySelector('.search-box input[type="text"]');
    if (newFilterSelect) newFilterSelect.value = departmentId;
    if (newSearchInput) newSearchInput.value = searchTerm;

    if (currentSort) {
        const activeBtn = document.querySelector(`[data-sort="${currentSort}"]`);
        if (activeBtn) {
            activeBtn.classList.add('active');
            updateClearButtons();
        }
    }
}); 
