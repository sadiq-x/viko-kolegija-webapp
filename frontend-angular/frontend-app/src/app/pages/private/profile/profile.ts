import { CommonModule } from '@angular/common';
import { Component, OnInit, effect, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../../../services/profile';
import { ModelUserProfile } from '../../../models/modelUser';
import { AuthService } from '../../../services/auth';

const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-profile',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss']
})
export class Profile implements OnInit {
  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  imagePreview = signal<string | null>(null);

  constructor(private fb: FormBuilder, private profile: ProfileService, private auth: AuthService) {
    //Start the user form empty - if empty the fields going empty
    this.profileForm = this.fb.group({
      username: [{ value: '', disabled: true }, [Validators.required]],
      name: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      image: [null],
      numberPhone: ['', [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      address: ['', [Validators.required, Validators.maxLength(200)]],
    });
    //Start the password form empty - if empty the fields going empty
    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.profile.getProfile().subscribe({
      next: (res: ModelUserProfile | false) => {
        if (res === false) {
          this.fillFormEmpty();
          return;
        }

        const u: any = res;
        //Mapped of data with Camel Case and Pascal Case
        const mapped: ModelUserProfile = {
          Id: u.Id ?? u.id ?? 0,
          Username: u.Username ?? u.username ?? '',
          Name: u.Name ?? u.name ?? '',
          Email: u.Email ?? u.email ?? '',
          Image: u.Image ?? u.image ?? null,
          NumberPhone: u.NumberPhone ?? u.numberPhone ?? '',
          Address: u.Address ?? u.address ?? '',
        };

        //Form filled with the correct data
        this.profileForm.patchValue({
          username: mapped.Username,
          name: mapped.Name,
          email: mapped.Email,
          numberPhone: mapped.NumberPhone,
          address: mapped.Address,
        });
        if (mapped.Image) this.imagePreview.set(mapped.Image);

        this.profileForm.markAsPristine();
      },
      error: (err) => {
        this.fillFormEmpty();
      }
    });
  }

  //Function to make the form empty
  private fillFormEmpty() {
    this.profileForm.patchValue({
      username: '',
      name: '',
      email: '',
      numberPhone: '',
      address: ''
    });
    this.imagePreview.set(null);
    this.profileForm.markAsPristine();
  }

  // ======= handlers =======
  onImageSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    const file = input.files[0];
    this.profileForm.patchValue({ image: file });
    this.profileForm.get('image')?.markAsDirty();

    const reader = new FileReader();
    reader.onload = () => this.imagePreview.set(reader.result as string);
    reader.readAsDataURL(file);
  }

  //Function to save the new data of user
  saveProfile() {
    console.log(this.profileForm)
    if (this.profileForm.invalid || !this.profileForm.dirty) {
      this.profileForm.markAllAsTouched();

      return;
    }
    const payload = {
      numberPhone: this.profileForm.value.numberPhone,
      address: this.profileForm.value.address,
    };
    const imageFile = this.profileForm.value.image as File | null;

    console.log('📤 PROFILE UPDATE payload:', payload, imageFile);
    this.profileForm.markAsPristine();
    this.profileForm.markAsUntouched();
    // TODO: chamar serviço para update da password

  }

  //Function to save the new password of user
  savePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.passwordForm.value;

    //Validation of password and confirmPassword 
    if (password !== confirmPassword) {
      console.error("❌ As passwords não coincidem!");
      this.passwordForm.get('confirmPassword')?.setErrors({ mismatch: true });
      return;
    }
    const localStorageItem = localStorage.getItem("authUser");
    const EntityId = JSON.parse(localStorageItem!);
    console.log(EntityId.entityId)
    var payload = {
      "EntityId": EntityId.entityId,
      "Username": this.profileForm.get('username')?.value,
      "PasswordHash": password
    };

    this.auth.updatePassword(payload).subscribe({
      next: (res: boolean) => {
        console.log(res)
        if (!res) {
          console.warn('⚠️ updatePassword devolveu false');
          return;
        }
        alert('Password atualizada com sucesso!');

        this.passwordForm.reset();
        this.passwordForm.markAsPristine();
        this.passwordForm.markAsUntouched();
      },
      error: (err) => {
        console.error('❌ Erro no updatePassword:', err);
      }
    });
  }

  get profileDisabled() { return this.profileForm.invalid || !this.profileForm.dirty; }
  get passwordDisabled() { return this.passwordForm.invalid; }
}