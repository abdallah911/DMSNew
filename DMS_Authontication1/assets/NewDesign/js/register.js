/* =================================Register==================================== */

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


  // ===================validation===========================

let emailMsg = document.getElementById('emailMsg')
let signUpPass = document.getElementById('signUpPass')
let userEmailRigester = document.getElementById('userEmailRigester')
let passMsg = document.getElementById('passMsg')
let userNameRigester = document.getElementById('userNameRigester')
let signUpRePass = document.getElementById('signUpRePass')
let userEmailRigesterTouched = false;
let signUpPassTouched = false;
let userNameTouched = false;
let signUpRePassTouched = false;

userEmailRigester.addEventListener('focus',()=>{
  userEmailRigesterTouched = true ; 
})
signUpRePass.addEventListener('focus',()=>{
  signUpRePassTouched = true ; 
})

signUpPass.addEventListener('focus',()=>{
  signUpPassTouched = true;
})
userNameRigester.addEventListener('focus',()=>{
  userNameTouched = true;
})

function nameValidation() {
  return /^[a-zA-Z]{2,}$/.test(userNameRigester.value);
}

function emailValidation() {
  return /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/.test(
    userEmailRigester.value
  );
}
function passValidation() {
  return /^[A-Za-z0-9]{8,10}$/.test(
    signUpPass.value
  );
}
function rePassValidation() {
  return document.getElementById("signUpRePass").value == document.getElementById("signUpPass").value;

}

function inputsValidation() {
  if(userNameTouched){
    if(nameValidation()){
      document.getElementById('nameMsg').classList.replace('d-block','d-none')
    }else{
      document.getElementById('nameMsg').classList.replace('d-none','d-block')
    
    }
  }
  if(userEmailRigesterTouched){
    if(emailValidation()){
      document.getElementById('emailMsg').classList.replace('d-block','d-none')
    }else{
      document.getElementById('emailMsg').classList.replace('d-none','d-block')
    
    }
  }
  if(signUpPassTouched){
    if(passValidation()){
      document.getElementById('passMsg').classList.replace('d-block','d-none')
    }else{
      document.getElementById('passMsg').classList.replace('d-none','d-block')
    
    }
  }
  if(signUpRePassTouched){
    if(rePassValidation()){
      document.getElementById('rePassMsg').classList.replace('d-block','d-none')
    }else{
      document.getElementById('rePassMsg').classList.replace('d-none','d-block')
    
    }
  }
 
  if (
    nameValidation() &&
    emailValidation() &&
    passValidation() &&
    rePassValidation() 
  ) {
    document.getElementById("signUp").removeAttribute("disabled");
  } else {
    document.getElementById("signUp").setAttribute("disabled", true);
  }
}

