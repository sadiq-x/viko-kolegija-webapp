import { Component, computed } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from '@angular/forms';
import { RouterLink } from '@angular/router';

const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;

function matchFieldsValidator(a: string, b: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const v1 = group.get(a)?.value;
    const v2 = group.get(b)?.value;
    return v1 && v2 && v1 !== v2 ? { fieldsMismatch: true } : null;
  };
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.scss']
})
export class Register {

  // declara primeiro; será atribuído no constructor
  form!: FormGroup;
  
  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      countryCode: ['+370', [Validators.required]],
      phone: ['', [Validators.required, Validators.maxLength(20), Validators.pattern(PHONE_PATTERN)]],
      address: ['', [Validators.required, Validators.maxLength(200)]],
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    }, { validators: [matchFieldsValidator('password', 'confirmPassword')] });
  }

  get submitDisabled() { return this.form.invalid; }


  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const fullPhone = `${this.form.value.countryCode}${this.form.value.phone}`;
    const payload = { ...this.form.value, phone: fullPhone };
    console.log('Register payload:', payload);
  }
}