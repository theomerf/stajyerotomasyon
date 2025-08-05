window.openIframeFullscreen = function () {
    const iframe = document.querySelector('.cv-pdf');
    if (iframe.requestFullscreen) {
        iframe.requestFullscreen();
    } else if (iframe.webkitRequestFullscreen) {
        iframe.webkitRequestFullscreen();
    } else if (iframe.msRequestFullscreen) { 
        iframe.msRequestFullscreen();
    } else {
        alert('Tarayıcınız tam ekran modunu desteklemiyor.');
    }
}

window.removeText = function () {
    setTimeout(() => {
        const textarea = document.querySelector('.notes-textarea');
        if (textarea) {
            textarea.value = '';
        }
    }, 200);
}

textarea = document.querySelector('.notes-textarea');
if (textarea) {
    textarea.addEventListener('input', function () {
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 400) + 'px';
    });
}

window.openTeams = function () {
    window.location.href = "msteams://teams.microsoft.com/l/meeting/new";
}

paperNotes = document.querySelectorAll('.paper-note');
paperNotes.forEach(note => {
    note.addEventListener('mouseenter', function () {
        this.style.zIndex = '10';
    });

    note.addEventListener('mouseleave', function () {
        this.style.zIndex = 'auto';
    });
});