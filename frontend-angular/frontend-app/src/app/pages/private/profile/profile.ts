import { CommonModule } from '@angular/common';
import { Component, OnInit, effect, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService } from '../../../services/profile';
import { ModelUserProfile } from '../../../models/modelUser';

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

  constructor(private fb: FormBuilder, private profile: ProfileService) {
    // Forms vazios (readonly onde aplicável)
    this.profileForm = this.fb.group({
      username: [{ value: '', disabled: true }, [Validators.required]],
      name: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      image: [null],
      numberPhone: ['', [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      address: ['', [Validators.required, Validators.maxLength(200)]],
    });

    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.profile.getProfile().subscribe({
      next: (res: ModelUserProfile | false) => {
        console.log('📥 Res (do serviço):', res);

        if (res === false) {
          console.warn('⚠️ Serviço devolveu false (sem dados ou erro).');
          this.fillFormEmpty();
          return;
        }

        // 🔀 Mapa tolerante a casing (PascalCase/camelCase)
        const u: any = res;
        console.log(u)
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

        console.log('✅ Mapeado para o formulário:', mapped);
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
        console.error('❌ Erro no subscribe do perfil:', err);
        this.fillFormEmpty();
      }
    });
  }

  // ======= helpers =======
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

    console.log('📤 PROFILE UPDATE payload:', payload, imageFile);
  }

  savePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.passwordForm.value;

    // ✅ Validação explícita antes de enviar
    if (password !== confirmPassword) {
      console.error("❌ As passwords não coincidem!");
      this.passwordForm.get('confirmPassword')?.setErrors({ mismatch: true });
      return;
    }

    console.log('📤 PASSWORD UPDATE payload:', { password });
    // TODO: chamar serviço para update da password
  }

  get profileDisabled() { return this.profileForm.invalid || !this.profileForm.dirty; }
  get passwordDisabled() { return this.passwordForm.invalid; }
}