window.initSectionScripts = function ()
{
	window.sectionCount = 0;
	window.sectionsList = [];

	window.departmentSelect = document.getElementById('departmentSelect');
	window.departmentError = document.getElementById('departmentError');

	window.sectionNameInput = document.getElementById('sectionNameInput');
	window.sectionsListContainer = document.getElementById('sectionsList');
	window.emptyState = document.getElementById('emptyState');
	window.sectionsError = document.getElementById('sectionsError');
}

sectionNameInput.addEventListener('keypress', function (e) {
	if (e.key === 'Enter') {
		e.preventDefault();
		addSection();
	}
});

window.addSection = function() {
	const sectionName = sectionNameInput.value.trim();

	if (!sectionName) {
		showError(sectionsError, 'Lütfen bölüm adını giriniz!');
		sectionNameInput.focus();
		return;
	}

	if (sectionsList.some(section => section.name.toLowerCase() === sectionName.toLowerCase())) {
		showError(sectionsError, 'Bu bölüm zaten eklenmiş!');
		sectionNameInput.focus();
		return;
	}

	hideError(sectionsError);

	const sectionId = ++sectionCount;
	sectionsList.push({
		id: sectionId,
		name: sectionName
	});

	emptyState.style.display = 'none';

	const sectionItem = document.createElement('div');
	sectionItem.className = 'addsection-item';
	sectionItem.setAttribute('data-section-id', sectionId);
	sectionItem.innerHTML = `
			   <div class="addsection-item-content">
				   <i class="fas fa-layer-group addsection-item-icon"></i>
				   <span>${sectionName}</span>
				   <input type="hidden" name="SectionNames" value="${sectionName}" />
			   </div>
			   <button type="button" class="addsection-remove-btn" onclick="removeSection(${sectionId})">
				   <i class="fas fa-minus"></i>
			   </button>
		   `;

	sectionsListContainer.appendChild(sectionItem);

	sectionNameInput.value = '';
	sectionNameInput.focus();

	sectionItem.style.transform = 'scale(0.8)';
	setTimeout(() => {
		sectionItem.style.transform = 'scale(1)';
	}, 50);
}

function removeSection(sectionId) {
	const index = sectionsList.findIndex(section => section.id === sectionId);
	if (index > -1) {
		sectionsList.splice(index, 1);
	}

	const sectionItem = document.querySelector(`[data-section-id="${sectionId}"]`);
	if (sectionItem) {
		sectionItem.style.animation = 'fadeOut 0.3s ease-out forwards';
		setTimeout(() => {
			sectionItem.remove();

			if (sectionsList.length === 0) {
				emptyState.style.display = 'block';
			}
		}, 300);
	}
}

const style = document.createElement('style');
style.textContent = `
		   keyframes fadeOut {
			   from {
				   opacity: 1;
				   transform: translateY(0);
			   }
			   to {
				   opacity: 0;
				   transform: translateY(-20px);
			   }
		   }
	   `;
document.head.appendChild(style);

window.departmentFormSubmitHandler = function (e) {
	let hasError = false;

	if (!departmentSelect.value) {
		showError(departmentError, 'Lütfen bir departman seçiniz!');
		hasError = true;
	} else {
		hideError(departmentError);
	}

	if (sectionsList.length === 0) {
		showError(sectionsError, 'En az bir bölüm eklemelisiniz!');
		hasError = true;
	} else {
		hideError(sectionsError);
	}

	if (hasError) {
		e.preventDefault();
		return false;
	}

	const submitBtn = document.getElementById('submitBtn');
	submitBtn.classList.add('loading');
}

function showError(errorElement, message) {
	errorElement.textContent = message;
	errorElement.style.display = 'block';
	errorElement.style.animation = 'shake 0.5s ease-in-out';
}

function hideError(errorElement) {
	errorElement.style.display = 'none';
}