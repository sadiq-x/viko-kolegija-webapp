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
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      remember: [false],
    });
  }

  onLoginSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    };

    var credentials = {
      Username: this.form.value.username,
      PasswordHash: this.form.value.password,
    };

    this.authService.login(credentials, this.form.value.remember).subscribe({
      next: (res: boolean) => {},
      error: (e) => {
        console.log(e);
      },
    });
  }
}
