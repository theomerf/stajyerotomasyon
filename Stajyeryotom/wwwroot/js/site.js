// İleri geri gitme işlevi

window.addEventListener('popstate', function (event) {
    if (event.state && event.state.view) {
        loadContent(event.state.view);

        $('.sidebar-nav-item.active').removeClass('active');
        $('.sidebar-nav-item').each(function () {
            if ($(this).data('target') === event.state.view) {
                $(this).addClass('active');
                return false;
            }
        });
    }
});

window.reloadSidebar = function () {
    htmx.ajax('GET', '/Home/Sidebar', { target: '#sidebar' });
}

// Toast gönderme işlevi

window.toastInstance = null;

window.showToast = function(message, type) {
    if (toastInstance) {
        toastInstance.hide();
    }

    const $toast = $('#ajaxToast');
    const toast = $toast.get(0);

    $toast.removeClass('bg-success bg-danger bg-warning bg-info');

    switch (type) {
        case 'success':
            $toast.addClass('bg-success');
            break;
        case 'danger':
            $toast.addClass('bg-danger');
            break;
        case 'warning':
            $toast.addClass('bg-warning');
            break;
        case 'info':
            $toast.addClass('bg-info');
            break;
        default:
            $toast.addClass('bg-success');
    }

    $('#ajaxToastBody').text(message);

    toastInstance = new bootstrap.Toast(toast, {
        autohide: true,
        delay: 3000
    });

    toastInstance.show();

    $toast.on('hidden.bs.toast', function () {
        toastInstance = null;
    });
}


// Page-Header toggle işlevi

window.toggleHeaderCollapse = function (event) {
    const header = document.querySelector('.page-header');
    if (header.classList.contains('collapsed')) {
        header.classList.remove('collapsed');
        const days = 7;
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        document.cookie = "Collapsed=" + "false" + "; expires=" + date.toUTCString() + "; path=/";
    }
    else {
        header.classList.add('collapsed');
        const days = 7;
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        document.cookie = "Collapsed=" + "true" + "; expires=" + date.toUTCString() + "; path=/";
    }
}

// Form dönüşü içerik ve toast yükleme işlevi

document.body.addEventListener('htmx:configRequest', function (evt) {
    const triggeredElement = evt.detail.elt;
    if (triggeredElement && triggeredElement.classList.contains("hxform")) {
        evt.detail.headers['Form-Send'] = 'true';
    }
    if (triggeredElement && triggeredElement.classList.contains("date-send")) {
        evt.detail.headers['Date-Send'] = 'true';
    }
    if (triggeredElement && triggeredElement.classList.contains("pagination-send")) {
        evt.detail.headers['Pagination-Send'] = 'true';
    }
    if (triggeredElement && triggeredElement.classList.contains("workUpdateForm")) {
        window.updateWorkFormSubmitHandler(evt);
    }
    if (triggeredElement && triggeredElement.classList.contains("workAddForm")) {
        window.addWorkFormSubmitHandler(evt);
    }
    if (triggeredElement && triggeredElement.classList.contains("messageAddForm")) {
        window.addMessageFormSubmitHandler(evt);
    }
    if (triggeredElement && triggeredElement.classList.contains("messageUpdateForm")) {
        window.updateMessageFormSubmitHandler(evt);
    }
});

document.body.addEventListener("htmx:afterRequest", function (event) {
    if (event.detail.requestConfig.headers["Form-Send"] == "true") {
        const xhr = event.detail.xhr;

        if (xhr.getResponseHeader("content-type")?.includes("application/json")) {
            const response = JSON.parse(xhr.responseText);

            if (response.success && response.html) {
                const tempDiv = document.createElement("div");
                tempDiv.innerHTML = response.html;

                const oobContent = tempDiv.querySelector("[hx-swap-oob]");
                if (oobContent) {
                    const targetId = oobContent.id;
                    const target = document.getElementById(targetId);
                    if (target) {
                        target.innerHTML = oobContent.innerHTML;
                        htmx.process(target);
                        $.validator.unobtrusive.parse(".validation");
                    }
                }
                reloadSidebar();
                window.toastInstance = null;
                showToast(response.message, response.type);
                if (event.detail.requestConfig.headers["Date-Send"] == "true") {
                    return;
                }
                else {
                    window.scrollBy({ top: -100, behavior: 'smooth' })
                }

                return;
            }
            else if (response.success) {
                if (response.message) {
                    window.toastInstance = null;
                    showToast(response.message, "success");
                }
                if (response.loadComponent) {
                    document.querySelector(`.sidebar-nav-item[data-target='${response.loadComponent}'] a`)?.click();
                    if (event.detail.requestConfig.headers["Note-Send"] != "true") {
                        window.scrollTo({ top: 0, behavior: 'smooth' });
                    }
                }
                reloadSidebar();
                return;
            }
            else if (response.html) {
                const tempDiv = document.createElement("div");
                tempDiv.innerHTML = response.html;

                const oobContent = tempDiv.querySelector("[hx-swap-oob]");
                if (oobContent) {
                    const targetId = oobContent.id;
                    const target = document.getElementById(targetId);
                    if (target) {
                        target.innerHTML = oobContent.innerHTML;
                        htmx.process(target);
                        $.validator.unobtrusive.parse(".validation");
                    }
                }
                window.toastInstance = null;
                window.scrollTo({ top: 0, behavior: 'smooth' })
                setTimeout(showToast(response.message, response.type || "danger"), 100);
                return;
            }


        }
    }
    else if (event.detail.requestConfig.headers["Pagination-Send"] == "true") {
        document.querySelector('.scroll-target')?.scrollIntoView({
            behavior: 'smooth',
            block: 'center'
        });
    }
    else if (event.detail.requestConfig.headers["Filter-Send"] != "true" && event.detail.requestConfig.headers["Date-Send"] != "true") {
        window.scrollTo({ top: 0, behavior: 'smooth' });
        return;
    }
});

// Ajax ile uyumlu jquery script çalıştırma işlevi ve sidebar aktif element işlevi

document.body.addEventListener("htmx:afterSwap", function (event) {
    const $scripts = $(event.target).find('script');

    const path = window.location.pathname;
    let firstSegment = path.split('/')[1];

    const roleMeta = document.querySelector('meta[name="user-role"]');
    const role = roleMeta ? roleMeta.getAttribute('content') : null;

    if (firstSegment === "Home") {
        firstSegment = role === "Admin" ? "HomeAdmin" : "Home";
    }

    const sidebarItems = document.querySelectorAll('.sidebar-nav-item');

    sidebarItems.forEach(item => {
        if (item.dataset.target === firstSegment) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
    $scripts.each(function () {
        $.globalEval(this.innerText || this.textContent || '');
    });
});


// İlk Sayfa açılışında AdminHome ve Home yönlendirmesi

document.addEventListener("DOMContentLoaded", function () {
    const path = window.location.pathname;
    let firstSegment = path.split('/')[1];

    const roleMeta = document.querySelector('meta[name="user-role"]');
    const role = roleMeta ? roleMeta.getAttribute('content') : null;

    if (firstSegment === "Home") {
        firstSegment = role === "Admin" ? "HomeAdmin" : "Home";
    }

    const sidebarItems = document.querySelectorAll('.sidebar-nav-item');

    sidebarItems.forEach(item => {
        if (item.dataset.target === firstSegment) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
});

// Genel form focus style işlemleri

window.handleInputFocus = function (event) {
    const input = event.target;
    const label = input.parentElement.querySelector('.form-label');
    if (label) label.style.color = '#2a9b5b';
}


window.handleInputBlur = function (event) {
    const input = event.target;
    const label = input.parentElement.querySelector('.form-label');
    if (label) label.style.color = '#2a4d3a';
}





            