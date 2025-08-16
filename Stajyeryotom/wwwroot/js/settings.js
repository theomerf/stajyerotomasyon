window.initSettingsScript = function () {
    var activeView = getCookie("View") || "List";
    var radios = document.querySelectorAll('input[name="broadcast"]');
    radios.forEach(function (radio) {
        radio.checked = radio.value.toLowerCase() === activeView.toLowerCase();
    });

    window.settingsFormSubmit = function (e) {
        e.preventDefault();
        var selected = document.querySelector('input[name="broadcast"]:checked');
        if (selected) {
            setCookie("View", selected.value, 365);
            showToast('Ayarlar güncellendi', 'success');
        }
    }

    function getCookie(name) {
        var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? match[2] : null;
    }

    function setCookie(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + value + expires + "; path=/";
    }
}
