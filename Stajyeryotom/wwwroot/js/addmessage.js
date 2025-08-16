var selectedUsers = [];
var uploadedFiles = [];
var searchTimeout;

window.initAddMessageScripts = function () {
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
}

document.addEventListener('DOMContentLoaded', function () {
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
});

// Gönderim seçenekleri işlevleri
function initializeBroadcastOptions() {
    const broadcastRadios = document.querySelectorAll('input[name="broadcast"]');

    broadcastRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            updateVisibility();
        });
    });
}

function updateVisibility() {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');
    const userSearchSection = document.querySelector('.user-search-section');
    const departmentSection = document.querySelector('.select-section');
    const departmentSelect = document.getElementById('departmentSelect');
    const sectionsContainer = document.getElementById('sectionsContainer');

    if (!selectedBroadcast) {
        userSearchSection.style.display = 'none';
        departmentSection.style.display = 'none';
        return;
    }

    window.value = selectedBroadcast.value;

    switch (value) {
        case 'All':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'none';
            break;

        case 'Users':
            userSearchSection.style.display = 'block';
            departmentSection.style.display = 'none';
            break;

        case 'Department':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'block';
            sectionsContainer.style.display = 'none';
            departmentSelect.parentElement.style.display = 'block';
            break;

        case 'Section':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'block';
            departmentSelect.parentElement.style.display = 'block';
            if (departmentSelect.value) {
                loadSections(departmentSelect.value, departmentSection.value);
                sectionsContainer.style.display = 'block';
            }
            break;
    }
}

// Kullanıcı arama işlemleri
function initializeUserSearch() {
    const userSearchInput = document.getElementById('user-search');
    const searchResults = document.getElementById('search-results');
    const selectedUsersContainer = document.getElementById('selected-users');

    selectedUsersContainer.innerHTML = '';

    userSearchInput.addEventListener('input', function () {
        const query = this.value.trim();

        clearTimeout(searchTimeout);

        if (query.length >= 2) {
            searchTimeout = setTimeout(() => {
                searchUsers(query);
            }, 300);
        } else {
            searchResults.classList.remove('show');
        }
    });

    userSearchInput.addEventListener('focus', function () {
        if (this.value.trim().length >= 2) {
            searchResults.classList.add('show');
        }
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.user-search-container')) {
            searchResults.classList.remove('show');
        }
    });
}

// Kullanıcı arama ajax işlevi
function searchUsers(query) {
    fetch(`/Interns/SearchInterns?userName=${encodeURIComponent(query)}`, {
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(response => response.json())
        .then(data => {
            displaySearchResults(data);
        })
        .catch(error => {
            console.error('Kullanıcı arama hatası:', error);
        });
}

// Arama sonuçlarını göster
function displaySearchResults(users) {
    const searchResults = document.getElementById('search-results');

    if (users.length === 0) {
        searchResults.innerHTML = '<div class="user-result-item" style="text-align: center; color: #718096;">Kullanıcı bulunamadı</div>';
    } else {
        searchResults.innerHTML = users.map(user => `
            <div class="user-result-item" onclick="selectUser('${user.id}', '${user.firstName} ${user.lastName}')">
                <div class="user-avatar">${user.firstName.charAt(0)}${user.lastName.charAt(0)}</div>
                <div class="user-info">
                    <div class="user-name">${user.firstName} ${user.lastName}</div>
                    <div class="user-email">${user.email}</div>
                </div>
            </div>
        `).join('');
    }

    searchResults.classList.add('show');
}

// Kullanıcı seç
function selectUser(userId, fullName) {
    if (selectedUsers.find(u => u.id === userId)) {
        console.log(userId)
        alert('Bu kullanıcı zaten seçili!');
        return;
    }

    selectedUsers.push({
        id: userId,
        name: fullName,
    });

    updateSelectedUsersUI();

    document.getElementById('user-search').value = '';
    document.getElementById('search-results').classList.remove('show');
}

function updateSelectedUsersUI() {
    const selectedUsersContainer = document.getElementById('selected-users');

    selectedUsersContainer.innerHTML = selectedUsers.map(user => `
        <div class="selected-user-tag" data-user-id="${user.id}">
            ${user.name}
            <button type="button" class="remove-user" onclick="removeSelectedUser('${user.id}')">×</button>
        </div>
    `).join('');
}

function removeSelectedUser(userId) {
    selectedUsers = selectedUsers.filter(u => u.id !== userId);
    updateSelectedUsersUI();
}

// Form gönderme handler'ı
window.addMessageFormSubmitHandler = function (event) {

    clearAllErrors();

    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');

    if (!selectedBroadcast) {
        const broadcastSection = document.querySelector('.broadcast-section');
        const errorElement = broadcastSection.querySelector('.text-danger') || createErrorElement(broadcastSection);
        showError(errorElement, 'Lütfen bir gönderim seçeneği seçiniz.');
        event.preventDefault();
        return;
    }

    var value = selectedBroadcast.value;

    if (value == 'All') {
        event.detail.parameters.append('BroadcastType', 'All');
    }
    else if (value == 'Users') {
        if (selectedUsers.length === 0) {
            const userSearchSection = document.querySelector('.user-search-section');
            const errorElement = userSearchSection.querySelector('.text-danger') || createErrorElement(userSearchSection);
            showError(errorElement, 'Lütfen en az bir kullanıcı seçiniz.');
            event.preventDefault();
            return;
        }

        selectedUsers.forEach((user, index) => {
            event.detail.parameters.append(`InternsId[${index}]`, user.id);
        });

        event.detail.parameters.append(`BroadcastType`, 'Users');
    }
    else if (value == 'Department') {
        var departmentSelect = document.getElementById('departmentSelect');

        if (!departmentSelect.value) {
            const departmentSection = document.querySelector('.department-section');
            const errorElement = departmentSection.querySelector('.text-danger') || createErrorElement(departmentSection);
            showError(errorElement, 'Lütfen bir departman seçiniz.');
            event.preventDefault();
            return;
        }

        event.detail.parameters.append(`BroadcastType`, 'Department');
        event.detail.parameters.append('DepartmentId', departmentSelect.value);
    }
    else if (value == 'Section') {
        var departmentSelect = document.getElementById('departmentSelect');
        var selectedSection = document.querySelector('input[name="SectionId"]:checked');

        if (!departmentSelect.value) {
            const departmentSection = document.querySelector('.department-section');
            const errorElement = departmentSection.querySelector('.text-danger') || createErrorElement(departmentSection);
            showError(errorElement, 'Lütfen önce bir departman seçiniz.');
            event.preventDefault();
            return;
        }

        if (!selectedSection) {
            const sectionsContainer = document.getElementById('sectionsContainer');
            const errorElement = sectionsContainer.querySelector('.text-danger') || createErrorElement(sectionsContainer);
            showError(errorElement, 'Lütfen bir bölüm seçiniz.');
            event.preventDefault();
            return;
        }

        event.detail.parameters.append(`BroadcastType`, 'Section');
        event.detail.parameters.append(`SectionId`, selectedSection.value);
        event.detail.parameters.append(`DepartmentId`, departmentSelect.value);
    }

    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.classList.add('loading');
        submitBtn.disabled = true;
    }
}

// Hata gösterme fonksiyonları
function showError(errorElement, message) {
    errorElement.textContent = message;
    errorElement.style.display = 'block';
    errorElement.style.animation = 'shake 0.5s ease-in-out';
}

function hideError(errorElement) {
    errorElement.style.display = 'none';
}

// Tüm hataları temizleme fonksiyonu
function clearAllErrors() {
    const errorElements = document.querySelectorAll('.text-danger');
    errorElements.forEach(errorElement => {
        hideError(errorElement);
    });
}

// Hata elementi oluşturma fonksiyonu (eğer mevcut değilse)
function createErrorElement(parentElement) {
    const errorElement = document.createElement('span');
    errorElement.className = 'text-danger';
    errorElement.style.display = 'none';
    parentElement.appendChild(errorElement);
    return errorElement;
}

// Server'dan gelen hataları gösterme fonksiyonu
function showServerErrors(errors) {
    Object.keys(errors).forEach(fieldName => {
        const errorMessages = errors[fieldName];
        if (errorMessages && errorMessages.length > 0) {
            // Field'a göre uygun error elementi bul
            const fieldElement = document.querySelector(`[name="${fieldName}"]`);
            if (fieldElement) {
                const formGroup = fieldElement.closest('.form-group');
                if (formGroup) {
                    const errorElement = formGroup.querySelector('.text-danger') || createErrorElement(formGroup);
                    showError(errorElement, errorMessages[0]);
                }
            }
        }
    });
}

// Bölüm ve departman seçme kodları

document.body.addEventListener("htmx:afterRequest", function (event) {
    if (event.detail.requestConfig.headers["Form-Send"] == "true") {
        const selectElement = document.getElementById('departmentSelect');
        const selectedSectionData = document.getElementById('dataHolder');

        if (selectElement && selectedSectionData) {
            var selectedValue = selectElement.value;
            var sectionId = selectedSectionData.dataset.section;
            loadSections(selectedValue, sectionId);
            return;
        }
        else if (selectElement) {
            var selectedValue = selectElement.value;
            loadSections(selectedValue);
            return;
        }
    }
}, { once: true });

window.sectionsContainer = document.getElementById('sectionsContainer');
window.sectionsGrid = document.getElementById('sectionsGrid');

window.loadSections = function (departmentId, selectedSectionId = null) {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');
    window.sections = departmentSections[departmentId];
    window.sectionsContainer = document.getElementById('sectionsContainer');
    window.sectionsGrid = document.getElementById('sectionsGrid');

    if (selectedBroadcast.value == 'Section') {


        if (sections && sections.length > 0) {
            sectionsGrid.innerHTML = '';

            sections.forEach(section => {
                const sectionItem = document.createElement('div');
                sectionItem.className = 'section-item active';
                sectionItem.innerHTML = `
                    <input type="radio" name="SectionId" value="${section.id}" class="section-checkbox" id="section-${section.id}" required />
                    <label class="section-label" for="section-${section.id}">${section.name}</label>
                `;
                sectionsGrid.appendChild(sectionItem);
            });

            sectionsContainer.style.display = 'block';
            sectionsContainer.style.animation = 'slideIn 0.5s ease-out';
            if (selectedSectionId) {
                setTimeout(function () {
                    const sectionRadio = document.querySelector(`input[name="SectionId"][value="${selectedSectionId}"]`);
                    if (sectionRadio) {
                        sectionRadio.checked = true;
                    }
                }, 50);
            }
        } else {
            sectionsContainer.style.display = 'none';
        }
    }
}

window.departmentSelectHandler = function (e) {
    const departmentId = parseInt(e.target.value);
    if (departmentId && window.departmentSections && departmentSections[departmentId]) {
        loadSections(departmentId);
    } else {
        sectionsContainer.style.display = 'none';
    }
};

textarea = document.querySelector('.description');
if (textarea) {
    textarea.addEventListener('input', function () {
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 400) + 'px';
    });
}