document.body.addEventListener("htmx:afterRequest", function (event) {
    if (event.detail.requestConfig.headers["Form-Send"] == "true") {
        const selectElement = document.getElementById('updateIntern-departmentSelect');
        const selectedSectionData = document.getElementById('updateIntern-dataHolder');
        if (selectElement && selectedSectionData) {
            var selectedValue = selectElement.value;
            var sectionId = selectedSectionData.dataset.section;
            loadSectionsForUpdate(selectedValue, sectionId);
            return;
        }
        else if (selectElement)
        {
            var selectedValue = selectElement.value;
            loadSectionsForUpdate(selectedValue);
            return;
        }
    }
}, { once: true });

document.addEventListener("DOMContentLoaded", function () {
    initUpdateInternScripts();
});

window.initUpdateInternScripts = function () {
     window.sectionsContainerForUpdate = document.getElementById('updateIntern-sectionsContainer');
     window.sectionsGridForUpdate = document.getElementById('updateIntern-sectionsGrid');
     const selectedValue = window.selectedDepartmentIdForUpdate;
     const sectionId = window.selectedSectionIdForUpdate;

     loadSectionsForUpdate(selectedValue, sectionId);
}

window.loadSectionsForUpdate = function (departmentId, selectedSectionId = null) {
    window.sections = departmentSections[departmentId];
    window.sectionsContainerForUpdate = document.getElementById('updateIntern-sectionsContainer');
    window.sectionsGridForUpdate = document.getElementById('updateIntern-sectionsGrid');

    if (sections && sections.length > 0) {
        sectionsGridForUpdate.innerHTML = '';

        sections.forEach(section => {
            const sectionItem = document.createElement('div');
            sectionItem.className = 'section-item active';
            sectionItem.innerHTML = `
                    <input type="radio" name="SectionId" value="${section.id}" class="section-checkbox" id="updateIntern-section-${section.id}" required />
                    <label class="section-label" for="updateIntern-section-${section.id}">${section.name}</label>
                `;
            sectionsGridForUpdate.appendChild(sectionItem);
        });

        sectionsContainerForUpdate.style.display = 'block';
        sectionsContainerForUpdate.style.animation = 'slideIn 0.5s ease-out';
        if (selectedSectionId) {
            setTimeout(function () {
                const sectionRadio = document.querySelector(`input[name="SectionId"][value="${selectedSectionId}"]`);
                if (sectionRadio) {
                    sectionRadio.checked = true;
                }
            }, 50);
        }
    }
    else {
        sectionsContainerForUpdate.style.display = 'none';
    }
}

window.departmentSelectHandlerForUpdate = function (e) {
    const departmentId = parseInt(e.target.value);
    if (departmentId && window.departmentSections && departmentSections[departmentId]) {
        loadSectionsForUpdate(departmentId);
    } else {
        sectionsContainerForUpdate.style.display = 'none';
    }
};

window.handleFormSubmitForUpdate = function () {
    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.classList.add('loading');
    }
}

