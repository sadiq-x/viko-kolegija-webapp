import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  EventListByIdRequest,
  EventListResponse,
  ModelEventsRequest,
} from '../../../models/modelEvents';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { TeacherService } from '../../../services/teacher';
import { CommonModule } from '@angular/common';
import { TopicsService } from '../../../services/topics';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-teacher',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './teacher.html',
  styleUrl: './teacher.scss',
})
export class Teacher implements OnInit {
  form!: FormGroup;

  topics = signal<ModelTopicsResponse[]>([]);
  myCourses = signal<EventListResponse[]>([]);
  loading = signal<boolean>(false);
  submitting = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private teacherService: TeacherService,
    private topicService: TopicsService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      topicId: [null, [Validators.required]],
    });
  }


  ngOnInit(): void {
    this.loadTopics();
    this.loadMyCourses();
  }

  private loadTopics() {
    this.loading.set(true);
    this.topicService.getTopics().subscribe({
      next: (res) => {
        if (Array.isArray(res)) {
          this.topics.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Type: x.Type ?? x.type,
              Description: x.Description ?? x.description,
            }))
          ); // ✅ quando é array, coloca na signal
        } else {
          this.topics.set([]); // falso/erro → lista vazia
        }
      },
      error: () => this.topics.set([]),
      complete: () => this.loading.set(false),
    });
  }

  private loadMyCourses() {
    this.loading.set(true);
    console.log(this.getCurrentTeacherId());

    if (!this.getCurrentTeacherId()) {
      return;
    }

    const teacherId: EventListByIdRequest = {
      CreateById: this.getCurrentTeacherId()!,
    };

    this.teacherService.getEventById(teacherId).subscribe({
      next: (res) => {
        console.log(res);

        if (Array.isArray(res)) {
          this.myCourses.set(
            res.map((x: any) => ({
              Id: x.Id ?? x.id,
              Name: x.Name ?? x.name,
              Description: x.Description ?? x.description,
              TopicName: x.TopicName ?? x.topicName,
              CreateById: x.CreateById ?? x.createById,
              DateCreate: x.DateCreate ?? x.dateCreate,
              Status: x.Status ?? x.status,
              Results: x.Results ?? x.results,
            }))
          );
        }
      },
      error: () => this.myCourses.set([]),
      complete: () => this.loading.set(false),
    });
  }

  private getCurrentTeacherId(): number | null {
    const raw = localStorage.getItem('authUser');
    if (!raw) return null;
    try {
      const parsed = JSON.parse(raw);
      return Number(parsed.entityId) || null;
    } catch {
      return null;
    }
  }

  onCreateCourse() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const teacherId = this.getCurrentTeacherId();
    if (!teacherId) {
      alert('No teacher logged in.');
      return;
    }

    const payload: ModelEventsRequest = {
      Name: this.form.value.name,
      Description: this.form.value.description,
      TopicsId: this.form.value.topicId,
      CreateById: teacherId,
      DateCreate: new Date().toISOString(),
    };

    this.submitting.set(true);
    this.teacherService.createEvent(payload).subscribe({
      next: (res) => {
        if (!res) {
          alert('Could not create course.');
          return;
        }
        console.log(res);
        alert('Course created successfully.');
        this.form.reset();
        this.form.markAsPristine();
        this.form.markAsUntouched();
        this.loadMyCourses();
      },
      error: () => alert('Error creating course.'),
      complete: () => this.submitting.set(false),
    });
  }

  // helper de estado do botão
  get submitDisabled() {
    return this.form.invalid || this.submitting();
  }
}
