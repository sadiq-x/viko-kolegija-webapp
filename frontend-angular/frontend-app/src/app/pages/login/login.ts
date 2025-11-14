import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/authService';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  //Login form
  form!: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.form = this.fb.group({
      username: ['', [Validators.required, this.noSpacesValidator]],
      password: ['', [Validators.required]],
      remember: [false],
    });
  }
  //Function to mage login
  onLoginSubmit() {
    var credentials = {
      Username: this.form.value.username.replace(/\s+/g, ''),
      PasswordHash: this.form.value.password,
    };

    if (this.form.invalid && credentials == null) {
      this.form.markAllAsTouched();
      return;
    }

    this.authService.login(credentials, this.form.value.remember).subscribe({
      next: (res: boolean) => {},
      error: (e) => {},
    });
  }
  //Function to validate spaces
  noSpacesValidator(control: any) {
    const hasSpaces = /\s/.test(control.value);
    return hasSpaces ? { spacesNotAllowed: true } : null;
  }
}
