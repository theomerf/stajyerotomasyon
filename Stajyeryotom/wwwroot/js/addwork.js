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
            <button class="photo-remove" onclick="removePhoto('${fileId}')">×</button>
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

    const value = selectedBroadcast.value;

    switch (value) {
        case 'all':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'none';
            break;

        case 'users':
            userSearchSection.style.display = 'block';
            departmentSection.style.display = 'none';
            break;

        case 'department':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'block';
            sectionsContainer.style.display = 'none';
            departmentSelect.parentElement.style.display = 'block';
            break;

        case 'section':
            userSearchSection.style.display = 'none';
            departmentSection.style.display = 'block';
            departmentSelect.parentElement.style.display = 'block';
            if (departmentSelect.value) {
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
            <button class="remove-user" onclick="removeSelectedUser(${user.id})">×</button>
        </div>
    `).join('');
}

function removeSelectedUser(userId) {
    console.log('asd');
    selectedUsers = selectedUsers.filter(u => u.id !== userId);
    updateSelectedUsersUI();
}

// Form gönderme handler'ı
function handleFormSubmit() {
    // Fotoğrafları forma ekle (FormData kullanarak)
    const form = document.getElementById('internForm');

    // Seçili kullanıcıları hidden input olarak ekle
    if (selectedUsers.length > 0) {
        selectedUsers.forEach((user, index) => {
            const hiddenInput = document.createElement('input');
            hiddenInput.type = 'hidden';
            hiddenInput.name = `InternsId`;
            hiddenInput.value = user.id;
            form.appendChild(hiddenInput);
        });
    }

    // Fotoğrafları ekle (Bu kısmı backend'inizin dosya upload yapısına göre düzenleyin)
    if (uploadedFiles.length > 0) {
        const fileInput = document.createElement('input');
        fileInput.type = 'hidden';
        fileInput.name = 'HasPhotos';
        fileInput.value = 'true';
        form.appendChild(fileInput);

        // Dosyaları ayrı bir FormData ile göndermeniz gerekebilir
        // veya base64 olarak encode edip gönderebilirsiniz
    }
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

    if (selectedBroadcast.value == 'section') {


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