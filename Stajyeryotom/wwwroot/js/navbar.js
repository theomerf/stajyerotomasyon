function initNavbarScripts() {
    var topNavbar = document.querySelector('.custom-navbar');

    var dropdownElementList = [].slice.call(document.querySelectorAll('.user-dropdown .dropdown-toggle'));

    dropdownElementList.forEach(function (dropdownToggleEl) {
        var dropdownMenu = dropdownToggleEl.nextElementSibling;

        if (dropdownMenu) {
            dropdownMenu.classList.add('animate__animated');
        }

        dropdownToggleEl.addEventListener('shown.bs.dropdown', function () {
            dropdownMenu.classList.remove('animate__fadeOutDown');
            dropdownMenu.classList.add('animate__fadeInUp');
        });

        dropdownToggleEl.addEventListener('hide.bs.dropdown', function (e) {
            if (!dropdownMenu.classList.contains('animate__fadeOutDown')) {
                e.preventDefault();
                dropdownMenu.classList.remove('animate__fadeInUp');
                dropdownMenu.classList.add('animate__fadeOutDown');

                setTimeout(function () {
                    var bsDropdown = bootstrap.Dropdown.getInstance(dropdownToggleEl);
                    if (bsDropdown) {
                        bsDropdown.hide();
                    }
                    dropdownMenu.classList.remove('animate__fadeOutDown');
                }, 300);
            }
        });
    });

    window.addEventListener('scroll', function () {
        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;

        if (scrollTop > 50) {
            topNavbar.classList.add('scrolled');
        } else {
            topNavbar.classList.remove('scrolled');
        }
    });
}
