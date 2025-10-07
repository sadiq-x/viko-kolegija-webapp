import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})

export class Login {

  form!: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
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
    }
    const payload = this.form.value; // { username, password, remember }
    console.log('Login payload:', payload);

    var credentials = {
      "Username": this.form.value.username,
      "PasswordHash": this.form.value.password
    };
    console.log(credentials);

    this.authService.login(credentials).subscribe({
      next: (status: boolean) => {
        this.router.navigate(['/dashboard'])
        console.log(status)
      }, error: (e) => {
        console.log(e)
      }
    })
  }
}

