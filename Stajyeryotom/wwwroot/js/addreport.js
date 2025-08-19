var uploadedFiles = [];

window.initAddReportScripts = function () {
    uploadedFiles = [];

    const workIdHidden = document.getElementById('workIdHolder');
    const workId = workIdHidden ? workIdHidden.value : null;

    if (workId) {
        window.workId = workId;
    } else {
        window.workId = null;
    }

    initializeBroadcastOptions();
    initializePhotoUpload();
    initializeTextarea();

    if (workId && workId !== '0') {
        autoSelectWorkOptions(workId);
    } else {
        updateVisibility();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const workIdHidden = document.getElementById('workIdHolder');
    const workId = workIdHidden ? workIdHidden.value : null;

    if (workId) {
        window.workId = workId;
    } else {
        window.workId = null;
    }

    initializeBroadcastOptions();
    initializePhotoUpload();
    initializeTextarea();

    if (workId && workId !== '0') {
        autoSelectWorkOptions(workId);
    } else {
        updateVisibility();
    }
});

// WorkId geldiğinde otomatik seçim yapan fonksiyon
function autoSelectWorkOptions(workId) {
    const workRadio = document.querySelector('input[name="broadcast"][value="Work"]');
    if (workRadio) {
        workRadio.checked = true;
    }

    updateVisibility();

    setTimeout(() => {
        selectWork(workId);
    }, 100);
}

function initializePhotoUpload() {
    const photoInput = document.getElementById('photo-input');
    const uploadArea = document.querySelector('.photo-upload-area');
    const thumbnailsContainer = document.getElementById('photo-thumbnails');

    if (thumbnailsContainer) {
        thumbnailsContainer.innerHTML = '';
    }

    if (!photoInput || !uploadArea) return;

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

window.addReportFormSubmitHandler = function (event) {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');

    if (!selectedBroadcast) {
        const broadcastSection = document.querySelector('.broadcast-section');
        const errorElement = broadcastSection.querySelector('.text-danger') || createErrorElement(broadcastSection);
        showError(errorElement, 'Lütfen bir gönderim seçeneği seçiniz.');
        event.preventDefault();
        return;
    }

    var value = selectedBroadcast.value;

    switch (value) {
        case 'Daily':
            event.detail.parameters.append('ReportType', 'Daily');
            break;
        case 'Work':
            event.detail.parameters.append('ReportType', 'Work');
    }

    if (uploadedFiles.length > 0) {
        uploadedFiles.forEach(fileObj => {
            event.detail.parameters.append('files', fileObj.file);
        });
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

// Tüm hataları temizleme fonksiyonu
function clearAllErrors() {
    const errorElements = document.querySelectorAll('.text-danger');
    errorElements.forEach(errorElement => {
        hideError(errorElement);
    });
}

// Broadcast seçenekleri işlevleri
function initializeBroadcastOptions() {
    const broadcastRadios = document.querySelectorAll('input[name="broadcast"]');

    broadcastRadios.forEach(radio => {
        radio.checked = false;
    });

    broadcastRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            updateVisibility();
        });
    });
}

function updateVisibility() {
    const selectedBroadcast = document.querySelector('input[name="broadcast"]:checked');
    const workSelectionSection = document.getElementById('workSelectionSection');

    if (workSelectionSection) {
        workSelectionSection.style.display = 'none';
    }

    if (!selectedBroadcast) {
        return;
    }

    switch (selectedBroadcast.value) {
        case 'Daily':
            if (workSelectionSection) {
                workSelectionSection.style.display = 'none';
            }
            clearWorkSelection();
            break;
        case 'Work':
            if (workSelectionSection) {
                workSelectionSection.style.display = 'block';
            }
            if (window.workId && window.workId !== '0') {
                const currentSelected = document.querySelector('input[name="WorkId"]:checked');
                if (!currentSelected || currentSelected.value !== window.workId) {
                    selectWork(window.workId);
                }
            }
            break;
    }
}

// Work seçimini temizle
function clearWorkSelection() {
    const workRadios = document.querySelectorAll('.work-radio');
    workRadios.forEach(radio => {
        radio.checked = false;
    });
}

// Textarea auto-resize işlevi
function initializeTextarea() {
    const textarea = document.querySelector('.description');
    if (textarea) {
        textarea.removeEventListener('input', textareaResizeHandler);
        textarea.addEventListener('input', textareaResizeHandler);
    }
}

function textareaResizeHandler() {
    this.style.height = 'auto';
    this.style.height = Math.min(this.scrollHeight, 400) + 'px';
}

// HTMX after request handler
document.body.addEventListener("htmx:afterRequest", function (event) {
    if (event.detail.requestConfig.headers["Form-Send"] === "true") {
        const selectedSectionData = document.getElementById('dataHolder');

        if (selectedSectionData) {
            var sectionId = selectedSectionData.dataset.section;
            if (sectionId && sectionId !== '0') {
                window.workId = sectionId;
                autoSelectWorkOptions(sectionId);
            } else {
                window.workId = null;
                initializeBroadcastOptions();
                updateVisibility();
            }
            return;
        }

        window.workId = null;
        initializeBroadcastOptions();
        updateVisibility();
    }
});

window.selectWork = function (workId) {
    if (!workId || workId === '0') return;

    const sectionRadio = document.querySelector(`input[name="WorkId"][value="${workId}"]`);
    if (sectionRadio) {
        sectionRadio.checked = true;
    }
}