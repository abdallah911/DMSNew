let eyeIcons = document.querySelectorAll(".eyeIcon");

eyeIcons.forEach((eyeIcon) => {
  eyeIcon.addEventListener("click", () => {
    let pwField = eyeIcon.parentElement.querySelectorAll(".password");
    pwField.forEach((hamozo) => {
      if (hamozo.type === "password") {
        hamozo.type = "text";
        eyeIcon.classList.replace("fa-eye-slash", "fa-eye");
        return;
      } else {
        hamozo.type = "password";
        eyeIcon.classList.replace("fa-eye", "fa-eye-slash");
      }
    });
  });
});





