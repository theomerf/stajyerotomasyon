$(document).ready(function () {
    $('body').on('submit', '#loginForm', function (e) {
        e.preventDefault();

        clearFormErrors();

        var token = $('input[name="__RequestVerificationToken"]').val();
        if (!token) {
            console.error("CSRF token bulunamadı!");
            return;
        }

        var formData = $(this).serialize();

        var currentName = $("#loginForm input[name='Name']").val();
        var currentPassword = $("#loginForm input[name='Password']").val();
        var currentRememberMe = $("#loginForm input[name='RememberMe']").is(":checked");

        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: formData,
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            },
            success: function (response) {
                if (response.success) {
                    showMessage(response.message || "Giriş başarılı!", "success");

                    setTimeout(function () {
                        window.location.href = response.redirectUrl || "/";
                    }, 1000);
                } else {

                    if (response.errors && response.errors.length > 0) {
                        displayFormErrors(response.errors);
                    }

                    $.ajax({
                        url: window.location.href,
                        type: "GET",
                        success: function (data) {
                            let $data = $(data);
                            let $newForm = $data.find("#loginForm");
                            if ($newForm.length) {
                                $("#loginForm").replaceWith($newForm);
                                $("#loginForm input[name='Name']").val(currentName);
                                $("#loginForm input[name='Password']").val(currentPassword);
                                $("#loginForm input[name='RememberMe']").prop("checked", currentRememberMe);
                                showMessage(response.message || "Giriş başarısız!", "error");
                                initializeLoginScripts();
                            }
                        },
                        error: function () {
                            window.location.reload();
                        }
                    });
                }
            },
            error: function () {
                showMessage("Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.", "error");
            }
        });
    });
});



function clearFormErrors() {
    $(".text-danger").html(""); 
    $(".login-message").remove(); 
}

function showMessage(message, type) {
    $(".login-message").remove();

    var alertClass = type === "success" ? "alert alert-success" : "alert alert-danger";

    var alertHtml = '<div class="' + alertClass + ' login-message">' + message + '</div>';
    $("#loginForm").prepend(alertHtml);
}

function displayFormErrors(errors) {
    errors.forEach(function (error) {
        var fieldName = error.Key;
        var errorMessages = error.Errors.join("<br>");

        $("span[data-valmsg-for='" + fieldName + "']").html(errorMessages);
    });
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("toggle-login-password")?.addEventListener("click", function () {
        togglePassword("login-password", "login-eye-icon");
    });
});

function initializeLoginScripts() {
    document.getElementById("toggle-login-password")?.addEventListener("click", function () {
        togglePassword("login-password", "login-eye-icon");
    });
}

function togglePassword(inputId, iconId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(iconId);
    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        input.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
}




