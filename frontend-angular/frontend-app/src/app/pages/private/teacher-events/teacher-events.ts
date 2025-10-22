import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { catchError, of } from 'rxjs';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-teacher-event',
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink],
  templateUrl: './teacher-events.html',
  styleUrls: ['./teacher-events.scss'],
})
export class TeacherEvents {
  eventId!: number;
  course: any = null;
  loading = true;


  // TODO: REVER CODIGO
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.eventId = Number(this.route.snapshot.paramMap.get('id'));

    // 1) tenta obter do router state (vem da lista)
    const passed = history.state?.['course']; // funciona após navegação normal
    console.log(passed)
    if (passed) {
      this.course = passed;
      this.loading = false;
      return;
    }

    // 2) fallback opcional (caso refresh/URL direta)
    const url = `http://localhost:4200/teacher/course/${this.eventId}`;
    this.http.get<{success:boolean; course:any}>(url)
      .pipe(
        catchError((e) => { console.error(e); return of({success:false, course:null}); })
      )
      .subscribe(res => {
        this.course = res.success ? res.course : null;
        this.loading = false;
      });
  }
}