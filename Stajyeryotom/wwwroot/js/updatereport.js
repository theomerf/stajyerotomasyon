var uploadedFiles = [];

window.initUpdateReportScripts = function () {
    initializePhotoUpload();
    initializeTextarea();
    loadExistingPhotos();
}

document.addEventListener('DOMContentLoaded', function () {
    initializePhotoUpload();
    initializeTextarea();
    loadExistingPhotos();
});

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
            const photoUrl = `/images/reports/${fileName}`;

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


window.updateReportFormSubmitHandler = function (event) {
    var value = window.updateWorkModel.reportType;

    switch (value) {
        case 0:
            event.detail.parameters.append('ReportType', 'Daily');
            break;
        case 1:
            event.detail.parameters.append('ReportType', 'Work');
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
}

function initializeTextarea() {
    const textarea = document.querySelector('.description');
    if (textarea) {
        textarea.addEventListener('input', function () {
            this.style.height = 'auto';
            this.style.height = Math.min(this.scrollHeight, 400) + 'px';
        });
    }
}