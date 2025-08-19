var selectedUsers = [];
var uploadedFiles = [];
var searchTimeout;

window.initUpdateWorkScripts = function () {
    initializePhotoUpload();
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
    initializeDataForUpdate()
}

document.addEventListener('DOMContentLoaded', function () {
    initializePhotoUpload();
    initializeBroadcastOptions();
    initializeUserSearch();
    updateVisibility();
    initializeDataForUpdate()
});

function initializeDataForUpdate() {

    const selectedDepartmentId = window.selectedDepartmentId;
    const selectedSectionId = window.selectedSectionId;

    const broadcastType = window.updateWorkModel?.broadcastType;

    if (!broadcastType) return; 

    let radioValue = '';
    if (broadcastType) {
        switch (broadcastType) {
            case 'All':
                radioValue = 'All';
                break;
            case 'Users':
                radioValue = 'Users';
                loadSelectedUsers(); 
                break;
            case 'Department':
                radioValue = 'Department';
                break;
            case 'Section':
                radioValue = 'Section';
                break;
        }
    }

    if (radioValue) {
        const radioButton = document.querySelector(`input[name="broadcast"][value="${radioValue}"]`);
        if (radioButton) {
            radioButton.checked = true;
            updateVisibility();
        }
    }

    if (selectedDepartmentId && (broadcastType === 'Department' || broadcastType === 'Section')) {
        const departmentSelect = document.getElementById('departmentSelect');
        if (departmentSelect) {
            departmentSelect.value = selectedDepartmentId;

            if (broadcastType === 'Section' && selectedSectionId) {
                setTimeout(() => {
                    loadSections(selectedDepartmentId, selectedSectionId);
                }, 100);
            } else if (broadcastType === 'Department') {
                loadSections(selectedDepartmentId);
            }
        }
    }

    loadExistingPhotos();
}
function loadSelectedUsers() {
    const workInternsId = window.updateWorkModel?.internsId;

    if (workInternsId && workInternsId.length > 0) {
        fetch('/Interns/GetInternsByIds', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(workInternsId)
        })
            .then(response => response.json())
            .then(users => {
                selectedUsers = users.map(user => ({
                    id: user.id,
                    name: `${user.firstName} ${user.lastName}`
                }));

                updateSelectedUsersUI();
            })
            .catch(error => {
                console.error('Kullanıcı bilgileri yüklenirken hata:', error);
            });
    }
}

// Mevcut fotoğrafları yükle
function loadExistingPhotos() {
    const existingPhotosData = window.updateWorkModel?.workPhotos;

    if (existingPhotosData && existingPhotosData.length > 0) {
        const thumbnailsContainer = document.getElementById('photo-thumbnails');

        let photoFileNames = [];
        try {
            photoFileNames = typeof existingPhotosData === 'string'
                ? JSON.parse(existingPhotosData)
                : existingPhotosData;
        } catch (e) {
            console.error('Fotoğraf verisi parse edilemedi:', e);
            return;
        }

        photoFileNames.forEach((fileName, index) => {
            const fileId = `existing_${index}_${fileName}`;
            const photoUrl = `/images/works/${fileName}`;

            uploadedFiles.push({
                id: fileId,
                file: null, 
                name: fileName,
                isExisting: true,
                fileName: fileName,
                photoUrl: photoUrl
            });

            createExistingThumbnail({
                fileName: fileName,
                photoUrl: photoUrl
            }, fileId);
        });
    }
}

// Mevcut fotoğraflar için thumbnail oluştur
function createExistingThumbnail(photo, fileId) {
    const thumbnailsContainer = document.getElementById('photo-thumbnails');

    const thumbnailDiv = document.createElement('div');
    thumbnailDiv.className = 'photo-thumbnail';
    thumbnailDiv.setAttribute('data-file-id', fileId);

    thumbnailDiv.innerHTML = `
        <img src="${photo.photoUrl}" alt="${photo.fileName}">
        <button type="button" class="photo-remove" onclick="removePhoto('${fileId}')">×</button>
        <div class="existing-photo-indicator">Mevcut</div>
    `;

    thumbnailsContainer.appendChild(thumbnailDiv);
}


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
    const fileToRemove = uploadedFiles.find(f => f.id == fileId);

    if (fileToRemove && fileToRemove.isExisting) {
        fileToRemove.markedForDeletion = true;

        const thumbnail = document.querySelector(`[data-file-id="${fileId}"]`);
        if (thumbnail) {
            thumbnail.style.opacity = '0.5';
            thumbnail.style.filter = 'grayscale(100%)';

            const removeButton = thumbnail.querySelector('.photo-remove');
            removeButton.textContent = '↶';
            removeButton.setAttribute('onclick', `restorePhoto('${fileId}')`);
            removeButton.title = 'Geri Al';
        }
    } else {
        uploadedFiles = uploadedFiles.filter(f => f.id != fileId);

        const thumbnail = document.querySelector(`[data-file-id="${fileId}"]`);
        if (thumbnail) {
            thumbnail.remove();
        }
    }
}

function restorePhoto(fileId) {
    const fileToRestore = uploadedFiles.find(f => f.id == fileId);

    if (fileToRestore && fileToRestore.isExisting) {
        fileToRestore.markedForDeletion = false;

        const thumbnail = document.querySelector(`[data-file-id="${fileId}"]`);
        if (thumbnail) {
            thumbnail.style.opacity = '1';
            thumbnail.style.filter = 'none';

            const removeButton = thumbnail.querySelector('.photo-remove');
            removeButton.textContent = '×';
            removeButton.setAttribute('onclick', `removePhoto('${fileId}')`);
            removeButton.title = 'Sil';
        }
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

// Form gönderme işlevi
window.updateWorkFormSubmitHandler = function (event) {
    const form = document.getElementById('workForm');
    const selectedUserIds = selectedUsers.map(u => u.id);
    clearAllErrors();

    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');

    if (!selectedBroadcast) {
        const broadcastSection = document.querySelector('.broadcast-section');
        const errorElement = broadcastSection.querySelector('.text-danger') || createErrorElement(broadcastSection);
        showError(errorElement, 'Lütfen bir gönderim seçeneği seçiniz.');
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

        const selectedUserIds = selectedUsers.map(u => String(u.id));
        const workInternsId = (window.updateWorkModel?.internsId || []).map(id => String(id));

        selectedUserIds.forEach((userId, index) => {
             event.detail.parameters.append(`UpdatedInternsId[${index}]`, userId);
        });

        workInternsId.forEach((userId, index) => {
            event.detail.parameters.append(`InternsId[${index}]`, userId);
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

    const photosToDelete = uploadedFiles
        .filter(f => f.isExisting && f.markedForDeletion)
        .map(f => f.fileName);

    if (photosToDelete.length > 0) {
        photosToDelete.forEach((fileName, index) => {
            event.detail.parameters.append(`PhotosToDelete[${index}]`, fileName);
        });
    }

    const newFiles = uploadedFiles.filter(f => !f.isExisting && f.file);
    if (newFiles.length > 0) {
        newFiles.forEach(fileObj => {
            event.detail.parameters.append('files', fileObj.file);
        });
    }

    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.classList.add('loading');
        submitBtn.disabled = true;
    }
}

function showError(errorElement, message) {
    errorElement.textContent = message;
    errorElement.style.display = 'block';
    errorElement.style.animation = 'shake 0.5s ease-in-out';
}

function hideError(errorElement) {
    errorElement.style.display = 'none';
}

function clearAllErrors() {
    const errorElements = document.querySelectorAll('.text-danger');
    errorElements.forEach(errorElement => {
        hideError(errorElement);
    });
}

function createErrorElement(parentElement) {
    const errorElement = document.createElement('span');
    errorElement.className = 'text-danger';
    errorElement.style.display = 'none';
    parentElement.appendChild(errorElement);
    return errorElement;
}

function showServerErrors(errors) {
    Object.keys(errors).forEach(fieldName => {
        const errorMessages = errors[fieldName];
        if (errorMessages && errorMessages.length > 0) {
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