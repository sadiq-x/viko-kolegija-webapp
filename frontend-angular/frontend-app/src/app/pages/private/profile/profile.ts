import { CommonModule } from '@angular/common';
import { Component, effect, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-profile',
  standalone: true,                           // ✅ obrigatório porque usas `imports`
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss']               // ✅ plural e array
})
export class Profile {
  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  imagePreview = signal<string | null>(null);

  constructor(private fb: FormBuilder) {
    const user = {
      username: 'john.doe',
      name: 'John Doe',
      email: 'john@viko.app',
      imageUrl: '/avatar-placeholder.png',
      numberPhone: '+351 912 345 678',
      address: 'Rua das Flores, 123, Lisboa'
    };

    this.profileForm = this.fb.group({
      username: [{ value: user.username, disabled: true }, [Validators.required]],
      name:     [{ value: user.name,     disabled: true }, [Validators.required]],
      email:    [{ value: user.email,    disabled: true }, [Validators.required, Validators.email]],
      image:    [null],
      numberPhone: [user.numberPhone, [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      address:     [user.address,     [Validators.required, Validators.maxLength(200)]],
    });

    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });

    effect(() => {
      const p = this.passwordForm.get('password')?.value ?? '';
      const c = this.passwordForm.get('confirmPassword')?.value ?? '';
      const mismatch = !!p && !!c && p !== c;
      if (mismatch) {
        this.passwordForm.get('confirmPassword')?.setErrors({ mismatch: true });
      } else if (this.passwordForm.get('confirmPassword')?.hasError('mismatch')) {
        const ctrl = this.passwordForm.get('confirmPassword')!;
        const errors = { ...(ctrl.errors ?? {}) };
        delete (errors as any).mismatch;
        ctrl.setErrors(Object.keys(errors).length ? errors : null);
      }
    });
  }

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

  saveProfile() {
    if (this.profileForm.invalid || !this.profileForm.dirty) {
      this.profileForm.markAllAsTouched();
      return;
    }
    const payload = {
      numberPhone: this.profileForm.value.numberPhone,
      address: this.profileForm.value.address,
    };
    const imageFile = this.profileForm.value.image as File | null;
    console.log('PROFILE UPDATE payload:', payload, imageFile);
  }

  savePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    const { password } = this.passwordForm.value;
    console.log('PASSWORD UPDATE payload:', { password });
  }

  get profileDisabled() { return this.profileForm.invalid || !this.profileForm.dirty; }
  get passwordDisabled() { return this.passwordForm.invalid; }
}