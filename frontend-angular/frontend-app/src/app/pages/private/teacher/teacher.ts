import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventListResponse, ModelEventsRequest } from '../../../models/modelEvents';
import { ModelTopicsResponse } from '../../../models/modelTopics';
import { TeacherService } from '../../../services/teacher';
import { CommonModule } from '@angular/common';
import { TopicsService } from '../../../services/topics';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { EventService } from '../../../services/events';
import { AuthService } from '../../../services/authService';

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
    private eventService: EventService,
    private topicService: TopicsService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      topicId: [null, [Validators.required]],
    });
  }

  ngOnInit(): void {
    this.getTopics();
    this.loadMyCourses();
  }
  //Get the all topics from backend
  private getTopics() {
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
          );
        } else {
          this.topics.set([]);
        }
      },
      error: () => this.topics.set([]),
      complete: () => this.loading.set(false),
    });
  }
  //Load all curses of a specific teacher
  private loadMyCourses() {
    this.loading.set(true);

    if (!this.getCurrentTeacherId()) {
      return;
    }

    this.eventService.getEventByCreatedById().subscribe({
      next: (res) => {
        if (Array.isArray(res) && !!res) {
          const mapped = res.map((x: any) => ({
            Id: x.Id ?? x.id,
            Name: x.Name ?? x.name,
            Description: x.Description ?? x.description,
            TopicName: x.TopicName ?? x.topicName,
            CreateBy: x.CreateBy ?? x.createBy,
            DateCreate: x.DateCreate ?? x.dateCreate,
            DateClose: x.DateClose ?? x.dateClose,
            Status: x.Status ?? x.status,
            Results: x.Results ?? x.results,
          }));

          mapped.sort((a, b) => {
            const da = new Date(a.DateCreate).getTime();
            const db = new Date(b.DateCreate).getTime();
            return db - da;
          });

          this.myCourses.set(mapped);
        }
      },
      error: () => this.myCourses.set([]),
      complete: () => this.loading.set(false),
    });
  }
  //Get the current id of teacher
  private getCurrentTeacherId(): number | null {
    const raw = this.authService.getAuthUser();

    if (!raw) return null;
    try {
      const parsed = JSON.parse(raw);
      return Number(parsed.entityId) || null;
    } catch {
      return null;
    }
  }
  //Button to create a event
  onCreateCourse() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: ModelEventsRequest = {
      Name: this.form.value.name,
      Description: this.form.value.description,
      TopicsId: this.form.value.topicId,
    };
    
    this.submitting.set(true);
    this.eventService.createEvent_teacher(payload).subscribe({
      next: (res) => {
        if (!res) {
          alert('Could not create course.');
          return;
        } else {
          alert('Course created successfully.');
          this.form.reset();
          this.form.markAsPristine();
          this.form.markAsUntouched();
          this.submitting.set(false);
          this.loadMyCourses();
        }
      },
      error: () => {
        this.submitting.set(false);
        alert('Error creating course.');
        return;
      },
    });
  }
  // helper of button state
  get submitDisabled() {
    return this.form.invalid || this.submitting();
  }
}
