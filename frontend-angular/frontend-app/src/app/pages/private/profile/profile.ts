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
  styleUrls: ['./profile.scss']
})
export class Profile implements OnInit {
  profileForm!: FormGroup; //Form of profile
  passwordForm!: FormGroup; //Form of password update
  imagePreview = signal<string | null>(null);

  dialCodes: ModelDialCode[] = DIAL_CODES;
  trackByDial = (_: number, d: ModelDialCode) => d.dial;

  constructor(private fb: FormBuilder, private profile: ProfileService, private auth: AuthService) {
    //Start the user form empty - if empty the fields going empty
    this.profileForm = this.fb.group({
      username: [{ value: '', disabled: true }, [Validators.required]],
      name: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      image: [null],
      countryCode: ['+370', Validators.required],
      numberPhone: ['', [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      address: ['', [Validators.required, Validators.maxLength(200)]],
    });
    //Start the password form empty - if empty the fields going empty
    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });
  };

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
      address: ''
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
      const base64WithPrefix = reader.result as string;    //Full image code prefix+base64 ex: "data:image/jpeg;base64,AAAA..."
      //const base64Only = base64WithPrefix.split(',')[1];   //Just the bytes string base64

      //Update the preview of image
      this.imagePreview.set(base64WithPrefix);

      //Save in the form the string base64 of image
      this.profileForm.patchValue({ image: base64WithPrefix });
    };
    reader.readAsDataURL(file);
  };
  //Function to save the new data of user
  updateProfile() {
    if (this.profileForm.invalid || !this.profileForm.dirty) {
      this.profileForm.markAllAsTouched();

      return;
    };

    const localStorageItem = localStorage.getItem("authUser");
    const localStorageItemParsed = JSON.parse(localStorageItem!);
    const payload = {
      "EntityId": localStorageItemParsed.entityId,
      "Username": this.profileForm.get('username')?.value,
      "Email": this.profileForm.get('email')?.value,
      "Image": this.profileForm.value.image,
      "NumberPhone": this.getFullPhone(),
      "Address": this.profileForm.value.address,

    };
    console.log(this.profileForm.value)
    console.log('📤 PROFILE UPDATE payload:', payload);

    this.profile.updateProfile(payload).subscribe({
      next: (res) => {
        console.log(res)
        if (res) {
          alert('Profile updated successful.');

          this.profileForm.markAsPristine();
          this.profileForm.markAsUntouched();
          this.loadProfile();
        }
      }
    })
  };
  //Function to load user profile
  private loadProfile() {
    this.profile.getProfile().subscribe({
      next: (res: ModelUserProfileResponse | false) => {
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
          NumberPhone: u.NumberPhone ?? u.numberPhone ?? '', // vem tipo "351912345678"
          Address: u.Address ?? u.address ?? '',
        };

        // 👇 separa indicativo e número (fallback para +351, ajusta se preferires outro)
        const { countryCode, number } = splitDialAndNumber(mapped.NumberPhone);

        // Preenche o form
        this.profileForm.patchValue({
          username: mapped.Username,
          name: mapped.Name,
          email: mapped.Email,
          countryCode,            // novo controlo no form
          numberPhone: number,   // só os dígitos do número
          address: mapped.Address,
        });

        if (mapped.Image) this.imagePreview.set(mapped.Image);

        this.profileForm.markAsPristine();
      },
      error: (_) => {
        this.fillFormEmpty();
      }
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
    };

    //Get the localStorage to pick EntityId and Username
    const localStorageItem = localStorage.getItem("authUser");
    const EntityId = JSON.parse(localStorageItem!);
    var payload = {
      "EntityId": EntityId.entityId,
      "Username": this.profileForm.get('username')?.value,
      "PasswordHash": password
    };

    //Http request subscribe
    this.auth.updatePassword(payload).subscribe({
      next: (res: boolean) => {
        if (!res) {
          alert('Password impossible to updated.');
          return;
        }
        alert('Password updated successful.');

        this.passwordForm.reset();
        this.passwordForm.markAsPristine();
        this.passwordForm.markAsUntouched();
      },
      error: (err) => {
      }
    });
  };
  //Function to logout user
  logout() {
    this.auth.logout();
    this.profileForm.reset();
    this.passwordForm.reset();

    this.profileForm.markAsPristine();
    this.passwordForm.markAsPristine();

    alert("You have been logged out successfully.");
  };
  //Get full number phone with dial code and number phone
  getFullPhone(): string {
    const { countryCode, numberPhone } = this.profileForm.getRawValue();
    return `${countryCode}${numberPhone.replace(/\s+/g, '')}`;
  };

  get profileDisabled() { return this.profileForm.invalid || !this.profileForm.dirty; }
  get passwordDisabled() { return this.passwordForm.invalid; }
}