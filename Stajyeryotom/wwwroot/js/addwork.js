var selectedUsers = [];
var uploadedFiles = [];
var searchTimeout;

window.initAddWorkScripts = function () {
    initializePhotoUpload();
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
}

document.addEventListener('DOMContentLoaded', function () {
    initializePhotoUpload();
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
});

// Fotoğraf yükleme, önizleme, kaldırma işlemleri
function initializePhotoUpload() {
    const photoInput = document.getElementById('photo-input');
    const uploadArea = document.querySelector('.photo-upload-area');
    const thumbnailsContainer = document.getElementById('photo-thumbnails');

    thumbnailsContainer.innerHTML = '';

    photoInput.addEventListener('change', function (e) {
        handleFiles(e.target.files);
    });

    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        uploadArea.addEventListener(eventName, () => uploadArea.classList.add('dragover'), false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        uploadArea.addEventListener(eventName, () => uploadArea.classList.remove('dragover'), false);
    });

    uploadArea.addEventListener('drop', function (e) {
        const files = e.dataTransfer.files;
        handleFiles(files);
    });
}

function handleFiles(files) {
    const maxSize = 5 * 1024 * 1024;
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];

    Array.from(files).forEach(file => {
        if (file.size > maxSize) {
            alert(`${file.name} dosyası çok büyük. Maksimum 5MB olmalıdır.`);
            return;
        }

        if (!allowedTypes.includes(file.type)) {
            alert(`${file.name} desteklenmeyen format. Sadece PNG, JPG, GIF formatları kabul edilir.`);
            return;
        }

        const fileId = Date.now() + Math.random();
        uploadedFiles.push({
            id: fileId,
            file: file,
            name: file.name
        });

        createThumbnail(file, fileId);
    });
}

function createThumbnail(file, fileId) {
    const reader = new FileReader();
    reader.onload = function (e) {
        const thumbnailsContainer = document.getElementById('photo-thumbnails');

        const thumbnailDiv = document.createElement('div');
        thumbnailDiv.className = 'photo-thumbnail';
        thumbnailDiv.setAttribute('data-file-id', fileId);

        thumbnailDiv.innerHTML = `
            <img src="${e.target.result}" alt="${file.name}">
            <button type="button" class="photo-remove" onclick="removePhoto('${fileId}')">×</button>
        `;

        thumbnailsContainer.appendChild(thumbnailDiv);
    };
    reader.readAsDataURL(file);
}

function removePhoto(fileId) {
    uploadedFiles = uploadedFiles.filter(f => f.id != fileId);

    const thumbnail = document.querySelector(`[data-file-id="${fileId}"]`);
    if (thumbnail) {
        thumbnail.remove();
    }
}

// Gönderim seçenekleri işlevleri
function initializeBroadcastOptions() {
    const broadcastRadios = document.querySelectorAll('input[name="broadcast"]');

    broadcastRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            updateVisibility();
        });
    });
}

function updateVisibility(type = null) {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');
    const userSearchSection = document.querySelector('.user-search-section');
    const departmentSection = document.querySelector('.select-section');
    const departmentSelect = document.getElementById('departmentSelect');
    const sectionsContainer = document.getElementById('sectionsContainer');

    let value;

    if (type) {
        value = type;
        // Eğer type parameter olarak geliyorsa, ilgili radio button'ı da seç
        const broadcastRadio = document.querySelector(`input[name="broadcast"][value="${type}"]`);
        if (broadcastRadio && !broadcastRadio.checked) {
            broadcastRadio.checked = true;
        }
    } else if (selectedBroadcast) {
        value = selectedBroadcast.value;
    } else {
        userSearchSection.style.display = 'none';
        departmentSection.style.display = 'none';
        return;
    }

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
            if (departmentSelect && departmentSelect.parentElement) {
                departmentSelect.parentElement.style.display = 'block';
            }
            break;

        case 'Section':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'block';
            if (departmentSelect && departmentSelect.parentElement) {
                departmentSelect.parentElement.style.display = 'block';
            }
            if (departmentSelect && departmentSelect.value) {
                loadSections(departmentSelect.value);
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
    console.log('asd');
    selectedUsers = selectedUsers.filter(u => u.id !== userId);
    updateSelectedUsersUI();
}

// Form gönderme handler'ı
window.addWorkFormSubmitHandler = function (event) {

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

    if (uploadedFiles.length > 0) {
        uploadedFiles.forEach(fileObj => {
            event.detail.parameters.append('files', fileObj.file);
        });
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
        const selectedBroadcastData = document.getElementById('dataHolderForBroadcast');
        const selectedDepartmentData = document.getElementById('dataHolderForDepartment');
        const selectedSectionData = document.getElementById('dataHolderForSection');

        // Broadcast type'ı restore et
        if (selectedBroadcastData && selectedBroadcastData.dataset.type) {
            updateVisibility(selectedBroadcastData.dataset.type);

            // Radio button'ı seç
            const broadcastRadio = document.querySelector(`input[name="broadcast"][value="${selectedBroadcastData.dataset.type}"]`);
            if (broadcastRadio) {
                broadcastRadio.checked = true;
            }
        }

        // Department ve Section'ı restore et
        if (selectedDepartmentData && selectedDepartmentData.dataset.department) {
            const departmentSelect = document.getElementById('departmentSelect');
            if (departmentSelect) {
                departmentSelect.value = selectedDepartmentData.dataset.department;

                // Section varsa onu da restore et
                if (selectedSectionData && selectedSectionData.dataset.section) {
                    loadSections(selectedDepartmentData.dataset.department, selectedSectionData.dataset.section);
                } else {
                    loadSections(selectedDepartmentData.dataset.department);
                }
            }
        }

        // Selected users'ları restore et (eğer varsa)
        const selectedUsersData = document.getElementById('dataHolderForUsers');
        if (selectedUsersData && selectedUsersData.dataset.users) {
            try {
                const users = JSON.parse(selectedUsersData.dataset.users);
                selectedUsers = users;
                updateSelectedUsersUI();
            } catch (error) {
                console.error('Users data parse error:', error);
            }
        }
    }
});

window.sectionsContainer = document.getElementById('sectionsContainer');
window.sectionsGrid = document.getElementById('sectionsGrid');

window.loadSections = function (departmentId, selectedSectionId = null) {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');
    const selectedBroadcastData = document.getElementById('dataHolderForBroadcast');

    let selectedBroadcastValue;
    if (selectedBroadcast) {
        selectedBroadcastValue = selectedBroadcast.value;
    } else if (selectedBroadcastData && selectedBroadcastData.dataset.type) {
        selectedBroadcastValue = selectedBroadcastData.dataset.type;
    } else {
        return;
    }

    window.sections = window.departmentSections ? window.departmentSections[departmentId] : null;
    window.sectionsContainer = document.getElementById('sectionsContainer');
    window.sectionsGrid = document.getElementById('sectionsGrid');

    if (selectedBroadcastValue === 'Section') {
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
                }, 100);
            }
        } else {
            sectionsContainer.style.display = 'none';
        }
    }
}

window.departmentSelectHandler = function (e) {
    const departmentId = parseInt(e.target.value);
    if (departmentId && window.departmentSections && window.departmentSections[departmentId]) {
        loadSections(departmentId);
    } else {
        const sectionsContainer = document.getElementById('sectionsContainer');
        if (sectionsContainer) {
            sectionsContainer.style.display = 'none';
        }
    }
};

textarea = document.querySelector('.description');
if (textarea) {
    textarea.addEventListener('input', function () {
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 400) + 'px';
    });
}