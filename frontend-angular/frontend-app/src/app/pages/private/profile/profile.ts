import { CommonModule } from '@angular/common';
import { Component, OnInit, effect, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../../../services/profile';
import { ModelUserProfileResponse } from '../../../models/modelUser';
import { AuthService } from '../../../services/authService';
import { DIAL_CODES } from '../../../data/dial-codes';
import { ModelDialCode } from '../../../models/model-dial-codes';
import { splitDialAndNumber } from '../../../services/dial-code-helper';

//Annotation for phone and password
const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-profile',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss'],
})
export class Profile implements OnInit {

  profileForm!: FormGroup; //Form of profile
  passwordForm!: FormGroup; //Form of password update
  imagePreview = signal<string | null>(null); //Variables of image preview

  //Dial code helpers
  dialCodes: ModelDialCode[] = DIAL_CODES;
  trackByDial = (_: number, d: ModelDialCode) => d.dial;

  constructor(private fb: FormBuilder, private profileService: ProfileService, private authService: AuthService) {
    //Start the user form empty - if empty the fields going empty
    this.profileForm = this.fb.group({
      username: [{ value: '', disabled: true }, [Validators.required]],
      name: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      image: [null],
      countryCode: ['+370', Validators.required],
      numberPhone: [
        '',
        [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)],
      ],
      address: ['', [Validators.required, Validators.maxLength(200)]],
      birthday: ['', [Validators.required]],
      nationality: ['', [Validators.required, Validators.maxLength(20)]],
      gender: ['', [Validators.required, Validators.maxLength(20)]],
    });
    //Start the password form empty - if empty the fields going empty
    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  };

  //Function to make the form empty
  private fillFormEmpty() {
    this.profileForm.patchValue({
      username: '',
      name: '',
      email: '',
      countryCode: '+370',
      numberPhone: '',
      address: '',
      birthday: '',
      nationality: '',
      gender: '',
    });
    this.imagePreview.set(null);
    this.profileForm.markAsPristine();
  };
  //Function on image select
  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    this.profileForm.get('image')?.markAsDirty();

    const reader = new FileReader();
    reader.onload = () => {
      const base64WithPrefix = reader.result as string; //Full image code prefix+base64 ex: "data:image/jpeg;base64,AAAA..."
      //const base64Only = base64WithPrefix.split(',')[1];   //Just the bytes string base64

      //Update the preview of image
      this.imagePreview.set(base64WithPrefix);

      //Save in the form the string base64 of image
      this.profileForm.patchValue({ image: base64WithPrefix });
    };
    reader.readAsDataURL(file);
  }
  //Function to save the new data of user
  updateProfile() {
    if (this.profileForm.invalid || !this.profileForm.dirty) {
      this.profileForm.markAllAsTouched();

      return;
    }

    const payload = {
      Username: this.profileForm.get('username')?.value,
      Email: this.profileForm.get('email')?.value,
      Image: this.profileForm.value.image,
      NumberPhone: this.getFullPhone(),
      Address: this.profileForm.value.address,
      Birthday: this.profileForm.value.birthday,
      Nationality: this.profileForm.value.nationality,
      Gender: this.profileForm.value.gender,
    };

    this.profileService.updateProfile(payload).subscribe({
      next: (res) => {
        console.log(res);
        if (res) {
          alert('Profile updated successful.');

          this.profileForm.markAsPristine();
          this.profileForm.markAsUntouched();
          this.loadProfile();
          return;
        }
        alert('Profile not updated successful.');
        return;
      },
    });
  }
  //Function to load user profile
  private loadProfile() {
    this.profileService.getProfile().subscribe({
      next: (res) => {
        if (res === false) {
          this.fillFormEmpty();
          return;
        }

        const u: any = res;
        const mapped: ModelUserProfileResponse = {
          Id: u.Id ?? u.id ?? 0,
          Username: u.Username ?? u.username ?? '',
          Name: u.Name ?? u.name ?? '',
          Email: u.Email ?? u.email ?? '',
          Image: u.Image ?? u.image ?? null,
          NumberPhone: u.NumberPhone ?? u.numberPhone ?? '',
          Address: u.Address ?? u.address ?? '',
          Birthday: u.Birthday ?? u.birthday ?? '',
          Nationality: u.Nationality ?? u.nationality ?? '',
          Gender: u.Gender ?? u.gender ?? '',
        };

        // 👇 separa indicativo e número (fallback para +351, ajusta se preferires outro)
        const { countryCode, number } = splitDialAndNumber(mapped.NumberPhone);

        // Preenche o form
        this.profileForm.patchValue({
          username: mapped.Username,
          name: mapped.Name,
          email: mapped.Email,
          countryCode,
          numberPhone: number,
          address: mapped.Address,
          birthday: mapped.Birthday,
          nationality: mapped.Nationality,
          gender: mapped.Gender,
        });
        if (mapped.Image) this.imagePreview.set(mapped.Image);

        this.profileForm.markAsPristine();
      },
      error: (_) => {
        this.fillFormEmpty();
      },
    });
  };
  //Function to save the new password of user
  updatePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.passwordForm.value;

    //Validation of password and confirmPassword
    if (password !== confirmPassword) {
      this.passwordForm.get('confirmPassword')?.setErrors({ mismatch: true });
      return;
    }

    var payload = {
      Username: this.profileForm.get('username')?.value,
      PasswordHash: password,
    };

    //Http request subscribe
    this.authService.updatePassword(payload).subscribe({
      next: (res) => {
        if (!res) {
          alert('Password impossible to updated.');
          return;
        }
        alert('Password updated successful.');

        this.passwordForm.reset();
        this.passwordForm.markAsPristine();
        this.passwordForm.markAsUntouched();
      },
      error: (err) => {},
    });
  };
  //Function to logout user
  logout() {
    this.authService.logout();
    this.profileForm.reset();
    this.passwordForm.reset();

    this.profileForm.markAsPristine();
    this.passwordForm.markAsPristine();

    alert('You have been logged out successfully.');
  };
  //Get full number phone with dial code and number phone
  getFullPhone(): string {
    const { countryCode, numberPhone } = this.profileForm.getRawValue();
    return `${countryCode}${numberPhone.replace(/\s+/g, '')}`;
  };
  //Verify if the input number phone is only numbers
  allowOnlyNumbers(event: Event) {
    const input = event.target as HTMLInputElement;

    //Remove everything with character or symbol
    input.value = input.value.replace(/[^0-9]/g, '');

    //Update the form with clean value
    this.profileForm.patchValue({ numberPhone: input.value });
  };
  //Properties to disable the buttons
  get profileDisabled() {
    return this.profileForm.invalid || !this.profileForm.dirty;
  };
  get passwordDisabled() {
    return this.passwordForm.invalid;
  };
}
