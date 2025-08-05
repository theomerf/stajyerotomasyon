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
    window.sections = departmentSections[departmentId];
    window.sectionsContainer = document.getElementById('sectionsContainer');
    window.sectionsGrid = document.getElementById('sectionsGrid');

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

window.departmentSelectHandler = function (e) {
    const departmentId = parseInt(e.target.value);
    if (departmentId && window.departmentSections && departmentSections[departmentId]) {
        loadSections(departmentId);
    } else {
        sectionsContainer.style.display = 'none';
    }
};

window.handleFormSubmit = function() {
    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.classList.add('loading');
    }
}