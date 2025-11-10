import { Component, computed, signal } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  FormGroup,
} from '@angular/forms';
import { Route, Router, RouterLink } from '@angular/router';
import { DIAL_CODES } from '../../data/dial-codes';
import { ModelDialCode } from '../../models/model-dial-codes';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/authService';
import { ModelUserRegisterRequest } from '../../models/modelUser';

//Annotation for phone and password
const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

//Function to verify password and confirmPassword, if are the same
function matchFieldsValidator(a: string, b: string): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const v1 = group.get(a)?.value;
    const v2 = group.get(b)?.value;
    return v1 && v2 && v1 !== v2 ? { fieldsMismatch: true } : null;
  };
}

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
})
export class Register {
  formRegister!: FormGroup; //Form of register
  imagePreview: string | null = null; //Variables of image preview

  dialCodes: ModelDialCode[] = DIAL_CODES; //Dial code helpers

  trackByDial = (_: number, d: ModelDialCode) => d.dial;

  constructor(private fb: FormBuilder, private auth: AuthService, private route: Router) {
    this.formRegister = this.fb.group(
      {
        username: ['', [Validators.required]],
        name: ['', [Validators.required, Validators.maxLength(100)]],
        email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
        countryCode: ['+370', [Validators.required]],
        phone: [
          '',
          [Validators.required, Validators.maxLength(20), Validators.pattern(PHONE_PATTERN)],
        ],
        address: ['', [Validators.required, Validators.maxLength(200)]],
        image: [null],
        birthday: ['', [Validators.required]],
        nationality: ['', [Validators.required, Validators.maxLength(20)]],
        gender: ['', [Validators.required, Validators.maxLength(20)]],
        password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: [matchFieldsValidator('password', 'confirmPassword')] }
    );
  }

  //Function to submit new register
  onSubmit() {
    if (this.formRegister.invalid) {
      this.formRegister.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.formRegister.value;

    //Validation of password and confirmPassword
    if (password !== confirmPassword) {
      this.formRegister.get('confirmPassword')?.setErrors({ mismatch: true });
      return;
    }

    //Payload to send to backend
    const v = this.formRegister.value;
    const payload: ModelUserRegisterRequest = {
      Username: v.username,
      PasswordHash: v.password,
      ConfirmPasswordHash: v.confirmPassword,
      Name: v.name,
      Email: v.email,
      NumberPhone: this.getFullPhone(),
      Address: v.address,
      Birthday: v.birthday,
      Nationality: v.nationality,
      Gender: v.gender,
      Image: v.image,
    };
    console.log('Register payload:', payload);

    this.auth.register(payload).subscribe({
      next: (res: boolean) => {
        console.log(res);
        if (res) {
          this.route.navigate(['/register/profile'], { state: {user : payload} });
          //Need to create a page to show the new user created.
          //TODO: Todo this ->
        }
      },
    });
  }
  //Function on image select
  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.formRegister.get('image')?.markAsDirty();

    const reader = new FileReader();
    reader.onload = () => {
      const base64WithPrefix = reader.result as string; //Full image code prefix+base64 ex: "data:image/jpeg;base64,AAAA..."
      //const base64Only = base64WithPrefix.split(',')[1];   //Just the bytes string base64

      //Update the preview of image
      this.imagePreview = base64WithPrefix;

      //Save in the form the string base64 of image
      this.formRegister.patchValue({ image: base64WithPrefix });
    };
    reader.readAsDataURL(file);
  }
  //Get full number phone with dial code and number phone
  getFullPhone(): string {
    const { countryCode, phone } = this.formRegister.getRawValue();
    return `${countryCode}${phone.replace(/\s+/g, '')}`;
  }
  //Verify if the input number phone is only numbers
  allowOnlyNumbers(event: Event) {
    const input = event.target as HTMLInputElement;

    //Remove everything with character or symbol
    input.value = input.value.replace(/[^0-9]/g, '');

    //Update the form with clean value
    this.formRegister.patchValue({ numberPhone: input.value });
  }
  //Property to disable the buttons
  get submitDisabled() {
    return this.formRegister.invalid;
  }
}
